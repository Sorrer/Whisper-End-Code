using System;
using Game.Dialogue;
using Game.Interactable.Highlight;
using UnityEngine;

namespace Game.Interactable.Interactables {
    public class DialogueInteractable : MonoBehaviour, IInteractable {

        public DialogueGraph NpcDialogueGraph;
        public InteractHighlightEffect HighlightEffect;


        private DialogueSystem dialogueSystem;
        private void Start() {
            dialogueSystem = MainInstances.Get<DialogueSystem>();
        }

        public bool CanInteract() {
            return true;
        }

        public void Use() {
            Debug.Log("Interactable dialogue " + NpcDialogueGraph.name);
            dialogueSystem.Present(NpcDialogueGraph);
        }

        public void Unavailable() {
            
        }

        public InteractHighlightEffect GetHighlightEffect() {
            return HighlightEffect;
        }
    }
}
