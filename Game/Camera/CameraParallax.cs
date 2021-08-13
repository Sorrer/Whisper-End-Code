using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallax : MonoBehaviour
{
    [Serializable]
    public struct ParallaxObject {

        public GameObject GameObj;
        
        public float FocalMultplier;
        
        public Vector3 FocalPoint;
        
        public bool ParallaxX, ParallaxY, AlignToFocalPoint;
        
    }

    public List<ParallaxObject> ParallaxObjects = new List<ParallaxObject>();

    // Update is called once per frame
    void Update() {

        var cam = Camera.main;
        for (int i = 0; i < ParallaxObjects.Count; i++) {
            var obj = ParallaxObjects[i];

            Vector3 pos = obj.FocalPoint;
            pos = (pos - cam.transform.position) * obj.FocalMultplier;
            if (!obj.ParallaxX) pos.x = 0;
            if (!obj.ParallaxY) pos.y = 0;

            pos.z = 0;
            
            obj.GameObj.transform.position = pos + (obj.AlignToFocalPoint ? obj.FocalPoint : Vector3.zero);
        }
    }
}
