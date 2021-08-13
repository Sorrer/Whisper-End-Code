using System.Collections.Generic;
using System.Linq;
using Game.HealthSystem;
using Game.Util;
using UnityEngine;

namespace Game.Weapons.Guns {
    public class HitscanBullet : Projectile {

        public int projectileMaxBounces = 0;
        public int maxShotRange = 50;
        public float decayTime;
        public float recessionSpeed;

        private int layerMask;
        private LineRenderer lr;
        private float startTime;
        private float curTime;
        //private int nextPt = 2;

        private void Awake() {  
            layerMask = LayerMask.GetMask("Default");
            lr = gameObject.GetComponent<LineRenderer>();
        }

        public override void UpdateProjectile(float deltaTime) {
            curTime += deltaTime;

            /* TODO -
            var back = lr.GetPosition(0);
            var next = lr.GetPosition(1);
            lr.SetPosition(0, back + (next - back) * (recessionSpeed * deltaTime));

            if (Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1)) <= recessionSpeed) {
                if (lr.positionCount < nextPt) {
                    poolParent.PoolObject(this);
                    Muzzle.Remove(this);
                    return;
                }
                lr.SetPosition(1, lr.GetPosition(nextPt));
                nextPt++;
            }
            */
            
            var back = lr.GetPosition(0);
            var next = lr.GetPosition(1);
            lr.SetPosition(0, Vector3.Lerp(back, next, (curTime - startTime) / decayTime));
            
            if (curTime - startTime >= decayTime) {
                PoolParent.PoolObject(this);
                Muzzle.Remove(this);
            }
        }

        public override void InitProjectile(Gun gun, Muzzle muzzle) {
            base.InitProjectile(gun,muzzle);
            startTime = curTime = Time.time;
            
            List<Vector2> hits = new List<Vector2>();
            
            Vector2 dir = PoolParent.transform.right.normalized;
            Vector2 pos = PoolParent.transform.position;

            for (var i = 0; i < projectileMaxBounces + 1; i++) {
                var hit = Physics2D.Raycast(pos, dir, maxShotRange, layerMask);
                if (!hit.collider) { // Shot hit open air
                    hits.Add(pos + dir * maxShotRange);
                    break;
                }

                var hm = hit.collider.GetComponent<HealthManager>();
                if (hm) {
                    hm.Damage(Mathf.RoundToInt(GunUtils.GetStat(Gun, GunStat.DAMAGE)));
                    break;
                }
                
                hits.Add(hit.point);
                
                // New dir is reflected off the normal of the surface hit
                dir = Vector2.Reflect(dir, hit.normal).normalized; 
                
                // New pos is end of last raycast + a small offset based off the normal to account for ray ending slightly inside a surface
                pos = hit.point + hit.normal * 0.1f; 
            }

            var posList = hits.Select(Utility.Vec2ToVec3).ToList();
            posList.Insert(0, PoolParent.transform.position); // Insert muzzle position as first point
            lr.positionCount = posList.Count;
            lr.SetPositions(posList.ToArray());
        }

        public override void Pool() {
            gameObject.SetActive(false);
        }  

        public override void Unpool() {
            gameObject.SetActive(true);
        }

        public override void Clean() {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }
    }
}