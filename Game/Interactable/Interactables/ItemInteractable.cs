using System;
using Game.Interactable.Highlight;
using Game.Registry.Objects;
using UnityEngine;

namespace Game.Interactable.Interactables {
    public class ItemInteractable : MonoBehaviour, IInteractable {
        public InteractHighlightEffect highlightEffect;

        public string ItemID = "";
        public string ItemData = "";
        
        public ItemProperties Properties;
        
        public bool IsInteractable = true;


        public SpriteRenderer SRenderer;
        public BoxCollider2D Collider2D;
        
        
        private void Awake() {
            Load();
        }

        [ContextMenu("Load item")]
        public void Load() {
            var sprite = Properties.PhysicalSprite;
            SRenderer.sprite = sprite;

            Rect croppedRect = new Rect(
                (sprite.textureRectOffset.x + sprite.textureRect.width / 2f) / sprite.pixelsPerUnit,
                (sprite.textureRectOffset.y + sprite.textureRect.height / 2f) / sprite.pixelsPerUnit,
                sprite.textureRect.width / sprite.pixelsPerUnit,
                sprite.textureRect.height / sprite.pixelsPerUnit);
       
            // offset is relative to sprite's pivot
            Collider2D.offset = croppedRect.position - sprite.pivot / sprite.pixelsPerUnit;
            Collider2D.size = croppedRect.size;


            ItemID = Properties.ItemID;
        }

        public bool CanInteract() {
            return IsInteractable;
        }

        public void Use() {
        }


        public void Unavailable() {
            
        }

        public InteractHighlightEffect GetHighlightEffect() {
            return highlightEffect;
        }
    }
}
