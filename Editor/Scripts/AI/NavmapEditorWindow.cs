using System.Collections.Generic;
using Game.AI.Navmap;
using Game.AI.Navmap.Nodes;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.AI {
    public class NavmapEditorWindow : EditorWindow
    {
    
        public static int Selected;

        public static Navmap2D CurrentlySelectedNavmap;
        public static bool ConnectOneWays;

        [SerializeField]
        private Texture editorIcon;
        
        public bool ConnectWithOneWays = false;
        [MenuItem("Window/NavmapEditor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            NavmapEditorWindow window = (NavmapEditorWindow)EditorWindow.GetWindow(typeof(NavmapEditorWindow));
            window.titleContent = new GUIContent() {
                text = "Navmap Editor",
                image = window.editorIcon,
                tooltip = "Editor used to create navmap"
            };
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            GameObject[] objs = GameObject.FindGameObjectsWithTag("Navmap2D");

            List<Navmap2D> navmaps = new List<Navmap2D>();
            List<string> options = new List<string>();

            for (int i = 0; i < objs.Length; i++) {
                var navmap = objs[i].GetComponent<Navmap2D>();
                if (navmap != null) {
                    navmaps.Add(navmap);
                    options.Add(objs[i].name);
                }
            }

            string[] optionsArr = options.ToArray();
            
            if (GUILayout.Button("Create New Navmap")) {
                var gameObject = new GameObject(ObjectNames.GetUniqueName(optionsArr, "Navmap2D"));
                gameObject.tag = "Navmap2D";
                gameObject.AddComponent<Navmap2D>();
            }

            

            if (optionsArr.Length == 0) {
                return;
            }
            
            Selected = EditorGUILayout.Popup("SelectedNavmap", Selected, optionsArr);
            Selected = Mathf.Clamp(Selected, 0, optionsArr.Length - 1);
            CurrentlySelectedNavmap = navmaps[Selected];

            bool changed = false;
            
            
            string name = EditorGUILayout.TextField("Navmap Name", CurrentlySelectedNavmap.name);
            if (name != CurrentlySelectedNavmap.name) {
                CurrentlySelectedNavmap.name = name;
                changed = true;
            }

            bool display = EditorGUILayout.Toggle("Display", CurrentlySelectedNavmap.DrawMap);

            if (display != CurrentlySelectedNavmap.DrawMap) {
                CurrentlySelectedNavmap.DrawMap = display;
                changed = true;
            }

            if (display) {
                CurrentlySelectedNavmap.DisplayMap();
            } else {
                CurrentlySelectedNavmap.HideMap();
            }

            ConnectWithOneWays = EditorGUILayout.Toggle("Oneway Connector Tool", ConnectWithOneWays);
            ConnectOneWays = ConnectWithOneWays;
            //Node creations
            GUILayout.Label("Node Creation", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            Ray worldRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
            Vector3 dist = worldRay.GetPoint(0);
            dist.z = 0;

            GameObject nodeObject = null;
            
            if (GUILayout.Button("GenericNode")) 
                nodeObject = CurrentlySelectedNavmap.CreateNode(
                    Navmap2DNode.NodeType.GENERIC, 
                    ObjectNames.GetUniqueName(CurrentlySelectedNavmap.GetAllNodeNames(), "GenericNode"),
                    dist
                    );
            if (GUILayout.Button("ClimbableNode")) 
                nodeObject = CurrentlySelectedNavmap.CreateNode(
                    Navmap2DNode.NodeType.CLIMBABLE, 
                    ObjectNames.GetUniqueName(CurrentlySelectedNavmap.GetAllNodeNames(), "ClimableNode"), 
                    dist
                    );
            if (GUILayout.Button("StateNode")) 
                nodeObject = CurrentlySelectedNavmap.CreateNode(
                    Navmap2DNode.NodeType.STATE, 
                    ObjectNames.GetUniqueName(CurrentlySelectedNavmap.GetAllNodeNames(), "StateNode"), 
                    dist
                    );

            if (nodeObject != null) {
                Selection.activeGameObject = nodeObject;
            }
            
            EditorGUILayout.EndHorizontal();
            
            if(changed) EditorUtility.SetDirty(CurrentlySelectedNavmap);
        }
    }
}
