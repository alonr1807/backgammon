using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public GameObject checkerStart;
    public GameObject checkerDown;
    public GameObject checkerRight;

    public List<GameObject> checkers = new List<GameObject>();
    //public List<Vector3> startPos = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPosition = checkerStart.transform.position;
        Vector3 distanceDown = checkerDown.transform.position - checkerStart.transform.position;
        Vector3 distanceRight = checkerRight.transform.position - checkerStart.transform.position;

        for (int j = 0; j < 6; ++j)
        {
            for (int i = 0; i < 5; ++i)
            {
                GameObject copy = Instantiate(checkerStart);
                copy.SetActive(true);
                copy.transform.position = checkerStart.transform.position + distanceDown * i + distanceRight * j;
                checkers.Add(copy);
                //startPos.Add(copy.transform.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //for(int i = 0; i < checkers.Count; ++i)
        //{
        //    checkers[i].transform.position = startPos[i] + 0.1f * Vector3.up * Mathf.Sin(10f * Time.time) + 0.1f * Vector3.right * Mathf.Cos(10 * Time.time);
        //}
    }
}
