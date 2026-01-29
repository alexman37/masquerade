using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    public float curviness = 2.0f;
    public bool startSoft = true;

    public Camera mainCam;
    public Transform[] cycledPositions;
    private int currCameraInCycle;

    private Coroutine activeCameraMovement;

    public static event Action<bool, int> cameraTransition = (_,__) => { };

    private void Start()
    {
        Transform initial = cycledPositions[0].transform;
        mainCam.transform.SetPositionAndRotation(initial.position, initial.rotation);
    }

    private void OnEnable()
    {
        CameraCycler.cycleCamera += cycleThroughCameras;
    }

    private void OnDisable()
    {
        CameraCycler.cycleCamera -= cycleThroughCameras;
    }


    public void cycleThroughCameras(int by)
    {
        int intermediate = (currCameraInCycle + by);
        if (intermediate >= 0) currCameraInCycle = intermediate % cycledPositions.Length;
        else currCameraInCycle = cycledPositions.Length - 1;

        cameraTransition.Invoke(false, currCameraInCycle);

        xerpMoveToPosition(cycledPositions[currCameraInCycle]);
    }


    private void lerpMoveToPosition(Transform t)
    {
        if(activeCameraMovement != null)
        {
            // TODO maybe we want to "queue it up" instead of ignoring it.
        } else
        {
            activeCameraMovement = StartCoroutine(lerpMovement(t, 2.0f));
        }
    }

    private void xerpMoveToPosition(Transform t)
    {
        if (activeCameraMovement != null)
        {
            // TODO maybe we want to "queue it up" instead of ignoring it.
        }
        else
        {
            activeCameraMovement = StartCoroutine(xerpMovement(t, 2.0f));
        }
    }

    private IEnumerator lerpMovement(Transform t, float time)
    {
        Vector3 startPos = mainCam.transform.position;
        Vector3 endPos = t.position;
        Quaternion startRot = mainCam.transform.rotation;
        Quaternion endRot = t.rotation;

        float tick = 0;
        while(tick < time)
        {
            tick += Time.deltaTime;

            mainCam.transform.position = Vector3.Lerp(startPos, endPos, tick / time);
            mainCam.transform.rotation = Quaternion.Lerp(startRot, endRot, tick / time);

            yield return null;
        }

        cameraTransition.Invoke(true, currCameraInCycle);
        yield return null;
        activeCameraMovement = null;
    }

    private IEnumerator xerpMovement(Transform t, float time)
    {
        Vector3 startPos = mainCam.transform.position;
        Vector3 endPos = t.position;
        Quaternion startRot = mainCam.transform.rotation;
        Quaternion endRot = t.rotation;

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, startSoft ? curviness : 1.0f / curviness);

            mainCam.transform.position = Vector3.Lerp(startPos, endPos, factor);
            mainCam.transform.rotation = Quaternion.Lerp(startRot, endRot, factor);

            yield return null;
        }

        cameraTransition.Invoke(true, currCameraInCycle);
        yield return null;
        activeCameraMovement = null;
    }
}
