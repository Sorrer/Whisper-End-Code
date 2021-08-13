using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.Scripts.LevelSystem {
    public class LevelDoorGeneration : EditorWindow {
        private static bool RebuildAllScenes;


        private static bool IsWorking = false;

        [MenuItem("Tools/Door Cache Builder")]
        static void ShowWindow() {
            LevelDoorGeneration window = (LevelDoorGeneration) EditorWindow.GetWindow(typeof(LevelDoorGeneration), true, "Door Cache Builder");
            window.Show();
        }
        
        void Init() {
            IsWorking = true;
            RebuildAllScenes = false;
        }
        
        void InitAll() {
            IsWorking = true;
            RebuildAllScenes = true;
        }

        
        

        private void OnGUI() {

            GUILayout.Label("Door Cache Builder");

            if (GUILayout.Button("Rebuild all scenes")) {
                InitAll();
            }

            if (GUILayout.Button("Rebuild current scene")) {
                Init();
            }

            if (GUILayout.Button("Verify door cache - TEMP")) {
                
            }
            
            String currentScene = EditorSceneManager.GetActiveScene().path;
            if (!IsWorking) return;

            try {

                Directory.CreateDirectory("Assets/Resources/Doors/");
                if(RebuildAllScenes){

                    //EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), true);
                    
                    List<string> currentNamesIterated = new List<string>();
                    
                    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {

                        Scene scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);
                        
                        //Debug.Log($"Building door cache for scene: {scene.name}");


                        if (currentNamesIterated.Contains(scene.name)) {
                            Debug.LogError($"Found duplicate scene name at {scene.path} " +
                                             $"\nRequires distinct scene names. Using first instance of scene name.\n" +
                                             $"THIS WILL NOT WORK IF YOU HAVE A DOOR REFERENCE TO THIS SCENE");
                            continue;
                        }
                        
                        currentNamesIterated.Add(scene.name);
                        
                        
                        RebuildDoors(scene);
                        
                        //EditorSceneManager.CloseScene(scene, true);


                        if(EditorUtility.DisplayCancelableProgressBar("Building Door Cache In All Scenes", 
                            $"Currently building scene {i} / {SceneManager.sceneCountInBuildSettings}",
                            (float)i /SceneManager.sceneCountInBuildSettings
                            )) {
                            
                            break;
                        }

                    }

                    EditorSceneManager.OpenScene(currentScene);
                    
                    Stop();
                    
                } else {

                    Scene scene = SceneManager.GetActiveScene();
                    RebuildDoors(scene);
                    Stop();
                }


            } catch (Exception e){
                Debug.LogException(e);
                EditorSceneManager.OpenScene(currentScene);
                Stop();
            }
        }


        private void Stop() {
            EditorUtility.ClearProgressBar();
            IsWorking = false;
        }

        public void RebuildDoors(Scene scene) {

            if (!scene.isLoaded || !scene.IsValid()) {
                Debug.LogError($"Could not rebuild doors for scene: {scene.name}. Scene is either not loaded or invalid");
                return;
            }
            

            Dictionary<string, Game.LevelSystem.LevelSystem.DoorInfo> doorsDict = new Dictionary<string, Game.LevelSystem.LevelSystem.DoorInfo>();
            
            
            var rootObjects = new List<GameObject>();
            
            scene.GetRootGameObjects(rootObjects);

            var doors = new List<LevelDoor>();
        
            foreach (var root in rootObjects) {
                CrawlForDoors(root.transform, ref doors);
            }

            foreach (var door in doors) {

                //We want to force the rebuild to stop for this level so someone knows that this is broken
                if (doorsDict.Keys.Contains(door.DoorName)) {
                    Debug.LogError($"Tried to find door in next scene. Failed due to duplicate doors in next scene {scene.path}");
                    return;
                }

                var doorInfo = door.GetDoorInfo();

                doorsDict.Add(door.DoorName, doorInfo);
            }

            if (doorsDict.Count > 0) {
                
                var setting = new JsonSerializerSettings();
                setting.Formatting = Formatting.Indented;
                
                setting.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

                var json = JsonConvert.SerializeObject(doorsDict, setting);

                var filePath = $"Assets/Resources/Doors/{scene.name}.json";
                using (FileStream fs = new FileStream(filePath, FileMode.Create)) {
                    using (StreamWriter writer = new StreamWriter(fs)) {
                        writer.Write(json);
                    }
                }
                
                UnityEditor.AssetDatabase.ImportAsset(filePath);
            
                Debug.Log($"Loaded doors for scene {scene.name} {scene.path}. Found {doorsDict.Count} doors");
            } else {
                Debug.Log($"No doors found for scene {scene.name}. Did not generate.");
            }
            
        }


        public void CrawlForDoors(Transform parent, ref List<LevelDoor> doors) {
            var levelDoor = parent.gameObject.GetComponent<LevelDoor>();
            if (!(levelDoor is null)) {
                doors.Add(levelDoor);
            }


            foreach (Transform child in parent) {
                CrawlForDoors(child, ref doors);
            }
        }


        private void OnInspectorUpdate() {
            Repaint();
        }
    }
}
