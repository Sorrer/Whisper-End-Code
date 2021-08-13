using Game;
using Game.Input;
using UnityEngine;

public class PlayerArms : MonoBehaviour {
    private GameInput gi;
    private SpriteRenderer sr;
    private Camera mainCam;
    
    void Start() {
        gi = MainInstances.Get<GameInput>();
        sr = GetComponent<SpriteRenderer>();
        mainCam = Camera.main;
    }
    
    private void LateUpdate() {
        var aimVec = gi.GetAimDir(mainCam.WorldToScreenPoint(transform.position));

        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(aimVec.y, aimVec.x) * Mathf.Rad2Deg);
        
        transform.localScale = new Vector3(1, aimVec.x > 0 ? 1 : -1);
        
        //Vector2 offVec;
        
        //offVec = new Vector2(0.16f, 0f);

        //var eulers = transform.eulerAngles;

        /*if (aimVec.x < 0) {
            transform.eulerAngles = new Vector3(0, 180, eulers.z);
        } else {
            transform.eulerAngles = new Vector3(0, 0, eulers.z);
        }*/
        
        
        

        //transform.localPosition = /* aimVec.normalized + */ offVec; 
           
    }
}
