namespace Assets.KnifeGame.Scripts.Core.Knife
{
    // KnifeModel class represents the state and behavior of a knife
    // in the game no monobehavior functionality, just data and state management
    public class KnifeModel
    {
        public KnifeData KnifeData { get; private set; }

        public KnifeState State { get; private set; } = KnifeState.Inactive;


        public KnifeModel(KnifeData knifeData)
        {
            KnifeData = knifeData;
            State = KnifeState.Ready;
        }

        public bool CanThrow()
        {
            return State == KnifeState.Ready;
        }
        public void Throw()
        {
            State = KnifeState.Flying;
        }

        public void Stick()
        {
            State = KnifeState.Stuck;
        }

    }
}