
using Game.Interactable.Highlight;

namespace Game.Interactable {

    public interface IInteractable {

        //Whether or not this item can be interacted with currently
        bool CanInteract();

        //Called when the item is attempted to be used, and CanInteract() returns true
        void Use();

        //Called when the item is attempted to be used, but CanInteract() returns false
        void Unavailable();

        //Returns the InteractHighlightEffect associated with this object
        InteractHighlightEffect GetHighlightEffect();
    }
}