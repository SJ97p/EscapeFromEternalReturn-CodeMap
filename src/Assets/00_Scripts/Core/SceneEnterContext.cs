namespace HBDinosaur_ER_Project.SceneSystem
{
    public class SceneEnterContext
    {
        public GameScene PreviousScene { get; set; }
        public object Payload { get; set; }

        public bool TryGetPayload<T>(out T payload)
        {
            if (Payload is T value)
            {
                payload = value;
                return true;
            }

            payload = default;
            return false;
        }
    }
}