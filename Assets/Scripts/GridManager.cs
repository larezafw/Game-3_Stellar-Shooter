using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject DotObject;
    public GameObject ActorObject;
    float summonTImer;

    Vector3[,] GridVector;

    [SerializeField] int MaxWidth;
    [SerializeField] int MaxHeight;
    [SerializeField] int Resolution;

    int col;
    int row;
   
    //void Start()
    //{
    //    col = MaxWidth / Resolution;
    //    row = MaxHeight / Resolution;
    //    GridVector = new Vector3[col,row];

    //    float xTarget = MaxWidth / 2;
    //    float yTarget = MaxHeight / 2;
    //    for (int i = 0; i < col; i++) 
    //    {
    //        for (int j = 0; j < row; j++) 
    //        {
    //            Vector3 desired = new Vector3(xTarget - i, yTarget - j, 0f);
    //            GridVector[i, j] = desired;
               
    //        }
    //    }

    //    for(int i=0; i < col; i++)
    //    {
    //        for (int j = 0; j < row; j++) 
    //        {
    //            Vector3 vector = GridVector[i, j];
    //            GameObject dot =  Instantiate(DotObject);
    //            dot.transform.position = new Vector3(i-xTarget, j-yTarget, 0f);
    //            dot.transform.eulerAngles = new Vector3(0f,0f, (Mathf.Atan2(vector.y,vector.x)*Mathf.Rad2Deg)+90);
    //        }
    //    }
    //}

    private void Update()
    {
        if (summonTImer > 0) summonTImer -= Time.deltaTime;
        if (Input.GetMouseButton(0) && summonTImer <= 0) 
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Instantiate(ActorObject).transform.position = pos;
            summonTImer = 0.2f;
        }
    }

    public Vector3 GetDesiredVector(Vector3 location)
    {
        int xCordIndex = (int)((location.x + (MaxWidth / 2)) / Resolution);
        int yCordIndex = (int)(location.y + (MaxHeight / 2) / Resolution);

        xCordIndex = Mathf.Clamp(xCordIndex, 0, col-1);
        yCordIndex = Mathf.Clamp(yCordIndex, 0, row-1);

        return GridVector[xCordIndex, yCordIndex];
    }
}
