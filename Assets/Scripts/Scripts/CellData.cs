using UnityEngine;


// what each cell contains
public sealed class CellData
{
    public int Row; // row position in the grid

    public int Column; // column position in the grid

    public int TypeID;

    public CellData(int row, int column)
    {
        Row = row;
        Column = column;
        TypeID = -1;
    }

 
    public GridPosition ToGridView()
    {
       return new GridPosition(Row, Column);
    }


}
