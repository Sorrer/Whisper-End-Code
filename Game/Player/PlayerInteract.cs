using System;
using System.Collections.Generic;
using System.Linq;
using Game.Input;
using Game.Interactable;
using Game.Interactable.Interactables;
using Game.Weapons.Guns;
using UnityEngine;

namespace Game.Player {
    public class PlayerInteract : MonoBehaviour {
        public BoxCollider2D PlayerCollider;
        public float InteractRadius;
        private PlayerMain Player;
        
    
        private Vector2 playerPosition;

        private ContactFilter2D interactableContactFilter;
        private ContactFilter2D wallContactFilter;

        private readonly RaycastHit2D[] walls = new RaycastHit2D[1];
        private readonly List<Collider2D> interactablesCollisions = new List<Collider2D>();

        private IInteractable oldClosest = null;
        private bool lastInteractableStatus;

        private GameInput gameInput;

        
        
        // Start is called before the first frame update
        void Start() {
            interactableContactFilter = new ContactFilter2D();
            interactableContactFilter.SetLayerMask(LayerMask.GetMask("Interactables")); // Interactable layer(s)
            interactableContactFilter.useLayerMask = true;

            wallContactFilter = new ContactFilter2D();
            wallContactFilter.SetLayerMask(LayerMask.GetMask("Default")); // Terrain/walls/obstacles layer(s)
            wallContactFilter.useLayerMask = true;

            gameInput = MainInstances.Get<GameInput>();
            Player = MainInstances.Get<PlayerMain>();
        }
    
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.magenta;
            
            Gizmos.DrawWireSphere(transform.position, InteractRadius);
        }
        #endif

        // Update is called once per frame
        void Update() {
            playerPosition = PlayerCollider.transform.position;
        
            Physics2D.OverlapCircle(playerPosition, InteractRadius, interactableContactFilter, interactablesCollisions);

            var curClosestItem = (
                from i in interactablesCollisions
                where i.gameObject.GetComponent<IInteractable>() != null // Filter out non-interactables
                orderby Vector2.Distance(i.transform.position, playerPosition) // Sort by distance to player
                select i
                ).FirstOrDefault(i => 
                    Physics2D.Linecast(playerPosition,i.transform.position, wallContactFilter, walls) == 0 // Verifies LOS between interactable and player
                );
        
            if (!(curClosestItem is null)) { // There is a item in reach
                var interactableComp = curClosestItem.gameObject.GetComponent<IInteractable>();
                var interactableUsable = interactableComp.CanInteract();
                var newClosest = interactableComp != oldClosest;

                if (newClosest || interactableUsable != lastInteractableStatus) { // Re-evaluate highlights
                    oldClosest?.GetHighlightEffect().UnHighlight();
                    oldClosest = interactableComp;
                
                    if (interactableUsable) {
                        interactableComp.GetHighlightEffect().Highlight();
                    } else {
                        interactableComp.GetHighlightEffect().HighlightInvalid();
                    }

                    lastInteractableStatus = interactableUsable;
                }

                if (gameInput.InteractPressed) { // TODO Swap to abstract input
                    if (interactableComp.CanInteract()) {
                        interactableComp.Use();

                        if (interactableComp.GetType() == typeof(ItemInteractable)) {
                            var item = interactableComp as ItemInteractable;
                            
                            bool didPlace = Player.Inventory.AddItem(item.ItemID, item.ItemData);

                            if (item.GetComponent<Gun>()) { // TODO - rework this
                                Player.GetComponentInChildren<Hands>().PickupWeapon(item.GetComponent<Gun>());
                            } else {
                                if (didPlace) {
                                    oldClosest.GetHighlightEffect().UnHighlight();
                                    Destroy(item.gameObject);
                                    oldClosest = null;
                                }
                            }

                        }
                    } else {
                        interactableComp.Unavailable();
                    }
                }
            } else if (oldClosest != null) { // Flush out the old closest item, since no items in reach
                oldClosest.GetHighlightEffect().UnHighlight();
                oldClosest = null; 
            }
        }

    }
}