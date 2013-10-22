//#define ENABLE_ONGUI

using UnityEngine;
using System.Collections;
using UniverseEngine;

public class AvatarView : UniverseObjectView
{
    private float inputHorizontal;
    private bool inputJump;
    
    public override void OnUniverseObjectUpdated(float deltaTime)
    {
        UpdatePosition();
    }
    
    public override void OnParentChanged (TilemapCircle parent)
    {
        base.OnParentChanged (parent);
    }
    
    private void UpdateTilesModification()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                int tileX, tileY;
                if (GetTileCoordinatesUnderTouch(out tileX, out tileY))
                    parentView.TilemapCircle.SetTile(tileX, tileY, 0);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                int tileX, tileY;
                if (GetTileCoordinatesUnderMouse(out tileX, out tileY))
                    parentView.TilemapCircle.SetTile(tileX, tileY, 0);
            }
            else if (Input.GetMouseButton(1))
            {
                int tileX, tileY;
                if (GetTileCoordinatesUnderMouse(out tileX, out tileY))
                    parentView.TilemapCircle.SetTile(tileX, tileY, 1);
            }
        }
	}
    
    public void UpdateFromInput()
    {
        UniverseEngine.Avatar avatar = (UniverseEngine.Avatar) universeObject;
        
        if (avatar.CanWalk())
            avatar.Walk(inputHorizontal);

        if (inputJump)
            if (avatar.CanJump())
                avatar.Jump();

        if (inputHorizontal == 0 && inputJump == false)
            UpdateTilesModification();
    }

    public void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int touchCount = Input.touchCount;
            Touch touch1 = (touchCount > 0 ? Input.GetTouch(0) : new Touch());
            Touch touch2 = (touchCount > 1 ? Input.GetTouch(1) : new Touch());

            inputHorizontal = 0;
            inputJump = false;

            if (touchCount >= 1)
            {
                if (touch1.position.x > Screen.width / 2.0f && touch1.position.y < Screen.height * 0.33f ||
                    touchCount > 1 && touch2.position.x > Screen.width / 2.0f  && touch2.position.y < Screen.height * 0.33f)
                {
                    inputJump = true;
                }

                if (touch1.position.x < Screen.width / 4.0f  && touch1.position.y < Screen.height * 0.33f ||
                    touchCount > 1 && touch2.position.x < Screen.width / 4.0f && touch2.position.y < Screen.height * 0.33f)
                {
                    inputHorizontal = -1.0f;
                }
                else if (touch1.position.x < Screen.width / 2.0f  && touch1.position.y < Screen.height * 0.33f ||
                         touchCount > 1 && touch2.position.x < Screen.width / 2.0f && touch2.position.y < Screen.height * 0.33f)
                {
                    inputHorizontal = 1.0f;
                }
            }
        }
        else
        {
            inputHorizontal = Input.GetAxis("Horizontal");

            inputJump = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space);
        }
    }

    private bool GetTileCoordinatesUnderMouse(out int tileX, out int tileY)
    {
        Camera cam = Camera.main;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        return parentView.TilemapCircle.GetTileCoordinatesFromPosition(worldPos, out tileX, out tileY);
    }
    
    private bool GetTileCoordinatesUnderTouch(out int tileX, out int tileY)
    {
        Camera cam = Camera.main;
  
        Vector2 touchPosition = Input.GetTouch(0).position;
        
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0));

        return parentView.TilemapCircle.GetTileCoordinatesFromPosition(worldPos, out tileX, out tileY);
    }

#if ENABLE_ONGUI
    public void OnGUI()
    {
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            return;
            
        float bottom = Screen.height - 10;
        float left = 10;
        float size = (int) (Mathf.Max(Screen.width, Screen.height) / 10);
        float space = size / 6;
        
        inputHorizontal = 0;
        inputJump = false;
        
        if (GUI.RepeatButton(new Rect(left, bottom - size, size, size), "Left"))
            inputHorizontal = -1;

        if (GUI.RepeatButton(new Rect(left + (size + space) * 1, bottom - size, size, size), "Right"))
            inputHorizontal = 1;

        if (GUI.RepeatButton(new Rect(Screen.width - (size + space) * 1, bottom - size, size, size), "Jump"))
            inputJump = true;
    }
#endif

    public override void OnDrawGizmos ()
    {
        float sizeY = 1.05f;
        float sizeX = 0.75f;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * sizeY);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + transform.up * sizeY * 0.5f - transform.right * sizeX * 0.5f, transform.position + transform.up * sizeY * 0.5f + transform.right * sizeX * 0.5f);
    }
}
