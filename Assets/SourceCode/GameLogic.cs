using UnityEngine;
using System.Collections;
using UniverseEngine;

public class GameLogic : MonoBehaviour 
{
    public UniverseViewCamera universeCamera;
    public UniverseView universeView;
    
    public int universeSeed;
    
    public void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
	public void Start () 
    {
        universeView.Init(universeSeed);
        
        universeCamera.FollowingObject = universeView.avatarView;
        universeCamera.cameraDistance = 10;
	}
	
	void Update () 
    {
        universeView.UpdateUniverse(Time.deltaTime);
        
        universeView.SetVisible(universeCamera.cameraDistance > 70);
	}
}
