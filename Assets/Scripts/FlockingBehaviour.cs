using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingBehaviour : MonoBehaviour
{
    public Actors actorBehaviour { get; private set; }

    private void Awake()
    {
        actorBehaviour = new Actors(0.05f, 0.001f, Vector3.zero);
    }
    private void Update()
    {
        Flocking();
        actorBehaviour.TeleportInsideBoundry(transform);
        actorBehaviour.CalculateVelocity();

        transform.position += actorBehaviour.GetVelocity() * 100 * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, 0f, actorBehaviour.GetVelocityDegree());

        actorBehaviour.ResetAcceleration();
        if (actorBehaviour.OutsideOfBroundry(transform)) Destroy(gameObject);
    }

    void Seperating()
    {
        GameObject[] allActor = GameObject.FindGameObjectsWithTag("actor");

        Vector3 sumDesired = Vector3.zero;
        int numCrossedActor = 0;

        foreach (GameObject actor in allActor)
        {
            float distance = (transform.position - actor.transform.position).magnitude;
            if (distance > 0f && distance < 1f)
            {
                Vector3 diff = transform.position - actor.transform.position;
                diff = diff.normalized / distance;
                sumDesired += diff;
                numCrossedActor += 1;
            }
        }

        if (numCrossedActor > 0)
        {
            sumDesired /= numCrossedActor;
            actorBehaviour.AddSteerForce(sumDesired, true);
        }
    }

    void Alligmenting()
    {
        GameObject[] allActor = GameObject.FindGameObjectsWithTag("actor");

        Vector3 sumDesired = Vector3.zero;
        int numCrossedActor = 0;

        foreach (GameObject actor in allActor)
        {
            float distance = (transform.position - actor.transform.position).magnitude;
            if (distance > 0f && distance < 3f)
            {
                Vector3 actorVelocity = actor.GetComponent<FlockingBehaviour>().actorBehaviour.GetVelocity();
                sumDesired += actorVelocity;
                numCrossedActor += 1;
            }
        }

        if (numCrossedActor > 0)
        {
            sumDesired /= numCrossedActor;
            actorBehaviour.AddSteerForce(sumDesired);
        }
    }

    void Cohesioning()
    {
        GameObject[] allActor = GameObject.FindGameObjectsWithTag("actor");

        Vector3 sumDesired = Vector3.zero;
        int numCrossedActor = 0;

        foreach (GameObject actor in allActor)
        {
            float distance = (transform.position - actor.transform.position).magnitude;
            if (distance > 0f && distance < 3f)
            {
                sumDesired += actor.transform.position;
                numCrossedActor += 1;
            }
        }

        if (numCrossedActor > 0)
        {
            sumDesired /= numCrossedActor;
            sumDesired -= transform.position;
            actorBehaviour.AddSteerForce(sumDesired);
        }
    }

    void Flocking()
    {
        GameObject[] allActor = GameObject.FindGameObjectsWithTag("actor");

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
                sumAlignment += actor.GetComponent<FlockingBehaviour>().actorBehaviour.GetVelocity();
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
            actorBehaviour.AddSteerForce(sumSeperation,true);
        }
    }
}

public class Actors
{
    float MaxSpeed;
    float MaxForce;
    bool versa;

    Vector3 Acceletation;
    Vector3 Velocity;

    public Actors(float maxSpd, float maxForce, Vector3 firstVelocity)
    {
        MaxSpeed = maxSpd;
        MaxForce = maxForce;
        Acceletation = Vector3.zero;
        Velocity = firstVelocity.normalized * MaxSpeed;
    }

    public void AddForce(Vector3 force)
    {
        Acceletation += force;
    }

    public void CalculateVelocity()
    {
        Velocity += Acceletation;
        if (Velocity.magnitude >= MaxSpeed) Velocity = Velocity.normalized * MaxSpeed;
    }

    public void ResetAcceleration()
    {
        Acceletation = Vector3.zero;
    }

    public void AddSteerForce(Vector3 desired)
    {
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    public void AddSteerForce(Vector3 desired,bool isSeperating)
    {
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce * 1.4f);
    }

    public bool OutsideOfBroundry(Transform transform)
    {
        float xLeftLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float xRIghtLimit = Camera.main.ViewportToWorldPoint(Vector3.right).x;
        float yUpLiimit = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        float yDownLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).y;

        return transform.position.x < xLeftLimit || transform.position.x > xRIghtLimit || transform.position.y < yDownLimit ||
        transform.position.y > yUpLiimit;
    }

    public void TeleportInsideBoundry(Transform transform)
    {
        float xLeftLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float xRIghtLimit = Camera.main.ViewportToWorldPoint(Vector3.right).x;
        float yUpLiimit = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        float yDownLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).y;

        if (transform.position.x < xLeftLimit) transform.position = new Vector3(xRIghtLimit, transform.position.y, 0f);
        if (transform.position.x > xRIghtLimit) transform.position = new Vector3(xLeftLimit, transform.position.y, 0f);
        if (transform.position.y < yDownLimit) transform.position = new Vector3(transform.position.x, yUpLiimit, 0f);
        if (transform.position.y > yUpLiimit) transform.position = new Vector3(transform.position.x, yDownLimit, 0f);
    }

    public void StayOutsideBase(Transform transform)
    {
        float xLeftLimit = Camera.main.ViewportToWorldPoint(new Vector3(0.4f,0f,0f)).x;
        float xRIghtLimit = Camera.main.ViewportToWorldPoint(new Vector3(0.6f,0f,0f)).x;
        float yUpLiimit = Camera.main.ViewportToWorldPoint(new Vector3(0f,0.6f,0f)).y;
        float yDownLimit = Camera.main.ViewportToWorldPoint(new Vector3(0f,0.4f,0f)).y;

        if (transform.position.x > xLeftLimit && transform.position.x < xRIghtLimit && transform.position.y > yDownLimit && transform.position.y < yUpLiimit)
        {
            float leftDis = Mathf.Abs(xLeftLimit - transform.position.x);
            float rightDis = Mathf.Abs(xRIghtLimit - transform.position.x);
            float upDis = Mathf.Abs(yUpLiimit - transform.position.y);
            float downDis = Mathf.Abs(yDownLimit - transform.position.y);

            float[] allDIs = new float[] { leftDis, rightDis, upDis, downDis };

            float shortestDis = leftDis;
            for (int i=0; i < allDIs.Length; i++) if (allDIs[i] < shortestDis) shortestDis = allDIs[i];

            if (shortestDis == leftDis) ApplyLeftBaseForce();
            else if (shortestDis == rightDis) ApplyRightBaseForce();
            else if (shortestDis == upDis) applyUpBaseForce();
            else applyDownBaseForce();
        }
    }

    void ApplyLeftBaseForce()
    {
        Vector3 desired = new Vector3(-MaxSpeed, Velocity.y);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    void ApplyRightBaseForce()
    {
        Vector3 desired = new Vector3(MaxSpeed, Velocity.y);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    void applyDownBaseForce()
    {
        Vector3 desired = new Vector3(Velocity.x, -MaxSpeed);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    void applyUpBaseForce()
    {
        Vector3 desired = new Vector3(Velocity.x, MaxSpeed);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    public void StayInBoundry(Transform transform)
    {
        float xLeftLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float xRIghtLimit = Camera.main.ViewportToWorldPoint(Vector3.right).x;
        float yUpLiimit = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        float yDownLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).y;

        if (transform.position.x < (xLeftLimit + 1f)) ApplyLeftHorizontalBoundryForce();
        if (transform.position.x > (xRIghtLimit - 1f)) ApplyRightHorizontalBoundryForce();
        if (transform.position.y < (yDownLimit + 1f)) applyDownVertikalBoundryForce();
        if (transform.position.y > (yUpLiimit - 1f)) applyUpVertikalBoundryForce();
    }

    void ApplyLeftHorizontalBoundryForce()
    {
        Vector3 desired = new Vector3(MaxSpeed, Velocity.y);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    void ApplyRightHorizontalBoundryForce()
    {
        Vector3 desired = new Vector3(-MaxSpeed, Velocity.y);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    void applyDownVertikalBoundryForce()
    {
        Vector3 desired = new Vector3(Velocity.x, MaxSpeed);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }

    void applyUpVertikalBoundryForce()
    {
        Vector3 desired = new Vector3(Velocity.x, -MaxSpeed);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        AddForce(steerForce);
    }


    public Vector3 GetVelocity()
    {
        return Velocity;
    }

    public float GetVelocityDegree()
    {
        return (Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg) + 90;
    }
}
