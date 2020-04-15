using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class AIReact : MonoBehaviour
{
    private GameObject controller;
    private GameObject body;
    private GameObject shouder;
    private GameObject arm;
    public GameObject itDepth;
    public GameObject varPos;
    public GameObject varInten;
    public GameObject exchPos;
    private GameObject computingTime;
    private List<Ray> lasers;
    private int generationSize;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Controller");
        body = GameObject.Find("model");
        shouder = GameObject.Find("Shouder_Joint");
        arm = GameObject.Find("Arm_Joint");
        itDepth = GameObject.Find("IterationDepth");
        varPos = GameObject.Find("VariationPossibility");
        varInten = GameObject.Find("VariationIntensity");
        exchPos = GameObject.Find("ExchangePossibility");
        computingTime = GameObject.Find("ComputeTime");
        generationSize = 100;
    }

    public void OnClick() {
        lasers = controller.GetComponent<Controller>().lasers;
        int itd = int.Parse(itDepth.transform.Find("Text").GetComponent<Text>().text);
        float variationPossibility = float.Parse(varPos.transform.Find("Text").GetComponent<Text>().text);
        float exchangePossibility = float.Parse(exchPos.transform.Find("Text").GetComponent<Text>().text);
        float variationIntensity = float.Parse(varInten.transform.Find("Text").GetComponent<Text>().text);

        Stopwatch sw = new Stopwatch();
        sw.Start();
        Reaction(itd,variationPossibility,exchangePossibility,variationIntensity);
        sw.Stop();
        controller.GetComponent<Controller>().hasDrawn = false;
        computingTime.GetComponent<Text>().text = (sw.ElapsedMilliseconds).ToString();
        
    }

    private void Reaction(int itd, float variationPossibility, float exchangePossibility, float variationIntensity)
    {
        List<Individual> individualPool = new List<Individual>();
        // Intialize first generation
        while (individualPool.Count<= generationSize) {
            // randomly create individuals

            Action a = new Action(UnityEngine.Random.Range(   0f, 360f),
                                  UnityEngine.Random.Range( 235f, 315f),
                                  UnityEngine.Random.Range( 310f, 360f),
                                  UnityEngine.Random.Range(  20f,  90f),
                                  UnityEngine.Random.Range(   0f,  70f),
                                  UnityEngine.Random.Range( 300f, 360f),
                                  UnityEngine.Random.Range( 210f, 315f));

            Individual i = new Individual(a, body, shouder, arm,lasers);

            individualPool.Add(i);
        }

        // Evolution
        for (int i = 0; i < itd; i++) {
            // Select $generationSize best individuals as parents 
            individualPool = individualPool.GetRange(0, generationSize);

            // Create next generation
            // make sure there are even elements in pool
            if (individualPool.Count % 2 == 1) {
                individualPool.Add(individualPool[0]);
            }
            // Create new actions
            List<Action> newActions = new List<Action>();
            for (int j = 0; j < individualPool.Count / 2; j+=2) {
                individualPool[j].action.Generation(individualPool[j + 1].action, newActions,variationPossibility,exchangePossibility,variationIntensity);
            }

            // Build individual pool for next generation
            List<Individual> next = new List<Individual>();

            // heuristic, retrun the result if we get a prefect solution, otherwise copy the best individual of this gerneration to next generation
            if (individualPool[0].result == 0)
            {
                DoOperation(individualPool[0]);
                return;
            }
            else {
                next.Add(individualPool[0]);
            }

            foreach(var a in newActions) {
                next.Add(new Individual(a, body, shouder, arm,lasers));
            }
            individualPool = next;

            // Sort individual pool 
            individualPool.Sort((left, right) =>
            {
                return left.result - right.result;
            });
        }
        // Do the best action in current generation pool
        DoOperation(individualPool[0]);
        

    }

    private void DoOperation(Individual individual)
    {
        body.transform.localRotation = Quaternion.Euler(0, individual.action.body_y, 0);
        shouder.transform.localRotation = Quaternion.Euler(individual.action.shouder_x, individual.action.shouder_y, individual.action.shouder_y);
        arm.transform.localRotation = Quaternion.Euler(individual.action.arm_x, individual.action.arm_y, individual.action.arm_z);
        controller.GetComponent<Controller>().hasDrawn = false;
    }

    class Individual {
        public Action action;
        public int result;
        private GameObject body;
        private GameObject shouder;
        private GameObject arm;
        private List<Ray> lasers;

        public Individual(Action a ,GameObject b, GameObject s, GameObject ar,List<Ray> l) {
            action = a;
            body = b;
            shouder = s;
            arm = ar;
            lasers = l;
            ActionResult();
        }

        private void ActionResult() {
            // transform the model
            body.transform.localRotation = Quaternion.Euler(0, action.body_y, 0);
            shouder.transform.localRotation = Quaternion.Euler(action.shouder_x, action.shouder_y, action.shouder_y);
            arm.transform.localRotation = Quaternion.Euler(action.arm_x, action.arm_y, action.arm_z);
            Physics.SyncTransforms();

            // count the collide
            int count = 0;
            foreach (var l in lasers) {
                RaycastHit[] h = Physics.RaycastAll(l, 30f);
                if (h.Length > 0)
                {
                    if (h[0].transform.tag == "Body")
                    {
                        count++;
                    }
                }
            }
            result = count;
        }
    }


    
    class Action {
        // encoding
            // body
            public float body_y;

            // shouder
            public float shouder_x;
            public float shouder_y;
            public float shouder_z;

            // arm
            public float arm_x;
            public float arm_y;
            public float arm_z;

        // constructure
        public Action(float by,float sx,float sy,float sz,float ax,float ay,float az) {
            body_y = by;
            shouder_x = sx;
            shouder_y = sy;
            shouder_z = sz;
            arm_x = ax;
            arm_y = ay;
            arm_z = az;
        }

        // variation
        private void Variation(float variationIntensity) {
            int rand = Random.Range(0, 7);
            float offset = Random.value >= 0.5 ?Random.Range(0f,variationIntensity) : Random.Range(- variationIntensity,0f);
            switch (rand) {
                case 0:
                    body_y += offset;
                    break;
                case 1:
                    shouder_x += offset;
                    break;
                case 2:
                    shouder_y += offset;
                    break;
                case 3:
                    shouder_z += offset;
                    break;
                case 4:
                    arm_x += offset;
                    break;
                case 5:
                    arm_y += offset;
                    break;
                default:
                    arm_z += offset;
                    break;
            }
        }

        // generation
        public void Generation(Action another, List<Action> list, float variationPossibility, float exchangePossibility, float variationIntensity) {
            for (int i = 0; i < 2; i++) {
                // copy the value from parent
                Action a1 = new Action(body_y, shouder_x, shouder_y, shouder_z, arm_x, arm_y, arm_z);
                Action a2 = new Action(another.body_y, another.shouder_x, another.shouder_y, another.shouder_z, another.arm_x, another.arm_y, another.arm_z);
                if (Random.value <= exchangePossibility) {
                    Exchange(a1, a2);
                }
                if (Random.value <= variationPossibility) {
                    a1.Variation(variationIntensity);
                }
                if (Random.value <= variationPossibility)
                {
                    a2.Variation(variationIntensity);
                }
                list.Add(a1);
                list.Add(a2);
            }
            return;
        }

        private void Exchange(Action a1, Action a2)
        {
            int rand = Random.Range(0, 7);
            float temp;
            switch (rand)
            {
                case 0:
                    temp = a1.body_y;
                    a1.body_y = a2.body_y;
                    a2.body_y = temp;
                    break;
                case 1:
                    temp = a1.shouder_x;
                    a1.shouder_x = a2.shouder_x;
                    a2.shouder_x = temp;
                    break;
                case 2:
                    temp = a1.shouder_y;
                    a1.shouder_y = a2.shouder_y;
                    a2.shouder_y = temp;
                    break;
                case 3:
                    temp = a1.shouder_z;
                    a1.shouder_z = a2.shouder_z;
                    a2.shouder_z = temp;
                    break;
                case 4:
                    temp = a1.arm_x;
                    a1.arm_x = a2.arm_x;
                    a2.arm_x = temp;
                    break;
                case 5:
                    temp = a1.arm_y;
                    a1.arm_y = a2.arm_y;
                    a2.arm_y = temp;
                    break;
                default:
                    temp = a1.arm_z;
                    a1.arm_z = a2.arm_z;
                    a2.arm_z = temp;
                    break;
            }
        }

        public bool CheckVaild() {
            if (shouder_x < 235) return false;
            if (shouder_x > 315) return false;

            if (shouder_y < 310) return false;

            if (shouder_z < 20) return false;
            if (shouder_z > 90) return false;

            if (arm_x > 70) return false;


            if (arm_z <= 300) return false;

            return true;
        }
    }
}
