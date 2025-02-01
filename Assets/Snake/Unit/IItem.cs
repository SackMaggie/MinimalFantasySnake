namespace Snake.Unit
{
    public interface IItem
    {
        /// <summary>
        /// When the unit collied with an item
        /// </summary>
        /// <param name="unit"></param>
        void OnPickUp(IUnit unit);
    }
}
