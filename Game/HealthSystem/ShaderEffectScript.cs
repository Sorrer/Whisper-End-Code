using System.Collections;
using UnityEngine;

namespace Game.HealthSystem {
    public class ShaderEffectScript : MonoBehaviour {

        private Material material;

        public Gradient FlashColor;
        public Gradient LowHealthFlashColor;
        public float EffectTime;

        public AnimationCurve EffectCurve;
    
        public float MaxWaveStrength, MaxWaveFrequency, MaxWaveAmount;

        private Coroutine effectCoroutine;


        private MaterialPropertyBlock materialPropertyBlock;

        private SpriteRenderer sRenderer;
        
        // Start is called before the first frame update
        void Start() {
            sRenderer = GetComponent<SpriteRenderer>();
            material = sRenderer.material;
            
            

            nStength = Shader.PropertyToID("_WaveStrength");
            nFreq = Shader.PropertyToID("_WaveFrequency");
            nAmount = Shader.PropertyToID("_WaveAmount");
            nPercent = Shader.PropertyToID("_Percent");
            nColor = Shader.PropertyToID("_Color");
            
            materialPropertyBlock = new MaterialPropertyBlock();
            
        }

        public IEnumerator DoEffect() {

            float currentEffectTime = 0;

            while (currentEffectTime < EffectTime) {

                currentEffectTime += Time.deltaTime;
                float progress = currentEffectTime / EffectTime;
                UpdateEffect(EffectCurve.Evaluate(progress), progress);
            
                yield return null;
            }
        
            ClearEffect();
        }

        private int nStength, nFreq, nAmount, nPercent, nColor;

        private float curStrength, curFreq, curAmount, curPercent;
        private Color curColor = Color.white;
    
        public void UpdateEffect(float progress, float nonCurvedProgress) {

            curPercent = progress;
            curFreq = MaxWaveFrequency * progress;
            curAmount = MaxWaveAmount * progress;
            curStrength = MaxWaveStrength * progress;

            if (IsLowHealth) {
                curColor = LowHealthFlashColor.Evaluate(nonCurvedProgress);
            } else {
                curColor = this.FlashColor.Evaluate(nonCurvedProgress);
            }
        
            UpdateMaterial();
        }


        public void UpdateMaterial() {
            sRenderer.GetPropertyBlock(materialPropertyBlock);
            
            materialPropertyBlock.SetFloat(nPercent, curPercent);
            materialPropertyBlock.SetFloat(nStength, curStrength);
            materialPropertyBlock.SetFloat(nAmount, curAmount);
            materialPropertyBlock.SetFloat(nFreq, curFreq);
            materialPropertyBlock.SetColor(nColor, curColor);
            
            sRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void ClearEffect() {
            curPercent = 0;
            curStrength = 0;
            curAmount = 0;
            curFreq = 0;
            UpdateMaterial();
        }
    
    
        private bool IsLowHealth;
        public HealthBase healthSystem;
    
        public void ActivateEffect() {
            if (healthSystem != null) {
                IsLowHealth = healthSystem.dHealth < 0.2 * healthSystem.dMaxHealth;
            } else {
                IsLowHealth = false;
            }
            
            if (effectCoroutine != null) {
                StopCoroutine(effectCoroutine);
                effectCoroutine = null;
            }

            effectCoroutine = StartCoroutine((DoEffect()));
        }
    }
}
