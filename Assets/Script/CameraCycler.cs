using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraCycler : MonoBehaviour
{
    public static event Action<int> cycleCamera = (_) => { };
    
    public void cycle(int by)
    {
        cycleCamera.Invoke(by);
    }
}
