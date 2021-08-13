using System;
using System.Collections.Generic;
using Game.Player;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.LevelSystem {
    public class LevelSystem {

        public static string DoorRef;
        public static string SceneRef;
        public static Vector3 NextDoorPos;
    
        public static void LoadNewScene(string name, string door = "none") {

            if (door.Equals("none")) {
                Debug.LogWarning("Entering a new scene without a door referenced. Not changing.");
                return;
            }
            

            DoorInfo foundDoor;

            if (!FindDoor(name, door, out foundDoor)) {
                return;
            }

            Debug.Log("Transporting to new door!\n" + foundDoor);
            
            NextDoorPos = foundDoor.Position;

            try {
                SceneManager.sceneLoaded -= OnSceneLoad;
            } catch { // ignored
            }

            SceneManager.sceneLoaded += OnSceneLoad;

            SceneManager.LoadScene(name);

        }

    

        public class DoorInfo {
            public Vector3 Position;
            public String SceneRef;
            public String DoorRef;

            public bool AllowExit;
            public bool AllowEnterance;

            public override string ToString() {
                return $"{nameof(Position)}: {Position}, {nameof(SceneRef)}: {SceneRef}, {nameof(DoorRef)}: {DoorRef}, {nameof(AllowExit)}: {AllowExit}, {nameof(AllowEnterance)}: {AllowEnterance}";
            }
        }
    
    
        /// <summary>
        /// Loads door cache in resources for specific scene and gets the DoorInfo if found
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="door"></param>
        public static bool FindDoor(string sceneName, string door, out DoorInfo doorInfo) {
            doorInfo = null;

            string json = "";


            var textAsset = Resources.Load<TextAsset>($"Doors/{sceneName}");

            if (textAsset is null) {
                Debug.LogError($"Could not find specific scene {sceneName}");
                return false;
            }

            json = textAsset.text;
            
            
            var doorsDict = JsonConvert.DeserializeObject<Dictionary<string, DoorInfo>>(json);

            if (doorsDict.TryGetValue(door, out doorInfo)) {

                if (!doorInfo.AllowExit) {
                    return false;
                }
                
                return true;
            }
            

            return false;
        }


        public static void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        
            //If using a loading scene, we would have to make a different method for it.
            //First if a we need to load new scene, we would load the loading scene
            //When the loading scene is loaded we'll have a component on load,
            //take the string of SceneRef and DoorRef and loadasync with it,
            //When sceneLoaded is called again check if its not the loading scene
            //If it is not, then go ahead and teleport player to the door.
        
            //All objects in the scene will independently handle their registry values and loading
            //So npcs on start will check the registry for their required position and stuff, and go forward from there
        
        
            //Teleport player to required places
            var player = MainInstances.Get<PlayerMain>();

            var hit = Physics2D.Raycast(NextDoorPos, Vector2.down, 32.0f, LayerMask.GetMask("Default"));
            Vector3 pos;
            if (hit) {
                pos = hit.point;
                pos.y += 1.88f/2;
            } else {
                Debug.LogWarning("Failed to hit ground with door. May be intentional. Teleporting player directly on door");
                pos = NextDoorPos;
            }
        
            player.Teleport(pos);
            Debug.Log($"Teleported player to scene {SceneRef} at door {DoorRef}");
        
        }

    }
}
