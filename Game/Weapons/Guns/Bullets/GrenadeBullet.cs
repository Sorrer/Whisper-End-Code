using Game.HealthSystem;
using Game.Util;
using UnityEngine;

namespace Game.Weapons.Guns.Bullets {
    public class GrenadeBullet : Projectile {

        public float maxTimeAlive;
        public float speed;
        public float hitRadius;
        public float gravity;
        public LayerMask hitLayerMask;
        
        public Vector3 lastPos;
        private float curTimeAlive = 0;

        public int damage;
        public DamageType typeDamage;
        
        private Vector2 velocity;
        public override void InitProjectile(Gun gun, Muzzle muzzle) {
            base.InitProjectile(gun,muzzle);

            curTimeAlive = 0;

            //this.transform.forward = gun.transform.forward;
            lastPos = transform.position = PoolParent.transform.position;

            var shootDir = muzzle.transform.right;
            velocity = shootDir.normalized * speed;
            explodeOnHit = true;
        }


        private void SetAngle(Vector3 dir) {
            velocity = dir.normalized * speed;
        }


        public override void UpdateProjectile(float deltaTime) {

            velocity.y -= gravity * deltaTime;
            
            transform.position += Utility.Vec2ToVec3(velocity * deltaTime);

            Vector3 dir = transform.position - lastPos;

            RaycastHit2D hit2D = Physics2D.CircleCast(lastPos, hitRadius, dir.normalized, dir.magnitude, hitLayerMask);

            if (hit2D.collider) {
                
                var health = hit2D.transform.gameObject.GetComponent<HealthBase>();
                if (health != null) {
                    health.Damage(damage, typeDamage);
                }
                Explode(lastPos + Utility.Vec2ToVec3(hit2D.normal * hitRadius * 0.25f));
                
                return;
            }

            curTimeAlive += deltaTime;
            if (curTimeAlive > maxTimeAlive) {
                Explode(transform.position);
                return;
            } 
            
            lastPos = transform.position;
        }

        public bool explodeOnHit = true;

        private void Explode(Vector3 position) {
            PoolParent.PoolObject(this);
            Muzzle.Remove(this);
            
            if (!explodeOnHit) return;

            float currentAngle = 0;
            float angleOffset = 360 / 20f;
            for (int i = 0; i < 20; i++) {
                currentAngle += angleOffset;

                Muzzle.Track(out var projectile);

                var grenadeBullet = projectile as GrenadeBullet;

                grenadeBullet.explodeOnHit = false;
                grenadeBullet.transform.position = position;
                grenadeBullet.lastPos = position;
                grenadeBullet.SetAngle(Quaternion.Euler(0,0,currentAngle) * Vector3.up);
            }
        }

        public override void Pool() {
            gameObject.SetActive(false);
        }

        public override void Unpool() {
            gameObject.SetActive(true);
        }

        public override void Clean() {
        }
    }
}
