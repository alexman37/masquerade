using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TableMaster : MonoBehaviour
{
    public static TableMaster instance;

    public List<TableColumn> columns = new List<TableColumn>();

    private List<string> tempValuesShown = new List<string>();
    private List<GameObject> buttons = new List<GameObject>();
    [SerializeField] GameObject buttonTemplate;
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject selectionScreen;
    [SerializeField] TextMeshProUGUI selectionScreenTitle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        columns = new List<TableColumn>
        {
            new TableColumn("Characters", new List<string>{"Duke", "Butler", "Heir", "Socialite", "Author", "Outcast"}),
            new TableColumn("Items", new List<string>{"Wine", "Camera", "Gun", "Crown", "Lockbox", "Goodbyes"}),
            new TableColumn("Roles", new List<string>{"Victim", "Witness", "Detective", "Red Herring", "Accomplice", "Murderer"}),
        };
    }

    private void OnEnable()
    {
        TableFieldButton.fieldSelected += updateSelectedField;
    }

    private void OnDisable()
    {
        TableFieldButton.fieldSelected -= updateSelectedField;
    }



    public void displaySelection(int columnId)
    {
        selectionScreen.SetActive(true);
        selectionScreenTitle.text = columns[columnId].colName;
        tempValuesShown = columns[columnId].getUnmarked();

        for (int i = 0; i < tempValuesShown.Count; i++)
        {
            string val = tempValuesShown[i];

            // Instantiate each button
            GameObject newGuy = GameObject.Instantiate(buttonTemplate, selectionScreen.transform);
            //newGuy.GetComponent<RectTransform>().pivot = new Vector2();
            newGuy.transform.localPosition = new Vector3(0, -160 + 400 - (i * 90), 0);
            newGuy.GetComponent<TableFieldButton>().code = i;
            newGuy.GetComponent<TableFieldButton>().textMesh.text = val;

            buttons.Add(newGuy);
        }

        GameObject newGuyC = GameObject.Instantiate(cancelButton, selectionScreen.transform);
        newGuyC.transform.localPosition = new Vector3(0, -160 + 400 - (tempValuesShown.Count * 90), 0);
        buttons.Add(newGuyC);
    }

    public void cleanup()
    {
        foreach (GameObject g in buttons)
        {
            Destroy(g);
        }
        selectionScreen.SetActive(false);
    }

    public string getAt(int code)
    {
        return tempValuesShown[code];
    }

    private void updateSelectedField(int code)
    {
        TableField.finalizeSelection(code);
    }
}


public class TableColumn
{
    public string colName;
    private List<string> allValues;
    private HashSet<string> usedValues = new HashSet<string>();

    public TableColumn(string colName, List<string> values)
    {
        this.colName = colName;
        allValues = values;
    }

    public List<string> getUnmarked()
    {
        List<string> unmarked = new List<string>();
        foreach(string s in allValues)
        {
            if(!usedValues.Contains(s)) unmarked.Add(s);
        }
        return unmarked;
    }

    public void mark(string val, bool mark)
    {
        if (mark) usedValues.Add(val);
        else usedValues.Remove(val);
    }
}