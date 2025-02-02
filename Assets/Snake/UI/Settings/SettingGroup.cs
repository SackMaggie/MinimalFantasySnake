using TMPro;
using UnityEngine;

namespace Snake.Settings
{
    public class SettingGroup : CustomMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fieldNameText;
        public RectTransform container;

        public void SetFieldName(string fieldName) => fieldNameText.text = fieldName;
    }
}
