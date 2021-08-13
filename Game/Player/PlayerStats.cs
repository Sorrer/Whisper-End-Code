using System;
using UnityEngine;

namespace Game.Player {
    public class PlayerStats : MonoBehaviour {
        
        public PlayerMain MainPlayer;
        public enum StatType {
            HEALTH
        }

        public struct Stat {
            public StatType Type;
            public float Value;
        }


        public void ApplyStat(Stat[] stat) {
            for (int i = 0; i < stat.Length; i++) {
                ApplyStat(stat[i]);
            }
        }
        public void ApplyStat(Stat stat) {
            switch (stat.Type) {
                case StatType.HEALTH:
                    MainPlayer.healthManager.MaxHealth += (int)stat.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        public void RemoveStat(Stat[] stat) {
            for (int i = 0; i < stat.Length; i++) {
                RemoveStat(stat[i]);
            }
        }
        
        public void RemoveStat(Stat stat) {
            switch (stat.Type) {
                case StatType.HEALTH:
                    MainPlayer.healthManager.MaxHealth -= (int)stat.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
        
        
        
    }
}
