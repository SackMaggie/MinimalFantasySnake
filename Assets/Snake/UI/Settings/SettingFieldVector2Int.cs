using UnityEngine;

namespace Snake.Settings
{
    public class SettingFieldVector2Int : SettingFieldXY<Vector2Int>
    {
        protected override void OnSubmit(string inputX, string inputY)
        {
            if (int.TryParse(inputX, out int valueX) && int.TryParse(inputY, out int valueY) && ValidateValue(new Vector2Int(valueX, valueY)))
                WriteSettingValue(new Vector2Int(valueX, valueY));
            else
                ResetValue();
        }

        public override void ResetValue()
        {
            Vector2Int vector2 = ReadSettingValue();
            inputFieldX.text = $"{vector2.x}";
            inputFieldY.text = $"{vector2.y}";
        }
    }
}
