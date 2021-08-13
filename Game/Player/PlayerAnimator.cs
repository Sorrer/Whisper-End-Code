using System.Collections;
using System.Collections.Generic;
using Game.Audio;
using UnityEngine;
namespace Game.Player {
    public class PlayerAnimator : MonoBehaviour {

        public Animator animator;
        public PlayerController controller;
        public PlayerClimbController ClimbController;

        public GameObject spriteObject;

        public BoxCollider2D boxCollider;
        private Vector2 colliderSize;

        public float LandEffectLimiter = -0.25f;

        public ParticleSystem SlideEffects;
        public ParticleSystem JumpEffects;
        public ParticleSystem LandEffects;

        
        private int animationGoingUp;
        private int animationGoingDown;
        private int animationProne;
        private int animationProneIdle;
        private int animationSlide;
        private int animationRunning;
        private int animationWalking;
        private int animationSpeed;
        private int animationClimbing;
        private int animationClimbingTrigger;
        private int animationClimbingSpeed;
        // Start is called before the first frame update
        void Start() {
            colliderSize = boxCollider.size;

            animationGoingUp = Animator.StringToHash("GoingUp");
            animationGoingDown = Animator.StringToHash("GoingDown");
            animationProne = Animator.StringToHash("Prone");
            animationProneIdle = Animator.StringToHash("ProneIdle");
            animationSlide  = Animator.StringToHash("Slide");
            animationRunning = Animator.StringToHash("Running");
            animationWalking = Animator.StringToHash("Walking");
            animationSpeed = Animator.StringToHash("Speed");
            animationClimbing = Animator.StringToHash("Climbing");
            animationClimbingSpeed = Animator.StringToHash("ClimbingSpeed");
            animationClimbingTrigger = Animator.StringToHash("ClimbingTrigger");
            
        }

        private bool lastGround = true;
        private float lastVerticalVelocity = 0;


        private bool lastClimbing = false;
        // Update is called once per frame
        void Update() {

            if (controller.sRenderer.flipX) {
                var pos = StepParticleSystem.transform.localPosition;
                pos.x = Mathf.Abs(pos.x);
                StepParticleSystem.transform.localPosition = pos;
                
                StepParticleSystem.transform.rotation = Quaternion.Euler(0, 0, 315);
            } else {
                
                var pos = StepParticleSystem.transform.localPosition;
                pos.x = Mathf.Abs(pos.x) * -1;
                StepParticleSystem.transform.localPosition = pos;
                
                StepParticleSystem.transform.rotation = Quaternion.Euler(0, 0, 135);
            }
            
            animator.SetBool(animationGoingUp, false);
            animator.SetBool(animationGoingDown, false);
            animator.SetBool(animationProne, false);
            animator.SetBool(animationProneIdle, false);
            animator.SetBool(animationSlide, false);
            animator.SetBool(animationRunning, false);
            animator.SetBool(animationWalking, false);
            
            animator.SetBool(animationClimbing, ClimbController.IsClimbing);
            
            if(lastClimbing != ClimbController.IsClimbing && ClimbController.IsClimbing)animator.SetTrigger(animationClimbingTrigger);
            lastClimbing = ClimbController.IsClimbing;
            
            animator.SetFloat(animationClimbingSpeed, controller.CurrentGameInput.Up - controller.CurrentGameInput.Down);
            
            boxCollider.size = new Vector2(controller.isSliding ? colliderSize.x * 2 : colliderSize.x, (controller.isCrouching ? controller.CrouchPercent * colliderSize.y : colliderSize.y));

            if (controller.isSliding || controller.isCrouching) {
                spriteObject.transform.localPosition = new Vector3(0, controller.CrouchPercent * colliderSize.y * 0.5f, 1 / 16.0f);
            } else {
                spriteObject.transform.localPosition = new Vector3(0,0, 1 / 16.0f);
            }
            
            if (controller.isSliding) {
                animator.SetBool(animationSlide, true);
                if(!SlideEffects.isPlaying) SlideEffects.Play();
                var pos = SlideEffects.gameObject.transform.localPosition;
                var rot = SlideEffects.gameObject.transform.eulerAngles;
                if (controller.sRenderer.flipX) {
                    pos.x = Mathf.Abs(pos.x) * -1;
                    rot.z = 195;
                } else {
                    rot.z = -15;
                    pos.x = Mathf.Abs(pos.x);
                }

                SlideEffects.gameObject.transform.localPosition = pos;
                SlideEffects.gameObject.transform.eulerAngles = rot;
            } else if (controller.isCrouching) {
                animator.SetBool(animationProneIdle, true);
                if (controller.isWalking) {
                    animator.SetBool(animationProne, true);
                }
            } else if (!controller.PlayerCollider.isGrounded) {
                if (controller.VerticalVelocity > 0) {
                    animator.SetBool(animationGoingUp, true);
                } else {
                    animator.SetBool(animationGoingDown, true);
                }
            } else {

                animator.SetBool(controller.SlowMode ? animationWalking : animationRunning, controller.isWalking);

            }
            
            animator.SetFloat(animationSpeed, Mathf.Abs(controller.CurrentGameInput.Left - controller.CurrentGameInput.Right));
            
            if (controller.CanStartJump()) {
                JumpEffects.Play();
            }
            
            if(controller.PlayerCollider.isGrounded && !lastGround && lastVerticalVelocity <= LandEffectLimiter) LandEffects.Play();
            
            if (!controller.isSliding || (controller.isSliding && !controller.PlayerCollider.isGrounded)) {
                SlideEffects.Stop();
            }

            lastVerticalVelocity = controller.VerticalVelocity;
            lastGround = controller.PlayerCollider.isGrounded;
            animator.Update(0);
            animator.Update(0);
            animator.Update(0);
        }

        public ParticleSystem StepParticleSystem;
        public AudioRandomClip FootstepSounds;
        public void OnPlayerStep() {
            StepParticleSystem.Play(true);
            FootstepSounds.Play();
        }
    }
}