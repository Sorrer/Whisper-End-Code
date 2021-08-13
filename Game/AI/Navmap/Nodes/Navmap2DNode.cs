using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

namespace Game.AI.Navmap.Nodes {
    public class Navmap2DNode : MonoBehaviour
    {   
        public enum NodeType {GENERIC, CLIMBABLE, STATE}

        [Serializable]
        public struct Connection {
            public float distance;
            public Navmap2DNode node;
        }
        
        public List<Connection> connections = new List<Connection>();
    }
}
