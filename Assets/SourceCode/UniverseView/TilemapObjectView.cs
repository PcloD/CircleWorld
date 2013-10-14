using UnityEngine;
using Universe;

public class TilemapObjectView : MonoBehaviour 
{
    protected TilemapCircleView tilemapCircleView;
    protected TilemapObject tilemapObject;
    
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
    
    public void Init(TilemapObject tilemapObject, TilemapCircleView tilemapCircleView)
    {
        this.tilemapCircleView = tilemapCircleView;
        this.tilemapObject = tilemapObject;
        
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
}

