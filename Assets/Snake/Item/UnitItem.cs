using Snake.Unit;
using UnityEngine;

namespace Snake.Item
{
    public class UnitItem : UnitBase, IItem
    {
        public ItemProperty ItemProperty { get; set; }

        public void OnPickUp(IUnit unit)
        {
            throw new System.NotImplementedException();
        }
    }
}
