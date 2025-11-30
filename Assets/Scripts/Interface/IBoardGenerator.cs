using System.Collections.Generic;
using UnityEngine;

//Generates a board with given rows and columns does not implement how the board is generated
// Does not matters who generates the board as long as it follows this interface
public interface IBoardGenerator
{
    public IEnumerable<CellData> GenrateBoard(int rows, int column);
}
