using UnityEngine;
using System.Collections;
using UniverseEngine;

public enum GameLogicState
{
    Playing,
    Travelling
}

public class GameLogic : MonoBehaviour 
{
    static public GameLogic Instace;
    
    public UniverseViewCamera universeCamera;
    public UniverseView universeView;
    
    public int universeSeed;
    
    private GameLogicState state;
    private float stateTime;
    
    private float universeTimeMultiplier = 1.0f;
    private float universeTimeMultiplierVelocity;
    
    public GameLogicState State
    {
        get { return state; }
    }
    
    public void SwitchState(GameLogicState toState)
    {
        this.state = toState;
        stateTime = 0.0f;
    }
    
    public void Awake()
    {
        Instace = this;
        
        Application.targetFrameRate = 60;
    }
    
	public void Start () 
    {
        universeView.Init(universeSeed);
        
        universeCamera.FollowingObject = universeView.avatarView;
        universeCamera.cameraDistance = 10;
	}
	
	public void Update() 
    {
        stateTime += Time.deltaTime;
        
        switch(state)
        {
            case GameLogicState.Playing:
                universeTimeMultiplier = Mathf.SmoothDamp(universeTimeMultiplier, 1.0f, ref universeTimeMultiplierVelocity, 0.25f);
                universeView.UpdateUniverse(Time.deltaTime * universeTimeMultiplier);
                universeCamera.UpdatePosition();
                universeView.avatarView.avatarInput.UpdateInput();
                universeView.avatarView.ProcessInput();
                universeCamera.UpdateZoomInput();
                
                if (AvatarInput.mode == AvatarInputMode.Move)
                    universeCamera.UpdateClickOnPlanetToTravel(universeView);
                break;
                
            case GameLogicState.Travelling:
                universeTimeMultiplier = Mathf.SmoothDamp(universeTimeMultiplier, 0.1f, ref universeTimeMultiplierVelocity, 0.25f);
                universeView.UpdateUniverse(Time.deltaTime * universeTimeMultiplier);
                
                universeCamera.UpdateZoomInput();
                if (universeCamera.UpdatePositionSmooth())
                {
                    universeView.avatarView.avatarInput.ResetInput();
                    SwitchState(GameLogicState.Playing);
                }
                break;
        }
        
        //universeView.SetVisible(universeCamera.cameraDistance > 70);
	}
    
    public void TravelToPlanet(PlanetView targetPlanetView)
    {
        universeView.avatarView.UniverseObject.SetParent(
            targetPlanetView.TilemapCircle,
            targetPlanetView.TilemapCircle.GetPositionFromTileCoordinate(0, targetPlanetView.TilemapCircle.Height)
        );
        
        //Force update to update AvatarView position
        universeView.UpdateUniverse(Time.deltaTime);
        
        SwitchState(GameLogicState.Travelling);
    }
}
