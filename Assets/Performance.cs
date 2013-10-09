using UnityEngine;
using System.Collections;

public class Performance : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    
    public void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), (System.GC.GetTotalMemory(false) / 1024).ToString() + "kb");
    }
}
