using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

public class TableField : MonoBehaviour
{
    public int columnId;

    public string currGivenField = null;

    private static TableField activeField;

    public int critPosition = -1;
    public int critIndex = -1;

    [SerializeField] private TextMeshProUGUI textField;

    public static event Action<int, char> critLetterFound = (_, _) => { };


    public void clickToSelect()
    {
        TableMaster.instance.displaySelection(columnId);
        activeField = this;
    }

    // Finalize only for the active field
    public static void finalizeSelection(int code)
    {
        if(code != -1)
        {
            string newSelection = TableMaster.instance.getAt(code);
            string prior = activeField.textField.text;
            activeField.textField.text = newSelection;
            if (activeField.currGivenField == null)
            {
                TableMaster.instance.columns[activeField.columnId].mark(newSelection, true);
            }
            else
            {
                TableMaster.instance.columns[activeField.columnId].mark(prior, false);
                TableMaster.instance.columns[activeField.columnId].mark(newSelection, true);
            }

            if (activeField.critPosition != -1)
            {
                critLetterFound.Invoke(activeField.critIndex, newSelection[activeField.critPosition]);
            }

            activeField.GetComponent<Image>().color = new Color(93f / 255f, 153f / 255f, 141f / 255f, 1);
        } else
        {
            // Clear existing selection
            string prior = activeField.textField.text;
            activeField.textField.text = "?????";
            TableMaster.instance.columns[activeField.columnId].mark(prior, false);
            activeField.GetComponent<Image>().color = new Color(1,1,1,1);
        }

        
        TableMaster.instance.cleanup();
    }
}
