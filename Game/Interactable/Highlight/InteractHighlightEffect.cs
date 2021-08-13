using UnityEngine;

namespace Game.Interactable.Highlight {

    public abstract class InteractHighlightEffect : MonoBehaviour {
        public abstract void Highlight();
        public abstract void HighlightInvalid();
        public abstract void UnHighlight();
    }
}