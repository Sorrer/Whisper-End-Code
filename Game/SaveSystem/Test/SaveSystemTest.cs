using System;
using UnityEngine;

namespace Game.SaveSystem.Test {
    
    [SavableBundle("TestFile")]
    public class SaveSystemTest : MonoBehaviour, ISaveBundle, IGameInstance
    {
        private void Awake() {
            MainInstances.Add(this);
        }

        public void LoadData(string saveData) {
            Debug.Log("SaveSystemTest Loaded: " + saveData);
        }

        public string SaveData() {
            return "This is test save data. For testing";
        }
    }
}
