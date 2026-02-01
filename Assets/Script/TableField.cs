using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TableField : MonoBehaviour
{
    public int columnId;

    public string currGivenField = null;

    [SerializeField] private TextMeshProUGUI textField;


    private void OnEnable()
    {
        TableFieldButton.fieldSelected += finalizeSelection;
    }

    private void OnDisable()
    {
        TableFieldButton.fieldSelected -= finalizeSelection;
    }


    public void clickToSelect()
    {
        TableMaster.instance.displaySelection(columnId);
    }

    public void finalizeSelection(int code)
    {
        if(code != -1)
        {
            string newSelection = TableMaster.instance.getAt(code);
            string prior = textField.text;
            textField.text = newSelection;
            if (currGivenField == null)
            {
                TableMaster.instance.columns[columnId].mark(newSelection, true);
            }
            else
            {
                TableMaster.instance.columns[columnId].mark(prior, false);
                TableMaster.instance.columns[columnId].mark(newSelection, true);
            }
        }

        TableMaster.instance.cleanup();
    }
}
