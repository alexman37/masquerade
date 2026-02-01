using UnityEngine;
using TMPro;

public class TableCritLetter : MonoBehaviour
{
    public int position;
    [SerializeField] private TextMeshProUGUI textMesh;

    private void OnEnable()
    {
        TableField.critLetterFound += critLetterFound;
    }

    private void OnDisable()
    {
        TableField.critLetterFound -= critLetterFound;
    }

    public void critLetterFound(int pos, char ch)
    {
        if(position == pos)
        {
            textMesh.text = ch.ToString();
        }
    }
}
