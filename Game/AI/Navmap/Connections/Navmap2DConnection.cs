using Game.AI.Navmap.Nodes;
using UnityEngine;

namespace Game.AI.Navmap.Connections {
    public class Navmap2DConnection {

        
        public bool IsOneway = false;

        public Navmap2DNode From, To;

        public Navmap2DNode GetOtherNode(Navmap2DNode from) {
            if (from == From) {
                return To;
            }else if (from == To) {
                if (IsOneway) {
                    return null;
                }

                return From;
            }

            //This should not happen, if it does then the node graph is broken.
            Debug.LogWarning("Tried to get connection with node that was not part of the connection!");
        
            return null;
        }
    }
}
