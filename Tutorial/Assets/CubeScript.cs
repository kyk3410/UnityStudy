using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public Transform sphereTransform;

    void Start()
    {
        sphereTransform.parent = transform;
        sphereTransform.localScale = Vector3.one * 2;
    }

    void Update()
    {
        //transform.eulerAngles += new Vector3(0, 180 * Time.deltaTime, 0); 
        //transform.eulerAngles += Vector3.up * 180 * Time.deltaTime;
        //Vector3.up 은 (0,1,0)이다
        //Vector3.forward 은 (0,0,1)이다
        transform.Rotate(Vector3.up * Time.deltaTime * 180, Space.World);
        transform.Translate(Vector3.forward * Time.deltaTime * 7, Space.World);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            sphereTransform.localPosition = Vector3.zero;
        }
    }
}
