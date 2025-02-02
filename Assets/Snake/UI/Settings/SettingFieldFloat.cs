namespace Snake.Settings
{
    public class SettingFieldFloat : SettingField<float>
    {
        public TMPro.TMP_InputField inputField;

        public override void ResetValue()
        {
            inputField.text = $"{ReadSettingValue()}";
        }

        protected override void Start()
        {
            base.Start();
            inputField.onSubmit.AddListener(OnSubmit);
            inputField.onEndEdit.AddListener(OnSubmit);
        }

        private void OnSubmit(string arg0)
        {
            if (float.TryParse(arg0, out float value) && ValidateValue(value))
                WriteSettingValue(value);
            else
                ResetValue();
        }
    }
}
