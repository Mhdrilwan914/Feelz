using System.Collections.Generic;

public class MoveHistory : IMoveHistory
{
    private readonly Stack<MoveRecord> _moveStack = new();
    public void ClearHistory()
    {
        _moveStack.Clear();
    }

    public void RecordMove(MoveRecord moveRecord)
    {
        _moveStack.Push(moveRecord);
    }

    public bool TryGetLastMove(out MoveRecord moveRecord)
    {
        if (_moveStack.Count <= 0)
        {
            moveRecord = default;

            return false;
        }

        moveRecord = _moveStack.Pop();
        return true;
    }
}
