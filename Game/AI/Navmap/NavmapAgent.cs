using System;
using System.Collections.Generic;
using Game.AI.Navmap;
using Game.AI.Navmap.Nodes;
using UnityEngine;
using Game.Collider;






public class NavmapAgent : MonoBehaviour {

    public Transform Target;
    
    private Transform currentTarget;
    private Transform lastTarget;

    private Navmap2DPathfinder.Path currentPath;

    public Navmap2D navmap;
    public float Speed = 6;
    public float StoppingDistance = 0.15f;
    public float Gravity = 9.81f;
    public float MinJumpHeight = 0.5f;
    public float MaxJumpVelocity = 25.5f;

    RaycastCollider2D rayCollider;


    private bool canJump = false;
    
    public bool Move = true;
    public bool ApplyVerticalSpeed = true;
    
    private void Start() {
        rayCollider = GetComponent<RaycastCollider2D>();
        
    }

    private float verticalVelocity;
    
    private void Update() {
        currentPath = Navmap2DPathfinder.FindPath(navmap, this.transform.position, Target.transform.position);
        
        if (currentPath.nodes.Count == 0) return;
        if (currentTarget == null) {
            currentTarget = currentPath.nodes.Pop().transform;
            if (currentTarget == lastTarget && currentPath.nodes.Count > 0) {
                currentTarget = currentPath.nodes.Pop().transform;
            }
        }

        canJump = true;
        if (currentPath.nodes.Count <= 1) {
            currentTarget = Target.transform;
            canJump = false;
        }

        if(Move) MoveTowardsTarget();

        if (ApplyVerticalSpeed) {
            verticalVelocity -= Gravity * Time.deltaTime;
        }
        
        
        rayCollider.Move(Vector3.up * (verticalVelocity * Time.deltaTime));

        if (rayCollider.isGrounded) verticalVelocity = -Gravity * Time.deltaTime;


    }
    
    private void OnDrawGizmos() {
        if (currentPath.nodes == null) return;
        
        Stack<Navmap2DNode> navmapNodes = new Stack<Navmap2DNode>();
        
        
        
        
        Vector3 lastPos = this.transform.position;
        Vector3 position;
        
        if (currentTarget != null) {
            Gizmos.color = Color.green;
            position = currentTarget.transform.position;
            Gizmos.DrawWireSphere(position, 0.25f);
            Gizmos.DrawLine(lastPos, position);
            lastPos = position;
        }
        



        Gizmos.color = Color.magenta;
        while (currentPath.nodes.Count > 0) {
            var node = currentPath.nodes.Pop();
            navmapNodes.Push(node);

            position = node.transform.position;
            Gizmos.DrawWireSphere(position, 0.25f);
            Gizmos.DrawLine(lastPos, position);

            lastPos = position;
        }

        while (navmapNodes.Count > 0) {
            currentPath.nodes.Push(navmapNodes.Pop());
        }
        
    }


    public void MoveTowardsTarget() {
        
        if (currentTarget == null) {
            return;
        }

        IsMovingRight = false;
        IsMovingLeft = false;
        float distance = currentTarget.transform.position.x - this.transform.position.x;
        if (distance > 0) {
            MoveRight(Mathf.Abs(distance));
            IsMovingRight = true;
        } else {
            MoveLeft(Mathf.Abs(distance));
            IsMovingLeft = true;
        }

        if (rayCollider.isGrounded && currentTarget.transform.position.y > this.transform.position.y + MinJumpHeight) {
            Jump(currentTarget.transform.position.y - this.transform.position.y);
        }
        
        if (Vector3.Distance(this.transform.position, currentTarget.transform.position) < StoppingDistance) {
            Debug.Log("Within distance, next target");
            lastTarget = currentTarget;
            currentTarget = null;
        }
    }

    public void Jump(float height) {
        if (!canJump) return;
        verticalVelocity = CalculateJumpVelocity(height) * (height < 1.25f ? 1.65f : 1.3f);
        Debug.Log("Jumped " + verticalVelocity + " " + height);
    }

    public bool IsMovingLeft;
    public bool IsMovingRight;
    
    public void MoveLeft(float distance) {
        if (distance < StoppingDistance) {
            rayCollider.Move(Vector3.left * (Time.deltaTime * Speed * (distance/StoppingDistance)));
        } else {
            rayCollider.Move(Vector3.left * (Time.deltaTime * Speed));
        }
    }

    public void MoveRight(float distance) {
        if (distance < StoppingDistance) {
            rayCollider.Move(Vector3.right * (Time.deltaTime * Speed * (distance/StoppingDistance)));
        } else {
            rayCollider.Move(Vector3.right * (Time.deltaTime * Speed));
        }
    }

    public float CalculateJumpVelocity(float height) {
        return Mathf.Sqrt(2 * Gravity * height);
    }
}



/*
namespace Game.AI.Navmap {
    public class NavmapAgent : MonoBehaviour {


        public Transform Target;

        public float Speed;
        public float StoppingDistance;

        public float Gravity = 9.81f;

        public float MinJumpHeight= 0.5f;
        public float MaxJumpVelocity = 5.5f;

        public Navmap2D Navmap;
        private Navmap2DPath path;
        
        public List<Ellipse> Ellipses = new List<Ellipse>();
        [Serializable]
        public struct Ellipse {
            public float offsetX, offsetY;
            public float a, b;
        }
        
        public RaycastCollider2D raycastCollider2D;
        // Start is called before the first frame update
        void Awake() {
            path = Navmap.FindPath(this.transform.position, Target.position);
            raycastCollider2D = this.GetComponent<RaycastCollider2D>(); 
            Target.position = Navmap.Pathmap.CellToWorld(path.path.Pop().position) + Navmap.offset;
        }

        [ContextMenu("Find path")]
        public void FindPath() {
            path = Navmap.FindPath(this.transform.position, Target.position);
        }


        private float verticalVelocity = 0;
        private Vector3 direction = Vector3.zero;
        private float groundedVelocityMax = -1;
        // Update is called once per frame
        void Update() {
            
            
            direction = Target.transform.position - this.transform.position;
            float distance = direction.magnitude;
            direction /= distance;


            if (distance < StoppingDistance) {
                //return;
            }

            
            if (Target.transform.position.y > this.transform.position.y + MinJumpHeight && raycastCollider2D.isGrounded) {
                bool withinRange = false;

                for (int i = 0; i < Ellipses.Count; i++) {
                    var elip = Ellipses[i];
                    if(WithinEllipse(Target.transform.position.x, Target.transform.position.y, 
                        this.transform.position.x + elip.offsetX, this.transform.position.y + elip.offsetY, 
                        elip.a, elip.b) <= 1)
                    {
                        Debug.Log("Point within elipse!");
                        withinRange = true;
                        break;
                    }
                }

                if (withinRange) {
                    float velocity = CalculateJumpVelocity((Target.transform.position.y - this.transform.position.y) + 2.0f);
                    if (velocity <= MaxJumpVelocity) {
                        verticalVelocity = velocity;
                    }
                }
            }

            if (Vector3.Distance(this.transform.position, Target.position) < 1.1f) {
                Debug.Log("Next Point!");
                Target.position = Navmap.Pathmap.CellToWorld(path.path.Pop().position) + Navmap.offset;
            }
            
            verticalVelocity -= Gravity * Time.deltaTime;
            
            if (verticalVelocity <= groundedVelocityMax && raycastCollider2D.isGrounded) {
                verticalVelocity = groundedVelocityMax;
            }

            direction.y = 0;
            raycastCollider2D.Move(direction * (Time.deltaTime * Speed) + Vector3.up * verticalVelocity * Time.deltaTime);
            
            
            
        }

        public float CalculateJumpVelocity(float height) {
            return Mathf.Sqrt(Gravity * height * 2);
        }

        public int WithinEllipse(float x, float y, float h, float k, float a, float b) {
            
            return ((int)Math.Pow((x - h), 2) / (int)Math.Pow(a, 2)) + ((int)Math.Pow((y - k), 2)/ (int)Math.Pow(b,2));

        }
        
        public void OnDrawGizmos() {
            return;
            if (path != null) {
            
                bool first = true;
                Vector3 lastPos = Vector3Int.zero;

                var newStack = new Stack<Navmap2D.MapNode>();

                while(path.path.Count > 0) {
                    var node = path.path.Pop();

                    var currPos = Navmap.Pathmap.CellToWorld(node.position);


                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(currPos + Navmap.offset, 0.25f);
                    if (!first) {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(currPos + Navmap.offset, lastPos + Navmap.offset);
                    }


                    first = false;

                    lastPos = currPos;


                    newStack.Push(node);
                }


                while(newStack.Count > 0) {
                    path.path.Push(newStack.Pop());
                }
            }
            
            
            return;
            
            bool canReach = true;
            float maxJumpHeight =
                    CalculateMaxJumpHeight(MaxJumpVelocity, Gravity);
                float maxJumpWidth = GetMaxJumpWidth(Speed, MaxJumpVelocity,
                    Gravity
                );

                int maxX = Mathf.CeilToInt(maxJumpWidth);
                int maxY = Mathf.CeilToInt(maxJumpHeight);
                
                Vector3 a = this.transform.position;
                Vector3 b = Target.transform.position;

                Vector3 dir = b - a;
                //Out of reach?
                if (dir.y > maxY || dir.x > maxX) {
                    canReach = false;
                }
                
                //Raycast trajectory

                Vector3 fromPos = a;
                Vector3 curPos = fromPos;
                Vector3 toPos = b;
                if (fromPos.x == toPos.x) {
                    canReach = false;
                }
                float horizontalTime = GetHorizontalTime(Mathf.Abs(toPos.x - fromPos.x), Speed);
                
                float currentVerrVelocity = GetInitalYVelocity( (toPos.y - fromPos.y) * 2, Gravity, horizontalTime);
                if (currentVerrVelocity > MaxJumpVelocity) canReach = false;
                float timeStep = 0.012f;

                Vector3 horizontalDirection;
                
                if (dir.x > 0) {
                    horizontalDirection = Vector3.right;
                } else {
                    horizontalDirection = Vector3.left;
                }

                horizontalDirection *= Speed * timeStep;

                float gravityStep = Gravity * timeStep;
                Debug.Log("Checking new reach");

                int maxIterations = 100;
                int curIterations = 0;
                //Check if trajectory will hit
                while (canReach && curIterations < maxIterations) {
                    curIterations++;
                    // yield return null;
                    Debug.Log("Checking reach: " + horizontalTime + " " + curPos + " " + currentVerrVelocity + " " + toPos + "   |   " + gravityStep + " " + timeStep + " ");
                    
                    if (float.IsNaN(currentVerrVelocity) || float.IsInfinity(currentVerrVelocity)) {
                        Debug.Log("ExitHere");
                        canReach = false;
                        break;
                    }
                    if (currentVerrVelocity < 0 && toPos.y > curPos.y) {
                        Debug.Log("ExitHere 2");
                        canReach = false;
                        break;
                    }
                    
                    
                    
                    currentVerrVelocity -= gravityStep;
                    Vector3 lastPos = curPos;
                    curPos += (Vector3.up * (currentVerrVelocity * timeStep));

                    if (Mathf.Abs(curPos.x - toPos.x) > 0.1f) {
                        curPos += horizontalDirection;
                    }
                    
                    Vector3 curDir = curPos - lastPos;
                    float mag = curDir.magnitude;
                    curDir /= mag;
                    
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(lastPos, curPos);
                    
                    if (Physics2D.Raycast(lastPos, curDir, mag, this.raycastCollider2D.collisionMask)) {
                        canReach = false;
                        Debug.Log("ExitHere 3");
                        break;
                    }

                    if ((curPos - toPos).magnitude < 1f) {
                        Debug.Log("ExitHere 4");
                        break;
                    }
                }
            
            
            
            
            
            int segments = 30;
            Vector3 pos = this.transform.position;

            float currentVerVelocity = this.verticalVelocity;

            Gizmos.color = Color.green;
            for (int i = 0; i < segments; i++) {
                Vector3 lastPos = pos;
                pos += this.Speed * Time.fixedDeltaTime * direction;
                pos += currentVerVelocity * Time.fixedDeltaTime * Vector3.up;
                currentVerVelocity -= Time.fixedDeltaTime * Gravity;
                
                
                Gizmos.DrawLine(lastPos, pos);
            }
            Gizmos.DrawLine(this.transform.position, Target.position);
        }
        
        
        public static float CalculateMaxJumpHeight(float maxVelocity, float gravity) {
            return Mathf.Pow(maxVelocity, 2) / (2 * gravity);
        }

        public static float GetMaxJumpWidth(float speed, float maxVelocity, float gravity) {
            return speed * ((2 * maxVelocity)/gravity);
        }
    
        public static float CalculateJumpVelocity(float gravity, float height) {
            return Mathf.Sqrt(gravity * height * 2);
        }

        public static float GetHorizontalTime(float distance, float speed) {
            return distance / speed;
        }

        public static float GetInitalYVelocity(float yDistance, float gravity, float time) {
            return (yDistance - (0.5f * gravity * Mathf.Pow(time, 2)))/time;
        }
    }
    
    
}
//GetHorizontalTime
//GetInitalYVelocity*/