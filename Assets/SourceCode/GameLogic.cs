using UnityEngine;
using System.Collections;
using UniverseEngine;

public enum GameLogicState
{
    PlayingAvatar,
    PlayingShip,
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
        
        switch(toState)
        {
            case GameLogicState.PlayingAvatar:
                universeCamera.FollowObject(universeView.avatarView.trans, FollowCameraParameters.FollowRotation | FollowCameraParameters.FollowScale, true);
                break;
                
            case GameLogicState.PlayingShip:
                universeCamera.FollowObject(universeView.shipView.trans, FollowCameraParameters.None, true);
                break;
        }
    }
    
    public void Awake()
    {
        Instace = this;
        
        Application.targetFrameRate = 60;
    }
    
	public void Start () 
    {
        universeView.Init(universeSeed);
        
        universeCamera.cameraDistance = 10;
        
        SwitchState(GameLogicState.PlayingAvatar);
	}
	
	public void Update() 
    {
        stateTime += Time.deltaTime;
        
        switch(state)
        {
            case GameLogicState.PlayingAvatar:
                universeTimeMultiplier = Mathf.SmoothDamp(universeTimeMultiplier, 1.0f, ref universeTimeMultiplierVelocity, 0.25f);
                universeView.UpdateUniverse(Time.deltaTime * universeTimeMultiplier);
                break;
                
            case GameLogicState.PlayingShip:
                universeTimeMultiplier = Mathf.SmoothDamp(universeTimeMultiplier, 1.0f, ref universeTimeMultiplierVelocity, 0.25f);
                universeView.UpdateUniverse(Time.deltaTime * universeTimeMultiplier);
                break;
                
            case GameLogicState.Travelling:
                universeTimeMultiplier = Mathf.SmoothDamp(universeTimeMultiplier, 0.1f, ref universeTimeMultiplierVelocity, 0.25f);
                universeView.UpdateUniverse(Time.deltaTime * universeTimeMultiplier);
                if (stateTime > 1.25f)
                    SwitchState(GameLogicState.PlayingAvatar);
                break;
        }
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
    
    public void SwitchToShip()
    {
        SwitchState(GameLogicState.PlayingShip);
    }

    public void SwitchToAvatar()
    {
        SwitchState(GameLogicState.PlayingAvatar);
    }
}
