using UnityEngine;
using UniverseEngine;
using System.Collections.Generic;

public class UniverseView : MonoBehaviour, IUniverseListener
{
    public UniverseViewFactory universeFactory;
    
    public int MaxActivePlanetViews = 2;
    
    [HideInInspector]
    [System.NonSerialized]
    public AvatarView avatarView;

    [HideInInspector]
    [System.NonSerialized]
    public ShipView shipView;
    
    private Universe universe = new Universe();
    
    private PlanetView[] planetViews = new PlanetView[Universe.MAX_THINGS];
    private List<PlanetView> activePlanetViews = new List<PlanetView>(32);
    private List<UniverseObjectView> tilemapObjectViews = new List<UniverseObjectView>(32);

    private Mesh mesh1;
    private Mesh mesh2;
    private int frameCount;
    
    private Renderer rend;
    private Transform trans;
    
    private PlanetType[] planetTypes;

    private MeshFilter meshFilter;
    
    public Universe Universe
    {
        get { return universe; }
    }
        
    public void Awake()
    {
        rend = renderer;
        trans = transform;
        
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
        
        renderer.sharedMaterial.mainTexture = SpriteMeshEngine.SpriteSheetManager.GetSpriteSheet("Planets").Texture;
        
        planetTypes = PlanetTypes.GetPlanetTypes();

        meshFilter = GetComponent<MeshFilter>();
        
        mesh1 = new Mesh();
        
        mesh2 = new Mesh();
    }
    
    public void Init(int seed)
    {
        CreateUniverse(seed);
        
        UpdateMesh(true);
    }
    
    private void CreateUniverse(int seed)
    {
        universe = new Universe();
        
        universe.Init(seed, this);
    }
    
    public PlanetView GetPlanetView(TilemapCircle tilemapCircle)
    {
        if (tilemapCircle is Planet)
            return GetPlanetView(((Planet) tilemapCircle).ThingIndex);
        
        return null;
    }
    
    public bool ExistsPlanetView(TilemapCircle tilemapCircle)
    {
        return false;
    }
    
    public PlanetView GetPlanetView(ushort thingIndex)
    {
        if (planetViews[thingIndex] != null)
            return planetViews[thingIndex];
        
        if (activePlanetViews.Count >= MaxActivePlanetViews)
            universe.ReturnPlanet(activePlanetViews[0].Planet);
        
        Planet planet = universe.GetPlanet(thingIndex);
        
        PlanetView planetView = universeFactory.GetPlanet(planet.Height);
        
        planetView.InitPlanet(planet, this);
        
        activePlanetViews.Add(planetView);
        
        planetViews[thingIndex] = planetView;
        
        //Debug.Log(planetViews.Count);
        
        return planetView;
    }
    
    public void OnPlanetReturned(Planet planet)
    {
        planetViews[planet.ThingIndex] = null;
        
        for (int i = 0; i < activePlanetViews.Count; i++)
        {
            if (activePlanetViews[i].Planet == planet)
            {
                PlanetView planetView = activePlanetViews[i];
                
                universeFactory.ReturnPlanet(planetView);
                
                activePlanetViews.RemoveAt(i);
                
                break;
            }
        }
    }
    
    public void OnUniverseObjectAdded(UniverseObject universeObject)
    {
        if (universeObject is UniverseEngine.Avatar)
        {
            avatarView = universeFactory.GetAvatar();
            avatarView.Init((UniverseEngine.Avatar) universeObject, this);
            
            tilemapObjectViews.Add(avatarView);
        }
        else if (universeObject is UniverseEngine.Ship)
        {
            shipView = universeFactory.GetShip();
            shipView.Init((UniverseEngine.Ship) universeObject, this);
            
            tilemapObjectViews.Add(shipView);
        }
    }
 
    /// <summary>
    /// Called by GameLogic
    /// </summary>
    public void UpdateUniverse(float deltaTime)
    {
        universe.UpdateUniverse(deltaTime);
        
        if (IsVisible())
        {
            UEProfiler.BeginSample("UniverseView.UpdateMesh");
            UpdateMesh(false);
            UEProfiler.EndSample();
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
        Thing[] things = universe.Things;
        ThingPosition[] thingsPositions = universe.ThingsPositions;
        ushort[] thingsToRender = universe.ThingsToRender;
        ushort thingsToRenderAmount = universe.ThingsToRenderAmount;

        int vertexCount = thingsToRenderAmount * 4;
        int triangleCount = thingsToRenderAmount * 6;
        
        Vector3[] vertices = DataPools.poolVector3.GetArray(vertexCount);
        
        int vertexOffset = 0;
  
        //Update all positions
        for (int i = 0; i < thingsToRenderAmount; i++)
        {
            ushort thingIndex = thingsToRender[i];
            
            ThingPosition position = thingsPositions[thingIndex];
        
            vertices[vertexOffset + 0].x = position.x - position.radius;
            vertices[vertexOffset + 0].y = position.y - position.radius;

            vertices[vertexOffset + 1].x = position.x - position.radius;
            vertices[vertexOffset + 1].y = position.y + position.radius;

            vertices[vertexOffset + 2].x = position.x + position.radius;
            vertices[vertexOffset + 2].y = position.y + position.radius;

            vertices[vertexOffset + 3].x = position.x + position.radius;
            vertices[vertexOffset + 3].y = position.y - position.radius;

            vertexOffset += 4;
        }

        //Don't draw preview of active planets (except the first time that everything is draw)
        if (!firstTime)
        {   
            for (int i = 0; i < activePlanetViews.Count; i++)
            {
                ushort thingIndex = activePlanetViews[i].Planet.ThingIndex;

                int thingsToRenderIndex = System.Array.BinarySearch(thingsToRender, 0, thingsToRenderAmount, thingIndex);

                if (thingsToRenderIndex >= 0)
                {
                    vertexOffset = thingsToRenderIndex * 4;

                    vertices[vertexOffset + 0].x = 0;
                    vertices[vertexOffset + 0].y = 0;

                    vertices[vertexOffset + 1].x = 0;
                    vertices[vertexOffset + 1].y = 0;

                    vertices[vertexOffset + 2].x = 0;
                    vertices[vertexOffset + 2].y = 0;

                    vertices[vertexOffset + 3].x = 0;
                    vertices[vertexOffset + 3].y = 0;
                }
            }
        }

        if (firstTime)
        {
            //Update triangles and uvs only the first time that the mesh is updated
            
            int triangleOffset = 0;
            vertexOffset = 0;
            
            int[] triangles = DataPools.poolInt.GetArray(triangleCount);
            Vector2[] uvs = DataPools.poolVector2.GetArray(vertexCount);

            for (ushort i = 0; i < thingsToRenderAmount; i++)
            {
                Thing thing = things[thingsToRender[i]];
                
                PlanetType planetType;
                
                if (thing.type == (ushort) ThingType.Sun)
                    planetType = planetTypes[Mathf.Abs(thing.seed % 2) + 4]; //suns!
                else
                    planetType = planetTypes[Mathf.Abs(thing.seed % 4)]; //planets!
    
                Rect planetUV = planetType.planetSprite.UV;
                
                uvs[vertexOffset + 0] = new Vector2(planetUV.xMin, planetUV.yMax);
                uvs[vertexOffset + 1] = new Vector2(planetUV.xMax, planetUV.yMax);
                uvs[vertexOffset + 2] = new Vector2(planetUV.xMax, planetUV.yMin);
                uvs[vertexOffset + 3] = new Vector2(planetUV.xMin, planetUV.yMin);

                triangles[triangleOffset + 0] = vertexOffset + 0;
                triangles[triangleOffset + 1] = vertexOffset + 1;
                triangles[triangleOffset + 2] = vertexOffset + 2;

                triangles[triangleOffset + 3] = vertexOffset + 2;
                triangles[triangleOffset + 4] = vertexOffset + 3;
                triangles[triangleOffset + 5] = vertexOffset + 0;

                triangleOffset += 6;
                vertexOffset += 4;
            }            
            
            mesh1.vertices = vertices;
            mesh2.vertices = vertices;

            mesh1.triangles = triangles;
            mesh1.uv = uvs;
            mesh1.bounds = new Bounds(Vector3.zero, new Vector3(ushort.MaxValue * 2, ushort.MaxValue * 2, 0.0f));

            mesh2.triangles = triangles;
            mesh2.uv = uvs;
            mesh2.bounds = new Bounds(Vector3.zero, new Vector3(ushort.MaxValue * 2, ushort.MaxValue * 2, 0.0f));

            mesh1.Optimize();
            mesh2.Optimize();
            
            DataPools.poolInt.ReturnArray(triangles);
            DataPools.poolVector2.ReturnArray(uvs);
        }

        if ((frameCount % 2) == 0)
        {
            mesh1.vertices = vertices;
            meshFilter.sharedMesh = mesh1;
        }
        else
        {
            mesh2.vertices = vertices;
            meshFilter.sharedMesh = mesh2;
        }
        
        DataPools.poolVector3.ReturnArray(vertices);

        frameCount++;
    }
}


