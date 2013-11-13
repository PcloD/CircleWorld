using UnityEngine;

[ExecuteInEditMode]
public class GUICameraController : MonoBehaviour
{
    public void Update()
    {
        int orthographicSize = (int) (Screen.width / 2.0f);

        if (camera.orthographicSize !=  orthographicSize)
            camera.orthographicSize = orthographicSize;
    }
}


