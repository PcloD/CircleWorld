#define ENABLE_ONGUI

using UnityEngine;

public enum AvatarInputMode
{
    Move,
    Edit,
    TravelToPlanet
}

public enum AvatarInputEditTool
{
    None,
    Add,
    Remove,
    MoveCamera
}

public class AvatarInput : MonoBehaviour
{
    public AvatarView avatarView;
    
    public float walkDirection;
    public bool jump;
    
    static public AvatarInputMode mode = AvatarInputMode.Move;
    static public AvatarInputEditTool editTool = AvatarInputEditTool.None;
        
    static private string[] EditToolNames = new string[] {
        "None",
        "Add Tiles",
        "Remove Tiles",
        "Move Camera"
    };
    
    static private string[] EditToolTooltips = new string[] {
        "Select a tool",
        "Tap on empty spaces to add tiles",
        "Tap on tiles to remove them",
        "Move the camera"
    };
    
    private GUIStyle centeredLabelStyle;
    private GUIStyle centeredBoxStyle;
    
    public void Awake()
    {
        useGUILayout = false;
    }
    
    public void UpdateInput()
    {
        switch(mode)
        {
            case AvatarInputMode.Edit:
                UpdateTilesModification();
                UniverseViewCamera.Instance.UpdateZoomInput();
                break;
                
            case AvatarInputMode.Move:
                UpdateWalkAndJump();
                UniverseViewCamera.Instance.UpdateZoomInput();
                break;
                
            case AvatarInputMode.TravelToPlanet:
                UniverseViewCamera.Instance.UpdateZoomInput();
                UniverseViewCamera.Instance.UpdateClickOnPlanetToTravel(avatarView.UniverseView);
                break;
        }
    }
    
    public void ResetInput()
    {
        walkDirection = 0;
        jump = false;
    }
    
    private void UpdateWalkAndJump()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int touchCount = Input.touchCount;
            Touch touch1 = (touchCount > 0 ? Input.GetTouch(0) : new Touch());
            Touch touch2 = (touchCount > 1 ? Input.GetTouch(1) : new Touch());


            if (touchCount >= 1)
            {
                if (touch1.position.x > Screen.width / 2.0f && touch1.position.y < Screen.height * 0.25f ||
                    touchCount > 1 && touch2.position.x > Screen.width / 2.0f  && touch2.position.y < Screen.height * 0.25f)
                {
                    jump = true;
                }

                if (touch1.position.x < Screen.width / 4.0f  && touch1.position.y < Screen.height * 0.25f ||
                    touchCount > 1 && touch2.position.x < Screen.width / 4.0f && touch2.position.y < Screen.height * 0.25f)
                {
                    walkDirection = -1.0f;
                }
                else if (touch1.position.x < Screen.width / 2.0f  && touch1.position.y < Screen.height * 0.25f ||
                         touchCount > 1 && touch2.position.x < Screen.width / 2.0f && touch2.position.y < Screen.height * 0.25f)
                {
                    walkDirection = 1.0f;
                }
            }
        }
        else
        {
            walkDirection = Input.GetAxis("Horizontal");

            jump = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space);
        }
    }

    public void UpdateTilesModification()
    {
        bool modifyTile = false;
        int tileX = 0;
        int tileY = 0;
        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 1 && !InputAreas.IsInputArea(Input.GetTouch(0).position))
                modifyTile = GetTileCoordinatesUnderTouch(out tileX, out tileY);
        }
        else
        {
            if (Input.GetMouseButton(0) && !InputAreas.IsInputArea(Input.mousePosition))
                modifyTile = GetTileCoordinatesUnderMouse(out tileX, out tileY);
        }
        
        switch(editTool)
        {
            case AvatarInputEditTool.Add:
                if (modifyTile)
                    avatarView.ParentView.TilemapCircle.SetTile(tileX, tileY, 1);
                break;
                
            case AvatarInputEditTool.Remove:
                if (modifyTile)
                    avatarView.ParentView.TilemapCircle.SetTile(tileX, tileY, 0);
                break;
                
            case AvatarInputEditTool.MoveCamera:
                UniverseViewCamera.Instance.UpdateMove();
                break;
        }
    }

    private bool GetTileCoordinatesUnderMouse(out int tileX, out int tileY)
    {
        Camera cam = Camera.main;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        return avatarView.ParentView.TilemapCircle.GetTileCoordinatesFromPosition(worldPos, out tileX, out tileY);
    }
    
    private bool GetTileCoordinatesUnderTouch(out int tileX, out int tileY)
    {
        Camera cam = Camera.main;
  
        Vector2 touchPosition = Input.GetTouch(0).position;
        
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 0));

        return avatarView.ParentView.TilemapCircle.GetTileCoordinatesFromPosition(worldPos, out tileX, out tileY);
    }    
    
#if ENABLE_ONGUI
    public void OnGUI()
    {
        if (GameLogic.Instace.State != GameLogicState.PlayingAvatar)
            return;
        
        if (centeredLabelStyle == null)
        {
            centeredLabelStyle = new GUIStyle(GUI.skin.label);
            centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
            
            centeredBoxStyle = new GUIStyle(GUI.skin.box);
            centeredBoxStyle.alignment = TextAnchor.MiddleCenter;
        }
        
        InputAreas.ResetInputAreas();
        
        switch(mode)
        {
            case AvatarInputMode.Move:
                DrawMoveGUI();
                break;
                
            case AvatarInputMode.Edit:
                DrawEditGUI();
                break;
                
            case AvatarInputMode.TravelToPlanet:
                DrawTravelToPlanetGUI();
                break;
        }
    }
    
    private void DrawEditGUI()
    {
        //Draw toolbar
        InputAreas.AddInputArea(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width, Screen.height * 0.25f));
        editTool = (AvatarInputEditTool) GUI.Toolbar(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width, Screen.height * 0.25f - 50), (int) editTool, EditToolNames);
        GUI.Box(new Rect(0, Screen.height - 50, Screen.width, 50), EditToolTooltips[(int) editTool], centeredBoxStyle);
        
        //Draw cancel button
        InputAreas.AddInputArea(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 8));
        if (GUI.Button(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 8), "EXIT\nEDIT"))
            mode = AvatarInputMode.Move;
    }
    
    private void DrawTravelToPlanetGUI()
    {
        //Draw cancel button
        InputAreas.AddInputArea(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 8));
        if (GUI.Button(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 8), "EXIT\nTRAVEL"))
            mode = AvatarInputMode.Move;
        
        GUI.Box(new Rect(0, Screen.height - 50, Screen.width, 50), "Tap on a planet to travel", centeredBoxStyle);
    }
    
    private void DrawMoveGUI()
    {
        //Draw movement keys
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            InputAreas.AddInputArea(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width, Screen.height * 0.25f));
            
            GUI.Button(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width / 4, Screen.height * 0.25f), "Left");
            
            GUI.Button(new Rect(Screen.width / 4.0f, Screen.height - Screen.height * 0.25f, Screen.width / 4, Screen.height * 0.25f), "Right");
            
            GUI.Button(new Rect(Screen.width / 2.0f, Screen.height - Screen.height * 0.25f, Screen.width / 2, Screen.height * 0.25f), "Jump");
        }
        
        //Draw travel button
        InputAreas.AddInputArea(new Rect(Screen.width - (Screen.width / 8) * 2.0f, 0, Screen.width / 8, Screen.height / 8));
        if (GUI.Button(new Rect(Screen.width - (Screen.width / 8) * 2.0f, 0, Screen.width / 8, Screen.height / 8), "TRAVEL"))
            mode = AvatarInputMode.TravelToPlanet;
        
        //Draw edit button
        InputAreas.AddInputArea(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 8));
        if (GUI.Button(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 8), "EDIT"))
        {
            mode = AvatarInputMode.Edit;
            editTool = AvatarInputEditTool.None;
        }    
        
        //Draw switch to ship button
        InputAreas.AddInputArea(new Rect(Screen.width - (Screen.width / 8) * 3.0f, 0, Screen.width / 8, Screen.height / 8));
        if (GUI.Button(new Rect(Screen.width - (Screen.width / 8) * 3.0f, 0, Screen.width / 8, Screen.height / 8), "TO SHIP"))
            GameLogic.Instace.SwitchToShip();
    }
#endif

}


