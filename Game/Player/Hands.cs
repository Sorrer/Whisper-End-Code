using System.Collections.Generic;
using Game.Input;
using Game.Interactable;
using Game.Registry;
using Game.Weapons;
using Game.Weapons.Guns;
using UnityEngine;

namespace Game.Player {
    public class Hands : PoolBehaviour {
        public SpriteRenderer playerSr;
        public Muzzle curMuzzle;
        
        public int maxWeapons = 4;

        public ItemRegistry gunRegistry;
        
        private GameInput gi;
        private UnityEngine.Camera mainCam;

        
        private Gun equippedWeapon;
        private readonly List<Gun> holsteredWeapons = new List<Gun>();
        private readonly List<SpriteRenderer> childSpriteRenderers = new List<SpriteRenderer>();

        private float lastTriggerPull;
        private float gunRof;

        public List<Muzzle> activeMuzzles = new List<Muzzle>();

        void Start() {
            gi = MainInstances.Get<GameInput>();
            mainCam = UnityEngine.Camera.main;
        }

        public void Dequip() {
            equippedWeapon = null;
            gunRof = 1;
            lastTriggerPull = Time.time - gunRof;
        }

        /**
         * Convenience method
         */
        public void EquipWeapon(int idx) {
            EquipWeapon(holsteredWeapons[idx]);
        }
        
        public void EquipWeapon(Gun gun) {
            if (equippedWeapon != null) {
                childSpriteRenderers[holsteredWeapons.IndexOf(equippedWeapon)].enabled = false;
            }

            equippedWeapon = gun;
            
            childSpriteRenderers[holsteredWeapons.IndexOf(equippedWeapon)].enabled = true;
            
            gunRof = GunUtils.GetStat(equippedWeapon, GunStat.RATE_OF_FIRE);
            lastTriggerPull = Time.time - gunRof;
            
            if (curMuzzle) {
                curMuzzle.active = false;
            }

            var newMuzzle = (Muzzle) RetrieveObject();
            newMuzzle.AttachTo(gun, gunRegistry.GetProperties(gun.uid));
            Debug.Log(gunRegistry.GetProperties(gun.uid).ItemName);
            Debug.Log(gunRegistry.GetProperties(gun.uid).WeaponProp.projectile.name);
            
            activeMuzzles.Add(curMuzzle);
            curMuzzle = newMuzzle;
        }

        public void PickupWeapon(Gun gun) {
            // TODO - TEMP
            GunUtils.GenerateGun(gun, 1);

            gun.GetComponent<Collider2D>().enabled = false;
            
            if (holsteredWeapons.Count >= maxWeapons) {
                ThrowWeapon(equippedWeapon);
            }
            
            gun.transform.parent = transform;
            gun.transform.localPosition = gunRegistry.GetProperties(gun.uid).WeaponProp.gunOffset;
            gun.transform.localEulerAngles = Vector3.zero;
            gun.transform.localScale = new Vector3(1, 1);
            
            holsteredWeapons.Add(gun);
            childSpriteRenderers.Add(gun.GetComponent<SpriteRenderer>());
            EquipWeapon(gun);
        }

        public void ThrowWeapon(Gun gun) {
            // TODO - Actually throw the weapon

            if (equippedWeapon == gun) {
                equippedWeapon = null;
            }
            
            gun.GetComponent<Collider2D>().enabled = true;
            gun.transform.parent = null;
            
            childSpriteRenderers.RemoveAt(holsteredWeapons.IndexOf(gun));
            holsteredWeapons.Remove(gun);
        }
        
        private void Update() {
            if (gi.GetFire() && equippedWeapon) {
                if (Time.time - lastTriggerPull >= gunRof) {
                    curMuzzle.Shoot();
                    lastTriggerPull = Time.time;
                }
            }
        }
    }
}
