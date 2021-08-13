using System;
using System.Collections.Generic;
using Cinemachine;
using Game.Registry;
using Game.Registry.Objects;
using Game.Weapons.Guns;
using UnityEngine;

namespace Game.Weapons {
    public class Muzzle: PoolBehaviour, IPoolable {
        public Gun gun;
        public bool active = true;

        public PoolBehaviour PoolParent { get; set; }

        private ItemRegistry gunRegistry;
        private ItemProperties gunProperties;
        
        private readonly List<Projectile> projectiles = new List<Projectile>();
        private readonly List<Projectile> removeProjectiles = new List<Projectile>();
        private readonly List<Projectile> addProjectiles = new List<Projectile>();

        private CameraShaker camShaker;
        private ParticleSystem shootingParticles;
        
        //TODO - temp
        public float[] defShakeParams = {1f, 1f, .1f};
        
        int layerMask;

        private void Start() {
            camShaker = UnityEngine.Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera
                .VirtualCameraGameObject.GetComponentInChildren<CameraShaker>();

            shootingParticles = GetComponent<ParticleSystem>();
        }

        public void Shoot() {
            if (gun.ammo > 0 || gun.ammo == -1) { // -1 = infinity
                var newProj = (Projectile) RetrieveObject();
                newProj.PoolParent = this;
                newProj.InitProjectile(gun, this);
                addProjectiles.Add(newProj);
                
                camShaker.AddShake(defShakeParams[0], defShakeParams[1], defShakeParams[2]);
                shootingParticles.Play();
                
                if (gun.ammo > 0){
                    gun.ammo -= 1;
                }
            }
            
        }
        
        public void Track(out Projectile projectile) {
            projectile = (Projectile) RetrieveObject();
            projectile.InitProjectile(gun, this);
            addProjectiles.Add(projectile);
        }

        public void Remove(Projectile projectile) {
            removeProjectiles.Add(projectile);
        }

        public void AttachTo(Gun gun, ItemProperties gunProps) {
            transform.SetParent(gun.transform);
            transform.localPosition = gunProps.WeaponProp.muzzleOffset;
            transform.localEulerAngles = Vector3.zero;
            this.gun = gun;
            poolablePrefab = gunProps.WeaponProp.projectile;
        }
        
        private void Update() { 
            var dT = Time.deltaTime;

            for (int i = 0; i < addProjectiles.Count; i++) {
                projectiles.Add(addProjectiles[i]);
            }
            addProjectiles.Clear();
            
            projectiles.ForEach( proj => proj.UpdateProjectile(dT));

            for (int i = 0; i < removeProjectiles.Count; i++) {
                projectiles.Remove(removeProjectiles[i]);
            }
            removeProjectiles.Clear();

            if (!active && projectiles.Count <= 0) {
                PoolParent.PoolObject(this);
            }
        }

        public void Pool() {
            gameObject.SetActive(false);

            transform.parent = null;
        }
        
        public void Unpool() {
            active = true;
            gameObject.SetActive(true);
        }

        public void Clean() {
            projectiles.Clear();
            transform.position = Vector2.zero;
        }

        
    }
}

