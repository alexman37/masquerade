using UnityEngine;
using System.Collections;
using System;

public class Notebook : MonoBehaviour
{
    public static Notebook instance;

    public Sprite logicGridSpr_UNKNOWN;
    public Sprite logicGridSpr_ELIM_MANUAL;
    public Sprite logicGridSpr_ELIM_IMPLIED;
    public Sprite logicGridSpr_CONFIRMED;

    [SerializeField] private Vector3 enabledSpot;
    [SerializeField] private Vector3 disabledSpot;
    [SerializeField] private float time;

    private bool into = false;
    private Coroutine activeCo;

    private int currPage = 0; // front cover
    [SerializeField] private GameObject[] pageHinges;
    [SerializeField] private GameObject binding;
    [SerializeField] private GameObject[] canvases;
    [SerializeField] private GameObject pageTurners;
    private float[] defaultPagePositions;

    public static event Action notebookOpened = () => { };
    public static event Action notebookClosed = () => { };

    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        defaultPagePositions = new float[pageHinges.Length];
        for(int i = 0; i < defaultPagePositions.Length; i++)
        {
            defaultPagePositions[i] = pageHinges[i].transform.position.z;
        }
    }

    public void transition()
    {
        into = !into;
        if (activeCo != null)
        {
            StopCoroutine(activeCo);
        }

        if (into)
        {
            notebookOpened.Invoke();
            enablePageTurners();
            activeCo = StartCoroutine(slidingTransition(enabledSpot, time));
        }
        else
        {
            notebookClosed.Invoke();
            disablePageTurners();
            activeCo = StartCoroutine(slidingTransition(disabledSpot, time));
        }
    }


    public void turnPage(int by)
    {
        if(activeCo == null)
        {
            int temp = currPage + by;
            if(temp >= 0 && temp < pageHinges.Length + 1)
            {
                if(by > 0)
                {
                    if(currPage == 0)
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, 0, 0), time, true, true));
                    else
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, 179.99f, 0), time, false, true));
                } else
                {
                    if (currPage == 1)
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, -90, 0), time, true, false));
                    else
                        activeCo = StartCoroutine(pageTurnAnimation(temp, Quaternion.Euler(0, 0.01f, 0), time, false, false));
                }
            }
        }
    }



    // sliding transition
    private IEnumerator slidingTransition(Vector3 moveTo, float time)
    {
        SfxManager.instance.playSFXbyName("slide-away", null, 1);

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = moveTo;

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, 2);

            transform.localPosition = Vector3.Lerp(startPos, endPos, factor);

            yield return null;
        }

        activeCo = null;
        yield return null;
    }


    // sliding transition
    private IEnumerator pageTurnAnimation(int toWhichPage, Quaternion rotateTo, float time, bool withBinding, bool positive)
    {
        SfxManager.instance.playSFXbyName("page-turn", null, 1);

        if (!positive) currPage -= 1;

        Quaternion startRot = pageHinges[currPage].transform.localRotation;
        Quaternion endRot = rotateTo;

        enableNewCanvases(currPage, toWhichPage);

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, 2);

            pageHinges[currPage].transform.localRotation = Quaternion.Lerp(startRot, endRot, factor);
            if (withBinding)
            {
                binding.transform.localRotation = Quaternion.Lerp(startRot, endRot, factor);
            }

            yield return null;
        }
        pageHinges[currPage].transform.localRotation = endRot;

        disableOldCanvases(currPage, toWhichPage, positive);

        // move pages so that new rotations go on top of old ones
        Vector3 afterRot = pageHinges[currPage].transform.position;
        pageHinges[currPage].transform.position = new Vector3(afterRot.x, afterRot.y, getNewPagePosition(currPage, positive));

        

        activeCo = null;
        currPage = toWhichPage;
        yield return null;
    }

    private void enableNewCanvases(int cPage, int toWhichPage)
    {
        Debug.Log("Enabling " + toWhichPage);
        // swap out canvases in use
        if (toWhichPage > 0)
        {
            if (toWhichPage == 1)
            {
                canvases[0].SetActive(true);
            }
            else if (toWhichPage == pageHinges.Length)
            {
                canvases[canvases.Length - 1].SetActive(true);
            }
            else
            {
                Debug.Log("currpage " + cPage + " whichPage " + toWhichPage);
                if (toWhichPage < pageHinges.Length) canvases[toWhichPage * 2 - 2].SetActive(true);
                if (toWhichPage > 1) canvases[toWhichPage * 2 - 3].SetActive(true);
            }
        }
    }

    private void disableOldCanvases(int cPage, int toWhichPage, bool positive)
    {
        Debug.Log("Disabling " + cPage);
        // swap out canvases in use
        if (toWhichPage > 0)
        {
            if (toWhichPage == 1)
            {
                canvases[1].SetActive(false);
                canvases[2].SetActive(false);
            }
            else if (toWhichPage == pageHinges.Length)
            {
                canvases[cPage].SetActive(false);
            }
            else
            {
                if(positive)
                {
                    if (cPage < pageHinges.Length - 1) canvases[cPage * 2 - 2].SetActive(false);
                    if (cPage > 1) canvases[cPage * 2 - 3].SetActive(false);
                } else
                {
                    if (cPage < pageHinges.Length - 1) canvases[cPage * 2].SetActive(false);
                    if (cPage > 1) canvases[cPage * 2 - 1].SetActive(false);
                }
                
            }
        }
    }

    private float getNewPagePosition(int pageNum, bool turnedOver)
    {
        if (!turnedOver) return defaultPagePositions[pageNum];
        else return defaultPagePositions[defaultPagePositions.Length - 1 - pageNum];
    }

    private void enablePageTurners()
    {
        pageTurners.SetActive(true);
    }

    private void disablePageTurners()
    {
        pageTurners.SetActive(false);
    }
}
