using UnityEngine;
using UniverseEngine;

public class UniverseObjectView : MonoBehaviour, IUniverseObjectListener
{
    protected UniverseObject universeObject;
    protected UniverseView universeView;
    protected TilemapCircleView parentView;
    
    [HideInInspector]
    public Transform trans;
    
    public UniverseObject UniverseObject
    {
        get { return universeObject; }
    }
    
    public void Awake()
    {
        trans = transform;
    }
    
    public void Init(UniverseObject universeObject, UniverseView universeView)
    {
        this.universeView = universeView;
        this.universeObject = universeObject;
        
        universeObject.Listener = this;
        
        parentView = universeView.GetPlanetView(universeObject.parent);
        
        UpdatePosition();
    }

    public virtual void OnUniverseObjectUpdated(float deltaTime)
    {
        UpdatePosition();
    }
    
    public virtual void OnParentChanged(TilemapCircle parent)
    {
        parentView = universeView.GetPlanetView(universeObject.parent);
    }

    protected void UpdatePosition()
    {
        trans.localPosition = universeObject.Position;
        trans.localScale = Vector3.one * universeObject.Scale;
        trans.localRotation = Quaternion.AngleAxis(-universeObject.Rotation * Mathf.Rad2Deg, Vector3.forward);
    }

    public virtual void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }

    public virtual void OnDrawGizmos()
    {
        if (universeObject != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * universeObject.Size.y);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + transform.up * universeObject.Size.y * 0.5f, transform.position + transform.up * universeObject.Size.y * 0.5f + transform.right * universeObject.Size.x * 0.5f);
        }
    }
}

