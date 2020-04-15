using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public bool hasDrawn;
    public List<Ray> lasers;
    private GameObject attackTaken;
    // Start is called before the first frame update
    void Start()
    {
        hasDrawn = false;
        lasers = new List<Ray>();
        attackTaken = GameObject.Find("AttackTaken");
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasDrawn) {
            Draw();
        }
    }

    public void Draw()
    {
        hasDrawn = true;
        // clean lasers
        GameObject[] del = GameObject.FindGameObjectsWithTag("Laser");
        for (int i = del.Length; i > 0; i--) {
            Destroy(del[i - 1]);
        }

        // render the screen
        int count = 0;
        float length = 50f;
        foreach (var l in lasers) {
            RaycastHit[] h = Physics.RaycastAll(l, 30f);
            GameObject o = Instantiate(Resources.Load("DrawLaser") as GameObject);
            o.GetComponent<LineRenderer>().SetPosition(0, l.origin);
            if (h.Length == 0)
            {
                o.GetComponent<LineRenderer>().SetPosition(1, l.origin+l.direction*length);
            }
            else {
                RaycastHit hit;
                //hit = h[0].transform.tag == "Laser" ? h[1] : h[0];
                hit = h[0];
                o.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                //o.transform.Find("AttackPoint").position = hit.point;
                if (h[0].transform.tag == "Body")
                {
                    count++;
                }
            }
        }

        // update ui
        attackTaken.GetComponent<Text>().text = count.ToString();
    }
}
