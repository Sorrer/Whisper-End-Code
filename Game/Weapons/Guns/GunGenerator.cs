using System.Collections.Generic;
using Game.Registry;
using UnityEngine;
using Random = System.Random;

namespace Game.Weapons.Guns {
    public static class GunGenerator {

        public static List<string> rndUIDs = new List<string> {
            "base_ar"
        };

        public static ItemRegistry gunRegistry;
        public static void GenerateRandomGun(Gun g, int seed) {

            if (!gunRegistry) {
                gunRegistry = GameObject.Find("GunRegistry").GetComponent<ItemRegistry>();
            }
            
            var rnd = new Random(seed);
            //var newUID = rndUIDs[rnd.Next(0, rndUIDs.Count)];
            
            //g.uid = newUID;
            g.seed = seed;
            
            var gunProps = gunRegistry.GetProperties(g.uid).WeaponProp;
            
            g.maxAmmo = gunProps.maxAmmoCap;
            g.ammo = rnd.Next(0, g.maxAmmo + 1); // Give the gun a random amount of bullets in the clip
            
            //g.attachments = GenerateAttachments
            
            g.baseStats = new StatTable {
                ACCURACY = gunProps.accuracy,
                DAMAGE = gunProps.damage,
                RATE_OF_FIRE = gunProps.rateOfFire
            };
        }
    }
}
