using System;
using Game.Registry.Objects;
using UnityEngine;

namespace Game.Registry {
    public class ItemRegistry : MonoBehaviour, IGameInstance {
        [SerializeField]
        private ItemRegistryObject obj;

        public ItemProperties GetProperties(string itemID) {
            return obj.Get(itemID);
        }

        private void Start() {
            obj.Start();
        }
    }
}
