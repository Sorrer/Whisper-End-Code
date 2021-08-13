using Game.Input;
using UnityEngine;

namespace Game.Player {
    public class PlayerGunAnimator : MonoBehaviour {

        public Animator animator;
        public PlayerController controller;

        public GameObject spriteObject;

        public BoxCollider2D boxCollider;

        public int aimingSegments;

        public SpriteRenderer sr;

        public Hands hands;

        private GameInput gi;
        
        // Start is called before the first frame update
        void Start() {
            gi = MainInstances.Get<GameInput>();
        }
        
        // Update is called once per frame
        void Update() {
            var aimVec = gi.GetAimDir(UnityEngine.Camera.main.WorldToScreenPoint(transform.position));
            var theta = Mathf.Atan2(aimVec.y, aimVec.x);
            theta = theta > 0 ? theta : Mathf.PI * 2 + theta;
            var angle = Mathf.FloorToInt(Mathf.Abs((theta * aimingSegments - Mathf.PI) / (2 * Mathf.PI) + 1)) % 8;
            switch (angle) {
                case 3:
                    angle = 1;
                    break;
                case 4:
                    angle = 0;
                    break;
                case 5:
                    angle = 7;
                    break;
            }
            
            animator.SetInteger("Angle", angle);
            //sr.flipX = aimVec.x < 0;
            
            animator.Update(0);
            animator.Update(0);
        }

    }
}