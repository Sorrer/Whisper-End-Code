using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Registry.Objects {
    
    [CreateAssetMenu(fileName = "ItemRegistry", menuName = "Objects/ItemRegistry")]
    public class ItemRegistryObject : RegistryObject<ItemProperties>
    {
        public List<TestValueObjectPair> itemPairs = new List<TestValueObjectPair>();

        [Serializable]
        public struct TestValueObjectPair {
            public string id;
            public ItemProperties value;
        }
        
        protected override void Init() {
            for (int i = 0; i < itemPairs.Count; i++) {
                var pair = itemPairs[i];

                initPairList.Add(new ValueObjectPair { id = pair.id, value = pair.value }) ;
            }

            base.Init();
        }
        
    }
}
