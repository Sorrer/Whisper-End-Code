using Cinemachine;
using Game.Collider;
using Game.HealthSystem;
using System.Collections;
using System.Collections.Generic;
using Game.Audio;
using Game.Camera;
using Game.Registry;
using UnityEngine;


/// <summary>
/// Main reference to player
/// </summary>
namespace Game.Player {
    [RequireComponent(typeof(PlayerController), typeof(RaycastCollider2D), typeof(HealthManager))]
    public class PlayerMain : MonoBehaviour, IGameInstance {

        [HideInInspector]
        public PlayerController controller;
        [HideInInspector]
        public RaycastCollider2D raycastCollider;
        [HideInInspector]
        public HealthManager healthManager;
        //[HideInInspector]
        //public PlayerDeath death; - REMOVED FROM UNTITLED GAME



        public PlayerInventory Inventory;

        [HideInInspector]
        //public PlayerInventory inventory; - REMOVED FROM UNTITLED GAME
        public CinemachineVirtualCamera PlayerCamera;

        public void Teleport(Vector3 pos) {

            Vector3 delta = this.transform.position - pos;
            this.transform.position = pos;

            if (PlayerCamera) {
                PlayerCamera.PreviousStateIsValid = false;
                PlayerCamera.OnTargetObjectWarped(this.transform, delta);
            }
        }
        
        
        
        private void Awake() {
            MainInstances.Add(this);
        }
        
        // Start is called before the first frame update
        void Start() {
            controller = this.GetComponent<PlayerController>();
            raycastCollider = this.GetComponent<RaycastCollider2D>();
            healthManager = this.GetComponent<HealthManager>();

            PlayerCamera = MainInstances.Get<GameCameras>().MainCameraVC;
        }

        public float FootstepTimer;
        private float curFootstepTime;
        // Update is called once per frame
        void Update() {
            /*
            if (controller.isWalking && raycastCollider.isGrounded) {
                curFootstepTime += Time.deltaTime;
                if (curFootstepTime >= FootstepTimer * (controller.SlowMode ? 2 : 1)) {
                    curFootstepTime = 0;
                    // FootstepSource.Play();
                }
            } else {
                curFootstepTime = FootstepTimer;
            }

            if (controller.justJumped) {
                // JumpSource.Play();
            }

            if (healthManager.Health <= 0) {
                //death.Die(); - REMOVED FROM UNTITLED GAME
                healthManager.Heal(int.MaxValue);
            }*/
        }
    }
}