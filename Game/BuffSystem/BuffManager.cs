using System.Collections.Generic;
using Game.BuffSystem.Debuffs;
using Game.HealthSystem;
using Game.HealthSystem.Buffs;
using UnityEngine;

namespace Game.BuffSystem {

    
    public class BuffManager : MonoBehaviour {
        
        //List of buff values
        public HealthManager Health;
        
        
        
        
        //Main stuff
        [SerializeField]
        private List<BuffBase> CurrentBuffs = new List<BuffBase>();
        
        [SerializeField]
        private List<BuffBase.BuffID> PreventBuff = new List<BuffBase.BuffID>();
        public bool PreventAllBuffs = false;

        private void Start() {
            //Apply(new PosionDebuff(5, 0.5f,  5));
        }

        public void PreventId(BuffBase.BuffID id) {
            if(!PreventBuff.Contains(id)) PreventBuff.Add(id);
        }

        public void AllowId(BuffBase.BuffID id)         {
            PreventBuff.Remove(id);
        }
        
        public void Apply(BuffBase buff) {
            if (PreventAllBuffs) return;
            if (PreventBuff.Contains(buff.Id)) {
                return;
            }
            
            
            
            int firstFoundIndex = -1;
            int count = 0;
            
            if (buff.Id != BuffBase.BuffID.None) {
                for (int i = 0; i < CurrentBuffs.Count; i++) {
                    if (CurrentBuffs[i].Id == buff.Id) {
                        if (firstFoundIndex == -1) {
                            firstFoundIndex = i;
                        }

                        count++;

                        if (count >= buff.StackAmount) {
                            
                        }
                    }
                }
            }
            
            
            buff.Apply(this);
            CurrentBuffs.Add(buff);
        }

        public void Remove(BuffBase buff) {
            buff.Remove();
            CurrentBuffs.Remove(buff);
        }

        public void Remove(BuffBase.BuffID id, int amount = 1) {
            
            List<BuffBase> removeMe = new List<BuffBase>();

            for (int i = 0; i < CurrentBuffs.Count; i++) {
                if (amount <= 0) break;

                if (CurrentBuffs[i].Id == id) {
                    amount--;
                    CurrentBuffs[i].Remove();
                    removeMe.Add(CurrentBuffs[i]);
                }
            }

            for (int i = 0; i < removeMe.Count; i++) {
                CurrentBuffs.Remove(removeMe[i]);
            }
            
            
        }
        
        public void Clear(BuffBase.BuffID id) {
            List<BuffBase> removeMe = new List<BuffBase>();

            for (int i = 0; i < CurrentBuffs.Count; i++) {
                if (CurrentBuffs[i].Id == id) {
                    CurrentBuffs[i].Remove();
                    removeMe.Add(CurrentBuffs[i]);
                }
            }

            for (int i = 0; i < removeMe.Count; i++) {
                CurrentBuffs.Remove(removeMe[i]);
            }
        }

        public void Clear() {
            for (int i = 0; i < CurrentBuffs.Count; i++) {
                CurrentBuffs[i].Remove();
            }
        }
        
        private void Update() {
            for (int i = 0; i < CurrentBuffs.Count; i++) {
                CurrentBuffs[i].UpdateBuff();
            }
        }
    }
}
