using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeData : MonoBehaviour
{
    [SerializeField] Kind kind;

    public Kind getEscapeKind()
    {
        return kind;
    }
}
