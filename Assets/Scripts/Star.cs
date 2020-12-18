using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public BaseManager baseManager;

    StarType[] allType = { new SmallStar(), new MediumStar(), new BigStar() };
    StarType type;
    Actors behaviour;
    int point;

    private void Start()
    {
        GetNewBehaviour();
    }

    private void Update()
    {
        behaviour.CalculateVelocity();
        transform.position += behaviour.GetVelocity()*100*Time.deltaTime;
        transform.eulerAngles = new Vector3(0f,0f,behaviour.GetVelocityDegree()+180);

        behaviour.ResetAcceleration();
        behaviour.StayInBoundry(transform);
        behaviour.StayOutsideBase(transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("actor") && !baseManager.gameOver)
        {
            MainManager.Instance.PointUpSound.Play();
            baseManager.IncreaseTime();
            GetNewBehaviour();
        }
    }

    void GetNewBehaviour()
    {
        // BE MOVING OBJECT
        type = allType[Random.Range(0,allType.Length)];
        IdentifyType();

        behaviour = new Actors(Random.Range(0.01f, 0.05f), 0.005f, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f));
        transform.position = FindNewPosition();
    }

    void IdentifyType()
    {
        GetComponent<SpriteRenderer>().color= type.GetColor();
        transform.localScale = type.GetSize();
    }

    Vector3 FindNewPosition()
    {
        float xPos1 = Random.Range(0.05f, 0.4f);
        float xPos2 = Random.Range(0.6f, 0.95f);

        float yPos1 = Random.Range(0.05f, 0.4f);
        float yPos2 = Random.Range(0.6f, 0.95f);

        float choosenXPos;
        if (Random.Range(0, 2) == 0) choosenXPos = xPos1;
        else choosenXPos = xPos2;

        float choosenYPos;
        if (Random.Range(0, 2) == 0) choosenYPos = yPos1;
        else choosenYPos = yPos2;

        return Camera.main.ViewportToWorldPoint(new Vector3(choosenXPos, choosenYPos, 1f));
    }
}

public class StarType
{
    protected Vector3 starSize;
    protected Color starColor;

    public Vector3 GetSize()
    {
        return starSize;
    }

    public Color GetColor()
    {
        return starColor;
    }
}

public class SmallStar : StarType
{
    public SmallStar()
    {
        starSize = Vector3.one * 0.3f;
        starColor = Color.red;
    }
}

public class MediumStar : StarType
{
    public MediumStar()
    {
        starSize = Vector3.one * 0.5f;
        starColor = Color.yellow;
    }
}

public class BigStar : StarType
{
    public BigStar()
    {
        starSize = Vector3.one * 0.7f;
        starColor = Color.white;
    }
}
