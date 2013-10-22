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
        
        Camera.main.GetComponent<UniverseViewCamera>().UpdatePosition();
        
        if (!universeView.IsVisible())
        {
            UpdateMovementFromInput();
            
            if (inputHorizontal == 0 && inputJump == false)
                UpdateTilesModification();
        }
        else
        {
            UniverseEngine.Avatar avatar = (UniverseEngine.Avatar) universeObject;
            avatar.Walk(0);
        }
    }
    
    public override void OnParentChanged (TilemapCircle parent)
    {
        base.OnParentChanged (parent);
    }
    
    private void UpdateTilesModification()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 1)
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
    
    private void UpdateMovementFromInput()
    {
        UniverseEngine.Avatar avatar = (UniverseEngine.Avatar) universeObject;
        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //Input is updated in OnGUI()
            if (avatar.CanWalk())
                avatar.Walk(inputHorizontal);
   
            if (inputJump)
                if (avatar.CanJump())
                    avatar.Jump();
        }
        else
        {
            if (avatar.CanWalk())
                avatar.Walk(Input.GetAxis("Horizontal"));
            
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space))
                if (avatar.CanJump())
                    avatar.Jump();
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
