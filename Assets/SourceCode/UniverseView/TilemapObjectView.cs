using UnityEngine;
using Universe;

public class TilemapObjectView : MonoBehaviour, ITilemapObjectListener
{
    protected TilemapCircleView tilemapCircleView;
    protected TilemapObject tilemapObject;
    protected UniverseView universeView;
    
    [HideInInspector]
    public Transform trans;
    
    public TilemapObject TilemapObject
    {
        get { return tilemapObject; }
    }
    
    public void Awake()
    {
        trans = transform;
    }
    
    public void Init(TilemapObject tilemapObject, UniverseView universeView)
    {
        this.universeView = universeView;
        this.tilemapObject = tilemapObject;
        
        tilemapObject.Listener = this;
        
        tilemapCircleView = universeView.GetPlanetView(((Planet) tilemapObject.tilemapCircle).ThingIndex);
        UpdatePosition();
    }

    public virtual void OnTilemapObjectUpdated()
    {
        UpdatePosition();
    }

    protected void UpdatePosition()
    {
        trans.localPosition = tilemapObject.Position;
        trans.localScale = Vector3.one * tilemapObject.Scale;
        trans.localRotation = Quaternion.AngleAxis(-tilemapObject.Rotation * Mathf.Rad2Deg, Vector3.forward);
    }

    public void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }

    public void OnDrawGizmos()
    {
        if (tilemapCircleView)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * tilemapObject.Size.y);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + transform.up * tilemapObject.Size.y * 0.5f, transform.position + transform.up * tilemapObject.Size.y * 0.5f + transform.right * tilemapObject.Size.x * 0.5f);
        }
    }

    public void OnTilemapCircleChanged ()
    {
        tilemapCircleView = universeView.GetPlanetView(((Planet) tilemapObject.tilemapCircle).ThingIndex);
        UpdatePosition();
    }
}

