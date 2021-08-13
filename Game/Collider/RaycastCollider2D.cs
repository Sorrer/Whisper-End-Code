using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
namespace Game.Collider {

    public class RaycastCollider2D : MonoBehaviour {
        [HideInInspector]
        public RaycastPositions raycastPositions;
        private Collider2D PlayerCollider;

        public float SkinThickness;
        public int WidthRaycastsCount = 8;
        public int HeightRaycastsCount = 8;

        public LayerMask collisionMask;
        public LayerMask onewayColliderMask;
        private float WidthRaycastSpacing;
        private float HeightRaycastSpacing;
        public bool AutoUnstuck = true;

        public bool PassthroughOneways = false;

        //private bool UseJobs = false;

        public float Width { get; private set; }
        public float Height { get; private set; }

        public struct RaycastPositions {
            public Vector2 bottomLeft, bottomRight, topLeft, topRight;

            //Optional use
            public Vector2 trueBottomLeftEdge, trueBottomRightEdge;
        }

        void Start() {
            PlayerCollider = GetComponent<Collider2D>();
            UpdateRaycastPositions();

        }

        /// <summary>
        /// Used to get the bounds for the raycasting controller
        /// </summary>
        public void UpdateRaycastPositions() {
            raycastPositions = new RaycastPositions();
            Bounds bounds = PlayerCollider.bounds;

            raycastPositions.trueBottomLeftEdge = new Vector2(bounds.min.x, bounds.min.y);
            raycastPositions.trueBottomRightEdge = new Vector2(bounds.max.x, bounds.min.y);


            bounds.Expand(SkinThickness * -2);
            bounds.center = this.transform.position;

            raycastPositions.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastPositions.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastPositions.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastPositions.topRight = new Vector2(bounds.max.x, bounds.max.y);

            Width = Vector2.Distance(raycastPositions.bottomLeft, raycastPositions.bottomRight);
            Height = Vector2.Distance(raycastPositions.bottomLeft, raycastPositions.topLeft);

            WidthRaycastSpacing = Width / (WidthRaycastsCount - 1);
            HeightRaycastSpacing = Height / (HeightRaycastsCount - 1);
        }

        public bool isCollidingTop { get; private set; }
        public bool isCollidingBottom { get; private set; }
        public bool isGrounded { get; private set; }
        public bool isCollidingLeft { get; private set; }
        public bool isCollidingRight { get; private set; }

        private CollisionProperties collisionBottom;
        private CollisionProperties collisionTop;
        private CollisionProperties collisionLeft;
        private CollisionProperties collisionRight;

        private Vector3 LastPosition;

        /// <summary>
        /// Used to update collision booleans
        /// </summary>
        public void UpdateCollisions() {



            LastPosition = this.transform.position;

            UpdateRaycastPositions();

            //Top collision detection and rebounds

            collisionTop = CheckCollision(Vector2.up, raycastPositions.topLeft, raycastPositions.topRight, WidthRaycastsCount, WidthRaycastSpacing, SkinThickness);
            collisionBottom = CheckCollision(Vector2.down, raycastPositions.bottomLeft, raycastPositions.bottomRight, WidthRaycastsCount, WidthRaycastSpacing, SkinThickness);
            collisionLeft = CheckCollision(Vector2.left, raycastPositions.bottomLeft, raycastPositions.topLeft, HeightRaycastsCount, HeightRaycastSpacing, SkinThickness);
            collisionRight = CheckCollision(Vector2.right, raycastPositions.bottomRight, raycastPositions.topRight, HeightRaycastsCount, HeightRaycastSpacing, SkinThickness);



            isGrounded = CheckCollision(Vector2.down, raycastPositions.bottomLeft, raycastPositions.bottomRight, WidthRaycastsCount, WidthRaycastSpacing, SkinThickness + 0.075f).collided;

            isCollidingTop = collisionTop.collided;
            isCollidingBottom = collisionBottom.collided;
            isCollidingLeft = collisionLeft.collided;
            isCollidingRight = collisionRight.collided;
        }

        public void UpdateUnstucks() {
            if (isCollidingTop) Unstuck(collisionTop.hit, new Vector3(0, -1));
            if (isCollidingBottom) Unstuck(collisionBottom.hit, new Vector3(0, 1));
            if (isCollidingLeft) Unstuck(collisionLeft.hit, new Vector3(1, 0));
            if (isCollidingRight) Unstuck(collisionRight.hit, new Vector3(-1, 0));
        }

        public float MinStepDistance = 0.075f;

        /// <summary>
        /// This should be used after moving a large distance to make sure you didn't collide into a wall or anything
        /// </summary>
        public void Move(Vector3 Displacement) {

            if (MinStepDistance <= 0.0001) {
                this.transform.position += Displacement;
                return;
            }

            float magnitude = Displacement.magnitude;
            Vector3 direction = Displacement / magnitude;


            while (magnitude > MinStepDistance) {
                this.transform.position += MinStepDistance * direction;
                magnitude -= MinStepDistance;

                UpdateCollisions();
                UpdateUnstucks();
            }



            if (magnitude > 0) {
                //Do the remaining magnitude
                this.transform.position += magnitude * direction;
                UpdateCollisions();
                UpdateUnstucks();
            }


        }


        /// <summary>
        /// Shoots multiple rays out to check collisions along one line with a direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amount"></param>
        /// <param name="spacing"></param>
        /// <returns>Data of what happen with collision</returns>
        private CollisionProperties CheckCollision(Vector2 direction, Vector2 a, Vector2 b, int amount, float spacing, float rayLength) {
            Vector2 spacingDir = (b - a).normalized;
            CollisionProperties properties = new CollisionProperties();
            properties.collided = false;

            for (int i = 0; i < amount; i++) {
                RaycastHit2D hit = Physics2D.Raycast(a + (spacingDir * spacing * i), direction, rayLength, collisionMask);
                if (hit.collider != null && (!(hit.collider.gameObject.tag == "Oneway Platform" && direction != Vector2.down) && !(hit.collider.gameObject.tag == "Oneway Platform" && PassthroughOneways))) {

                    //Debug.DrawRay(a + (spacingDir * spacing * i), direction * rayLength, Color.green);
                    properties.collided = true;
                    properties.hit = hit;
                    return properties;
                } else {
                    //Debug.DrawRay(a + (spacingDir * spacing * i), direction * rayLength, Color.red);
                }
            }

            return properties;
        }


        private void Unstuck(RaycastHit2D hit2D, Vector3 direction) {
            if (hit2D.distance < SkinThickness) {
                float offset = SkinThickness - hit2D.distance;
                this.transform.position += offset * direction;
            }
        }

        private struct CollisionProperties {
            public bool collided;
            public RaycastHit2D hit;

        }


        void Update() {
            UpdateCollisions();
            if(AutoUnstuck)UpdateUnstucks();
        }

    }
}