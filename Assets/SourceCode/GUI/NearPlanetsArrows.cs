using UnityEngine;
using UniverseEngine;
using System.Collections.Generic;

public class NearPlanetsArrows : MonoBehaviour
{
    public GameObject arrowPrefab;
    public int maxArrows = 10;
    public UniverseView universeView;
    public float searchRadius = 500;
    
    private PlanetArrow[] arrows;
    
    public void Awake()
    {
        arrows = new PlanetArrow[maxArrows];
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i] = ((GameObject) GameObject.Instantiate(arrowPrefab)).GetComponent<PlanetArrow>();
            arrows[i].trans.parent = transform;
            arrows[i].gameObject.SetActive(false);
        }
    }
    
    public void Update()
    {
        UpdateArrows();
    }
    
    private List<ushort> closeThings;
    
    private void UpdateArrows()
    {
        //Deactivate all arrows
        for (int i = 0; i < arrows.Length; i++)
            arrows[i].gameObject.SetActive(false);
        
        if (GameLogic.Instace.State != GameLogicState.PlayingShip)
            return;
        
        if (!UniverseViewCamera.Instance.FollowingObject)
            return;
        
        UniverseObjectView followingObjectView = UniverseViewCamera.Instance.FollowingObject.GetComponent<UniverseObjectView>();
        
        if (!followingObjectView)
            return;
        
        closeThings = universeView.Universe.FindClosestRenderedThings(followingObjectView.UniverseObject.Position, searchRadius, closeThings);
        
        float arrowDistance = new Vector2(Screen.width, Screen.height).magnitude * 0.25f;
        
        for (int i = 0; i < arrows.Length && i < closeThings.Count; i++)
        {
            Thing thing = universeView.Universe.GetThing(closeThings[i]);
            ThingPosition thingPosition = universeView.Universe.GetThingPosition(closeThings[i]);
            
            Vector2 thingPositionV2 = new Vector2(thingPosition.x, thingPosition.y);
            
            Vector2 delta = thingPositionV2 - followingObjectView.UniverseObject.Position;
            
            Vector2 closestThingPositionV2 = thingPositionV2 - delta.normalized * thingPosition.radius * 0.75f;
            
            Vector3 thingPositionOnCamera = UniverseViewCamera.Instance.camera.WorldToViewportPoint(closestThingPositionV2);
            
            if (thingPositionOnCamera.x >= 0 && thingPositionOnCamera.x <= 1 &&
                thingPositionOnCamera.y >= 0 && thingPositionOnCamera.y <= 1)
            {
                //Thing on screen, no need to draw arrow!
                continue;
            }
            
            PlanetArrow arrow = arrows[i];
            
            arrow.UpdatePlanet(thing);
            
            arrow.trans.position = (Vector3) (delta.normalized * arrowDistance);
            arrow.trans.rotation = Quaternion.AngleAxis(Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg, Vector3.forward);
            arrow.trans.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 2, 1.0f - delta.magnitude / searchRadius);
                
            arrow.trans.gameObject.SetActive(true);
            
            
            //Debug.Log("Arrow " + i + " -> " + closeThings[i]);
        }
    }
}


