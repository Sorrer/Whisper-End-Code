using System;
using UnityEngine;

namespace Game.Registry.Objects {

    [Serializable]
    public struct WeaponProperties {
        public Vector2 muzzleOffset;
        public Vector2 gunOffset;
        public GameObject projectile;
        public int maxAmmoCap;
        public float accuracy;
        public float damage;
        public float rateOfFire;
    }
    [Serializable]
    public struct UpgradeProperties {
        public float MoreData;
    }
    [Serializable]
    public struct MiscProperties {
        public float YeetData;
    }
    [Serializable]
    public struct AttachmentsProperties {
        public float DaTaTa;
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "ItemProperty", menuName = "Objects/Item")]
    public class ItemProperties : ScriptableObject {

        public string ItemID;
        public string ItemName;
        public ItemType Type = ItemType.MISC;
        public Sprite PhysicalSprite;
        public Sprite MenuIcon;

        public string Description;

        
        public enum ItemType {
            WEAPON, UPGRADE, MISC, ATTACHMENTS
        }


        public WeaponProperties WeaponProp;
        public UpgradeProperties UpgradeProp;
        public MiscProperties MiscProp;
        public AttachmentsProperties AttachmentsProp;

    }
}
