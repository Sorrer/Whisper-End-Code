using System.Collections;
using System.Collections.Generic;
using Game.Interactable;
using Game.Interactable.Highlight;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable {

    public InteractHighlightEffect HighlightEffect;
    
    public bool CanInteract() {
        return true;
    }

    public void Use() {
    }

    public void Unavailable() {
    }

    public InteractHighlightEffect GetHighlightEffect() {
        return HighlightEffect;
    }
}
