using System;
using Game.Registry.Objects;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.Registry {
    [CustomEditor(typeof(ItemProperties))]
    public class ItemPropertyEditor : UnityEditor.Editor
    {
        SerializedProperty weaponProp;
        SerializedProperty upgradeProp;
        SerializedProperty miscProp;
        SerializedProperty attachmentsProp;
 
        ItemProperties item;
 
        private void OnEnable()
        {
            weaponProp = serializedObject.FindProperty("WeaponProp"); //name of your settings2 property in your other file
            upgradeProp = serializedObject.FindProperty("UpgradeProp"); //name of your settings2 property in your other file
            miscProp = serializedObject.FindProperty("MiscProp"); //name of your settings2 property in your other file
            attachmentsProp = serializedObject.FindProperty("AttachmentsProp"); //name of your settings2 property in your other file
            item = (ItemProperties)target;
        }
        
        public override void OnInspectorGUI() {
            serializedObject.Update();

            item.ItemID = EditorGUILayout.TextField("Item ID", item.ItemID); 
            item.ItemName = EditorGUILayout.TextField("Item Name", item.ItemName); 
            item.Type = (ItemProperties.ItemType) EditorGUILayout.EnumPopup("Item Type", item.Type);
            item.PhysicalSprite = (Sprite) EditorGUILayout.ObjectField("Physical Sprite", item.PhysicalSprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            item.MenuIcon = (Sprite) EditorGUILayout.ObjectField("Menu Icon" ,item.MenuIcon, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            
            
            EditorGUILayout.LabelField("Description");
            item.Description = EditorGUILayout.TextArea(item.Description, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5)); 
            
            switch (item.Type) {
                case ItemProperties.ItemType.WEAPON:
                    EditorGUILayout.PropertyField(weaponProp, true);
                    break;
                case ItemProperties.ItemType.UPGRADE:
                    EditorGUILayout.PropertyField(upgradeProp, true);
                    break;
                case ItemProperties.ItemType.MISC:
                    EditorGUILayout.PropertyField(miscProp, true);
                    break;
                case ItemProperties.ItemType.ATTACHMENTS:
                    EditorGUILayout.PropertyField(attachmentsProp, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(item);
        }
    }
}
