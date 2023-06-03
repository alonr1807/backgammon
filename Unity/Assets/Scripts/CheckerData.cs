using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerData : MonoBehaviour
{
    private Checker kind;
    private int position;

    public void setKind(Checker k)
    {
        kind = k;
    }
    public Checker getKind()
    {
        return kind;
    }
    public void setPosition(int p)
    {
        position = p;
    }
    public int getPosition()
    {
        return position;
    }
}
