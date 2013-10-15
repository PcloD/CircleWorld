using UnityEngine;
using System.Collections;
using Universe;

public class GameLogic : MonoBehaviour 
{
    public GameObject avatarPrefab;
    public GameObject planetPrefab;
    
    public UniverseViewCamera universeCamera;
    public UniverseView universeView;
    
    public int planetRadius = 25;
    public int planetSeed;
    public int universeSeed;
    
    private Universe.Avatar avatar;
    private AvatarView avatarView;
    
    private TilemapCircle planet;
    private TilemapCircleView planetView;
    
    public void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
	public void Start () 
    {
        universeView.Init(universeSeed);
        
        planet = new TilemapCircle();
        planet.Init(planetSeed, planetRadius);
        planet.Position = new Vector2(universeView.AvatarPlanetPosition.x, universeView.AvatarPlanetPosition.y);
        
        planetView = ((GameObject) GameObject.Instantiate(planetPrefab)).GetComponent<TilemapCircleView>();
        planetView.Init(planet);
        
        avatar = new Universe.Avatar();
        avatar.Init(new Vector2(0.75f, 1.5f), planet, planet.GetPositionFromTileCoordinate(0, planetRadius));
        
        avatarView = ((GameObject) GameObject.Instantiate(avatarPrefab)).GetComponent<AvatarView>();
        avatarView.Init(avatar, planetView);
        
        universeCamera.FollowingObject = avatarView;
        universeCamera.cameraDistance = 10;
	}
	
	void Update () 
    {
        universeView.UpdatePositionsAndMesh();
        
        planet.Position = new Vector2(universeView.AvatarPlanetPosition.x, universeView.AvatarPlanetPosition.y);
        
        planetView.OnTilemapCircleUpdated();
        
	    avatar.UpdatePosition(Time.deltaTime);
        
        avatarView.OnTilemapObjectUpdated();
        
        universeView.SetVisible(universeCamera.cameraDistance > 70);
	}
}
