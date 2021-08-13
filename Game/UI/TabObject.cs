using UnityEngine;

namespace Game.UI {
    public class TabObject : MonoBehaviour {

        
        public void Activate() {
            this.gameObject.SetActive(true);
        }

        public void Deactive() {
            this.gameObject.SetActive(false);
        }
    }
}
