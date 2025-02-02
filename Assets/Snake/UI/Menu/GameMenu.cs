using UnityEngine;
using UnityEngine.UI;

namespace Snake
{
    public class GameMenu : CustomMonoBehaviour
    {
        public Button startGameButton;

        public Button settingButton;

        public Button settingBackToMenu;

        [SerializeField] private GameObject settingMenu;


        protected override void Start()
        {
            base.Start();
            settingMenu.SetActive(false);
            settingButton.onClick.AddListener(() => settingMenu.SetActive(true));
            settingBackToMenu.onClick.AddListener(() => settingMenu.SetActive(false));
        }
    }
}
