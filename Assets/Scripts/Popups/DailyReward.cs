using BizzyBeeGames.DotConnect;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class DailyReward : Popup
    {
        [SerializeField] List<GameObject> Cards;
        [SerializeField] GameObject DailyRewardTimer, TimerInfo, ClaimButton;

        private bool show_timer = false;

        void OnEnable()
        {
            DateTime NextDailyReward = UserDataManager.Instance.GetDateTime("daily_reward_time");
            show_timer = DateTime.Compare(DateTime.Today, NextDailyReward) < 0;
            ToggleTimer(show_timer);
            Debug.Log(show_timer);
            if (!show_timer)
            {
                SetDailyRewardButtons();
                GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_DailyRewardSelected, onDailyRewardSelected);
            }
            UserDataManager.Instance.SetData("notification_dailyrewards", 0);
        }
        private void AnimateCardsAppearance(GameObject card, int index, bool hide = false, int selectedCardIndex = -1)
        {
            float delay;
            if (selectedCardIndex > -1)
            {
                if (index == selectedCardIndex)
                    delay = 0f;
                else if (selectedCardIndex < index)
                    delay = (index - selectedCardIndex) * 0.1f;
                else
                    delay = index * 0.1f;

            }
            else
            {
                delay = index * 0.1f;
            }
            StartCoroutine(Utils.ExecuteAfterDelay(delay, (args) =>
            {
                RectTransform rectT = card.transform as RectTransform;
                float posY = rectT.anchoredPosition.y;
                if (hide)
                {
                    SoundManager.Instance.Play("DailyRewardCardExit");
                    Utils.DoSwipeVerticalAnimation(rectT, posY, posY - 2000, 0.25f);
                }
                else
                {
                    card.GetComponent<DailyRewardButton>().ShowTrail(true);
                    SoundManager.Instance.Play("DailyRewardCardAppear");
                    Utils.DoSwipeVerticalAnimation(rectT, posY - 2000, posY, 0.25f);

                    StartCoroutine(Utils.ExecuteAfterDelay(0.4f, (args1) =>
                    {
                        card.GetComponent<DailyRewardButton>().ShowTrail(false);
                    }));
                }
                card.SetActive(true);
            }));
        }
        private void OnDisable()
        {
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_DailyRewardSelected, onDailyRewardSelected);
        }
        private void onDailyRewardSelected(string eventId, object[] data)
        {
            int index = (int)data[0];
            DateTime NextDailyReward = DateTime.Today.AddDays(1);
            UserDataManager.Instance.SetData("daily_reward_time", NextDailyReward);
            show_timer = true;
            SoundManager.Instance.Play("DailyRewardCardSelected");
            Cards[index].GetComponent<DailyRewardButton>().ShowBorderParticles();
            StartCoroutine(Utils.ExecuteAfterDelay(3f, (args) =>
            {
                for (int i = 0; i < Cards.Count; i++)
                {
                    if (index != i) { AnimateCardsAppearance(Cards[i], Cards.Count - i - 1, true, index); }
                }
                FocusCard(Cards[index]);
            }));
        }
        private void FocusCard(GameObject card)
        {
            StartCoroutine(Utils.ExecuteAfterDelay(Cards.Count * 0.1f, (args) =>
            {
                RectTransform rectT = card.transform as RectTransform;
                float posY = rectT.anchoredPosition.y;
                float posX = rectT.anchoredPosition.x;

                Utils.DoSwipeVerticalAnimation(rectT, posY, -584f, 0.5f);
                Utils.DoSwipeHorizontalAnimation(rectT, posX, 540f, 0.5f, 0f);
                Utils.DoSwipeVerticalAnimation(ClaimButton.transform as RectTransform, -1200f, -650f, 1f);
                StartCoroutine(Utils.ExecuteAfterDelay(0.6f, (a) =>
                {
                    card.GetComponent<DailyRewardButton>().ShowBlast();
                }));
            }));
        }
        private void ToggleTimer(bool enable)
        {
            TimerInfo.SetActive(enable);
        }
        private void Update()
        {
            if (show_timer)
            {
                ToggleTimer(true);
                int remaining_time = (int)(UserDataManager.Instance.GetDateTime("daily_reward_time") - DateTime.Now).TotalSeconds;
                DailyRewardTimer.GetComponent<Text>().text = Utils.ConvertSecondsToString(remaining_time);
            }
        }
        public void onDailyRewardClaim()
        {
            Hide(true);
            SoundManager.Instance.Play("DailyRewardsClaim");
        }
        private void SetDailyRewardButtons()
        {
            List<ProbabilityData> allData = new List<ProbabilityData>();
            List<DailyRewardConfig> config = GameConfiguration.Instance.DailyRewardConfig;
            for (int j = 0; j < config.Count; j++)
            {
                allData.Add(new ProbabilityData(config[j].probability, config[j]));
            }
            ProbabilityResult res = Utils.GetRandomNumberInWeights(allData);
            DailyRewardSet = (DailyRewardConfig)res.item;

            for (int i = 0; i < Cards.Count; i++)
            {
                GameObject card = Cards[i];
                card.SetActive(false);

                SelectableReward reward = GetRandomReward();
                card.GetComponent<DailyRewardButton>().SetDailyRewardButton(reward.type, reward.amount, i);
                AnimateCardsAppearance(card, i);
            }
        }
        DailyRewardConfig DailyRewardSet = null;

        SelectableReward GetRandomReward()
        {
            if (DailyRewardSet == null)
                return null;
            int i = UnityEngine.Random.Range(0, DailyRewardSet.reward.Count - 1);
            SelectableReward reward = DailyRewardSet.reward[i];
            DailyRewardSet.reward.RemoveAt(i);
            return reward;
        }

    }
}
