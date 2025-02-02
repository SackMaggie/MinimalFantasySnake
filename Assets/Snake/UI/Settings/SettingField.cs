using System;
using TMPro;
using UnityEngine;

namespace Snake.Settings
{
    public abstract class SettingField<T> : CustomMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fieldNameText;
        private Func<T> readValueFunc;
        private Action<T> writeValueFunc;

        public Func<T> ReadValueFunc
        {
            get => readValueFunc;
            set
            {
                readValueFunc = value;
                ResetValue();
            }
        }

        public Action<T> WriteValueFunc
        {
            get => writeValueFunc;
            set
            {
                writeValueFunc = value;
                ResetValue();
            }
        }

        public T ReadSettingValue()
        {
            return ReadValueFunc.Invoke();
        }

        public void WriteSettingValue(T value)
        {
            WriteValueFunc.Invoke(value);
        }

        public bool ValidateValue(T value)
        {
            return true;
        }

        public abstract void ResetValue();

        public void SetFieldName(string fieldName) => fieldNameText.text = fieldName;
    }
}
