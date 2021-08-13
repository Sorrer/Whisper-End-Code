using System.Collections.Generic;
using System.Security;
using System.Security.Policy;
using Game.AI.Navmap.Nodes;
using UnityEngine;

namespace Game.AI.Navmap {
    public class Navmap2DPathfinder : MonoBehaviour
    {
        public struct Path {
            public Stack<Navmap2DNode> nodes;
        }

        public struct AStarNode {
            public float TotalCost;
            public float MovementCost;
            public float DistanceToFinish;
            public Navmap2DNode Parent;
            public Navmap2DNode Node;
        }


        public static AStarNode GenerateAStarNode(Navmap2DNode node, Navmap2DNode parent,  Vector3 goal) {
            AStarNode aNode = new AStarNode();
        
            aNode.Node = node;

            var position = node.transform.position;
            
            aNode.DistanceToFinish = (position-goal).sqrMagnitude;

            if (parent != null) {
                aNode.MovementCost = (position - parent.transform.position).sqrMagnitude;
                aNode.Parent = parent;
            } else {
                aNode.Parent = null;
                aNode.MovementCost = 0;
            }
            
            aNode.TotalCost = aNode.MovementCost + aNode.DistanceToFinish;

            return aNode;
        }
        
        public static List<AStarNode> open = new List<AStarNode>();
        public static List<AStarNode> closed = new List<AStarNode>();
        
        
        public static Path FindPath(Navmap2D navmap, Vector3 From, Vector3 Target) {
            Navmap2DNode startNode = navmap.GetClosestNode(From);
            Navmap2DNode endNode = navmap.GetClosestNode(Target);
            
            open.Clear();
            closed.Clear();
            
            open.Add(GenerateAStarNode(startNode, null, endNode.transform.position));

            bool found = false;

            AStarNode endAStarNode = new AStarNode();

            int iterations = 0;
            
            while (!found || open.Count > 0) {
                iterations++;
                AStarNode lowestFNode = open[0];

                for (int i = 0; i < open.Count; i++) {
                    if (lowestFNode.TotalCost > open[i].TotalCost) {
                        lowestFNode = open[i];
                    }
                }

                open.Remove(lowestFNode);


                if (lowestFNode.Node == endNode) {
                    found = true;
                    endAStarNode = lowestFNode;
                    break;
                }


                var succesors = new List<AStarNode>();
                var connections = lowestFNode.Node.connections;
                for (int i = 0; i < connections.Count; i++) {
                    succesors.Add(GenerateAStarNode(connections[i].node, lowestFNode.Node, endNode.transform.position));
                }


                for (int i = 0; i < succesors.Count; i++) {
                    var curSuccesor = succesors[i];

                    curSuccesor.MovementCost += lowestFNode.MovementCost;
                    curSuccesor.TotalCost = curSuccesor.MovementCost + curSuccesor.DistanceToFinish;

                    bool foundNodeInList = false;
                    AStarNode foundNode = new AStarNode();
                    
                    for (int x = 0; x < open.Count; x++) {
                        if (open[x].Node == curSuccesor.Node) {
                            foundNode = open[x];
                            foundNodeInList = true;
                        }
                    }
                    
                    //Open set
                    if (foundNodeInList) {
                        if (foundNode.MovementCost < curSuccesor.MovementCost) {
                            continue;
                        }

                        open.Remove(foundNode);
                    }

                    foundNodeInList = false;
                    for (int x = 0; x < closed.Count; x++) {
                        if (closed[x].Node == curSuccesor.Node) {
                            foundNode = closed[x];
                            foundNodeInList = true;
                        }
                    }

                    //Closed set (This is a highly debated portion
                    if (foundNodeInList) {
                        continue;
                    }
                    
                    open.Add(curSuccesor);
                    
                }

                closed.Add(lowestFNode);
            }
            

            //Debug.Log("Ran for " + iterations + " iterations.");
            
            Path path = new Path();
            path.nodes = new Stack<Navmap2DNode>();
            if (found) {

                AStarNode curNode = endAStarNode;

                while (curNode.Node != startNode) {
                    
                    path.nodes.Push(curNode.Node);
                    
                    bool gotIt = false;
                    for (int i = 0; i < closed.Count; i++) {
                        if (closed[i].Node == curNode.Parent) {
                            curNode = closed[i];
                            gotIt = true;
                            break;
                        }
                    }

                    if (!gotIt) {
                        for (int i = 0; i < open.Count; i++) {
                            if (open[i].Node == curNode.Parent) {
                                curNode = open[i];
                                gotIt = true;
                                break;
                            }
                        }
                    }


                    if (!gotIt) {
                        break;
                    }
                }
                
                path.nodes.Push(curNode.Node);
            }
            
            open.Clear();
            closed.Clear();
            
            //Debug.Log("Path length: " + path.nodes.Count);
            
            
            return path;

        }
    
    
    }
}
