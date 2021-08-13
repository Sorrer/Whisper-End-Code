using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Weapons.Guns {
    public class GunUtils : MonoBehaviour {
        private GunUtils() { }

        /**
         * Returns the value of a stat on a Gun, looking at the base and all its attachments
         */
        public static float GetStat(Gun gun, GunStat stat) {
            /* TODO - Test this 
             * var prop = typeof(StatTable).GetProperty(stat.ToString());
             * return gun.attachments.Sum(attachment => 
                        prop.GetValue(attachment.statModifications2)
                    ) + prop.GetValue(gun.baseStats2);
             */
            
            switch (stat) {
                case GunStat.ACCURACY:
                    return gun.attachments.Sum(attachment => 
                        attachment.statModifications2.ACCURACY
                    ) + gun.baseStats.ACCURACY;
                case GunStat.DAMAGE:
                    return gun.attachments.Sum(attachment => 
                        attachment.statModifications2.DAMAGE
                    ) + gun.baseStats.DAMAGE;
                case GunStat.RATE_OF_FIRE:
                    return gun.attachments.Sum(attachment => 
                        attachment.statModifications2.RATE_OF_FIRE
                    ) + gun.baseStats.RATE_OF_FIRE;
                default:
                    return 0f;
            }
        }

        /**
         * Convenience method to get ALL stats of a gun, neatly packaged in a dictionary by stat
         */
        public static StatTable GetStatTable(Gun gun) {
            // TODO For each property (obtained through GunStat.iter), set
            return new StatTable {
                ACCURACY = GetStat(gun, GunStat.ACCURACY),
                DAMAGE = GetStat(gun, GunStat.DAMAGE),
                RATE_OF_FIRE = GetStat(gun, GunStat.RATE_OF_FIRE)
            };
        }

        /**
         * Attaches a new attachment to a gun, returning either the original attachment if the operation fails, null if the
         * attachment was added and no attachment was removed, and the attachment that was removed to make place for the
         * new attachment otherwise
         */
        public static GunAttachment Attach(GunAttachment attachment, Gun gun) {
            if (!attachment.compatibleGunTypes.Contains(gun.gunType)) return attachment; // Attachment is not compatible for this gun
        
            gun.attachments.Add(attachment);
            return Remove(attachment.attachmentType, gun);
        }

        /**
         * Removes an attachment of the specified type from a gun. Returns null if no attachment removed, and returns the
         * attachment removed if found
         */
        public static GunAttachment Remove(AttachmentType aType, Gun gun) {
            var toRemove = gun.attachments.Find(a => a.attachmentType == aType);
            gun.attachments.Remove(toRemove);
            return toRemove;
        }
        

        public static void GenerateGun(Gun g, int seed) {
            GunGenerator.GenerateRandomGun(g, seed);
        }
        
        public static void GenerateGunOfType(Gun g, GunType type, int seed) {
            g.gunType = type;
            GenerateGun(g, seed);
        }

        public static GameObject getProjectileType(Gun gun) {
            throw new NotImplementedException();
        }

        public static String SerializeGun(Gun gun) {
            return JsonConvert.SerializeObject(gun, Formatting.Indented);
        }
    }
}
