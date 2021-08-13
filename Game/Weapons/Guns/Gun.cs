using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Guns {

    [System.Serializable]
    public struct StatTable {
        public float ACCURACY;
        public float DAMAGE;
        public float RATE_OF_FIRE;
    }

    public enum GunType {
        PISTOL,
        ASSAULT_RIFLE,
        SMG,
        DMR,
        MACHINE_GUN,
        ROCKET_LAUNCHER,
        LAUNCHER,
    }
    
    public class Gun : MonoBehaviour {
        public string uid;

        public int maxAmmo;
        public int ammo;
        public int seed;
        
        public List<GunAttachment> attachments;
        public GunType gunType;
        public StatTable baseStats;
    }
}
