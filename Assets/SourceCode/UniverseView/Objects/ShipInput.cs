#define ENABLE_ONGUI

using UnityEngine;

public enum ShipInputMode
{
    Move
}

public class ShipInput : MonoBehaviour
{
    public ShipView shipView;
    
    public float moveDirection;
    public float rotateDirection;
    
    static public ShipInputMode mode = ShipInputMode.Move;
    
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
            case ShipInputMode.Move:
                UpdateMove();
                UniverseViewCamera.Instance.UpdateZoomInput();
                break;
        }
    }
    
    public void ResetInput()
    {
        moveDirection = 0;
        rotateDirection = 0;
    }
    
    private void UpdateMove()
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
                    moveDirection = 1.0f;
                }

                if (touch1.position.x < Screen.width / 4.0f  && touch1.position.y < Screen.height * 0.25f ||
                    touchCount > 1 && touch2.position.x < Screen.width / 4.0f && touch2.position.y < Screen.height * 0.25f)
                {
                    rotateDirection = -1.0f;
                }
                else if (touch1.position.x < Screen.width / 2.0f  && touch1.position.y < Screen.height * 0.25f ||
                         touchCount > 1 && touch2.position.x < Screen.width / 2.0f && touch2.position.y < Screen.height * 0.25f)
                {
                    rotateDirection = 1.0f;
                }
            }
        }
        else
        {
            rotateDirection = Input.GetAxis("Horizontal");
            moveDirection = Input.GetAxis("Vertical");
        }
    }
    
#if ENABLE_ONGUI
    public void OnGUI()
    {
        if (GameLogic.Instace.State != GameLogicState.PlayingShip)
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
            case ShipInputMode.Move:
                DrawMoveGUI();
                break;
        }
    }
    
    private void DrawMoveGUI()
    {
        //Draw movement keys
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            InputAreas.AddInputArea(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width, Screen.height * 0.25f));
            
            GUI.Button(new Rect(0, Screen.height - Screen.height * 0.25f, Screen.width / 4, Screen.height * 0.25f), "Rotate Left");
            
            GUI.Button(new Rect(Screen.width / 4.0f, Screen.height - Screen.height * 0.25f, Screen.width / 4, Screen.height * 0.25f), "Rotate Right");
            
            GUI.Button(new Rect(Screen.width / 2.0f, Screen.height - Screen.height * 0.25f, Screen.width / 2, Screen.height * 0.25f), "Move Forward");
        }
        
        //Draw switch to avatar button
        InputAreas.AddInputArea(new Rect(Screen.width - (Screen.width / 8) * 3.0f, 0, Screen.width / 8, Screen.height / 8));
        if (GUI.Button(new Rect(Screen.width - (Screen.width / 8) * 3.0f, 0, Screen.width / 8, Screen.height / 8), "TO AVATAR"))
            GameLogic.Instace.SwitchToAvatar();
    }
#endif

}


