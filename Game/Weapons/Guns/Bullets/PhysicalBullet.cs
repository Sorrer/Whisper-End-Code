using Game.HealthSystem;
using Game.Util;
using UnityEngine;

namespace Game.Weapons.Guns.Bullets {
    public class PhysicalBullet : Projectile
    {

        public float TimeAlive;
        public float Speed;
        public float HitRadius;
        public float Gravity;
        public LayerMask HitLayerMask;
        
        public Vector3 lastPos;
        private float curTimeAlive = 0;
        private float verticalVelocity = 0;

        public int Damage;
        public DamageType TypeDamage;
        
        public float RandomSpreadRadius = 0.1f;

        private Vector2 velocity;
        public override void InitProjectile(Gun gun, Muzzle muzzle) {
            base.InitProjectile(gun,muzzle);

            curTimeAlive = 0;

            this.transform.position = PoolParent.transform.position + Utility.Vec2ToVec3(Random.insideUnitCircle * RandomSpreadRadius);

            lastPos = this.transform.position;

            verticalVelocity = this.transform.forward.normalized.y * Speed;
            Vector3 shootDir = muzzle.transform.position - gun.transform.position;
            velocity = shootDir.normalized * Speed;
        }


        public void SetAngle(Vector3 dir) {
            velocity = dir.normalized * Speed;
        }


        public override void UpdateProjectile(float deltaTime) {

            velocity.y -= Gravity * deltaTime;
            
            transform.position = transform.position + Utility.Vec2ToVec3(velocity * deltaTime);


            this.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, this.transform.position - lastPos));
            
            
            
            Vector3 dir = transform.position - lastPos;

            RaycastHit2D hit2D = Physics2D.CircleCast(lastPos, HitRadius, dir.normalized, dir.magnitude, HitLayerMask);

            if (hit2D.collider) {
                
                var health = hit2D.transform.gameObject.GetComponent<HealthBase>();
                if (health != null) {
                    health.Damage(Damage, TypeDamage);
                }

                Hit(lastPos + Utility.Vec2ToVec3(hit2D.normal * this.HitRadius * 0.5f * HitRadius));
                return;
            }

            curTimeAlive += deltaTime;
            if (curTimeAlive > TimeAlive) {
                Hit(lastPos);
                return;
            } 
            
            
            
            
            
            lastPos = this.transform.position;
        }

        
        public void Hit(Vector3 position) {
            
            PoolParent.PoolObject(this);
            this.Muzzle.Remove(this);
            
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
