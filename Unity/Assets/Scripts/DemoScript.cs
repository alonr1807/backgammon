using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public GameObject checkerStart;
    public GameObject checkerDown;
    public GameObject checkerLeft;

    public Material blackMaterial;
    public Material whiteMaterial;

    public List<GameObject> checkers = new List<GameObject>();

    public int from;
    public int to;
    public bool go = false;

    //public List<Vector3> startPos = new List<Vector3>();


    /*
     * void DrawBoard(Cell[] board) {
     *   
        // Delete all children

        // Loop over board array
        // Find position of cell in the world
        // Create N checkers (set color by setting material)
     * }
     */

    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPosition = checkerStart.transform.position;
        Vector3 distanceDown = checkerDown.transform.position - checkerStart.transform.position;
        Vector3 distanceLeft = checkerLeft.transform.position - checkerStart.transform.position;

        for (int j = 0; j < 6; ++j)
        {
            for (int i = 0; i < 5; ++i)
            {
                GameObject copy = Instantiate(checkerStart);
                copy.SetActive(true);
                copy.transform.position = checkerStart.transform.position + distanceDown * i + distanceLeft * j;
                checkers.Add(copy);
                copy.transform.GetChild(0).GetComponent<MeshRenderer>().material = (j % 2 == 0) ? blackMaterial : whiteMaterial;
                //startPos.Add(copy.transform.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            Debug.Log("You want to move from " + from + " to " + to);
            go = false;
        }
    }
}
