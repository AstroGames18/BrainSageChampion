using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class ChapterPackScreen : Popup
    {
        [SerializeField] Text MovesAmount, HintAmount, ResetAmount, UndoAmount, CoinAmount, TimerText, PriceText;
        [SerializeField] Image[] MediumCards, SmallerCards;
        [SerializeField] Image TitleBg;
        [SerializeField] Sprite darkTilteBg;


        private string dataPrefix = "chapter_pack_";

        private void OnEnable()
        {
            ChapterTier tier = (ChapterTier)getCurrentChapterTier()[0];
            MovesAmount.text = "x " + tier.chapter_pack.pack.moves;
            HintAmount.text = "x " + tier.chapter_pack.pack.hint;
            ResetAmount.text = "x " + tier.chapter_pack.pack.reset;
            UndoAmount.text = "x " + tier.chapter_pack.pack.undo;
            CoinAmount.text = "x " + tier.chapter_pack.pack.coins;
            PriceText.text = "$ " + tier.chapter_pack.price;
            if (UserDataManager.Instance.IsDarkModeOn())
            {
                GameConfiguration.Instance.SetDarkModeOnCards(MediumCards, SmallerCards);
                TitleBg.sprite = darkTilteBg;
            }

        }
        private void Update()
        {
            int remaining_time = (int)(UserDataManager.Instance.GetDateTime(dataPrefix + (int)getCurrentChapterTier()[1]) - DateTime.Now).TotalSeconds;
            TimerText.text = Utils.ConvertToTime(remaining_time);
            if (remaining_time <= 0)
            {
                Hide(true);
                UserDataManager.Instance.SetCurrentChapterPack(null);
            }
        }
        private object[] getCurrentChapterTier()
        {
            ChapterTier data;
            int curr_level = UserDataManager.Instance.GetData("current_level");
            for (int i = 0; i < GameConfiguration.Instance.ChapterTiers.Count; i++)
            {
                data = GameConfiguration.Instance.ChapterTiers[i];
                if (curr_level >= data.min_level && curr_level < data.max_level)
                {
                    return new object[] { data, i + 1 };
                }
            }
            return null;
        }
    }
}
