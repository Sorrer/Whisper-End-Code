using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Game.AI.Navmap;
using Game.AI.Navmap.Nodes;
using UnityEngine;

public class NavmapPawn : MonoBehaviour {
    
    public Navmap2DPathfinder.Path path;

    public Navmap2D Navmap;

    public Transform Target;
    [ContextMenu("FindPath")]
    public void FindPath() {
        path = Navmap2DPathfinder.FindPath(Navmap, this.transform.position, Target.position);
    }

    public void Update() {
        FindPath();
    }


    private void OnDrawGizmos() {
        if (path.nodes == null) return;
        
        Stack<Navmap2DNode> navmapNodes = new Stack<Navmap2DNode>();
        Gizmos.color = Color.magenta;

        Vector3 lastPos = this.transform.position;
        
        while (path.nodes.Count > 0) {
            var node = path.nodes.Pop();
            navmapNodes.Push(node);

            var position = node.transform.position;
            Gizmos.DrawWireSphere(position, 0.25f);
            Gizmos.DrawLine(lastPos, position);

            lastPos = position;
        }

        while (navmapNodes.Count > 0) {
            path.nodes.Push(navmapNodes.Pop());
        }
        
    }
}
