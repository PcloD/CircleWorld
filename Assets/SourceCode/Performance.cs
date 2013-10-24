#define ENABLE_PERFORMANCE

using UnityEngine;
using System.Collections;

public class Performance : MonoBehaviour 
{
    private int frames;
    private float fps;
    private float time;
    private string performance;

#if ENABLE_PERFORMANCE
	// Update is called once per frame
	void Update () 
    {
        frames++;
        time += Time.deltaTime;

        if (time >= 1.0f)
        {
            fps = frames / time;
            time = 0.0f;
            frames = 0;

            performance = string.Format("{0} kb - {1} fps", 
                    System.GC.GetTotalMemory(false) / 1024,
                    fps);
        }
	}

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), performance);
    }
#endif
}
