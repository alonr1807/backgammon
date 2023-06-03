using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPackage
{
    public int[] moveData;
    public GameObject gObj;
    public Vector3 finalPos;
    public bool valid;

    public DataPackage(int[] moveData, GameObject gObj)
    {
        this.moveData = moveData;
        this.gObj = gObj;
    }
    public DataPackage(Vector3 finalPos, bool valid)
    {
        this.finalPos = finalPos;
        this.valid = valid;
    }

}
