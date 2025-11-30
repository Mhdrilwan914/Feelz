using System;

public readonly struct GridPosition // Position of the cell on the grid
{
    public int Row { get; }
    public int Column { get; }

    
    public GridPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public bool IsAdjecent(GridPosition other) // To make a match we only allow adjecent sides not diagonally 
    {
        bool isSameRow = Row == other.Row;
        bool isSameColumn = Column == other.Column;

        bool rowDifferenceOne = Math.Abs(Row - other.Row) == 1;
        bool columnDifferenceOne = Math.Abs(Column - other.Column) == 1;

        return (rowDifferenceOne && isSameColumn) || (columnDifferenceOne && isSameRow); // same row different column and different row same column

    }
}
