using System.Collections;
using Game.Collider;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Game.AI.AI {
    public class ScrapGroundRobotBehaviour : MonoBehaviour {

        public enum State {
            ATTACKPREWARM, ATTACK, ATTACKSWING, UPPERCUT, MOVING, IDLE, REST
        }

        private State currentState = State.MOVING;

        public GameObject swingObject;
        public AnimationClip swingClip;
        public Animator animator;

        public RaycastCollider2D raycastCollider;
        public NavmapAgent agent;

        public SpriteRenderer sRenderer;
        
        public Transform Target;

        public float AttackRange;
        public float UpperCutRange;


        public float AttackLungeSpeed = 5;

        public float AttackRangeTime;
        public float UpperCutTime;

        public float RestTimer = 0;
        
        
        public float AttackRangeCooldown;
        public float UpperCutCooldown;

        private float curAttackRangeCooldown;
        private float curUpperCutCooldown;

        private float curAttackTime;
        private float curUpperCutTime;
        
        private float curRestTime;

        private float attackWarmUp;

        private float timer;
        // Start is called before the first frame update
        void Start() {
            agent.Target = Target;
        }

        // Update is called once per frame
        void Update() {

            curUpperCutCooldown -= Time.deltaTime;
            curAttackRangeCooldown -= Time.deltaTime;
            curRestTime -= Time.deltaTime;
            curUpperCutTime -= Time.deltaTime;
            curAttackTime -= Time.deltaTime;
            
            if (curAttackRangeCooldown < 0) curAttackRangeCooldown = 0;
            if (curUpperCutCooldown < 0) curUpperCutCooldown = 0;
            if (curRestTime < 0) curRestTime = 0;
            if (curUpperCutTime < 0) curUpperCutTime = 0;
            if (curAttackTime < 0) curAttackTime = 0;
            
            
            switch (currentState) {
                
                case State.MOVING:
                    sRenderer.color = Color.white;
                    sRenderer.flipX = this.transform.position.x > agent.Target.position.x;
                    Vector3 dis = (this.transform.position - Target.position);
                    dis.z = 0;
                    float targetDistance = dis.magnitude;
                    
                    if (targetDistance < UpperCutRange) {
                        UpperCut();
                        break;
                    }
                    
                    
                    if (targetDistance < AttackRange && curAttackRangeCooldown <= 0) {
                        Attack();
                        break;
                    }


                    break;
                case State.ATTACKPREWARM:
                    sRenderer.color = Color.red;
                    sRenderer.flipX = this.transform.position.x > agent.Target.position.x;
                    attackWarmUp -= Time.deltaTime;
                    if (attackWarmUp <= 0) currentState = State.ATTACK;
                    
                    break;
                case State.ATTACK:
                    sRenderer.color = Color.green;
                    int dir = sRenderer.flipX ? -1 : 1; //True = Left, false = Right


                    raycastCollider.Move(Time.deltaTime * dir * AttackLungeSpeed * Vector2.right);

                    if (dir == -1 ? this.transform.position.x - Target.transform.position.x < 1 : this.transform.position.x - Target.transform.position.x > -1) {
                        AttackSwing();
                    }
                    
                    if (curAttackTime <= 0) {
                        MoveToTarget();
                    }
                    break;
                
                case State.ATTACKSWING:
                    
                    //Play swing Animation
                    //Update Collider Damager Box/SphereCase
                    //Wait until swing animation is done
                    
                    
                    

                    break;
                
                case State.UPPERCUT:
                    MoveToTarget();
                    
                    break;
                
            }
            
            
            
            
            
        }

        public IEnumerator SwingEnumerator(bool restAfter) {
            sRenderer.color = Color.red;
            
            yield return new WaitForSeconds(timer);
            sRenderer.color = Color.blue;
            swingObject.SetActive(true);
            swingObject.transform.localScale = new Vector3(sRenderer.flipX ? 1 : -1, 1, 1);
            this.currentState = State.ATTACKSWING;
            
            animator.Play("Base Layer.Swing");
            
            //Rest time + Swing Time
            yield return new WaitForSeconds(swingClip.length);
            
            swingObject.SetActive(false);
            
            if(restAfter)yield return new WaitForSeconds(RestTimer);
            sRenderer.color = Color.red;
            
            MoveToTarget();
        }


        public void AttackSwing(bool restAfter = true) {
            agent.Move = false;
            StartCoroutine(SwingEnumerator(restAfter));
        }

        public void Attack() {
            timer = 0;
            Debug.Log("Lunging");
            agent.Move = false;
            curAttackRangeCooldown = AttackRangeCooldown;
            curAttackTime = AttackRangeTime;
            this.currentState = State.ATTACKPREWARM;
            attackWarmUp = 0.35f;
        }

        public void UpperCut() {
            timer = 0.15f;
            agent.Move = false;
            curUpperCutCooldown = UpperCutCooldown;
            curUpperCutTime = UpperCutTime;
            AttackSwing(false);
        }

        public void MoveToTarget() {
            agent.Move = true;
            this.currentState = State.MOVING;
            
        }
    }
}
