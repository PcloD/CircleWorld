using UnityEngine;

[ExecuteInEditMode]
public class GUICameraController : MonoBehaviour
{
    public Transform followRotation;
    
    public void Update()
    {
        camera.orthographicSize = (int) (Screen.width / 2.0f);
        
        if (followRotation)
            transform.rotation = followRotation.rotation;
    }
}


