using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Player {
    
    public class InventoryRow : MonoBehaviour {
        public List<InventroyUIElement> RowElements = new List<InventroyUIElement>();

        public void SetRowElements(int[] indexes, Sprite[] sprites) {

            if (indexes.Length != sprites.Length) {
                Debug.LogError("Could not build row elements! Indexes length does not equal sprite length!");
                return;
            }

            if (indexes.Length > RowElements.Count) {
                Debug.LogError("Could not build row elements! Too many elements than row elements");
                return;
            }

            Debug.Log(indexes.Length);
            
            for (int i = 0; i < RowElements.Count; i++) {

                if (i > indexes.Length - 1) {
                    RowElements[i].gameObject.SetActive(false);
                    continue;
                }
                
                RowElements[i].gameObject.SetActive(true);
                RowElements[i].sprite = sprites[i];
                RowElements[i].Index = indexes[i];


            }



        }
        
    }
}