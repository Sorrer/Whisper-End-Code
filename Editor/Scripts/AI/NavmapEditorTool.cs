using System;
using Game.AI.Navmap.Nodes;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Scripts.AI {
    [EditorTool("Node Tool")]
    public class NavmapEditorTool : EditorTool {
        // Serialize this value to set a default value in the Inspector.
        [SerializeField] Texture2D m_ToolIcon;

        GUIContent m_IconContent;

        void OnEnable() {
            m_IconContent = new GUIContent() {
                image = m_ToolIcon,
                text = "Platform Tool",
                tooltip = "Platform Tool"
            };
            Selection.selectionChanged += SelectionChange;
        }
        
        
        
        
        public void SelectionChange() {
            if (Tools.current != Tool.Custom) {
                lastSelectedTransform = null;
                return;
            }
            
            
            var selected = Selection.activeTransform;
            if (selected != null && selected.GetComponent<Navmap2DNode>() != null) {

                if (lastSelectedTransform != null && lastSelectedTransform != selected) {
                    Debug.Log("Selected " + lastSelectedTransform.name + " to " +  selected.name);

                    if (NavmapEditorWindow.CurrentlySelectedNavmap != null) {

                        var navmap = NavmapEditorWindow.CurrentlySelectedNavmap;
                        var nodeFrom = lastSelectedTransform.GetComponent<Navmap2DNode>();
                        var nodeTo = selected.GetComponent<Navmap2DNode>();

                        if (navmap.DoesConnectionExist(nodeFrom, nodeTo)) {
                            navmap.DeleteConnection(nodeFrom, nodeTo);
                        } else {
                            navmap.ConnectNodes(NavmapEditorWindow.ConnectOneWays, nodeFrom, nodeTo);
                        }
                        
                        EditorUtility.SetDirty(navmap);
                        Undo.RecordObject(navmap.gameObject, "Changed Connection");
                        
                    }

                    Deselect = true;
                    
                    lastSelectedTransform = null;
                } else {
                    lastSelectedTransform = selected;
                    
                }
                    
            } else {

                lastSelectedTransform = null;
            }
        }
        
        public override GUIContent toolbarIcon {
            get { return m_IconContent; }
        }

        private bool Deselect = false;

        private Transform lastSelectedTransform;
        
        // This is called for each window that your tool is active in. Put the functionality of your tool here.
        public override void OnToolGUI(EditorWindow window) {
            if (Deselect) {
                Selection.objects = new Object[0];
                Selection.activeTransform = null;
                Selection.activeObject = null;
                Selection.activeGameObject = null;
                Deselect = false;
            }
            if (lastSelectedTransform != null) {
                Handles.color = Color.red;
                Handles.DrawSolidDisc(lastSelectedTransform.position, -Vector3.forward, 0.15f);
            }
            
        }
        

        private void OnDisable() {
            if (Selection.selectionChanged != null) Selection.selectionChanged -= SelectionChange;
        }
    }
}
