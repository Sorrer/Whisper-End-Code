using System;
using System.Collections.Generic;
using System.Linq;
using Game.AI.Navmap.Connections;
using Game.AI.Navmap.Nodes;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.AI.Navmap {
    public class Navmap2D : MonoBehaviour {

        public List<Navmap2DNode> nodes = new List<Navmap2DNode>();

        [SerializeField] private Texture2D arrowTexture;
        
        public bool DrawMap = false;

        private void Start() {
            HideMap();
        }

        public GameObject CreateNode(Navmap2DNode.NodeType nodeType, string name, Vector3 pos = new Vector3()) {
            var nodeObject = new GameObject(name);

            nodeObject.transform.parent = this.transform;
            nodeObject.transform.position = pos;

            string iconName = "";
            switch (nodeType) {
                case Navmap2DNode.NodeType.GENERIC:
                    iconName = "GenericNode";
                    break;
                case Navmap2DNode.NodeType.CLIMBABLE:
                    iconName = "ClimbableNode";
                    break;
                case Navmap2DNode.NodeType.STATE:
                    iconName = "StateNode";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }
            
            var spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Icons/" + iconName);
            spriteRenderer.sortingOrder = 100;
            var node = nodeObject.AddComponent<Navmap2DNode>();
            nodes.Add(node);
            
            return nodeObject;
        }
        

        public bool DoesConnectionExist(Navmap2DNode from, Navmap2DNode to) {
            return from.connections.Any(item => item.node == to) || 
                   to.connections.Any(item => item.node == from);
        }
        
        public void ConnectNodes(bool isOneway, Navmap2DNode from, Navmap2DNode to) {

            float distanceBetween = (from.transform.position - to.transform.position).magnitude;
            
            from.connections.Add(new Navmap2DNode.Connection(){distance = distanceBetween, node = to});
            if (!isOneway) {
                to.connections.Add(new Navmap2DNode.Connection(){distance = distanceBetween, node = from});
            }
        }

        public void DeleteConnection(Navmap2DNode from, Navmap2DNode to) {
            from.connections = from.connections.Where(item => item.node != to).ToList();
            to.connections = to.connections.Where(item => item.node != from).ToList();
            
        }

        public Navmap2DNode GetClosestNode(Vector3 pos) {

            if (nodes.Count == 0) return null;

            var curNode = nodes[0];
            float distance = (pos - curNode.transform.position).sqrMagnitude;
            
            for (int i = 0; i < nodes.Count; i++) {
                float curDist = (pos - nodes[i].transform.position).sqrMagnitude;
                
                if (distance > curDist) {
                    distance = curDist;
                    curNode = nodes[i];
                }
            }

            return curNode;
        }
        

        public void DisplayMap() {
            SyncMap();
            foreach (var node in nodes) {
                node.gameObject.SetActive(true);
            }

            DrawMap = true;
        }

        public void HideMap() {
            SyncMap();
            foreach (var node in nodes) {
                node.gameObject.SetActive(false);
            }
            DrawMap = false;
        }

        public void SyncMap() {
            foreach (var node in nodes) {
                node.connections = node.connections.Where(item => item.node != null).ToList();
            }
            nodes = nodes.Where(item => item != null).ToList();

        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            SyncMap();
            if (!DrawMap) {
                return;
            }
            int i = 0;
            foreach (var node in nodes) {
                i++;
                Random.InitState(i);
                Gizmos.color = Random.ColorHSV(0, 1, 0.9f, 1.0f, 0.8f, 1.0f);

                foreach (var connection in node.connections) {
                    DrawThickLine(node.transform.position, connection.node.transform.position, 0.5f);

                    if (connection.node.connections.All(item => item.node != node)) {

                        Vector3 dirVector = node.transform.position - connection.node.transform.position;
                        //Draw oneway texture
                        Vector3 worldCenterPoint = node.transform.position +
                                                   (connection.node.transform.position - node.transform.position) / 2.0f;
                        
                        
                        //Gizmos.DrawGUITexture(rect, arrowTexture);
                        dirVector.Normalize();
                        var newVector = Vector3.Cross(dirVector , Vector3.forward
                        ).normalized;
                        var newPoint = 0.25f*newVector+worldCenterPoint;
                        var newPoint2 = -0.25f*newVector+worldCenterPoint;

                        DrawThickLine(newPoint, worldCenterPoint  + dirVector * -0.35f, 0.45f);
                        DrawThickLine(newPoint2, worldCenterPoint + dirVector * -0.35f, 0.45f);
                        DrawThickLine(newPoint2, newPoint, 0.45f);

                    }
                }
                
            }
            
        }
        public static void DrawThickLine(Vector3 start, Vector3 end, float thickness)
        {
            Camera c = Camera.current;
            if (c == null) return;
 
            // Only draw on normal cameras
            if (c.clearFlags == CameraClearFlags.Depth || c.clearFlags == CameraClearFlags.Nothing)
            {
                return;
            }
 
            // Only draw the line when it is the closest thing to the camera
            // (Remove the Z-test code and other objects will not occlude the line.)
            var prevZTest = Handles.zTest;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
 
            Handles.color = Gizmos.color;
            Handles.DrawAAPolyLine(thickness * 10, new Vector3[] { start, end });
 
            Handles.zTest = prevZTest;
        }
        
#endif
        
        public string[] GetAllNodeNames() {
            string[] names = new string[nodes.Count];
            for (int i = 0; i < nodes.Count; i++) {
                names[i] = nodes[i].name;
            }

            return names;
        }


    }
}
