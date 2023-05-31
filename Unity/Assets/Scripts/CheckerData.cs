using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerData : MonoBehaviour
{
    private Checker kind;
    private int position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setKind(Checker k)
    {
        kind = k;
    }
    public Checker getKind()
    {
        return kind;
    }
    public void setPositon(int p)
    {
        position = p;
    }
    public int getPosition()
    {
        return position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
