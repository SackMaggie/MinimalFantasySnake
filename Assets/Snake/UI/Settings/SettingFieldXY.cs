namespace Snake.Settings
{
    public abstract class SettingFieldXY<T> : SettingField<T>
    {
        public TMPro.TMP_InputField inputFieldX;
        public TMPro.TMP_InputField inputFieldY;

        protected override void Start()
        {
            base.Start();
            inputFieldX.onSubmit.AddListener(OnSubmitX);
            inputFieldY.onSubmit.AddListener(OnSubmitY);
            inputFieldX.onEndEdit.AddListener(OnSubmitX);
            inputFieldY.onEndEdit.AddListener(OnSubmitY);
        }

        private void OnSubmitX(string arg0)
        {
            OnSubmit(arg0, inputFieldY.text);
        }

        private void OnSubmitY(string arg0)
        {
            OnSubmit(inputFieldX.text, arg0);
        }

        protected abstract void OnSubmit(string inputX, string inputY);
        public abstract override void ResetValue();
    }
}
