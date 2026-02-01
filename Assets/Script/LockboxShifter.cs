using UnityEngine;

public class LockboxShifter : MonoBehaviour
{
    public int forIndex;
    public int by;

    
    public void shift()
    {
        LockboxMaster.instance.changePos(forIndex, by);
    }
}
