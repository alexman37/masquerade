using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSc : MonoBehaviour
{
    public static GameManagerSc instance;

    public TextAsset textFile;

    public GameObject TitleCanvas;
    public GameObject[] OtherCanvases;
    public GameObject explanation;
    public GameObject FullCanvas;
    public GameObject finalCanvas;

    public void begin()
    {
        explanation.SetActive(true);
        TitleCanvas.SetActive(false);
    }

    public void begin2()
    {
        explanation.SetActive(false);
        foreach (GameObject g in OtherCanvases)
        {
            g.SetActive(true);
        }
        FullCanvas.SetActive(false);
    }

    public void end()
    {
        finalCanvas.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject g in OtherCanvases)
        {
            g.SetActive(false);
        }
        if (instance == null) instance = this;
        else Destroy(this);
    }
}
