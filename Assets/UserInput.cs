using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    float speed = 100.0f;

    GameObject co,mc;
    // Start is called before the first frame update
    void Start()
    {
        co = GameObject.Find("CameraOrigin");
        mc = co.transform.Find("MainCamera").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) {
            float x = Input.GetAxis("Mouse X") * speed * 0.02f;
            float y = -Input.GetAxis("Mouse Y") * speed * 0.02f;

            co.transform.Rotate(new Vector3(0, 1, 0), x);
            co.transform.Rotate(new Vector3(1, 0, 0), y);

            mc.transform.LookAt(new Vector3());

        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        mc.transform.position = mc.transform.position + mc.transform.forward*scroll*Time.deltaTime*100f;
    }
}
