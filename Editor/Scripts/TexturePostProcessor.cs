using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TexturePostProcessor : AssetPostprocessor {

    void OnPostprocessSprites(Texture2D texture, Sprite[] sprites) {
        ProcessTexture();
    }

    void OnPostprocessTexture(Texture2D texture) {
        ProcessTexture();
    }


    public void ProcessTexture() {

        if (!assetPath.Contains("Sprites")) {
            return;
        }

        EditorUtility.SetDirty(assetImporter);


        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spritePixelsPerUnit = 16;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.isReadable = true;

        Debug.Log("Imported sprite(s): " + textureImporter.assetPath);

        assetImporter.SaveAndReimport();
    }

}
