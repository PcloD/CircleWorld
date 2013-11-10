#define ENABLE_ONGUI

using UnityEngine;

public enum ShipInputMode
{
    Move
}

public class ShipViewInput : MonoBehaviour
{
    public ShipView shipView;
    
    static public ShipInputMode mode = ShipInputMode.Move;
    
    private GUIStyle centeredLabelStyle;
    private GUIStyle centeredBoxStyle;
    
    public void Awake()
    {
        useGUILayout = false;
    }
    
    public void Update()
    {
        if (GameLogic.Instace.State != GameLogicState.PlayingShip)
            return;
        
        switch(mode)
        {
            case ShipInputMode.Move:
                UpdateMove();
                UniverseViewCamera.Instance.UpdateZoomInput();
                break;
        }
    }
        
    private void UpdateMove()
    {
        UniverseEngine.ShipInput shipInput = ((UniverseEngine.Ship) shipView.UniverseObject).input;
        
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
                    shipInput.moveDirection = 1.0f;
                }

                if (touch1.position.x < Screen.width / 4.0f  && touch1.position.y < Screen.height * 0.25f ||
                    touchCount > 1 && touch2.position.x < Screen.width / 4.0f && touch2.position.y < Screen.height * 0.25f)
                {
                    shipInput.rotateDirection = -1.0f;
                }
                else if (touch1.position.x < Screen.width / 2.0f  && touch1.position.y < Screen.height * 0.25f ||
                         touchCount > 1 && touch2.position.x < Screen.width / 2.0f && touch2.position.y < Screen.height * 0.25f)
                {
                    shipInput.rotateDirection = 1.0f;
                }
            }
        }
        else
        {
            shipInput.rotateDirection = Input.GetAxisRaw("Horizontal");
            shipInput.moveDirection = Input.GetAxisRaw("Vertical");
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
        if (GUI.Button(new Rect(Screen.width - (Screen.width / 8) * 3.0f, 0, Screen.width / 8, Screen.height / 8), "LEAVE SHIP"))
        {
            int clickedThingIndex = shipView.UniverseView.Universe.FindClosestRenderedThing(shipView.UniverseObject.Position, 30.0f);
            if (clickedThingIndex >= 0)
                GameLogic.Instace.PlayerLeaveShip(shipView.UniverseView.Universe.GetPlanet((ushort) clickedThingIndex));
        }
    }
#endif

}


