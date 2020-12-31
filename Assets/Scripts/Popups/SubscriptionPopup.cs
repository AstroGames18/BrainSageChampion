using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class SubscriptionPopup : Popup
    {
        [SerializeField] GameObject PlayButton, ClaimButton;
        [SerializeField] Text NotificationText;
        [SerializeField] StartScreen startScreen = null;
        private int max_stars = 0;
        private bool outOfMoves = false;
        private string dataPrefix = "rate_us_in_chapter_";
        private void OnEnable()
        {
            max_stars = GameConfiguration.Instance.MiniGameMaxStars;
            int stars = UserDataManager.Instance.GetData("mini_game_stars");
            NotificationText.text = (Mathf.FloorToInt(stars / max_stars)).ToString();
            PlayButton.SetActive(stars >= max_stars);

            RefreshPopup();
        }
        private void RefreshPopup()
        {
            DateTime next_subscription_claim_time = UserDataManager.Instance.GetDateTime("subscription_claim_time");
            ClaimButton.SetActive(DateTime.Compare(next_subscription_claim_time, DateTime.Now) < 0);
        }
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            outOfMoves = false;
            if (inData != null && inData.Length > 0)
            {
                outOfMoves = (bool)inData[0];
            }
            SoundManager.Instance.Play("SubscriptionPopup");
        }
        public void OnPlayButton()
        {
            int stars = UserDataManager.Instance.GetData("mini_game_stars");

            if (stars >= max_stars)
            {
                SoundManager.Instance.Play("StartMiniGame");

                Hide(true);
                object[] tierInfo = getCurrentChapterTierInfo();
                int chapter_no = (int)tierInfo[1];
                bool show_rate_us = UserDataManager.Instance.GetData(dataPrefix + chapter_no) == 0;
                if (chapter_no % GameConfiguration.Instance.ChapterGapRateUsPopup == 0 && show_rate_us)
                {
                    UserDataManager.Instance.SetData("rate_us_in_chapter_" + chapter_no, 1);
                    UserDataManager.Instance.show_rate_us_popup = true;
                }
                else { UserDataManager.Instance.show_rate_us_popup = false; }
                UserDataManager.Instance.SetData("mini_game_stars", stars - max_stars);
                if (startScreen != null) { startScreen.ResetSocialMediaIcons(); }
                LoadingManager.Instance.LoadScene(LoadingManager.TOKGameScreen);
            }
        }
        public void OnClaimButton()
        {
            List<QuestReward> allRewards = new List<QuestReward>();
            DefiniteRewards subsReward = GameConfiguration.Instance.SubscriptionRewards;
            DateTime NextDailyReward = DateTime.Today.AddDays(1);
            UserDataManager.Instance.SetData("subscription_claim_time", NextDailyReward);

            if (subsReward.coins > 0)
            {
                QuestReward r = new QuestReward
                {
                    amount = subsReward.coins,
                    type = QuestReward.reward_types.COINS
                };
                allRewards.Add(r);
            }
            if (subsReward.hint > 0)
            {
                QuestReward r = new QuestReward
                {
                    amount = subsReward.hint,
                    type = QuestReward.reward_types.HINT
                };
                allRewards.Add(r);
            }
            if (subsReward.reset > 0)
            {
                QuestReward r = new QuestReward
                {
                    amount = subsReward.reset,
                    type = QuestReward.reward_types.RESET
                };
                allRewards.Add(r);
            }
            if (subsReward.undo > 0)
            {
                QuestReward r = new QuestReward
                {
                    amount = subsReward.undo,
                    type = QuestReward.reward_types.UNDO
                };
                allRewards.Add(r);
            }
            if (subsReward.moves > 0)
            {
                QuestReward r = new QuestReward
                {
                    amount = subsReward.moves,
                    type = QuestReward.reward_types.MOVES
                };
                allRewards.Add(r);
            }
            //InventoryManager.Instance.UpdateAllQuestRewards(allRewards);
            UserDataManager.Instance.SetUserGiftReward(allRewards,
                GiftType.RED, "SubscriptionPopup");
            PopupManager.Instance.Show("GiftScreen", null, onGiftScreenClosed);
        }
        private void onGiftScreenClosed(bool cancelled, object[] data)
        {
            RefreshPopup();
        }
        public void ClosePopup()
        {
            Hide(true);
            if (outOfMoves) { PopupManager.Instance.Show("Shop", new object[] { true }); }
        }
        private object[] getCurrentChapterTierInfo()
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
