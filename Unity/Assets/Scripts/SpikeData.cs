using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeData : MonoBehaviour
{
    private int index;
    private Transform spikePosition;

    public int getIndex()
    {
        return index;
    }

    public void setIndex(int i)
    {
        index = i;
    }

    public Transform getSpikePosition()
    {
        return spikePosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        spikePosition = gameObject.transform;
    }
}
