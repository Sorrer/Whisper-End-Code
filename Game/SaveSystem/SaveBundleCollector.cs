using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Game.SaveSystem {

    public struct SaveDataString {
        public string FileName;
        public string SaveData;
    }

    public struct SaveTypeData {
        public string FileName;
        public Type Type;
    }
    
    public static class SaveBundleCollector {

        public static SaveTypeData[] TypesWithAttribute = null;
        
        
        public static List<SaveDataString> CollectSaveData() {

            var saveBundles = GetSaveBundles();

            var saveDataStrings = new List<SaveDataString>();
            
            foreach (var bundle in saveBundles) {
                
                saveDataStrings.Add(new SaveDataString() {
                    FileName = bundle.Key,
                    SaveData = bundle.Value.SaveData()
                });
                
            }


            return saveDataStrings;
        }

        public static void LoadSaveData(SaveDataString[] dataStrings) {

            var saveBundles = GetSaveBundles();
            
            #if UNITY_EDITOR
            //Debug for duplicate data strings in editor.

            for(int i = 0; i <  dataStrings.Length; i++) {
                var data = dataStrings[i];
                
                for(int x = 0; x <  dataStrings.Length; x++) {
                    var data2 = dataStrings[x];

                    if (i == x) continue;
                    
                    if (data.FileName == data2.FileName) {
                        throw new Exception(
                            $"Failed to load save data, duplicated data string detected. {data.FileName}\n" +
                            $"Bundle1 - {data.SaveData}\n" +
                            $"Bundle2 - {data2.SaveData}");
                        return;
                    }
                }
            }
            
            
            #endif
            
            foreach (var dataString in dataStrings) {
                try {
                    saveBundles[dataString.FileName].LoadData(dataString.SaveData);
                } catch (Exception e) {
                    throw new Exception(
                        $"Failed to load save data in bundles. Most likely filename is not found in the system {dataString.FileName}\n" + e.StackTrace);
                }
            }
            
        }


        private static Dictionary<string, ISaveBundle> GetSaveBundles() {
            var types = GetAllTypesWithAttribute();

            var saveBundles = new Dictionary<string, ISaveBundle>();
            
            
            foreach (SaveTypeData saveType in types) {


                var obj = MainInstances.GetObject(saveType.Type);

                if (obj == null) {
                    throw new Exception(
                        $"Failed to recieve required save bundle obj ({saveType.Type.FullName}) from MainInstances. Check script execution order.");
                }
                
                ISaveBundle bundle = obj as ISaveBundle;

                if (bundle == null) {
                    throw new Exception(
                        "Failed to collect save bundle. Type received when searched for was not implemented as an ISaveBundle");
                }

                if (saveBundles.Keys.Contains(saveType.FileName)) {
                    throw new Exception("Failed to collect save bundle. Duplicate file names detected.\n" +
                                        $"Added: {saveBundles[saveType.FileName]} - {saveType.FileName}\n" +
                                        $"Tried to add: {bundle.GetType().FullName} - {saveType.FileName}");
                }
                
                saveBundles.Add(saveType.FileName, bundle);
            }

            return saveBundles;
        }


        /// <summary>
        /// Caches the types for future use. This helps to stop crawling multiple times throughout runtime.
        /// </summary>
        /// <returns></returns>
        private static SaveTypeData[] GetAllTypesWithAttribute() {

            if (TypesWithAttribute != null) {
                return TypesWithAttribute;
            }

            var results = new List<SaveTypeData>();

            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in appDomainAssemblies) {
                var types = assembly.GetTypes();
                foreach (Type type in types) {

                    var attributes = type.GetCustomAttributes(typeof(SavableBundleAttribute), false);
                    
                    SavableBundleAttribute attribute;
                    
                    if (attributes.Length > 0) {
                        attribute = (SavableBundleAttribute)attributes[0];
                    } else {
                        continue;
                    }


                    bool found = false;
                    foreach (var interfaceType in type.GetInterfaces()) {
                        if (interfaceType == typeof(IGameInstance)) {
                            
                            results.Add(new SaveTypeData() {
                                Type = type,
                                FileName = attribute.FileName
                            } );
                            found = true;
                            break;
                        }
                    }

                    if (found) continue;
                    
                    Debug.LogWarning($"Found ISaveBundle without necessary IGameInstance {type.FullName}");
                    
                }
            }


            TypesWithAttribute = results.ToArray();
            
            return TypesWithAttribute;
        }
    }
}
