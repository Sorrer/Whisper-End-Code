using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Game.Registry;
using Game.Registry.Objects;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

namespace Game.Player {

    
    [CreateAssetMenu(fileName = "PlayerInventory", menuName = "SingleObjects/PlayerInventory")]
    public class PlayerInventory : ScriptableObject {
        
        
        public int MaxWeapons = 4;
        public int MaxUpgrades = 30;
        public int MaxAttachments = 15;

        public int CurrentWeapons = 0;
        public int CurrentUpgrades = 0;
        public int CurrentAttachments = 0;
        
        public List<Item> CurrentItems = new List<Item>();

        [Serializable]
        public struct Item {
            public string Id;
            public string Data;
        }

        public ItemRegistryObject itemRegistry;
        
        public bool AddItem(string id, string data = "") {
            ItemProperties properties = itemRegistry.Get(id);
            if (properties == null) {
                //Debug.LogWarning("Tried to add invalid item id to player inventory " + id);
                return false;
            }

            
            switch (properties.Type) {
                case ItemProperties.ItemType.WEAPON:

                    if (CurrentWeapons >= MaxWeapons) {
                        return false;
                    }

                    CurrentWeapons++;
                    break;
                case ItemProperties.ItemType.UPGRADE:
                    if (CurrentUpgrades >= MaxUpgrades) {
                        return false;
                    }

                    CurrentUpgrades++;
                    break;
                case ItemProperties.ItemType.ATTACHMENTS:
                    if (CurrentAttachments >= MaxAttachments) {
                        return false;
                    }

                    CurrentAttachments++;
                    break;
                case ItemProperties.ItemType.MISC:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrentItems.Add(new Item() {Id = id, Data = data});
            
            return true;
        }

        public void RemoveItem(string id) {
            RemoveItem(id, (item) => { return true; });
        }
        
        public void RemoveItem(string id, Func<Item, bool> comparator) {
            ItemProperties properties = itemRegistry.Get(id);
            if (properties == null) {
                Debug.LogWarning("Tried to remove invalid item id to player inventory " + id);
                return;
            }


            bool removed = false;
            for (int i = 0; i < CurrentItems.Count; i++) {
                var item = CurrentItems[i];
                if (item.Id.Equals(id) && comparator.Invoke(item)) {
                    CurrentItems.RemoveAt(i);

                    removed = true;
                    
                    break;
                }
            }

            if (removed) {
                switch (properties.Type) {
                    case ItemProperties.ItemType.WEAPON:
                        CurrentUpgrades--;
                        if (CurrentUpgrades < 0) {
                            Debug.LogWarning("Negative current upgrades detected");
                        }
                        break;
                    case ItemProperties.ItemType.UPGRADE:
                        CurrentWeapons--;
                        if (CurrentWeapons < 0) {
                            Debug.LogWarning("Negative current weapons detected");
                        }
                        break;
                    case ItemProperties.ItemType.ATTACHMENTS:
                        CurrentAttachments--;
                        if (CurrentAttachments < 0) {
                            Debug.LogWarning("Negative current weapons detected");
                        }
                        break;
                    case ItemProperties.ItemType.MISC:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            
            
            
        }

        public void HasItem(string id) {
            HasItem(id, (item) => { return true; });
        }
        public bool HasItem(string id, Func<Item, bool> comparator) {
            ItemProperties properties = itemRegistry.Get(id);
            if (properties == null) {
                Debug.LogWarning("Tried to remove invalid item id to player inventory " + id);
                return false;
            }


            for (int i = 0; i < CurrentItems.Count; i++) {
                var item = CurrentItems[i];
                if (item.Id.Equals(id) && comparator.Invoke(item)) {
                    return true;
                }
            }

            return false;
        }


        private struct SaveData {
            public int MaxWeapons;
            public int MaxUpgrades;
            public int MaxAttachments;

            public Item[] Items;
        }

        public string GetJson() {

            var saveData = new SaveData() {
                MaxWeapons = MaxWeapons,
                MaxAttachments =  MaxAttachments,
                MaxUpgrades = MaxUpgrades,
                Items = CurrentItems.ToArray()
            };
            
            return JsonConvert.SerializeObject(saveData);
        }

        public void LoadJson(string json) {

            SaveData saveData;
            
            try {
                saveData = JsonConvert.DeserializeObject<SaveData>(json);
            } catch (Exception e) {
                Debug.LogError("Could not load JSON for Inventory! Invalid format");
                Debug.LogException(e);
                return;
            }

            this.MaxAttachments = saveData.MaxAttachments;
            this.MaxUpgrades = saveData.MaxUpgrades;
            this.MaxWeapons = saveData.MaxWeapons;
            
            CurrentItems.Clear();
            for (int i = 0; i < saveData.Items.Length; i++) {
                CurrentItems.Add(saveData.Items[i]);
            }
        }


        [ContextMenu("Test (Clears Inventory)")]
        public void Test() {
            
            CurrentItems.Clear();
            CurrentItems.Add(new Item(){Id = "This is an item", Data = "this is the data"});

            string json = GetJson();
            
            Debug.Log(json);
            
            CurrentItems.Clear();
            
            LoadJson(json);
            
            Debug.Log(GetJson());

            CurrentItems.Clear();
        }
        
        
        
    }
}
