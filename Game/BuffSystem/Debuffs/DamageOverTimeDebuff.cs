using Game.HealthSystem;
using Game.HealthSystem.Buffs;
using UnityEngine;

namespace Game.BuffSystem.Debuffs {
    public class DamageOverTimeDebuff : BuffBase {


        private int TickDamage;
        private float TimeAlive;
        private float DamageInterval;
        private float curTime;
        private float curInterval;
        private DamageType damageType;
        
        public DamageOverTimeDebuff(int tickDamage, float damageInterval, float timeAlive, DamageType type, bool damageOnFirstTick = false) {
            this.curInterval = damageOnFirstTick ? timeAlive : 0;
            this.TickDamage = tickDamage;
            this.TimeAlive = timeAlive;
            this.DamageInterval = damageInterval;
            this.damageType = type;
        }
        
        protected override void ApplyBuff() {}

        public override void UpdateBuff() {
            curTime += Time.deltaTime;
            curInterval += Time.deltaTime;

            if (curInterval > DamageInterval) {
                this.manager.Health.Damage(TickDamage, damageType);
                curInterval = 0;
            }
            
            if (curTime >= TimeAlive) {
                RemoveSelf();
            }
        }
        
        
        protected override void RemoveBuff() {}
    }
}
