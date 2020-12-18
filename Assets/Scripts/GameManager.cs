using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float MaxSpeed;
    [SerializeField] float MaxSterForce;

    Vector3 velocity;
    Vector3 acceleration;
    bool revese;

    private void Start()
    {
        velocity = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f));
        velocity = velocity.normalized * MaxSpeed;
        acceleration = Vector3.zero;
        GetComponent<SpriteRenderer>().color= Random.ColorHSV();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) revese = true;
        else revese = false;

        Vector3 mousePos = Input.mousePosition; 
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);
        targetPos.z = 0;

        //Vector3 steerFore = GetSteerForce(targetPos);
        //AddForce(steerFore);

        OutOfBoundry();
        velocity += acceleration;
        if (velocity.magnitude >= MaxSpeed) velocity = velocity.normalized * MaxSpeed;
        

        float velTheta = Mathf.Atan2(velocity.y, velocity.x);
        transform.eulerAngles = new Vector3(0f, 0f, (velTheta * Mathf.Rad2Deg)+90);
        transform.position += velocity;

        acceleration = Vector3.zero;
    }

    void AddForce(Vector3 force)
    {
        force.z = 0;
        acceleration += force;
    }

    Vector3 GetSteerForce(Vector3 targetPos)
    {
        Vector3 desired = targetPos - transform.position;
        float desiredMag = desired.magnitude;
        desired = desired.normalized;
        if (desiredMag < 3) desired *= (Mathf.Lerp(0, MaxSpeed, desiredMag / 3));
        else desired *= MaxSpeed;

        Vector3 steerForce = desired - velocity;
        if (steerForce.magnitude >= MaxSterForce) steerForce = (steerForce.normalized * MaxSterForce);
        return steerForce;
    }

    ////Vector3 GetWallSteerForce(Vector3 targetPos)
    //{
    //    return null;
    //}

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    void OutOfBoundry()
    {
        float xLeftLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float xRIghtLimit = Camera.main.ViewportToWorldPoint(Vector3.right).x;
        float yUpLiimit = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        float yDownLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).y;

        if (transform.position.x < (xLeftLimit+2f)) ApplyLeftHorizontalBoundryForce();
        if (transform.position.x > (xRIghtLimit-2f)) ApplyRightHorizontalBoundryForce();
        if (transform.position.y < (yDownLimit+2f)) applyDownVertikalBoundryForce(); 
        if(transform.position.y > (yUpLiimit-2f)) applyUpVertikalBoundryForce();
    }

    bool IsOutOfBoundry()
    {
        float xLeftLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float xRIghtLimit = Camera.main.ViewportToWorldPoint(Vector3.right).x;
        float yUpLiimit = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        float yDownLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).y;

        return transform.position.x < (xLeftLimit + 2f) ||
        transform.position.x > (xRIghtLimit - 2f) ||
        transform.position.y < (yDownLimit + 2f) ||
        transform.position.y > (yUpLiimit - 2f);
    }

    void ApplyLeftHorizontalBoundryForce()
    {
        Vector3 desired = new Vector3(MaxSpeed, velocity.y);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - velocity;
        if (steerForce.magnitude >= MaxSterForce) steerForce = (steerForce.normalized * MaxSterForce);
        AddForce(steerForce);
    }

    void ApplyRightHorizontalBoundryForce()
    {
        Vector3 desired = new Vector3(-MaxSpeed, velocity.y);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - velocity;
        if (steerForce.magnitude >= MaxSterForce) steerForce = (steerForce.normalized * MaxSterForce);
        AddForce(steerForce);
    }

    void applyDownVertikalBoundryForce()
    {
        Vector3 desired = new Vector3(velocity.x, MaxSpeed);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - velocity;
        if (steerForce.magnitude >= MaxSterForce) steerForce = (steerForce.normalized * MaxSterForce);
        AddForce(steerForce);
    }

    void applyUpVertikalBoundryForce()
    {
        Vector3 desired = new Vector3(velocity.x, -MaxSpeed);
        desired = desired.normalized * MaxSpeed;

        Vector3 steerForce = desired - velocity;
        if (steerForce.magnitude >= MaxSterForce) steerForce = (steerForce.normalized * MaxSterForce);
        AddForce(steerForce);
    }
    private void OnDrawGizmosSelected()
    {
        if (IsOutOfBoundry()) Gizmos.color = Color.red;
        else Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position+(velocity.normalized*2));
    }
}
