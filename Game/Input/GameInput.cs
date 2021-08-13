using System;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

namespace Game.Input {
    public class GameInput : MonoBehaviour, IGameInstance {
        public float Left => actionLeft.ReadValue<float>();
        public float Right => actionRight.ReadValue<float>();
        public float Up => actionUp.ReadValue<float>();
        public float Down => actionDown.ReadValue<float>();
        public float Crouch => actionCrouchSlide.ReadValue<float>();
        public float Jump => actionJump.ReadValue<float>();
        public float Interact => actionInteract.ReadValue<float>();
        
        public bool JumpPressed { get; private set; }
        public bool JumpReleased { get; private set; }
        
        public bool InteractPressed { get; private set; }
        public bool InteractReleased { get; private set; }
        
        public bool CrouchSlidePressed { get; private set; }
        public bool CrouchSlideReleased { get; private set; }
        
        public bool UpPressed  {get; private set;}
        public Vector2 ControllerAim => actionControllerAim.ReadValue<Vector2>();
        public Vector2 MousePos => Mouse.current.position.ReadValue();

        public float Fire => actionFire.ReadValue<float>();
        
        public PlayerInput Input;

        private bool Init = false;
        
        private InputAction actionLeft, actionRight, actionUp, actionDown;
        private InputAction actionInteract, actionJump, actionCrouchSlide, actionFire, actionControllerAim;

        private float lastUpValue;
        
        public bool IsController { get; private set; }
        private void Awake() {
            
            MainInstances.Add(this);
            
            if (Input == null) {
                throw new UnityException("GameInputAsset was not given! Will not work.");
            }
            
            Debug.Log("Game input hooked");
            
            Input.onControlsChanged += (device) => {
                
                IsController = device.currentControlScheme == "Gamepad";
                
            };
        }
        
        
        
        public void Update() {

            if (!Init && Input != null) {
                Init = true;

                actionLeft = Input.actions["Left"];
                actionRight = Input.actions["Right"];
                actionUp = Input.actions["Up"];
                actionDown = Input.actions["Down"];
                actionJump = Input.actions["Jump"];
                actionCrouchSlide = Input.actions["CrouchSlide"];
                actionFire = Input.actions["Fire"];
                actionInteract = Input.actions["Interact"];
                actionControllerAim = Input.actions["Aim"];
                SubscribeEvents();

            }else if (!Init && Input == null) {
                Debug.LogError("Could not hook player input!");
            }
            
            //Checks if the last value is 0 and not pressed
            if (Math.Abs(lastUpValue) < FloatComparer.kEpsilon && Math.Abs(lastUpValue - Up) > 0 + FloatComparer.kEpsilon) {
                UpPressed = true;
            }
            else {
                UpPressed = false;
            }
            
            lastUpValue = Up;
        }

        public Vector2 GetAimDir(Vector2 centerScreenPoint) {
            if (IsController) {
                return ControllerAim.normalized;
            }
            else {
                Vector2 aimDir = (Mouse.current.position.ReadValue() - centerScreenPoint).normalized;
                return aimDir;
            }
        }
        


        public bool GetLeft() {
            return Math.Abs(Left) > FloatComparer.kEpsilon;
        }

        public bool GetRight() {
            return Math.Abs(Right) > FloatComparer.kEpsilon;
        }

        public bool GetUp() {
            return Math.Abs(Up) > FloatComparer.kEpsilon;
        }
        public bool GetDown() {
            return Math.Abs(Down) > FloatComparer.kEpsilon;
        }
        
        public bool GetCrouch() {
            return Math.Abs(Crouch) > FloatComparer.kEpsilon;
        }
        
        public bool GetJump() {
            return Math.Abs(Jump) > FloatComparer.kEpsilon;
        }

        public bool GetInteract() {
            return Math.Abs(Interact) > FloatComparer.kEpsilon;
        }

        public bool GetFire() {
            return Math.Abs(Fire) > FloatComparer.kEpsilon || Mouse.current.leftButton.isPressed;
        }

        private void SetJumpPressed(InputAction.CallbackContext ctx) {
            JumpPressed = true;
        }
        private void SetJumpReleased(InputAction.CallbackContext ctx) {
            JumpReleased = true;
        }
        
        private void SetCrouchSlidePressed(InputAction.CallbackContext ctx) {
            CrouchSlidePressed = true;
        }
        private void SetCrouchSlideReleased(InputAction.CallbackContext ctx) {
            CrouchSlideReleased = true;
        }
        
        private void SetInteractPressed(InputAction.CallbackContext ctx) {
            InteractPressed = true;
        }
        private void SetInteractReleased(InputAction.CallbackContext ctx) {
            InteractReleased = true;
        }

        private void SubscribeEvents() {
            actionJump.performed += SetJumpPressed;
            actionJump.canceled += SetJumpReleased;

            actionCrouchSlide.performed += SetCrouchSlidePressed;
            actionCrouchSlide.canceled += SetCrouchSlideReleased;
            
            actionInteract.performed += SetInteractPressed;
            actionInteract.canceled += SetInteractReleased;
        }

        private void UnsubscribeEvents() {
            actionJump.performed -= SetJumpPressed;
            actionJump.canceled -= SetJumpReleased;
            
            actionCrouchSlide.performed -= SetCrouchSlidePressed;
            actionCrouchSlide.canceled -= SetCrouchSlideReleased;
            
            actionInteract.performed -= SetInteractPressed;
            actionInteract.canceled -= SetInteractReleased;
        }
		

        private void LateUpdate() {
            JumpPressed = false;
            JumpReleased = false;
            CrouchSlidePressed = false;
            CrouchSlideReleased = false;

            InteractPressed = false;
            InteractReleased = false;
        }

        private void OnDestroy() {
            try {
                UnsubscribeEvents();
            }
            catch {
                Debug.LogWarning("Failed to unsubscribe from events!!!! - LEAK");
            }
        }

    }
    
}
