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
                Color color;

                switch ((ThingType) things[i].type)
                {
                    case ThingType.Galaxy:
                        //color = Color.blue;
                        continue;
                        //break;

                    case ThingType.SolarSystem:
                        //color = Color.cyan;
                        continue;
                        //break;

                    case ThingType.Sun:
                        color = Color.yellow;
                        break;

                    default:
                        color = Color.green;
                        break;
                }

                Gizmos.color = color;

                //if (things[i].safeRadius > 0)
                //    Gizmos.DrawWireSphere(new Vector3(positions[i].x, positions[i].y, 0.0f), things[i].safeRadius); 

                if (things[i].radius > 0)
                    Gizmos.DrawSphere(new Vector3(positions[i].x, positions[i].y, 0.0f), things[i].radius); 
            }
        }
    }

}


