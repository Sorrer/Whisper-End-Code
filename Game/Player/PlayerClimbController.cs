using Game.Collider;
using Game.Input;
using UnityEngine;

namespace Game.Player {
    public class PlayerClimbController : MonoBehaviour {


        public CircleCollider2D ColliderLeft, ColliderRight, ColliderCenter, ColliderDown;

        public float ClimbingSpeed = 10;
        public float HorizontalClimbingSpeed = 4;
        public bool IsClimbing => climbing;
        public bool CanClimb = true;
        public LayerMask ClimableLayerMask;

        public float ClimbingCooldown = 0.15f;
        public float DownSpeedMultiplier = 1.15f;
        public float HopVelocity = 0.5f;
        private float currentClimbingCooldown = 0.15f;
        private GameInput gameInput;
        private bool climbing;
        private PlayerController playerController;
        private RaycastCollider2D raycastCollider2D;

        // Start is called before the first frame update
        void Start() {
            gameInput = MainInstances.Get<GameInput>();
            playerController = GetComponent<PlayerController>();
            raycastCollider2D = GetComponent<RaycastCollider2D>();
        }

        
        
        // Update is called once per frame
        void Update()
        {
        
            //On climb button
            if (currentClimbingCooldown < ClimbingCooldown) currentClimbingCooldown += Time.deltaTime;
            
            bool canClimb = CheckCanClimb();
            
            if (climbing) {
                if ((!canClimb || gameInput.JumpPressed)) {
                    DeactivateClimb();
                    if (gameInput.JumpPressed || gameInput.GetUp()) {
                        playerController.VerticalVelocity = HopVelocity;
                        playerController.Jump();
                        raycastCollider2D.AutoUnstuck = true;
                    }
                }

                Vector3 Displacement = Vector3.zero;
                if (gameInput.GetLeft() && CheckCanClimbLeft()) {
                    Displacement += Vector3.left * (gameInput.Left * Time.deltaTime * HorizontalClimbingSpeed);
                }

                if (gameInput.GetRight() && CheckCanClimbRight()) {
                    Displacement += Vector3.right * (gameInput.Right * Time.deltaTime * HorizontalClimbingSpeed);
                }

                float currentUpDown = gameInput.Up + -gameInput.Down;

                if (currentUpDown < 0 && !CheckCanClimbDown()) {
                    DeactivateClimb();
                    return;
                }

                if (currentUpDown < 0) {
                    currentUpDown *= DownSpeedMultiplier;
                }
                Displacement += Vector3.up * (currentUpDown * Time.deltaTime * ClimbingSpeed);

                //raycastCollider2D.Move(Displacement);
                this.transform.position += Displacement;

            }else if ((gameInput.GetUp() || gameInput.GetDown()) && Mathf.Abs(gameInput.Up - gameInput.Down) > 0.25f && canClimb && currentClimbingCooldown >= ClimbingCooldown) {
                ActivateClimb();
                raycastCollider2D.AutoUnstuck = false;
            }
            
            
        }


        private void ActivateClimb() {
            climbing = true;
            playerController.enabled = false;
            currentClimbingCooldown = 0;
        }

        private void DeactivateClimb() {
            climbing = false;
            playerController.enabled = true;
            currentClimbingCooldown = 0;
        }

        public bool CheckCanClimb() {
            return Physics2D.OverlapCircle(ColliderCenter.transform.position, ColliderCenter.radius,
                ClimableLayerMask) && CanClimb;
        }

        public bool CheckCanClimbLeft() {
            return Physics2D.OverlapCircle(ColliderLeft.transform.position, ColliderLeft.radius,
                ClimableLayerMask);
        }
        public bool CheckCanClimbRight() {
            return Physics2D.OverlapCircle(ColliderRight.transform.position, ColliderRight.radius,
                ClimableLayerMask);
        }

        public bool CheckCanClimbDown() {
            return Physics2D.OverlapCircle(ColliderDown.transform.position, ColliderDown.radius,
                ClimableLayerMask);
        }
    }
}
