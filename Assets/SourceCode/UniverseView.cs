using UnityEngine;
using Universe;

public class UniverseView : MonoBehaviour
{
    private ThingsContainer thingsContainer;
    private float time;

    public void Start()
    {
        thingsContainer = new ThingsContainer();
        thingsContainer.Create(0);

    }

    public void Update()
    {
        time += Time.deltaTime;

        thingsContainer.UpdatePositions(time);
    }

    public void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }

    public void OnDrawGizmos()
    {
        if (thingsContainer != null)
        {
            int amount = thingsContainer.thingsAmount;

            Thing[] things = thingsContainer.things;
            ThingPosition[] positions = thingsContainer.thingsPositions;

            for (int i = 0; i < amount; i++)
            {
                switch ((ThingType) things[i].type)
                {
                    case ThingType.Galaxy:
                        Gizmos.color = Color.blue;
                        break;

                    case ThingType.Sun:
                        Gizmos.color = Color.red;
                        break;

                    default:
                        Gizmos.color = Color.green;
                        break;
                }

                Gizmos.DrawSphere(new Vector3(positions[i].x, positions[i].y, 0.0f), things[i].radius); 
            }
        }
    }

}


