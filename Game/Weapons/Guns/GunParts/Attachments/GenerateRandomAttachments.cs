using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Guns.GunParts.Attachments {
    public static class GenerateRandomAttachments {
        private static float Seed;

        public static void SetSeed(float seed) {
            Seed = seed;
        }

        /**
         * Generates an attachment of specified type compatible with the specified gun and randomized stats
         */
        public static void GenerateAttachmentOfType(GunAttachment attachment, Gun gun, AttachmentType aType) {
            GenerateAttachmentOfType(attachment, gun.gunType, aType);
        }
        
        /**
         * Generates an attachment of specified type compatible with the specified gun type and randomized stats
         */
        public static void GenerateAttachmentOfType(GunAttachment attachment, GunType gType, AttachmentType aType) {
            attachment.attachmentType = aType;
            attachment.compatibleGunTypes = new List<GunType>{ gType };
            attachment.statModifications = GenStatTable(aType);
        }

        /**
         * TODO
         */
        private static Dictionary<GunStat, float> GenStatTable(AttachmentType aType) {
            return new Dictionary<GunStat, float>();
        }
    }
}