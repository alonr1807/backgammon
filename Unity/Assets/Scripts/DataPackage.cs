using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPackage
{

    public int moveTo;
    public GameObject checkerObject;
    public Vector3 finalPos;
    public bool valid;
    public bool escape;
    public GameObject escapeObject;

    //2 are used to send movement data and the other is used to send final position of the checker
    public DataPackage(int moveTo, GameObject checkerObject)
    {
        this.moveTo = moveTo;
        this.checkerObject = checkerObject;
        this.escape = false;
    }
    public DataPackage(bool escape, GameObject checkerObject, GameObject escapeObject)
    {
        this.escape = escape;
        this.checkerObject = checkerObject;
        this.escapeObject = escapeObject;
    }

    public DataPackage(Vector3 finalPos, bool valid)
    {
        this.finalPos = finalPos;
        this.valid = valid;
    }
    
}
