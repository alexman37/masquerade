using UnityEngine;
using System;
using TMPro;

public class TableFieldButton : MonoBehaviour
{
    public int code;

    public TextMeshProUGUI textMesh;

    public static event Action<int> fieldSelected;

    public void finalizeSelection()
    {
        fieldSelected.Invoke(code);
    }
}
