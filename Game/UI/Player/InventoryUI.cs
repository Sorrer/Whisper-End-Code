using System;
using System.Collections.Generic;
using Game.Player;
using Game.Registry.Objects;
using TMPro;
using UnityEngine;

namespace Game.UI.Player {
    public class InventoryUI : MonoBehaviour {
        public GameObject RowPrefab;
        public GameObject TextPrefab;


        public PlayerInventory Inventory;
        
        public Transform Container;

        public Sprite errorSprite;
        
        private void Start() {
            Init();
        }

        private struct ItemIndexPair {
            public Sprite sprite;
            public int index;
        }
        
        public void Init() {
            Clear();


            List<ItemIndexPair> weapons = new List<ItemIndexPair>();
            List<ItemIndexPair> attachments = new List<ItemIndexPair>();
            List<ItemIndexPair> upgradeNodes = new List<ItemIndexPair>();
            List<ItemIndexPair> misc = new List<ItemIndexPair>();


            var curItems = Inventory.CurrentItems;

            for (int i = 0; i < curItems.Count; i++) {
                var curItem = curItems[i];
                ItemProperties item = Inventory.itemRegistry.Get(curItem.Id);
                ItemIndexPair pair;
                
                pair = new ItemIndexPair() {
                    sprite = item.MenuIcon,
                    index = i
                };

                if (pair.sprite == null) {
                    pair.sprite = errorSprite;
                }
                
                switch (item.Type) {
                    case ItemProperties.ItemType.WEAPON:
                        weapons.Add(pair);
                        break;
                    case ItemProperties.ItemType.UPGRADE:
                        upgradeNodes.Add(pair);
                        break;
                    case ItemProperties.ItemType.MISC:
                        misc.Add(pair);
                        break;
                    case ItemProperties.ItemType.ATTACHMENTS:
                        attachments.Add(pair);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            
            
            
            
            
            AddCategoryTitle("Weapoons [" + Inventory.CurrentWeapons + "/" + Inventory.MaxWeapons + "]");
            AddElements(weapons.ToArray());
            AddCategoryTitle("Attachments [" + Inventory.CurrentAttachments + "/" + Inventory.MaxAttachments + "]");
            AddElements(attachments.ToArray());
            AddCategoryTitle("UpgradeNodes [" + Inventory.CurrentUpgrades + "/" + Inventory.MaxWeapons + "]");
            AddElements(upgradeNodes.ToArray());
            AddCategoryTitle("Misc");
            AddElements(misc.ToArray());

        }

        private GameObject AddCategoryTitle(string text) {
            GameObject initiated = Instantiate(TextPrefab, Container);

            var textMesh = initiated.GetComponent<TextMeshProUGUI>();
            textMesh.text = text;

            return initiated;
        }

        private void AddElements(ItemIndexPair[] pair) {
            if (pair.Length == 0) return;
            GameObject initiated = Instantiate(RowPrefab, Container);
            var invRow = initiated.GetComponent<InventoryRow>();
            int maxPerRow = invRow.RowElements.Count;

            int curCount = 0;


            List<Sprite> sprites = new List<Sprite>();
            List<int> indexes = new List<int>();
            
            
            for (int i = 0; i < pair.Length; i++) {
                
                indexes.Add(pair[i].index);
                sprites.Add(pair[i].sprite);
                
                curCount++;
                bool isLast = i + 1 == pair.Length;
                
                if (curCount == maxPerRow ||isLast) {
                    
                    invRow.SetRowElements(indexes.ToArray(), sprites.ToArray());
                    
                    sprites.Clear();
                    indexes.Clear();
                    
                    curCount = 0;

                    if (!isLast) {
                        initiated = Instantiate(RowPrefab, Container);
                        invRow = initiated.GetComponent<InventoryRow>();
                    }
                }

            }

        }
        
        public void Clear() {
            foreach (Transform child in Container) {
                
                Destroy(child.gameObject);
                
            }
        }


    }
}
