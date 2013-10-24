#define ENABLE_ONGUI

using UnityEngine;

public enum AvatarInputMode
{
    Move,
    Edit
}

public enum AvatarInputEditTool
{
    None,
    Add,
    Remove
}

public class AvatarInput : MonoBehaviour
{
    public AvatarView avatarView;
    
    public float walkDirection;
    public bool jump;
    
    static public AvatarInputMode mode = AvatarInputMode.Move;
    static public AvatarInputEditTool editTool = AvatarInputEditTool.None;
    static private Rect[] inputAreas = new Rect[2];
    
    static private string[] EditToolNames = new string[] {
        "None",
        "Add Tiles",
        "Remove Tiles"
    };
    
    static private string[] EditToolTooltips = new string[] {
        "Select a tool",
        "Tap on empty spaces to add tiles",
        "Tap on tiles to remove them"
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
                break;
                
            case AvatarInputMode.Move:
                UpdateWalkAndJump();
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
            if (Input.touchCount == 1 && !IsInputArea(Input.GetTouch(0).position))
                modifyTile = GetTileCoordinatesUnderTouch(out tileX, out tileY);
        }
        else
        {
            if (Input.GetMouseButton(0) && !IsInputArea(Input.mousePosition))
                modifyTile = GetTileCoordinatesUnderMouse(out tileX, out tileY);
        }
        
        if (modifyTile)
        {
            switch(editTool)
            {
                case AvatarInputEditTool.Add:
                    avatarView.ParentView.TilemapCircle.SetTile(tileX, tileY, 1);
                    break;
                    
                case AvatarInputEditTool.Remove:
                    avatarView.ParentView.TilemapCircle.SetTile(tileX, tileY, 0);
                    break;
            }
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
    
    static public bool IsInputArea(Vector2 inputPosition)
    {
        //Convert from Input coordinate system to GUI coordinate system
        inputPosition.y = Screen.height - inputPosition.y;
        
        for (int i = 0; i < inputAreas.Length; i++)
            if (inputAreas[i].Contains(inputPosition))
                return true;
        
        return false;
    }

#if ENABLE_ONGUI
    public void OnGUI()
    {
        if (centeredLabelStyle == null)
        {
            centeredLabelStyle = new GUIStyle(GUI.skin.label);
            centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
            
            centeredBoxStyle = new GUIStyle(GUI.skin.box);
            centeredBoxStyle.alignment = TextAnchor.MiddleCenter;
        }
        
        inputAreas[0] = new Rect(0, 0, 0, 0); //Reset first input area
        
        switch(mode)
        {
            case AvatarInputMode.Move:
                DrawMoveGUI();
                break;
                
            case AvatarInputMode.Edit:
                DrawEditGUI();
                break;
        }
        
        inputAreas[1] = new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 10);
        
        if (GUI.Button(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 8), (mode == AvatarInputMode.Move) ? "EDIT" : "MOVE"))
        {
            if (mode == AvatarInputMode.Move)
            {
                mode = AvatarInputMode.Edit;
                editTool = AvatarInputEditTool.None;
            }
            else
            {
                mode = AvatarInputMode.Move;
            }
        }       
    }
    
    private void DrawEditGUI()
    {
        editTool = (AvatarInputEditTool) GUI.Toolbar(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width, Screen.height * 0.25f - 50), (int) editTool, EditToolNames);
        
        GUI.Box(new Rect(0, Screen.height - 50, Screen.width, 50), EditToolTooltips[(int) editTool], centeredBoxStyle);
        
        inputAreas[0] = new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width, Screen.height * 0.25f);
    }
    
    private void DrawMoveGUI()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GUI.Button(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width / 4, Screen.height * 0.25f), "Left");
            
            GUI.Button(new Rect(Screen.width / 4.0f, Screen.height - Screen.height * 0.25f, Screen.width / 4, Screen.height * 0.25f), "Right");
            
            GUI.Button(new Rect(Screen.width / 2.0f, Screen.height - Screen.height * 0.25f, Screen.width / 2, Screen.height * 0.25f), "Jump");
            
            inputAreas[0] = new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width, Screen.height * 0.25f);
        }
    }
#endif

}


