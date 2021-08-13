using System;
using System.Collections;
using System.Collections.Generic;
using Game.Interactable;
using Game.Interactable.Highlight;
using Game.LevelSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour , IInteractable {


    #region Interaction
    
    public InteractHighlightEffect HighlightEffect;
    
    public bool CanInteract() {
        return AllowEnterance;
    }

    public void Use() {
        EnterDoor();
    }

    public void Unavailable() {
    }

    public InteractHighlightEffect GetHighlightEffect() {
        return HighlightEffect;
    }

    #endregion
    
    
    
    

    /// <summary>
    /// Allows the door to be entered
    /// </summary>
    public bool AllowEnterance = true;

    /// <summary>
    /// Allows the door to act like an exit for other doors. Aka one ways.
    /// </summary>
    public bool AllowExit = true;

    
    public string DoorName;
    
    
    [Space(4)]
    [Header("References this door leads to")]
    [Space(2)]
    public string LevelRef;
    public string DoorRef;

    public void EnterDoor() {
        if (!AllowEnterance) return;
        
        LevelSystem.LoadNewScene(LevelRef, DoorRef);
    }


    public LevelSystem.DoorInfo GetDoorInfo() {
        return new LevelSystem.DoorInfo() {
            Position = transform.position,
            AllowEnterance = this.AllowEnterance,
            AllowExit = this.AllowExit,
            DoorRef = this.DoorRef,
            SceneRef = this.LevelRef
        };
    }
}
