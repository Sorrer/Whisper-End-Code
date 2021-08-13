
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.SaveSystem {
    
    //TODO: Move savesystem from game master to its own save section. This will run when called. If ran make it refresh the scene it is in.
    //TODO: Make sure to check if the savelist is there.
    
    
    
    //IMPORTANT - All save data used by this system should be presistent as in when you change scenes, save data comes with and does not need to be reinstanted.
    public class SaveBundleManager : MonoBehaviour
    {
        private void Awake() {
            Save("Test");
            Load();
        }

        /// <summary>
        /// Will overwrite the whole directory if exists (aka delete everything and add the new stuff)
        /// </summary>
        /// <param name="saveFolderName"></param>
        public void Save(string saveFolderName) {

            DirectoryInfo saveDir;
            if (SaveBundleFiles.CheckSaveExists(saveFolderName)) {
                Debug.LogWarning($"Overriding save folder {saveFolderName}");
                saveDir = SaveBundleFiles.OverwriteSaveDirectory(saveFolderName);
            } else {
                saveDir = SaveBundleFiles.CreateSaveDirectory(saveFolderName);
            }
            
            
            var saveData = SaveBundleCollector.CollectSaveData(); 
            
            List<string> saveFileListings = new List<string>();


            string saveFolderLocation = Application.persistentDataPath + "/Saves/" + saveFolderName;
            
            foreach (var data in saveData) {
                saveFileListings.Add(data.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(saveFolderLocation + $"/{data.FileName}.dat") ?? throw new Exception($"Failed to find directory of {saveFolderLocation + "/" + data.FileName}"));
                File.WriteAllText( saveFolderLocation + $"/{data.FileName}.dat", data.SaveData);
            }


            string fileListingJSON = JsonConvert.SerializeObject(saveFileListings);
            
            
            File.WriteAllText(Application.persistentDataPath + "/Saves/" + saveFolderName + "/savelist.dat", fileListingJSON);

            
            
        }


        public void Load(string saveFolderName = null) {
            if (saveFolderName == null) {
                var recentDir = SaveBundleFiles.GetMostRecentDirectory();

                if (recentDir == null) {
                    Debug.LogWarning("Failed to load. No saves were detected. Checked - " + Application.persistentDataPath + "/Saves/");
                    return;
                }

                saveFolderName = recentDir.Name;
            }
            
            Debug.Log("Loading save - " + saveFolderName);


            string text = File.ReadAllText(Application.persistentDataPath + "/Saves/" + saveFolderName + "/savelist.dat");

            var saveFileListings = JsonConvert.DeserializeObject<List<string>>(text);


            List<SaveDataString> dataStrings = new List<SaveDataString>();
            
            string saveFolder = Application.persistentDataPath + "/Saves/" + saveFolderName;
            
            //Problems may occur if the file is currently locked. Should find a way around that.
            foreach(string fileName in saveFileListings) {
                string fileContents = File.ReadAllText(saveFolder + "/" + fileName + ".dat");
                
                dataStrings.Add(new SaveDataString() {
                    FileName = fileName,
                    SaveData = fileContents
                });

            }

            SaveBundleCollector.LoadSaveData(dataStrings.ToArray());
        }


        
        
        

    }
}
