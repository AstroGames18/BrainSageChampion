using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class ApplySettingsScreen : Popup
    {
        [SerializeField] Image[] cards;
        [SerializeField] Sprite[] images;
        [SerializeField] Text text;


        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            if (UserDataManager.Instance.IsDarkModeOn())
            {
                cards[0].sprite = images[0];
                cards[1].sprite = images[2];

                text.text = Lean.Localization.LeanLocalization.GetTranslationText("Change to Normal? ");
            }
            else
            {
                cards[0].sprite = images[1];
                cards[1].sprite = images[3];

                text.text = Lean.Localization.LeanLocalization.GetTranslationText("Change to Darkmode?");
            }
        }
    }
}
