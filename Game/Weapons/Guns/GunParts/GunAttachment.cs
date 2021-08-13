using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Guns {
    public enum AttachmentType {
        SCOPE,
        MUZZLE,
        TRIGGER,
        GRIP,
    }

    public abstract class GunAttachment : MonoBehaviour {
        public List<GunType> compatibleGunTypes; // List in case attachments can attach to multiple types of guns
        public AttachmentType attachmentType;
        public StatTable statModifications2;
        public Dictionary<GunStat, float> statModifications;
    }
}
