#define ENABLE_PERFORMANCE

using UnityEngine;
using UniverseEngine;
using System.Collections;

public class Performance : MonoBehaviour 
{
    private int frames;
    private float fps;
    private float time;
    private string performance;

#if ENABLE_PERFORMANCE
    public void Awake()
    {
        useGUILayout = false;
    }
    
	// Update is called once per frame
	public void Update () 
    {
        frames++;
        time += Time.deltaTime;

        if (time >= 1.0f)
        {
            fps = frames / time;
            time = 0.0f;
            frames = 0;

            performance = (System.GC.GetTotalMemory(false) / 1024).ToString();

            performance = string.Format("{0} kb\n{1} fps\nUpdatePositions {2} ms\nUpdateMesh {3} ms", 
                    System.GC.GetTotalMemory(false) / 1024,
                    fps,
                    UEProfiler.GetSampleTime("Universe.UpdatePositions").TotalMilliseconds,
                    UEProfiler.GetSampleTime("UniverseView.UpdateMesh").TotalMilliseconds);
            
            UEProfiler.Clear();
        }
	}

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 200), performance);
    }
#endif
}
