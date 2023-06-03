using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeData : MonoBehaviour
{
    [SerializeField] Checker kind;

    public Checker getEscapeKind()
    {
        return kind;
    }
}
