using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Player {
    public class InventroyUIElement : MonoBehaviour {

        public Image image;
        public int Index;
        
        public Sprite sprite {
            get {
                return currentSprite;
            }
            
            set {
                currentSprite = value;
                image.sprite = value;

            }
        }

        private Sprite currentSprite;

    }
}