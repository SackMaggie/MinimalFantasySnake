
namespace Snake.World
{
    public class WorldGrid : SnakeBehaviour
    {
        public static WorldGrid Instance;

        protected override void Start()
        {
            base.Start();
            Instance = this;
        }
    }
}