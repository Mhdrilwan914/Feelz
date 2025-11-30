using JetBrains.Annotations;
using UnityEngine;


// This Script is core brain of the board
public class GridSystem
{
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public float CellSize { get; private set; }

    public CellData[,] Cells { get; private set; }

    public GridSystem(int rows, int columns, float cellSize)
    {
        Rows = rows;
        Columns = columns;
        CellSize = cellSize;

        Cells = new CellData[rows, columns]; // create the grid base data

        for (int row =0; row< Rows; row++)
        {
            for (int column=0; column< Columns; column++)
            {
                Cells[row, column] = new CellData(row, column);
               
            }   
        }

       
    
    }

    public bool IsinsideGrid(int row, int column)
    {
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
    }

    public CellData GetCellData(int row, int column)
    {
        if (!IsinsideGrid(row, column))
        {
            return null;
        }
        return Cells[row, column];
    }

    public Vector3 GetCellWorldPosition(int row, int column)
    {
        return new Vector3(column * CellSize, 0, row * CellSize);
    }


}
