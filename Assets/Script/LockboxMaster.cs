using UnityEngine;
using System.Collections;

public class LockboxMaster : MonoBehaviour
{
    public static LockboxMaster instance;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject[] models;

    [SerializeField] private GameObject closedVersion;
    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject openVersion;

    public int[] currPositions;
    public int[] correctPositions;

    private Coroutine activeCo;

    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform openBoxView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public bool checkCorrect()
    {
        for(int i = 0; i < currPositions.Length; i++)
        {
            if(currPositions[i] != correctPositions[i])
            {
                Debug.Log("Lockbox combo failed: index " + i + ", expected " + correctPositions[i] + ", was " + currPositions[i]);
                return false;
            }
        }
        return true;
    }

    public void changePos(int index, int by)
    {
        int currValue = currPositions[index];
        if(currValue == 0 && by == -1)
        {
            currPositions[index] = currPositions.Length - 1;
        } else
        {
            currPositions[index] = (currValue + by) % currPositions.Length;
        }

        rotateModel(index, currPositions[index] * -60f);
    }

    private void rotateModel(int index, float newDeg)
    {
        //models[index].transform.localRotation = Quaternion.Euler(newDeg, 180, 0);
        if(activeCo != null)
        {
            StopCoroutine(activeCo);
        }
        activeCo = StartCoroutine(rotateModelCo(index, newDeg));
    }

    private IEnumerator rotateModelCo(int index, float newDeg)
    {
        Quaternion startRot = models[index].transform.localRotation;
        Quaternion endRot = Quaternion.Euler(newDeg, 180, 0);
        float time = 0.4f;

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, 1f / 2f);

            models[index].transform.localRotation = Quaternion.Lerp(startRot, endRot, factor);

            yield return null;
        }
        models[index].transform.localRotation = endRot;

        activeCo = null;
        yield return null;
    }


    public void attemptSubmit()
    {
        if(checkCorrect())
        {
            Debug.Log("Correct");
            // Open lockbox
            closedVersion.SetActive(false);
            lockObject.SetActive(false);
            openVersion.SetActive(true);

            canvas.SetActive(false);

            // Camera animation
            StartCoroutine(finalMovement(openBoxView, 5f));

            // Ending sequence
        } else
        {
            Debug.Log("Incorrect");
            // Play "denied" sound effect
        }
    }

    private IEnumerator finalMovement(Transform t, float time)
    {
        yield return new WaitForSeconds(3f);

        Vector3 startPos = mainCam.transform.position;
        Vector3 endPos = t.position;
        Quaternion startRot = mainCam.transform.rotation;
        Quaternion endRot = t.rotation;

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, 2);

            mainCam.transform.position = Vector3.Lerp(startPos, endPos, factor);
            mainCam.transform.rotation = Quaternion.Lerp(startRot, endRot, factor);

            yield return null;
        }

        // End the game (to be continued?)
        yield return null;
    }

    public void showCanvas(bool show)
    {
        canvas.SetActive(show);
    }
}
