using UnityEngine;
using UnityEngine.UI;

public class LogicGridCell : MonoBehaviour
{
    private Image img;
    [SerializeField] private LogicGridSubdivision subdivision;
    
    public int row;
    public int col;
    private bool locked = false;
    private GridCellState currState;

    private void Start()
    {
        img = GetComponent<Image>();
        subdivision.add(this);
    }

    // Clicked on a cell
    // The cycle is Unknown -> Eliminated -> Confirmed -> ...
    public void toggleCell()
    {
        if(!locked)
        {
            switch (currState)
            {
                case GridCellState.Unknown:
                    currState = GridCellState.Eliminated;
                    break;
                case GridCellState.Eliminated:
                    currState = GridCellState.Confirmed;
                    subdivision.confirm(this);
                    // convert all related unknown squares to implied
                    subdivision.imply(this, true);
                    break;
                case GridCellState.Confirmed:
                    currState = GridCellState.Unknown;
                    subdivision.unconfirm(this);
                    // convert all related implied squares to unknown
                    subdivision.imply(this, false);
                    break;
            }

            redrawCell();
        }
    }

    // When implied, prevent all changes to this gridCell until further notice
    public void imply(bool locking)
    {
        locked = locking;
        Debug.Log("imply: set locked to " + locked);
        if(currState == GridCellState.Unknown)
        {
            currState = GridCellState.Eliminated_Implied;
        }
        else if (currState == GridCellState.Eliminated_Implied && !subdivision.isCoveredByCheck(this))
        {
            currState = GridCellState.Unknown;
        }
        else if(currState == GridCellState.Confirmed)
        {
            Debug.LogError("Error! The logic grid has two confirmed entries at once!");
        }

        redrawCell();
    }


    public void redrawCell()
    {
        switch (currState)
        {
            case GridCellState.Unknown:
                img.sprite = Notebook.instance.logicGridSpr_UNKNOWN;
                break;
            case GridCellState.Eliminated:
                img.sprite = Notebook.instance.logicGridSpr_ELIM_MANUAL;
                break;
            case GridCellState.Eliminated_Implied:
                img.sprite = Notebook.instance.logicGridSpr_ELIM_IMPLIED;
                break;
            case GridCellState.Confirmed:
                img.sprite = Notebook.instance.logicGridSpr_CONFIRMED;
                break;
        }
    }
}

public enum GridCellState
{
    Unknown,
    Eliminated,
    Eliminated_Implied,
    Confirmed
}