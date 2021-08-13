using UnityEngine;

namespace Game.SaveSystem.Test {
    //[SavableBundle("Test/Test2")]
    public class SaveSystemTest2 : MonoBehaviour, ISaveBundle, IGameInstance
    {
        private void Awake() {
            //MainInstances.Add(this);
        }

        public void LoadData(string saveData) {
            Debug.Log("SaveSystemTest2: " + saveData);
        }

        public string SaveData() {
            return "This is the second test string to just test and have fun.";
        }
    }
}
