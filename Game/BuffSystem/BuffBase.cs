using System;
using Game.BuffSystem;
using UnityEngine;

namespace Game.HealthSystem.Buffs {
    public abstract class BuffBase {

        public enum BuffID {
            None, Fire, Posion
        }

        /// <summary>
        /// Assign buffID to make a unique buff that can not stack pass stack amount;
        /// </summary>
        public virtual BuffID Id {
            get {return BuffID.None;}
        }

        
        /// <summary>
        /// If this buff is unique how much does it stack? 0 = Once, 1 = Once, 2 = Twice, 3 = Thrice, etc
        /// Manager removes the first inserted (Closest to 0 index)
        /// </summary>
        public virtual int StackAmount {
            get { return 0; }
        }

        protected BuffManager manager;
        
        public void Apply(BuffManager manager1) {
            this.manager = manager1;
            ApplyBuff();
        }

        public void Remove() {
            manager = null;
            RemoveBuff();
        }
        
        protected abstract void ApplyBuff();

        public abstract void UpdateBuff();


        protected void RemoveSelf() {
            manager.Remove(this);
        }
        protected abstract void RemoveBuff();
    }
}
