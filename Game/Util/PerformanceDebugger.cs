using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceDebugger : MonoBehaviour
{

    public int LimitFPS;
    public bool VSync;

    [ContextMenu("UpdateFps")]
    void UpdateFPS() {
        Application.targetFrameRate = LimitFPS;
        QualitySettings.vSyncCount = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
