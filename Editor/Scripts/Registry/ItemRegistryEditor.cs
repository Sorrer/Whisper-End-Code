using System;
using Game.Registry;
using Game.Registry.Objects;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Registry {
    [CustomEditor(typeof(ItemRegistryObject))]
    public class ItemRegistryEditor : UnityEditor.Editor {

        ItemRegistryObject registry;
        private void OnEnable() {
            registry = (ItemRegistryObject) target;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();    


            if (GUILayout.Button("Auto Register All")) {
                Debug.Log(AssetDatabase.GetAssetPath(registry));
                registry.itemPairs.Clear();
                EditorUtility.DisplayProgressBar("AutoRegister", "Registering", 0);
                string[] propertiesPath = AssetDatabase.FindAssets("t:ItemProperties", null);

                for(int i = 0; i < propertiesPath.Length; i++) {
                    var guid = propertiesPath[i];
                    
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    EditorUtility.DisplayProgressBar("AutoRegister", "Registering - path", i/(float)propertiesPath.Length);


                    ItemProperties properties = AssetDatabase.LoadAssetAtPath<ItemProperties>(path);

                    bool foundDuplicate = false;
                    
                    foreach (var pair in registry.itemPairs) {
                        if (properties.ItemID == pair.id) {
                            Debug.LogError("Found duplicate item pair! \nPath: " + path + " \nID: "  + pair.id + " \nExisting: " + AssetDatabase.GetAssetPath(pair.value));
                            foundDuplicate = true;
                            break;
                        }
                    }

                    if (foundDuplicate) continue;
                    
                    registry.itemPairs.Add(new ItemRegistryObject.TestValueObjectPair(){id = properties.ItemID, value = properties});
                    
                    
                    
                    Debug.Log(path);
                    
                    
                }
                
                EditorUtility.ClearProgressBar();
                EditorUtility.SetDirty(registry);
            }
            
        }
    }
}
