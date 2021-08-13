using System;
using Game;
using Game.Camera;
using Game.Dialogue;
using Game.Registry;
using Game.Player;
using System.Collections;
using System.Collections.Generic;
using Game.Input;
using UnityEngine;



namespace Game.Injector {
    
    [Obsolete("This is not feasible. User should not have generic ease of use to change these components. " +
              "Rely on the script doing it please")]
    public class InstancesInjector : MonoBehaviour {

        [Serializable]
        public struct BlankStruct { }

        [Header("This is deprecated")] public BlankStruct Deprecated;

        /**
        public DialogueSystem dialogueSystem;
        public ValueRegistry valueRegistry;
        public AgentRegistry agentRegistry;
        public GameCameras gameCameras;
        public PlayerMain player;
        //public ItemRegistry ItemRegistry;
        public GameInput gameInput;
        // Start is called before the first frame update
        void Awake() {

            MainInstances.Add(dialogueSystem);
            MainInstances.Add(valueRegistry);
            MainInstances.Add(agentRegistry);
            MainInstances.Add(gameCameras);
            MainInstances.Add(player);
            MainInstances.Add(gameInput);
            //MainInstances.Add(ItemRegistry);

        }
        **/
    }

}
