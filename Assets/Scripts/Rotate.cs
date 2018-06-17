using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    private Quaternion startRotate1, startRotate2;
    public GameObject obj1, obj2;
    public Quaternion quat1, quat2;
    public GameObject MarkerlessDriver;
    public bool flag=false;

    void Start() {}
    void Update()
    {
        if (flag)
            if (Input.GetMouseButtonDown(0))
            {   
                startRotate1 = obj1.transform.rotation;
                startRotate2 = obj2.transform.rotation;
            }
            else
                if (Input.GetMouseButton(0))
            {
                quat1 = startRotate1 * Quaternion.AngleAxis(Input.mousePosition.x, Vector3.up);
                quat2 = startRotate2 * Quaternion.AngleAxis(Input.mousePosition.x, Vector3.up);
            }
    }

    public void ClickOnRotate()
    {
        flag = !flag;
    }
}
