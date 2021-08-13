using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePixel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        Vector3 pos = this.transform.position;
        pos.x = Mathf.Round(pos.x * 16) / 16;
        pos.y = Mathf.Round(pos.y * 16) / 16;
        //pos.z = Mathf.Round(pos.z * 16) / 16;

        this.transform.position = pos;
    }
}
