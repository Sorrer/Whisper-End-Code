using Game.HealthSystem;
using UnityEngine;

namespace Game.BuffSystem.Debuffs {
    public class PosionDebuff : DamageOverTimeDebuff {
        public override BuffID Id {
            get { return BuffID.Posion; }
        }

        public override int StackAmount {
            get { return 5; }
        }

        public PosionDebuff(int tickDamage, float damageInterval, float timeAlive) : 
            base(tickDamage, damageInterval, timeAlive, DamageType.POSION) { }
        
    }
}