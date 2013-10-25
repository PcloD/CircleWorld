using UnityEngine;

namespace SpriteMeshEngine
{
    [ExecuteInEditMode]
    public class SpriteMeshCamera : MonoBehaviour
    {
        public void OnPreRender()
        {
            camera.orthographicSize = (int) (Screen.height / 2.0f);
        }

        public void Start()
        {
            transform.position = new Vector3(0, 0, -10);
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public void Update()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                transform.position = new Vector3(0, 0, -10);
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
        }
    }
}

