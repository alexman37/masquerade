using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notepad : MonoBehaviour
{
    public static Notepad instance;

    public Sprite logicGridSpr_UNKNOWN;
    public Sprite logicGridSpr_ELIM_MANUAL;
    public Sprite logicGridSpr_ELIM_IMPLIED;
    public Sprite logicGridSpr_CONFIRMED;

    [SerializeField] private Vector2 enabledSpot;
    [SerializeField] private Vector2 disabledSpot;
    [SerializeField] private float time;

    private bool into = false;
    private Coroutine activeCo;

    private RectTransform rt;

    private void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        rt = this.GetComponent<RectTransform>();
    }

    public void transition()
    {
        into = !into;
        if(activeCo != null)
        {
            StopCoroutine(activeCo);
        }

        if (into)
        {
            activeCo = StartCoroutine(slidingTransition(enabledSpot, time));
        }
        else
        {
            activeCo = StartCoroutine(slidingTransition(disabledSpot, time));
        }
    }



    // sliding transition
    private IEnumerator slidingTransition(Vector2 moveTo, float time)
    {
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = moveTo;

        float tick = 0;
        while (tick < time)
        {
            tick += Time.deltaTime;

            float factor = Mathf.Pow(tick / time, 2);

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, factor);

            yield return null;
        }

        yield return null;
    }
}
