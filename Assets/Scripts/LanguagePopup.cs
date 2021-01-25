using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BizzyBeeGames.DotConnect
{
    public class LanguagePopup : Popup
    {
        [SerializeField] List<LanguageButton> languageButtons;
        [SerializeField] RectTransform selectButton;

        private void Start()
        {
            int index = PlayerPrefs.GetInt("Selected_Language", -1);

            Utils.DoSwipeVerticalAnimation(selectButton, selectButton.position.y, -2000, 0f, 0f);
            if (index >= 0)
            {
                languageButtons[index].Select(true);
                Utils.DoSwipeVerticalAnimation(selectButton, -2000, -600, 1f, 0f);
            }
        }

        public void Select(int index)
        {
            foreach(LanguageButton language in languageButtons){
                language.Select();
            }
            languageButtons[index].Select(true);
            Debug.Log(selectButton.position.y);
                Utils.DoSwipeVerticalAnimation(selectButton, -2000, -600, 1f, 0f);
        }

    }
}
