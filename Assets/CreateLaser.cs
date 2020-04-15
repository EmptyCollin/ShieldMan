using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateLaser : MonoBehaviour
{
    public GameObject laserNum;
    public GameObject dispersion;
    public GameObject accuaccy;
    public GameObject model;
    private GameObject controller;
    public Vector3 attackFrom;
    // Start is called before the first frame update
    void Start()
    {
        laserNum = GameObject.Find("LaserNumber");
        dispersion = GameObject.Find("Dispersion");
        accuaccy  = GameObject.Find("Accuracy");
        model = GameObject.Find("model");
        controller = GameObject.Find("Controller");
        //Vector3 attackFrom = new Vector3(5, 3, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Create() {
        controller.GetComponent<Controller>().lasers.Clear();
        int l_num = int.Parse(laserNum.transform.Find("Text").GetComponent<Text>().text);
        float dis = float.Parse(dispersion.transform.Find("Text").GetComponent<Text>().text);
        float acc = 1-float.Parse(accuaccy.transform.Find("Text").GetComponent<Text>().text);
        Vector3 start, destination;
        Vector3 attackerCenter = model.transform.position + attackFrom;
        for (int i = 0; i < l_num; i++) {
            start = attackerCenter + new Vector3(Random.value*2-1, Random.value*2-1, Random.value*2-1)*dis*attackFrom.magnitude;
            destination = model.transform.position + new Vector3(Random.value*2-1, Random.value*2-1, Random.value*2-1)*acc;
            Ray r = new Ray(start, destination-start);
            controller.GetComponent<Controller>().lasers.Add(r);
        }
        controller.GetComponent<Controller>().hasDrawn = false;
    }
}
