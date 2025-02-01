using Snake.Item;

namespace Snake.Unit
{
    public interface IItem : IUnit
    {
        ItemProperty ItemProperty { get; set; }

        /// <summary>
        /// When the unit collied with an item
        /// </summary>
        /// <param name="unit"></param>
        void OnPickUp(IUnit unit);
    }
}
