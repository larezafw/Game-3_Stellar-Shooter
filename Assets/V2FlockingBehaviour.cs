using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2FlockingBehaviour : MonoBehaviour
{
    public Actors actorBehaviour { get; private set; }
    float raycastTurnFriction = 0.01f;
    float numOfRaycast = 75f;


    private void Awake()
    {
        actorBehaviour = new Actors(0.08f, 0.001f, Vector3.zero);
    }
    private void FixedUpdate()
    {
        Raycasting();
        actorBehaviour.CalculateVelocity();

        transform.position += actorBehaviour.GetVelocity();
        transform.eulerAngles = new Vector3(0f, 0f, actorBehaviour.GetVelocityDegree());

        actorBehaviour.ResetAcceleration();
        actorBehaviour.TeleportInsideBoundry(transform);
    }

    void Flocking(List<GameObject> allActor)
    {
        Vector3 sumAlignment = Vector3.zero;
        Vector3 sumCohesion = Vector3.zero;
        Vector3 sumSeperation = Vector3.zero;

        int totalAlligmentActors = 0;
        int totalCohesionActors = 0;
        int totalSeperationActor = 0;

        foreach (GameObject actor in allActor)
        {
            float distance = (transform.position - actor.transform.position).magnitude;
            if (distance > 0f && distance < 3f)
            {
                // ALIGNMENT
                sumAlignment += actor.GetComponent<V2FlockingBehaviour>().actorBehaviour.GetVelocity();
                totalAlligmentActors += 1;

                // COHESION
                sumCohesion += actor.transform.position;
                totalCohesionActors += 1;

                // SEPERATION
                if (distance > 0 && distance < 1f)
                {
                    sumSeperation += (transform.position - actor.transform.position).normalized / distance;
                    totalSeperationActor += 1;
                }
            }
        }

        // CALCUATE ALLIGNMENT
        if (totalAlligmentActors > 0)
        {
            sumAlignment /= totalAlligmentActors;
            actorBehaviour.AddSteerForce(sumAlignment);
        }

        // CALCULATE COHESION
        if (totalCohesionActors > 0)
        {
            sumCohesion /= totalCohesionActors;
            sumCohesion -= transform.position;
            actorBehaviour.AddSteerForce(sumCohesion);
        }

        // CALCULATE SEPERATION
        if (totalSeperationActor > 0)
        {
            sumSeperation /= totalSeperationActor;
            actorBehaviour.AddSteerForce(sumSeperation, true);
        }
    }

    void Raycasting()
    {
        List<GameObject> allActors = new List<GameObject>();
        for(int i=0; i < numOfRaycast; i++)
        {
            //float dist = i / (numOfRaycast- 1f);
            float halfCircle = (1 / raycastTurnFriction) / 2;
            float angle = (transform.eulerAngles.z-90)* Mathf.Deg2Rad + ((((numOfRaycast / halfCircle) * -(Mathf.PI / 2))) + (2 * Mathf.PI * raycastTurnFriction * i));

            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);

            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position, new Vector3(x, y, 0).normalized, 3);

            for (int j = 0; j < hits.Length; j++)
            {
                RaycastHit hit = hits[j];
                allActors.Add(hit.transform.gameObject);
            }

            if (Physics.Raycast(transform.position, new Vector3(x, y, 0).normalized, 3)) 
            {
                Debug.DrawRay(transform.position, new Vector3(x, y,0).normalized *3, Color.blue);
            }
            else
            {
                Debug.DrawRay(transform.position, new Vector3(x, y, 0).normalized * 3, Color.white);
            }
        }
        Flocking(allActors);
    }
}

