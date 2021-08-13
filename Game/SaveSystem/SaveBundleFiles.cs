using System;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace Game.SaveSystem {
    public static class SaveBundleFiles {
        
        public static DirectoryInfo GetMostRecentDirectory() {
            var dirs = GetAllSaveFolders();

            DateTime latestTime;
            DirectoryInfo latestDir = null;

            bool first = true;
            
            foreach (var dir in dirs) {
                var lastTime = dir.LastAccessTime;

                if (first) {

                    latestTime = lastTime;
                    latestDir = dir;
                    
                    first = false;
                    continue;
                }
                
            }

            return latestDir;
        }

        public static DirectoryInfo OverwriteSaveDirectory(string saveFolderName) {
            var saveRootDir = GetSaveDirectory();

            foreach (var dir in GetAllSaveFolders()) {
                if (dir.Name == saveFolderName) {
                    dir.Delete(true);
                    return GetSaveDirectory().CreateSubdirectory(saveFolderName);
                }
            }


            return null;
        }

        public static bool CheckSaveExists(string saveFolderName) {
            foreach (var dir in GetAllSaveFolders()) {
                if (dir.Name.Equals(saveFolderName)) {
                    return true;
                }
            }

            return false;
        }

        public static DirectoryInfo[] GetAllSaveFolders() { 
            return GetSaveDirectory().GetDirectories();
        }

        public static DirectoryInfo GetSaveDirectory() {
            return Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }

        public static DirectoryInfo CreateSaveDirectory(string saveFolderName) {
            return GetSaveDirectory().CreateSubdirectory(saveFolderName);
        }
    }
}