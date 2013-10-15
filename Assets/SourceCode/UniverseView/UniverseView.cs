using UnityEngine;
using Universe;
using System.Collections.Generic;

public class UniverseView : MonoBehaviour
{
    public UniverseViewFactory universeFactory;
    
    [HideInInspector]
    [System.NonSerialized]
    public AvatarView avatarView;

    private UniverseContainer universeContainer = new UniverseContainer();
    
    private List<PlanetView> planetViews = new List<PlanetView>(32);
    private List<TilemapObjectView> tilemapObjectViews = new List<TilemapObjectView>(32);

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    
    private Renderer rend;
    private Transform trans;
    
    public UniverseContainer UniverseContainer
    {
        get { return universeContainer; }
    }
        
    public void Awake()
    {
        rend = renderer;
        trans = transform;
        
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
    }
    
    public void Init(int seed)
    {
        CreateUniverse(seed);
        
        AddAvatar();
        
        UpdateMesh(true);
    }
    
    private void CreateUniverse(int seed)
    {
        universeContainer = new UniverseContainer();
        
        universeContainer.Init(seed);
    }
    
    public PlanetView GetPlanetView(ushort thingIndex)
    {
        for (int i = 0; i < planetViews.Count; i++)
            if (planetViews[i].Planet.ThingIndex == thingIndex)
                return planetViews[i];
        
        if (planetViews.Count > 1)
        {
            universeContainer.ReturnPlanet(planetViews[0].Planet);
            universeFactory.ReturnPlanet(planetViews[0]);
            
            planetViews.RemoveAt(0);
        }
        
        Planet planet = universeContainer.GetPlanet(thingIndex);
        
        PlanetView planetView = universeFactory.GetPlanet(planet.Height);
        
        planetView.InitPlanet(planet, this);
        
        planetViews.Add(planetView);
        
        //Debug.Log(planetViews.Count);
        
        return planetView;
    }
    
    private void AddAvatar()
    {
        avatarView = universeFactory.GetAvatar();
        avatarView.Init(universeContainer.Avatar, this);
        
        AddTilemapObjectView(avatarView);
    }
 
    /// <summary>
    /// Called by GameLogic
    /// </summary>
    public void UpdateUniverse(float deltaTime)
    {
        universeContainer.UpdateUniverse(deltaTime);
        
        if (IsVisible())
        {
            UpdateMesh(false);
        
            UpdateClickOnPlanetToTravel();
        }
        
        for (int i = 0; i < planetViews.Count; i++)
            planetViews[i].OnTilemapCircleUpdated();
        
        for (int i = 0; i < tilemapObjectViews.Count; i++)
            tilemapObjectViews[i].OnTilemapObjectUpdated();
    }
    
    private void UpdateClickOnPlanetToTravel()
    {
        bool clickTravel = false;
        Vector2 clickPosition = Vector2.zero;
        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                clickTravel = true;
                clickPosition = Input.GetTouch(0).position;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickTravel = true;
                clickPosition = Input.mousePosition;
            }
        }
        
        if (clickTravel)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(clickPosition);
            Vector2 worldPosTolerance = Camera.main.ScreenToWorldPoint(clickPosition + Vector2.right * (Screen.dpi > 0 ? Screen.dpi : 72) / 2.54f); //1 cm tolerance
            
            float clickTolerance = (worldPosTolerance - worldPos).magnitude;
            
            ushort closestThingIndex = ushort.MaxValue;
            float closestThingDistance = float.MaxValue;
            
            ThingPosition[] thingsPositions = universeContainer.ThingsPositions;
            ushort[] thingsToRender = universeContainer.ThingsToRender;
            ushort thingsToRenderAmount = universeContainer.ThingsToRenderAmount;
            
            for (ushort i = 0; i < thingsToRenderAmount; i++)
            {
                ThingPosition thingPosition = thingsPositions[thingsToRender[i]];
                
                float distance = (worldPos - new Vector2(thingPosition.x, thingPosition.y)).sqrMagnitude;
                
                if (distance < (thingPosition.radius + clickTolerance) * (thingPosition.radius + clickTolerance) && 
                    distance < closestThingDistance)
                {
                    closestThingIndex = thingsToRender[i];
                    closestThingDistance = distance;
                }
            }
            
            if (closestThingIndex != ushort.MaxValue)
            {
                PlanetView targetPlanetView = GetPlanetView(closestThingIndex);
                
                avatarView.TilemapObject.SwitchToTilemapCircle(
                    targetPlanetView.TilemapCircle,
                    targetPlanetView.TilemapCircle.GetPositionFromTileCoordinate(0, targetPlanetView.TilemapCircle.Height)
                );
            }
        }
    }
    
    public void SetVisible(bool visible)
    {
        rend.enabled = visible;
    }
    
    public bool IsVisible()
    {
        return rend.enabled;
    }

    private void UpdateMesh(bool firstTime)
    {
        Thing[] things = universeContainer.Things;
        ThingPosition[] thingsPositions = universeContainer.ThingsPositions;
        ushort[] thingsToRender = universeContainer.ThingsToRender;
        ushort thingsToRenderAmount = universeContainer.ThingsToRenderAmount;

        int vertexCount = thingsToRenderAmount * 4;
        int triangleCount = thingsToRenderAmount * 6;

        if (vertices == null || vertices.Length != vertexCount)
            vertices = new Vector3[vertexCount];

        if (uvs == null || uvs.Length != vertexCount)
            uvs = new Vector2[vertexCount];

        if (triangles == null || triangles.Length != triangleCount)
            triangles = new int[triangleCount];

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
        }

        int vertexOffset = 0;

        if (firstTime)
        {
            float tx = 1.0f / 4.0f;
            float ty = 1.0f / 4.0f;
            float tt = 1.0f / 256.0f;

            int triangleOffset = 0;

            for (ushort i = 0; i < thingsToRenderAmount; i++)
            {
                Thing thing = things[thingsToRender[i]];

                int textureId = (int) (((uint) thing.seed) % 16);

                float uvx = (textureId % 4) / 4.0f;
                float uvy = (textureId / 4) / 4.0f;

                uvs[vertexOffset + 0] = new Vector2(uvx, 1.0f - uvy);
                uvs[vertexOffset + 1] = new Vector2(uvx + tx - tt, 1.0f - uvy);
                uvs[vertexOffset + 2] = new Vector2(uvx + tx - tt, 1.0f - (uvy + ty) + tt);
                uvs[vertexOffset + 3] = new Vector2(uvx, 1.0f - (uvy + ty) + tt);

                triangles[triangleOffset + 0] = vertexOffset + 0;
                triangles[triangleOffset + 1] = vertexOffset + 1;
                triangles[triangleOffset + 2] = vertexOffset + 2;

                triangles[triangleOffset + 3] = vertexOffset + 2;
                triangles[triangleOffset + 4] = vertexOffset + 3;
                triangles[triangleOffset + 5] = vertexOffset + 0;

                triangleOffset += 6;
                vertexOffset += 4;
            }
        }

        vertexOffset = 0;
  
        ushort avatarPlanet;
        
        if (!IsVisible())
            avatarPlanet = (avatarView.TilemapObject.tilemapCircle as Planet).ThingIndex;
        else
            avatarPlanet = ushort.MaxValue;
        
        for (int i = 0; i < thingsToRenderAmount; i++)
        {
            ushort thingIndex = thingsToRender[i];
            
            if (avatarPlanet != thingIndex)
            {
                ThingPosition position = thingsPositions[thingIndex];
            
                vertices[vertexOffset + 0].x = position.x - position.radius;
                vertices[vertexOffset + 0].y = position.y - position.radius;
    
                vertices[vertexOffset + 1].x = position.x - position.radius;
                vertices[vertexOffset + 1].y = position.y + position.radius;
    
                vertices[vertexOffset + 2].x = position.x + position.radius;
                vertices[vertexOffset + 2].y = position.y + position.radius;
    
                vertices[vertexOffset + 3].x = position.x + position.radius;
                vertices[vertexOffset + 3].y = position.y - position.radius;
            }
            else
            {
                vertices[vertexOffset + 0].x = 0;
                vertices[vertexOffset + 0].y = 0;
    
                vertices[vertexOffset + 1].x = 0;
                vertices[vertexOffset + 1].y = 0;
    
                vertices[vertexOffset + 2].x = 0;
                vertices[vertexOffset + 2].y = 0;
    
                vertices[vertexOffset + 3].x = 0;
                vertices[vertexOffset + 3].y = 0;
            }            

            vertexOffset += 4;
        }

        mesh.vertices = vertices;

        if (firstTime)
        {
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.bounds = new Bounds(Vector3.zero, new Vector3(ushort.MaxValue * 2, ushort.MaxValue * 2, 0.0f));

            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
    }
    
    public void AddTilemapObjectView(TilemapObjectView tilemapObjectView)
    {
        tilemapObjectViews.Add(tilemapObjectView);
    }
}


