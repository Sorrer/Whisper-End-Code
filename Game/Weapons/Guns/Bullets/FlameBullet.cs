using Game.HealthSystem;
using Game.Util;
using UnityEngine;

namespace Game.Weapons.Guns.Bullets {
    public class FlameBullet : Projectile
    {
        
        public float TimeAlive;
        public float Speed;
        public float HitRadius;
        public float Gravity;
        public LayerMask HitLayerMask;
        
        
        public Vector3 lastPos;
        private float curTimeAlive = 0;
        private float verticalVelocity = 0;


        public float RandomSpreadRadius = 0.1f;
        
        private Transform hitTransform;
        private Vector3 offset;
        
        public int Damage;
        public DamageType TypeDamage;
        
        private Vector2 velocity;
        
        public override void InitProjectile(Gun gun, Muzzle muzzle) {
            base.InitProjectile(gun, muzzle);
            
            curTimeAlive = 0;

            //this.transform.forward = gun.transform.forward;
            this.transform.position = PoolParent.transform.position + Utility.Vec2ToVec3(Random.insideUnitCircle * RandomSpreadRadius);
            
            lastPos = this.transform.position;

            verticalVelocity = this.transform.forward.normalized.y * Speed;
            Vector3 shootDir = muzzle.transform.position - gun.transform.position;
            velocity = shootDir.normalized * Speed;
            
            
            
        }

        public override void UpdateProjectile(float deltaTime) {
            
            
            velocity.y -= Gravity * deltaTime;
            
            transform.position = transform.position + Utility.Vec2ToVec3(velocity * deltaTime);

            
            
            
            
            Vector3 dir = transform.position - lastPos;

            RaycastHit2D hit2D = Physics2D.CircleCast(lastPos, HitRadius, dir.normalized, dir.magnitude, HitLayerMask);

            if (hit2D.collider) {
                
                var health = hit2D.transform.gameObject.GetComponent<HealthBase>();
                if (health != null) {
                    health.Damage(Damage, TypeDamage);
                }
                
                return;
            }

            curTimeAlive += deltaTime;
            if (curTimeAlive > TimeAlive) {
                return;
            } 
            
            
            
            
            
            lastPos = this.transform.position;
        }

        public override void Pool() {
            this.gameObject.SetActive(false);
        }

        public override void Unpool() {
            this.gameObject.SetActive(true);
        }

        public override void Clean() {
        }
    }
}
