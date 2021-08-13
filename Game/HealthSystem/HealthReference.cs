using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.HealthSystem {

    public class HealthReference : HealthBase {

        public HealthManager MainHealth;

        public List<DamageTypeModifer>  Modifiers = new List<DamageTypeModifer>();
        public List<DamageTypeResistance>  Resistances = new List<DamageTypeResistance>();
        
        public override void Damage(int amount, DamageType type) {

            for (int i = 0; i < Resistances.Count; i++) {
                var resistance = Resistances[i];

                if (resistance.Type == type || resistance.Type == DamageType.ALL) {
                    amount = Mathf.Max(0, amount - resistance.Resistance);
                }
            }
            
            for (int i = 0; i < Modifiers.Count; i++) {
                var modifier = Modifiers[i];
                if (modifier.Type == type || modifier.Type == DamageType.ALL) {
                    amount = Mathf.RoundToInt(amount * modifier.Modifier);
                }
            }
            
            Damage(amount);
        }
        
        
        public override void Damage(int amount) {
            MainHealth.Damage(amount);
        }

        public override void Heal(int amount) {
            MainHealth.Heal(amount);
        }

        public override int dHealth {
            get {
                return MainHealth.Health;
            }
            set {
                MainHealth.Health = value;
            }
        }

        public override int dMaxHealth {
            get {
                return MainHealth.MaxHealth;
            }
            set {
                MainHealth.MaxHealth = value;
            }
        }
    }
}