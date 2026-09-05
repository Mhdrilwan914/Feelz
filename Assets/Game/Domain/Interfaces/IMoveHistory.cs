public interface IMoveHistory
{
    public void RecordMove(MoveRecord moveRecord);
    public bool TryGetLastMove(out MoveRecord moveRecord);
    public void ClearHistory();
}