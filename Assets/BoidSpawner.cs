using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] float summonTImer;
    public GameObject ActorObject;
    
    void Update()
    {
        if (summonTImer > 0) summonTImer -= Time.deltaTime;
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Instantiate(ActorObject).transform.position = mousePos;
            summonTImer = 0.2f;
        }
    }
}
