using UnityEngine;

namespace Snake.Settings
{
    public class SettingFieldVector2 : SettingFieldXY<Vector2>
    {
        protected override void OnSubmit(string inputX, string inputY)
        {
            if (float.TryParse(inputX, out float valueX) && float.TryParse(inputY, out float valueY) && ValidateValue(new Vector2(valueX, valueY)))
                WriteSettingValue(new Vector2(valueX, valueY));
            else
                ResetValue();
        }

        public override void ResetValue()
        {
            Vector2 vector2 = ReadSettingValue();
            inputFieldX.text = $"{vector2.x}";
            inputFieldY.text = $"{vector2.y}";
        }
    }
}
