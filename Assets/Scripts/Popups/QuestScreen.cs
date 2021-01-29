using BizzyBeeGames.DotConnect;
using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class QuestScreen : Popup
    {
        public GameObject MovesText, TrophyText, QuestInfoText, QuestTimer, ProgressText, QuestIcon, QuestInfoClaimButton, QuestOfferClaimButton, CloseButtonObj, questInfoItems, delayTimeText;
        [SerializeField] AnimatedScrollbar QuestProgress;
        [SerializeField] Text QuestLevel, QuestInfo;
        [SerializeField] Text EarnedMovesText, EarnedTrophyText, TopHudTrophyText;
        [SerializeField] Image ProgressBG, QuestSlider;
        [SerializeField] Sprite IncompleteProgressBG, CompleteProgressBG, LollipopSlider, StarSlider, PairedCandySlider;
        public GameObject QuestOfferObject, MoveQuestOfferText, CardAnimatedShine;
        public GameObject CurrentPriceText, GiftImage, AnimationContainer;
        public Sprite HintSprite, ResetSprite, UndoSprite, TrophySprite, CoinSprite, StarsSpriteSmall, LollipopSpriteSmall, PairedCandySpriteSmall, DarkCardBG;
        public Image movesIcon, trophyIcon;

        public ParticleSystem[] animParticles;

        private bool outOfMoves = false;
        public class RewardDisplay
        {
            public int amount;
            public Sprite image;
            public string label;
            public string inventory_key;
        }
        private void Start()
        {
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_QuestUserUpdated, OnQuestScreenUpdated);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnQuestScreenUpdated);

            delayTimeText.SetActive(false);
            questInfoItems.SetActive(true);
        }

        private void OnDestroy()
        {
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_QuestUserUpdated, OnQuestScreenUpdated);
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnQuestScreenUpdated);
            if (QuestManager.Instance != null)
                QuestManager.Instance.waitForClaim = false;
        }
        private void OnEnable()
        {
            RefreshQuestPage();
            UserDataManager.Instance.SetData("notification_questscreen", 0);
        }
        private void OnQuestScreenUpdated(string eventId, object[] data)
        {
            if (gameObject != null)
            {
                RefreshQuestPage();
            }
        }
        public void RefreshQuestPage()
        {
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

            UpdateQuestInfo(user_quest);
            UpdateQuestOffer();
            QuestInfoClaimButton.SetActive(user_quest.amount_collected >= user_quest.max_amount);
            CloseButtonObj.SetActive(user_quest.amount_collected < user_quest.max_amount);
            Debug.Log("Quest Level: " + UserDataManager.Instance.GetData("quest_level"));
            QuestLevel.text = LeanLocalization.GetTranslationText("QuestLevel") + " - " + (UserDataManager.Instance.GetData("quest_level") + 1);
            if (user_quest.amount_collected >= user_quest.max_amount)
            {
                QuestInfo.text = LeanLocalization.GetTranslationText("Quest Accomplished");
            }
            else
            {
                QuestInfo.text = LeanLocalization.GetTranslationText("New Quest Unlocked");
            }
            CardAnimatedShine.SetActive(user_quest.amount_collected >= user_quest.max_amount);
            if (UserDataManager.Instance.IsDarkModeOn()) { ProgressBG.sprite = DarkCardBG; }
            else if (user_quest.amount_collected >= user_quest.max_amount) { ProgressBG.sprite = CompleteProgressBG; }
            else { ProgressBG.sprite = IncompleteProgressBG; }
        }
        private void UpdateQuestInfo(UserQuest user_quest)
        {
            RewardDisplay reward_display = GetQuestInfoReward(user_quest);
            GameConfiguration config = GameConfiguration.Instance;

            MovesText.GetComponent<Text>().text = GetMovesText();
            TrophyText.GetComponent<Text>().text = GetTrophyText();
            QuestInfoText.GetComponent<Text>().text = String.Format(reward_display.label, user_quest.max_amount);
            QuestTimer.GetComponent<Text>().text = Utils.ConvertSecondsToString((int)QuestManager.Instance.GetRemainingTimeForQuest());
            ProgressText.GetComponent<Text>().text = Math.Min(user_quest.amount_collected, user_quest.max_amount) + "/" + user_quest.max_amount;
            QuestProgress.SetValue(Math.Min(user_quest.amount_collected, user_quest.max_amount) / (float)user_quest.max_amount);
            QuestIcon.GetComponent<Image>().sprite = reward_display.image;
            QuestIcon.transform.localScale = GetQuestInfoIconScale(user_quest);
            GiftImage.GetComponent<Image>().sprite = config.GetGiftSprite(QuestManager.Instance.GetCurrentQuestTierInfo().TypeOfGift);

            Sprite slider_bg = null;
            switch (user_quest.type)
            {
                case QuestType.collectable_type.COLLECT_STARS:
                    slider_bg = StarSlider;
                    break;
                case QuestType.collectable_type.COLLLECT_LOLLIPOP:
                    slider_bg = LollipopSlider;
                    break;
                case QuestType.collectable_type.PAIR_CANDIES:
                    slider_bg = PairedCandySlider;
                    break;
            }
            if (slider_bg != null) { QuestSlider.sprite = slider_bg; }
        }
        public void OnMovesRewardAnimationComplete()
        {
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

            int moves_reward = user_quest.moves_rewarded;
            //ToDo
            //InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryMoves, moves_reward);

            Action<object[]> giftScreenCallBack = onGiftScreenClosed;
            PopupManager.Instance.Show("GiftScreen", new object[] { giftScreenCallBack, false });
            GetComponent<Animator>().SetBool("claim", false);
        }
        public void ClaimRewards()
        {

            QuestManager.Instance.waitForClaim = false;
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();

            int moves_reward = user_quest.moves_rewarded;
            GiftToReward gift = UserDataManager.Instance.GetUserGift("QuestScreen");
            gift.moves = moves_reward;
            UserDataManager.Instance.SetUserGift("", gift);

            GetComponent<Animator>().SetBool("claim", true);
            SoundManager.Instance.Play("QuestSCreenClaim");
            AnimationContainer.SetActive(true);
        }
        public void onGiftScreenClosed(object[] args)
        {
            QuestManager.Instance.UpdateQuest();
            MovesText.SetActive(true);
            movesIcon.color = new Color(1f, 1f, 1f, 1f);
            TrophyText.SetActive(true);
            trophyIcon.color = new Color(1f, 1f, 1f, 1f);
        }
        private void UpdateQuestOffer()
        {
            bool show_quest_offer = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves) <= GameConfiguration.Instance.MovesLowerThreshold;
            QuestOfferObject.SetActive(show_quest_offer);
            QuestOfferClaimButton.SetActive(show_quest_offer);
        }

        private void Update()
        {
            int remaining_time = (int)QuestManager.Instance.GetRemainingTimeForQuest();
            if (remaining_time <= 0 && !QuestManager.Instance.waitForClaim)
            {
                delayTimeText.SetActive(true);
                questInfoItems.SetActive(false);
                delayTimeText.GetComponent<Text>().text = Utils.ConvertSecondsToString((int)QuestManager.Instance.questDelayTime);
            }
            else
            {
                string remaining_time_str = Utils.ConvertSecondsToString(remaining_time);
                QuestTimer.GetComponent<Text>().text = remaining_time_str;
                delayTimeText.SetActive(false);
                questInfoItems.SetActive(true);
            }
        }


        string GetMovesText()
        {
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();
            moveCount = user_quest.moves_rewarded;
            return "x " + moveCount;
        }
        string GetTrophyText()
        {
            QuestTier current_tier = QuestManager.Instance.GetCurrentQuestTierInfo();
            trophyCount = current_tier.trophy_reward;
            return "x " + trophyCount;
        }
        private Vector3 GetQuestInfoIconScale(UserQuest user_quest)
        {
            if (user_quest.type == QuestType.collectable_type.PAIR_CANDIES)
            {
                return Vector3.one * 0.8f;
            }
            return Vector3.one;
        }

        private RewardDisplay GetQuestInfoReward(UserQuest user_quest)
        {
            RewardDisplay reward_display = new RewardDisplay();

            if (user_quest.type == QuestType.collectable_type.COLLECT_STARS)
            {
                reward_display.label = "Collect {0} stars";
                reward_display.image = StarsSpriteSmall;
            }
            else if (user_quest.type == QuestType.collectable_type.COLLLECT_LOLLIPOP)
            {
                reward_display.label = "Collect {0} lollipops";
                reward_display.image = LollipopSpriteSmall;
            }
            else if (user_quest.type == QuestType.collectable_type.PAIR_CANDIES)
            {
                reward_display.label = "Pair {0} candies";
                reward_display.image = PairedCandySpriteSmall;
            }
            return reward_display;
        }
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            outOfMoves = false;
            if (inData != null && inData.Length > 0)
            {
                if (inData[0] is bool)
                    outOfMoves = (bool)inData[0];
            }
        }
        public void ClosePopup()
        {
            Hide(true);
            if (outOfMoves) { LoadingManager.Instance.LoadScene(LoadingManager.StartScreen); }
        }

        int moveCount = 0;
        float moveDelay = 0;

        public void StartMoveCountdown()
        {
            movesIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            moveDelay = 0.5f / moveCount;
            StartCoroutine(MovesCountDown());
        }

        IEnumerator MovesCountDown()
        {
            yield return new WaitForSeconds(moveDelay);
            moveCount--;
            int count = int.Parse(EarnedMovesText.text.Substring(1));
            count++;
            EarnedMovesText.text = "+" + count;
            MovesText.GetComponent<Text>().text = "x " + moveCount;
            if (moveCount > 0)
            {
                StartCoroutine(MovesCountDown());
            }
            else
            {
                MovesText.SetActive(false);
            }
        }
        int trophyCount = 0;
        float TrohpyDelay = 0;
        public void StartTrophyCountdown()
        {
            trophyIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            TrohpyDelay = 0.5f / trophyCount;
            StartCoroutine(TrophyCountDown());
        }
        IEnumerator TrophyCountDown()
        {
            yield return new WaitForSeconds(TrohpyDelay);
            trophyCount--;
            int count = int.Parse(EarnedTrophyText.text.Substring(1));
            count++;
            TrophyText.GetComponent<Text>().text = "x " + trophyCount;
            EarnedTrophyText.text = "+" + count;
            if (trophyCount > 0)
            {
                StartCoroutine(TrophyCountDown());
            }
            else
            {
                TrophyText.SetActive(false);

            }
        }

        public void AddTrophyToHUD()
        {
            trophyCount = int.Parse(TopHudTrophyText.text);
            int count = int.Parse(EarnedTrophyText.text.Substring(1));

            TrohpyDelay = 0.4f / count;
            StartCoroutine(TrophyTopHUD(trophyCount + count));
        }
        IEnumerator TrophyTopHUD(int total)
        {
            yield return new WaitForSeconds(TrohpyDelay);
            trophyCount++;
            TopHudTrophyText.text = trophyCount.ToString();
            if (trophyCount < total)
            {
                StartCoroutine(TrophyTopHUD(total));
            }
        }

        public void PlaySound(String id)
        {
            SoundManager.Instance.Play(id);
        }

        public void PlayParticle()
        {
            foreach (ParticleSystem p in animParticles)
            {
                p.Play();
            }
        }
    }
}
