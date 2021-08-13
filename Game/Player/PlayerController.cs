using Game.Collider;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Input;
using UnityEngine;
using UnityEngine.Assertions.Comparers;


namespace Game.Player {
    public class PlayerController : MonoBehaviour {


        [Header("Activations")]
        [Space(4)]
        public bool EnableJump;
        public bool EnableSlide;
        public bool EnableCrouch;
        public bool EnableMove;
        public bool SlowMode;

        public bool FaceAim = false;

        [Space(4)]
        [Header("References")]
        public RaycastCollider2D PlayerCollider;
        public SpriteRenderer sRenderer;
        private BoxCollider2D ColliderBox;

        [Space(4)]
        [Header("General settings")]
        public PlayerSettingsObject settings;
        public PlayerSettingsObject slowSettings;

        private PlayerSettingsObject curSettings;

        //Walking vars
        private float speedAccel;
        private Vector3 dir;
        
        //Jumping/Crouch vars
        private float initialHeight;
        private bool isJumping = false;
        private bool canJumpCoyote = false;
        private bool tryingToUncrouch;

        //Status
        public bool isWalking { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isSliding { get; private set; }
        public float VerticalVelocity { get; set; }
        public float CrouchPercent { get { return curSettings.CrouchPercent; } }
        //Sliding Vars
        private float currentSlidingVelocity = 0;
        private Vector3 currentSlidingDirection = Vector2.zero;


        private GameInput gameInput;

        public GameInput CurrentGameInput => gameInput;
        // Start is called before the first frame update
        void Start() {
            ColliderBox = GetComponent<BoxCollider2D>();
            initialHeight = ColliderBox.size.y;
            curSettings = settings;

            gameInput = MainInstances.Get<GameInput>();

        }

        public GameObject AimThing;

#pragma warning disable 414
        private Coroutine walkAccelerationCoroutine;
        private Coroutine walkDecelerationCoroutine;
#pragma warning restore 414
        
        private Coroutine jumpCoroutine;
        private Coroutine coyoteCoroutine;
        private bool WasGrounded = false;
        // Update is called once per frame
        void Update() {
            
            if (SlowMode) {
                curSettings = slowSettings;
            } else {
                curSettings = settings;
            }

            //Crouching

            if (CanActivateCrouch(true)) {
                //Check if we are on the first frame of control, if we are then we can slide. Otherwise you can infinite slide while trying to uncrouch under something.
                if ((gameInput.CrouchSlidePressed && !isCrouching)) {
                    Crouch();

                    //Slide activation point
                    if (PlayerCollider.isGrounded && (gameInput.GetLeft() ^ gameInput.GetRight() && CanActivateSlide())) {
                        isSliding = true;
                        currentSlidingVelocity = curSettings.slidingVelocity;

                        currentSlidingDirection = gameInput.GetLeft() ? Vector3.left : Vector3.right;
                    }
                }


            } else {
                if (Math.Abs(gameInput.Crouch) < FloatComparer.kEpsilon) {
                    UpdateCrouchSlide();
                }

            }


            PlayerCollider.UpdateCollisions();

            //Sliding

            if (!PlayerCollider.isGrounded || isSliding) {
                currentSlidingVelocity -= currentSlidingVelocity * (PlayerCollider.isGrounded ? curSettings.slidingVelocityDrag : curSettings.slidingAirDrag) * Time.deltaTime;

                //If we change directions break the slide momentum

                Vector3 changeDirection = currentSlidingDirection;

                bool checkHit = false;

                if (CanMove(true)) {
                    changeDirection = Vector3.left;

                    if (PlayerCollider.isCollidingLeft) {
                        checkHit = true;
                    }
                } else if (CanMove(false)) {
                    changeDirection = Vector3.right;

                    if (PlayerCollider.isCollidingRight) {
                        checkHit = true;
                    }
                }

                if (changeDirection != currentSlidingDirection || checkHit) {
                    isSliding = false;
                    currentSlidingVelocity = 0;
                    currentSlidingDirection = Vector3.zero;
                }
                if (currentSlidingVelocity - curSettings.slidingMinVelocity <= 0) {
                    isSliding = false;
                    currentSlidingDirection = Vector3.zero;
                } else {
                    PlayerCollider.Move(currentSlidingDirection * (currentSlidingVelocity * Time.deltaTime));
                }
            } else {
                currentSlidingVelocity = 0;
                currentSlidingDirection = Vector3.zero;
            }



            if ((isCrouching && !isSliding) || VerticalVelocity > 0) {
                PlayerCollider.PassthroughOneways = true;
            } else {
                PlayerCollider.PassthroughOneways = false;
            }

            //Walking
            bool wasWalking = isWalking;
            isWalking = false;

            Vector3 displacement = Vector3.zero;
            Vector3 newDir = Vector3.zero;
            Vector2 aimDir = gameInput.GetAimDir(UnityEngine.Camera.main.WorldToScreenPoint(this.transform.position));
            float absoluteAimDirX = aimDir.x;
            
            if (!PlayerCollider.isCollidingLeft && CanMove(true) && !(currentSlidingDirection == Vector3.left && isSliding)) {

                if (!PlayerCollider.isGrounded && currentSlidingVelocity > 0 && currentSlidingDirection == Vector3.left) {

                } else {
                    newDir += Vector3.left * gameInput.Left;
                    if(!FaceAim || absoluteAimDirX < FloatComparer.kEpsilon) sRenderer.flipX = true;
                    isWalking = true;
                }

            }

            if (!PlayerCollider.isCollidingRight && CanMove(false) && !(currentSlidingDirection == Vector3.right && isSliding)) {

                if (!PlayerCollider.isGrounded && currentSlidingVelocity > 0 && currentSlidingDirection == Vector3.right) {

                } else {
                    newDir += Vector3.right * gameInput.Right;
                    if(!FaceAim || absoluteAimDirX < FloatComparer.kEpsilon) sRenderer.flipX = false;
                    isWalking = true;
                }
            }
            
            /*
            if (wasWalking != isWalking) {
                if (wasWalking) {
                    if (WalkDecelerationCoroutine != null) {
                        StopCoroutine(WalkDecelerationCoroutine);
                    }
                    
                    if (WalkAccelerationCoroutine != null) {
                        StopCoroutine(WalkAccelerationCoroutine);
                    }

                    if (!isCrouching) {
                        WalkDecelerationCoroutine = StartCoroutine(WalkDecelerationCurve());
                    }
                } else {
                    if (WalkDecelerationCoroutine != null) {
                        StopCoroutine(WalkDecelerationCoroutine);
                    }
                    
                    if (WalkAccelerationCoroutine != null) {
                        StopCoroutine(WalkAccelerationCoroutine);
                    }

                    WalkAccelerationCoroutine = StartCoroutine(WalkAccelerationCurve());
                }
            }
            
            if (WalkDecelerationCoroutine != null || newDir != Vector3.zero) {
                displacement =
                    dir * ((isCrouching
                        ? curSettings.CrouchMovementSpeed
                        : (curSettings.HorizontalSpeed * speedAccel)) * Time.deltaTime);
            }
            */
            
            if (FaceAim) {
                if (absoluteAimDirX > FloatComparer.kEpsilon) {
                    sRenderer.flipX = false;
                } else if(absoluteAimDirX < -FloatComparer.kEpsilon){
                    sRenderer.flipX = true;
                }
            }
            
            displacement =
                newDir * ((isCrouching ? curSettings.CrouchMovementSpeed : curSettings.HorizontalSpeed) * Time.deltaTime);

            PlayerCollider.Move(displacement);


            //Jump physics
            if (WasGrounded && !PlayerCollider.isGrounded) {

                coyoteCoroutine = StartCoroutine(CoyoteJumpTimer());
                WasGrounded = false;

            } else if (!WasGrounded && PlayerCollider.isGrounded) {

                if (coyoteCoroutine != null) {
                    StopCoroutine(coyoteCoroutine);
                    coyoteCoroutine = null;
                }
            }

            WasGrounded = PlayerCollider.isGrounded;

            //Jump checking
            if (CanStartJump()) {

                Jump();

            }

            if (PlayerCollider.isCollidingTop && (isJumping || VerticalVelocity > 0)) {
                if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
                VerticalVelocity = VerticalVelocity * -curSettings.HeadBounce;
                isJumping = false;
                jumpCoroutine = null;
            }

            if (!CanActivateJump() && jumpCoroutine != null) {
                StopCoroutine(jumpCoroutine);
                isJumping = false;
                jumpCoroutine = null;
            }


            if (isJumping) {
                VerticalVelocity = (isCrouching ? curSettings.CrouchJumpVelocity : curSettings.JumpVelocity);
            } else {
                if (!PlayerCollider.isCollidingBottom) {
                    VerticalVelocity -= (isCrouching ? curSettings.CrouchGravity : curSettings.Gravity) * curSettings.Mass * Time.deltaTime;

                    VerticalVelocity = Mathf.Max(curSettings.GravityTerminalVelocity, VerticalVelocity);
                } else {
                    VerticalVelocity = 0;
                }
            }



            PlayerCollider.Move(Vector3.up * (VerticalVelocity * Time.deltaTime));

            var aimPos = gameInput.GetAimDir(UnityEngine.Camera.main.WorldToScreenPoint(this.transform.position));
            if(AimThing != null) AimThing.transform.position = this.transform.position + new Vector3(aimPos.x, aimPos.y);
            
            
        }

        public bool CanStartJump() {
            return CanActivateJump(true) && (PlayerCollider.isGrounded || canJumpCoyote) &&
                   !(tryingToUncrouch && !CanActivateCrouch());
        }
        public void Jump() {
            if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);

            jumpCoroutine = StartCoroutine(JumpHoldTimer());
        }

        public void UpdateCrouchSlide() {




            if (!tryingToUncrouch) {
                return;
            }


            Uncrouch();
            PlayerCollider.UpdateCollisions();

            if (PlayerCollider.isCollidingBottom && PlayerCollider.isCollidingTop) {
                this.transform.position -= new Vector3(0, (initialHeight - (initialHeight * curSettings.CrouchPercent)) * 0.5f);
                Crouch();
                PlayerCollider.UpdateCollisions();
            } else {
                tryingToUncrouch = false;
                isSliding = false;
            }
        }

        private void Crouch() {

            if (!isCrouching) {
                isJumping = false;
            }
            isCrouching = true;
            tryingToUncrouch = true;
            ColliderBox.size = new Vector2(ColliderBox.size.x, initialHeight * curSettings.CrouchPercent);

            //This raycast prevents the player from crouching through terrain thus getting stuck in terrain
            RaycastHit2D hit;
            hit = Physics2D.Raycast(this.transform.position, Vector2.down, initialHeight * curSettings.CrouchPercent * 0.5f, PlayerCollider.collisionMask);
            if (hit) {
                this.transform.position -= new Vector3(0, Math.Abs(this.transform.position.y - hit.point.y) * 0.5f);
            } else {
                this.transform.position -= new Vector3(0, (initialHeight - (initialHeight * curSettings.CrouchPercent)) * 0.5f);
            }

            this.PlayerCollider.UpdateCollisions();
            this.PlayerCollider.UpdateUnstucks();
        }

        private void Uncrouch() {
            isCrouching = false;
            ColliderBox.size = new Vector2(ColliderBox.size.x, initialHeight);
            //this.transform.position += new Vector3(0, (initialHeight - (initialHeight * curSettings.CrouchPercent)) * 0.5f);
            PlayerCollider.Move(new Vector3(0, (initialHeight - (initialHeight * curSettings.CrouchPercent)) * 0.45f));
        }


        public bool CanActivateCrouch(bool onDown = false) {
            return (onDown ? gameInput.CrouchSlidePressed : gameInput.GetCrouch()) && EnableCrouch;
        }

        public bool CanActivateSlide() {
            return gameInput.GetCrouch() && EnableSlide;
        }

        public bool CanActivateJump(bool onDown = false) {
            return (onDown ? gameInput.JumpPressed : gameInput.GetJump()) && EnableJump;
        }

        public bool CanMove(bool left) {
            return (left ? gameInput.GetLeft() : gameInput.GetRight()) && EnableMove;
        }


        public void SetEnableCrouch(bool value) {
            EnableCrouch = value;
        }
        public void SetEnableJump(bool value) {
            EnableJump = value;
        }
        public void SetEnableSlide(bool value) {
            EnableSlide = value;
        }
        public void SetSlowMode(bool value) {
            SlowMode = value;
        }

        public IEnumerator JumpHoldTimer() {
            isJumping = true;
            yield return new WaitForSeconds(curSettings.JumpHoldTime);
            isJumping = false;

            jumpCoroutine = null;
        }

        public IEnumerator CoyoteJumpTimer() {
            canJumpCoyote = true;
            yield return new WaitForSeconds(curSettings.CoyoteTime);
            canJumpCoyote = false;

            coyoteCoroutine = null;
        }

        public IEnumerator WalkDecelerationCurve() {
            float startTime = Time.time;
            while ((Time.time - startTime) / curSettings.DecelTime <= 1) {
                speedAccel = curSettings.MovementEndAccel.Evaluate((Time.time - startTime) / curSettings.DecelTime);
                yield return null;
            }

            walkDecelerationCoroutine = null;
        }
        
        public IEnumerator WalkAccelerationCurve() {
            float startTime = Time.time;
            while ((Time.time - startTime) / curSettings.AccelTime <= 1) {
                speedAccel = curSettings.MovementStartAccel.Evaluate((Time.time - startTime) / curSettings.AccelTime);
                yield return null;
            }

            walkAccelerationCoroutine = null;
        }
    }
}

        //JOBS BASED COLLISION DETECTION
        /// <summary>
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! WARNING DOES NOT WORK BECAUSE OF RAYCAST2D NOT BEING ABLE TO BE USED OVER THREADS D: 
        /// </summary>
        /*
        NativeArray<CollisionJobDataIn> j_collisionData;
        NativeArray<bool> j_hits;
        NativeArray<CollisionProperties> j_hitproperties;
        CollisionJob j_currentJob;
        JobHandle j_currentJobHandler;
        bool j_started = false;
        bool j_disposed = true;

        struct CollisionJobDataIn {

            public Vector2 spacingDir;
            public Vector2 rayDir;
            public Vector2 startPoint;
            public float rayLength;
            public float spacing;
            public int collisionMask;
        }

        struct CollisionJob : IJobParallelFor {

            [ReadOnly]
            public NativeArray<CollisionJobDataIn> dataArray;

            public NativeArray<bool> hits;
            public NativeArray<CollisionProperties> properties;

            [ReadOnly]
            public float widthAmount;
            [ReadOnly]
            public float heightAmount;

            public void Execute(int i) {

                CollisionJobDataIn data = dataArray[getJobDataIndex(i)];

                RaycastHit2D hit = Physics2D.Raycast(data.startPoint + (data.spacingDir * data.spacing * i), data.rayDir, data.rayLength, data.collisionMask);
                if (hit.collider != null) {
                    Debug.DrawRay(data.startPoint + (data.spacingDir * data.spacing * i), data.rayDir * data.rayLength, Color.green);
                    CollisionProperties properties = new CollisionProperties();
                    properties.collided = true;
                    properties.hit = hit;

                    this.hits[i] = true;
                    this.properties[i] = properties;
                } else {
                    this.hits[i] = false;
                    Debug.DrawRay(data.startPoint + (data.spacingDir * data.spacing * i), data.rayDir * data.rayLength, Color.red);
                }
            }


            private int getJobDataIndex(int i) {

                if(i >= 0 && i < widthAmount) {
                    return 0; //Top
                }else if(i >= widthAmount && i < (widthAmount * 2)) {
                    return 1; //Bottom
                } else if( i >= (widthAmount * 2) && i < (widthAmount * 2) + heightAmount) {
                    return 2; //Left
                } else if(i >= (widthAmount * 2) + heightAmount && i < (widthAmount * 2) + (heightAmount * 2)) {
                    return 3; //Right
                }else if(i >= (widthAmount * 2) + (heightAmount * 2)) { // i < (widthAmount*3) + (heightamount*2)
                    return 4; //Grounded
                }
                return -1;
            }
        }

        private CollisionJobDataIn jGenerateCollisionDataIn(Vector2 direction, Vector2 a, Vector2 b, int amount, float spacing, float rayLength) {
            Vector2 g_spacingDir = (b - a).normalized;

            return new CollisionJobDataIn() {
                spacingDir = g_spacingDir,
                rayDir = direction,
                startPoint = a,
                rayLength = rayLength,
                spacing = spacing,
                collisionMask = this.collisionMask
            };
        }


        private void jStartCollisionJobs() {
            UpdateRaycastPositions();


            //amount - 1

            //widthAmount
            //heightAmount

            //0 Top       = 0                             -> widthAmount
            //1 Bottom    = widthAmount                   -> widthAmount*2
            //2 Left      = widthAmount*2                 -> widthAmount*2 + heightAmount
            //3 Right      = widthAmount*2 + heightAmount  -> widthAmount*2 + heightAmount*2
            //4 IsGrounded = widthAmount*2 + heightAmount*2 -> widthAmount*3 + heightAmount*2
            j_started = true;

            if (!j_disposed) {
                disposeJob();
            }

            int totalAmount = (WidthRaycastsCount * 3) + (HeightRaycastsCount * 2);

            j_collisionData = new NativeArray<CollisionJobDataIn>(5, Allocator.TempJob);

            j_collisionData[0] = jGenerateCollisionDataIn(Vector2.up, raycastPositions.topLeft, raycastPositions.topRight, WidthRaycastsCount, WidthRaycastSpacing, SkinThickness);
            j_collisionData[1] = jGenerateCollisionDataIn(Vector2.down, raycastPositions.bottomLeft, raycastPositions.bottomRight, WidthRaycastsCount, WidthRaycastSpacing, SkinThickness);
            j_collisionData[2] = jGenerateCollisionDataIn(Vector2.left, raycastPositions.bottomLeft, raycastPositions.topLeft, HeightRaycastsCount, HeightRaycastSpacing, SkinThickness);
            j_collisionData[3] = jGenerateCollisionDataIn(Vector2.right, raycastPositions.bottomRight, raycastPositions.topRight, HeightRaycastsCount, HeightRaycastSpacing, SkinThickness);
            j_collisionData[4] = jGenerateCollisionDataIn(Vector2.down, raycastPositions.bottomLeft, raycastPositions.bottomRight, WidthRaycastsCount, WidthRaycastSpacing, SkinThickness + 0.05f);

            j_hits = new NativeArray<bool>(totalAmount, Allocator.TempJob);
            j_hitproperties = new NativeArray<CollisionProperties>(totalAmount, Allocator.TempJob);

            j_currentJob = new CollisionJob {
                dataArray = j_collisionData,
                hits = j_hits,
                properties = j_hitproperties
            };


            j_currentJobHandler = j_currentJob.Schedule(totalAmount, Mathf.Max(1, totalAmount / 2, 32));
            j_disposed = false;
        }


        private void jUpdateCollisionsJobs() {
            if (!j_started) {
                jStartCollisionJobs();
            }

            int widthAmount = WidthRaycastsCount;
            int heightAmount = WidthRaycastsCount;

            j_currentJobHandler.Complete();

            var topTest = jHasHit(0, widthAmount);
            var bottomTest = jHasHit(widthAmount, (widthAmount * 2));
            var leftTest = jHasHit((widthAmount * 2), (widthAmount * 2) + heightAmount);
            var rightTest = jHasHit((widthAmount * 2) + heightAmount, (widthAmount * 2) + (heightAmount * 2));

            isGrounded = jHasHit((widthAmount * 2) + (heightAmount * 2), (widthAmount * 3) + (heightAmount * 2)).collided;

            isCollidingTop = topTest.collided;
            isCollidingBottom = bottomTest.collided;
            isCollidingLeft = leftTest.collided;
            isCollidingRight = rightTest.collided;

            if(isCollidingTop) collisionTop = jGetHitProperty(topTest.index);
            if (isCollidingBottom) collisionBottom = jGetHitProperty(bottomTest.index);
            if (isCollidingLeft) collisionLeft = jGetHitProperty(leftTest.index);
            if (isCollidingRight) collisionRight = jGetHitProperty(rightTest.index);

            disposeJob();
        }


        private (bool collided, int index) jHasHit(int startIndex, int endIndex) {
            int i = startIndex;
            for(; i < endIndex; i++) {
                if (j_currentJob.hits[i]) {
                    return (true, i);
                }
            }

            return (false, -1);
        }

        private CollisionProperties jGetHitProperty(int i) {
            return j_currentJob.properties[i];
        }

        private void disposeJob() {
            j_hits.Dispose();
            j_hitproperties.Dispose();
            j_collisionData.Dispose();
            j_disposed = true;
        }


        private void LateUpdate() {
            if (UseJobs) jStartCollisionJobs();
        }


        private void OnDestroy() {
            disposeJob();
        }
        // Update is called once per frame
        void Update()
        {
            if (UseJobs) {
                jUpdateCollisionsJobs();
            } else {
                UpdateCollisions();
            }
            UpdateUnstucks();
        }
        */