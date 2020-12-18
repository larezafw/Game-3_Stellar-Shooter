using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBehaviour : MonoBehaviour
{
    LineRenderer path;
    GridManager gridManager;
    float MaxSpeed;
    float MaxForce;
    bool versa;

    Vector3 Acceletate;
    Vector3 Velocity;

    Vector3 FollotPathTarget;
    Vector3 PredictLock;
    Vector3 NormalPoint;
    void Awake()
    {
        gridManager = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GridManager>();
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<LineRenderer>();
        GetComponent<SpriteRenderer>().color = Random.ColorHSV();

        MaxSpeed = 0.05f;
        MaxForce = 0.001f;
        Acceletate = Vector3.zero;
        Velocity = Vector3.right.normalized*MaxSpeed;
        versa = false;
    }

    private void Update()
    {
        PathFollowingForce();
        Velocity += Acceletate;
        if (Velocity.magnitude >= MaxSpeed) Velocity = Velocity.normalized * MaxSpeed;

        transform.position += Velocity;
        transform.eulerAngles = new Vector3(0f, 0f, (Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg) + 90);

        Acceletate = Vector3.zero;
        if (transform.position.x >= path.GetPosition(path.positionCount - 1).x) versa = true;
        else if (transform.position.x <= path.GetPosition(0).x) versa = false;
    }

    void AddForce(Vector3 target)
    {
        Acceletate += target;
    }

    Vector3 FlowFieldForce()
    {
        Vector3 desired = gridManager.GetDesiredVector(transform.position);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        return steerForce;
    }

    void PathFollowingForce()
    {
        PredictLock = transform.position + (Velocity.normalized * MaxSpeed * 10);
        float distance = float.MaxValue;
        Vector3 startPathPost = Vector3.zero;
        Vector3 endPathPos = Vector3.zero;

        for (int i = 0; i < (path.positionCount-1); i++)
        {
            Vector3 newPathStartPos = path.GetPosition(i);
            Vector3 newPathEndPos = path.GetPosition(i+1);

            Vector3 newNormalPoint = NormalPointCalculate(PredictLock, newPathStartPos, newPathEndPos);
            if (newNormalPoint.x < newPathStartPos.x || newNormalPoint.x > newPathEndPos.x) newNormalPoint = newPathEndPos;

            float newDistance = (PredictLock - newNormalPoint).magnitude;
            if (newDistance < distance)
            {
                distance = newDistance;
                NormalPoint = newNormalPoint;
                startPathPost = newPathStartPos;
                endPathPos = newPathEndPos;
            }

            if (transform.position.x <= (path.GetPosition(0).x + 0.6f)) 
            {
                NormalPoint = path.GetPosition(0);
                startPathPost = path.GetPosition(0);
                endPathPos = path.GetPosition(1);
                break;
            }
            else if (transform.position.x >= (path.GetPosition(path.positionCount - 1).x -0.5f))
            {
                int pathLastIndex = path.positionCount - 1;
                NormalPoint = path.GetPosition(pathLastIndex);
                startPathPost = path.GetPosition(pathLastIndex-1);
                endPathPos = path.GetPosition(pathLastIndex);
                break;
            }
        }
        
        Vector3 additionTargetPoint = (endPathPos-startPathPost).normalized;
        if (versa) additionTargetPoint *= -1;

        FollotPathTarget = NormalPoint + additionTargetPoint;

        if (distance > path.startWidth/2)
        {
            AddForce(SteerForce(FollotPathTarget));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //if (FollotPathTarget != null) Gizmos.DrawLine(transform.position, FollotPathTarget);
        if (NormalPoint != null && PredictLock != null) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, PredictLock);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(PredictLock, NormalPoint);
        }
    }
    Vector3 NormalPointCalculate(Vector3 predicPoin, Vector3 startPath,Vector3 endPath)
    {
        Vector3 a = predicPoin - startPath;
        Vector3 b = (endPath - startPath).normalized;

        Vector3 normalPoint = startPath + (b * Vector3.Dot(a, b));
        return normalPoint;
    }
    Vector3 SteerForce(Vector3 targetPos)
    {
        Vector3 desired = targetPos - transform.position;
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - Velocity;
        if (steerForce.magnitude >= MaxForce) steerForce = (steerForce.normalized * MaxForce);
        return steerForce;
    }
}

