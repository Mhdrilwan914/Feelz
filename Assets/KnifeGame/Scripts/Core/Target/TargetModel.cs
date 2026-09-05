
namespace Assets.KnifeGame.Scripts.Core.Target
{

    public class TargetModel
    {
        public TargetData Data { get; }

        public float RotationSpeed { get; private set; }

        public bool IsRotatingClockwise { get; private set; }

        public float RotationDirection => IsRotatingClockwise ? 1f : -1f;

        public TargetModel(TargetData data)
        {
            Data = data;
            RotationSpeed = data.RotationSpeed;
            IsRotatingClockwise = data.IsRotatingClockwise;
        }


        public void ReverseRotationDirection()
        {
            IsRotatingClockwise = !IsRotatingClockwise;
        }


        public void SetRotationSpeed(float speed)
        {
            RotationSpeed = speed;
        }



    }
}