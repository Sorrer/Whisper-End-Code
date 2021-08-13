using Game.Interactable.Highlight;
using UnityEngine;

namespace Game.Interactable {

    public class TestInteractable : MonoBehaviour, IInteractable {

        public DefaultItemHighlight defaultItemHighlight;

        private bool canInteract = true;
        public bool CanInteract() {
            return canInteract;
        }

        public void Use() {
            canInteract = !canInteract;
            Debug.Log("Interacted");
        }

        public void Unavailable() {
            canInteract = !canInteract;
            Debug.Log("Cannot Interact");
        }

        public InteractHighlightEffect GetHighlightEffect() {
            //Debug.Log("Highlight");
            return defaultItemHighlight;
        }
    }
}