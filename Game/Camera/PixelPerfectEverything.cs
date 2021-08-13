using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PixelPerfectEverything : MonoBehaviour
{
    
    //TODO: Add abiltiy to call from editor
    //TODO: Add fix for sprite images. To make sure they are pixel perfect. Get their bottom left origin, pixel perfect that, and move the origin accordingly to the difference.
    
    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects( rootObjects );
        
 
        // iterate root objects and do something
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[ i ];
            PixelPerfect(gameObject.transform);
        }
    }

    public void PixelPerfect(Transform parent) {

        Vector3 pos = parent.position;
        pos.x = Mathf.Round(pos.x * 16) / 16;
        pos.y = Mathf.Round(pos.y * 16) / 16;
        parent.transform.position = pos;
        foreach (Transform child in parent) {
            PixelPerfect(child);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }


}
