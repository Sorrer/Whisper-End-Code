

using System;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Game.Interactable.Highlight {

    public class DefaultItemHighlight : InteractHighlightEffect {

        public Material HighlightMaterial;
        public Material DefaultMaterial;

        public bool UseInverseColor = false;
        public Texture2D inverseColorSpriteTexture;
        private Color inverseColor;

        private MaterialPropertyBlock materialPropertyBlock;
        
        public MonoBehaviour lightScript;
        private void Start() {
            materialPropertyBlock = new MaterialPropertyBlock();
            
            if (!UseInverseColor) return;
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            Sprite currentSprite = sRenderer.sprite;
            Rect rect = currentSprite.rect;
            
            int CurrentColorCount = 0;
            float r = 0, g = 0, b = 0;
            
            for (int x = 0; x < rect.width; x++) {
                for (int y = 0; y < rect.height; y++) {

                    Color color = inverseColorSpriteTexture.GetPixel(x + (int) rect.x, y + (int) rect.y);

                    if (Math.Abs(color.a) < FloatComparer.kEpsilon) continue;
                    
                    r += color.r;
                    g += color.g;
                    b += color.b;

                    CurrentColorCount++;
                }
            }

            r /= CurrentColorCount;
            g /= CurrentColorCount;
            b /= CurrentColorCount;


            r = 1 - r;
            g = 1 - g;
            b = 1 - b;
            
            inverseColor = new Color(r, g, b, 1);
            
            

            sRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_Color", inverseColor);
            sRenderer.SetPropertyBlock(materialPropertyBlock);
        }


        public override void Highlight() {
            this.GetComponent<Renderer>().material = HighlightMaterial;
            //lightScript.enabled = true;
        }

        public override void HighlightInvalid() {
        }

        public override void UnHighlight() {
            //lightScript.enabled = false;
            this.GetComponent<Renderer>().material = DefaultMaterial;
        }
    }
}