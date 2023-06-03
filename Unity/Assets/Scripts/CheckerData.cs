using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerData : MonoBehaviour
{
    private Kind kind;
    private int position;

    public void setKind(Kind k)
    {
        kind = k;
    }
    public Kind getKind()
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
