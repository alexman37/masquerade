using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogicGridSubdivision : MonoBehaviour
{
    private LogicGridCell[,] gridCells = new LogicGridCell[6,6];
    private HashSet<int> checkedRows = new HashSet<int>();
    private HashSet<int> checkedCols = new HashSet<int>();

    public void add(LogicGridCell lgc)
    {
        gridCells[lgc.row, lgc.col] = lgc;
    }

    public void confirm(LogicGridCell lgc)
    {
        checkedRows.Add(lgc.row);
        checkedCols.Add(lgc.col);
    }

    public void unconfirm(LogicGridCell lgc)
    {
        checkedRows.Remove(lgc.row);
        checkedCols.Remove(lgc.col);
    }

    public void imply(LogicGridCell lgc, bool confirmed)
    {
        for(int r = 0; r < gridCells.GetLength(0); r++)
        {
            if(r != lgc.row)
            {
                gridCells[r, lgc.col].imply(confirmed);
            }
        }

        for (int c = 0; c < gridCells.GetLength(0); c++)
        {
            if(c != lgc.col)
            {
                gridCells[lgc.row, c].imply(confirmed);
            }
        }
    }

    // When disabling an implied square, check if it's already covered by another confirmed entry
    public bool isCoveredByCheck(LogicGridCell lgc)
    {
        return checkedRows.Contains(lgc.row) || checkedCols.Contains(lgc.col);
    }
}
