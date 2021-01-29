using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Localization;
using System.Collections;

namespace BizzyBeeGames.DotConnect
{
    public class LevelCompletePopup : Popup
    {
        #region Inspector Variables
        [Space]
        [SerializeField] private RectTransform nextLevelButton = null;
        public RectTransform homeButton = null;
        [SerializeField] private Text nextLevelButtonText = null;
        [SerializeField] private GameObject mainScreen = null;
        public Animator NextLevelButtonAnim = null;
        [Space]
        [SerializeField] private ProgressBar giftProgressBar = null;
        [SerializeField] private Text giftProgressText = null;
        [Space]
        [SerializeField] GameObject collection_text, collection_message, collection_move_reward_text;
        [SerializeField] AnimatedScrollbar QuestProgressBar, LollipopProgressBar;
        [SerializeField] Image[] stars = new Image[3];
        [SerializeField] Image[] yellowStars = new Image[3];
        [SerializeField] ParticleSystem[] starParticles;
        [SerializeField] Sprite greyStar, yellowStar, lollipopComplete, lollipopIncomplete, questComplete, questIncomplete, LollipopSlider, StarSlider, PairedCandySlider;
        [SerializeField] private Image lollipopImage = null, QuestProgressBarBG, LollipopProgressBarBG, QuestSlider;
        [Space]
        [SerializeField] RectTransform QuestProgress, LollipopProgress;
        [SerializeField] private Text lollipopText = null, questTimerText;
        [SerializeField] Text starText = null;

        [SerializeField] Image[] SmallCards;

        [SerializeField] GameObject questProgressContainer;
        [SerializeField] Text questDelayText;
        [SerializeField] List<ParticleSystem> fireworks1;
        [SerializeField] List<ParticleSystem> fireworks2;
        [SerializeField] List<ParticleSystem> fireworks3;
        [SerializeField] ParticleSystem[] confetti;
        public LeanToken noOfmovesUsed;
        public Image questIcon;
        public Sprite StarsSpriteSmall, LollipopSpriteSmall, PairedCandySpriteSmall;
        #endregion

        #region Member Variables

        private int animCounter = 0;
        private float progressOffset = 850f;
        private float buttonOffset = 550f;
        private float containerInitPos = -250f;
        private float homeButtonInitPos = -250f;
        private bool showLollipopProgress = false;
        private bool questCompleted = false;
        private bool cancelHomeButtonAnimation = false;
        private float lollipopProgress = 0f;
        private float questProgress = 0f;
        private List<QuestReward> lollipopRewards = null;
        #endregion

        #region Unity Methods

        private void Update()
        {
            int remaining_time = (int)QuestManager.Instance.GetRemainingTimeForQuest();

            if (remaining_time <= 0 && !QuestManager.Instance.waitForClaim)
            {
                questProgressContainer.SetActive(false);
                questDelayText.gameObject.SetActive(true);
                questDelayText.text = Utils.ConvertSecondsToString((int)QuestManager.Instance.questDelayTime);
            }
            else
            {
                if (remaining_time <= (GameConfiguration.Instance.questTimerVisibilityMininmumMinutes * 60) && !questCompleted)
                {
                    questTimerText.gameObject.SetActive(true);
                    string remaining_time_str = Utils.ConvertSecondsToString(remaining_time);
                    questTimerText.text = remaining_time_str;
                }
                else
                {
                    questTimerText.gameObject.SetActive(false);
                }
                questProgressContainer.SetActive(true);
                questDelayText.gameObject.SetActive(false);
                SetQuestProgress();
            }
        }
        #endregion


        #region Public Methods
        public void onHomeButton()
        {
            SoundManager.Instance.Play("LevelCompleteHomeButtonClicked");
            ShowLevelCompleteDissappearAnimation(() =>
            {
                LoadingManager.Instance.LoadScene(LoadingManager.StartScreen);
            });
        }

        public void SetStarTextMessage()
        {
            starText.text =
                LeanLocalization.GetTranslationText(GameManager.Instance.Grid.GetNumberOfStars() > 2 ? "You've earn three star" : "Collect 3 stars to earn lollopop");
        }

        public void ShowingNumMovesText()
        {
            SoundManager.Instance.Play("LevelCompleteNumMovesCompleted");
        }
        public void ShowLollipopAnimation()
        {
            showLollipopProgress = GameManager.Instance.Grid.lollipop;

            if (showLollipopProgress) { SoundManager.Instance.Play("LevelCompleteLollipopAwarded"); }
            else { SoundManager.Instance.Play("LevelCompleteLollipopNotAwarded"); }

            GetComponent<Animator>().SetBool("lollipop_appear", true);
        }

        int movesDone = 0;
        private void OnEnable()
        {
            Utils.DoSwipeVerticalAnimation(homeButton, 300, homeButtonInitPos, 0f, 0f);
            //Utils.DoSwipeVerticalAnimation(homeButton, containerInitPos, containerInitPos + buttonOffset, 0f, 0f);
            questProgress = 0f;
            lollipopProgress = 0f;


            cancelHomeButtonAnimation = false;
            lollipopRewards = new List<QuestReward>();
            int starsRewarded = 0;
            int movesDone = PlayerPrefs.GetInt("moves_done");

            noOfmovesUsed.SetValue(movesDone);
            noOfmovesUsed.enabled = false;
            StartCoroutine(Utils.ExecuteAfterDelay(0f, (args) =>
            {
                noOfmovesUsed.enabled = true;
            }));
            // sets all the collectable data to be used in main screen
            InventoryManager inventory_manager = InventoryManager.Instance;
            GameConfiguration config = GameConfiguration.Instance;
            GameGrid grid = GameManager.Instance.Grid;
            stars[0].transform.GetChild(0).gameObject.SetActive(false);
            stars[1].transform.GetChild(0).gameObject.SetActive(false);
            stars[2].transform.GetChild(0).gameObject.SetActive(false);
            lollipopImage.transform.GetChild(0).gameObject.SetActive(false);

            if (grid.three_stars)
            {
                stars[0].sprite = greyStar;
                stars[1].sprite = greyStar;
                stars[2].sprite = greyStar;
                yellowStars[0].gameObject.SetActive(true);
                yellowStars[1].gameObject.SetActive(true);
                yellowStars[2].gameObject.SetActive(true);
                starsRewarded = 3;
            }
            else if (grid.two_stars)
            {
                stars[0].sprite = greyStar;
                stars[1].sprite = greyStar;
                stars[2].sprite = greyStar;
                yellowStars[0].gameObject.SetActive(true);
                yellowStars[1].gameObject.SetActive(true);
                yellowStars[2].gameObject.SetActive(false);
                starsRewarded = 2;
            }
            else if (grid.one_star)
            {
                stars[0].sprite = greyStar;
                stars[1].sprite = greyStar;
                stars[2].sprite = greyStar;
                yellowStars[0].gameObject.SetActive(true);
                yellowStars[1].gameObject.SetActive(false);
                yellowStars[2].gameObject.SetActive(false);
                starsRewarded = 1;
            }
            else
            {
                stars[0].sprite = greyStar;
                stars[1].sprite = greyStar;
                stars[2].sprite = greyStar;
                yellowStars[0].gameObject.SetActive(false);
                yellowStars[1].gameObject.SetActive(false);
                yellowStars[2].gameObject.SetActive(false);
                starsRewarded = 0;
            }

            if (starsRewarded == 3)
            {
                ProbableRewards reward_lollipops = inventory_manager.CheckProfileScreenAcheivements(inventory_manager.GetInventory(InventoryManager.Key_InventoryLollipops), 1, config.ProfileLollipops);
                if (reward_lollipops != null)
                {
                    List<QuestReward> allRewards = inventory_manager.GetProbableRewards(reward_lollipops);
                    inventory_manager.UpdateAllQuestRewards(allRewards);
                    UserDataManager.Instance.SetUserProfileLollipopGift(allRewards, reward_lollipops.giftType);
                    grid.AddToAcheivementData(false, InventoryManager.Key_InventoryLollipops);
                }
                inventory_manager.UpdateInventory(InventoryManager.Key_InventoryLollipops, 1);
                UserDataManager.Instance.UpdateData("win_screen_lollipops", 1);
            }
            ProbableRewards reward_stars = inventory_manager.CheckProfileScreenAcheivements(inventory_manager.GetInventory(InventoryManager.Key_InventoryStars), starsRewarded, config.ProfileStars);
            if (reward_stars != null)
            {
                List<QuestReward> allRewards = inventory_manager.GetProbableRewards(reward_stars);
                inventory_manager.UpdateAllQuestRewards(allRewards);
                UserDataManager.Instance.SetUserProfileStarGift(allRewards, reward_stars.giftType);
                grid.AddToAcheivementData(false, InventoryManager.Key_InventoryStars);
            }
            inventory_manager.UpdateInventory(InventoryManager.Key_InventoryStars, starsRewarded);

            SetLollipopProgress();
            grid.moves_done = 0;
            SetQuestProgress();
            GameConfiguration.Instance.SetDarkModeOnCards(null, SmallCards);
        }
        public void SetLollipopProgress()
        {
            int lollipops_to_collect = GameConfiguration.Instance.LollipopCollectCount;
            int collected_lollipops = UserDataManager.Instance.GetData("win_screen_lollipops");
            lollipopProgress = collected_lollipops / (float)lollipops_to_collect;
            giftProgressText.text = collected_lollipops + "/" + lollipops_to_collect;
            string lollipop_message = "Collect {0} lollipops";

            lollipopText.text = String.Format(lollipop_message, lollipops_to_collect);
            if (collected_lollipops >= lollipops_to_collect)
            {
                LollipopProgressBarBG.sprite = lollipopComplete;
                UserDataManager.Instance.SetData("win_screen_lollipops", 0);
                ProbableRewards gift = GameConfiguration.Instance.LollipopCollectReward;

                lollipopRewards = InventoryManager.Instance.GetProbableRewards(gift);
                InventoryManager.Instance.UpdateAllQuestRewards(lollipopRewards);
            }
            else { LollipopProgressBarBG.sprite = lollipopIncomplete; }
        }

        bool stopFirework = false;

        public void onNextButton()
        {

            if (!stopFirework)
            {
                foreach (ParticleSystem item in fireworks1)
                {
                    if (item.isPlaying)
                    {
                        item.Stop();
                    }
                }
                foreach (ParticleSystem item in fireworks2)
                {
                    if (item.isPlaying)
                    {
                        item.Stop();
                    }
                }
                foreach (ParticleSystem item in fireworks2)
                {
                    if (item.isPlaying)
                    {
                        item.Stop();
                    }
                }
                foreach (ParticleSystem p in confetti)
                {
                    if (p.isPlaying)
                    {
                        p.Stop();
                    }
                }
            }
            stopFirework = true;

            if (animCounter == 1)
            {
                SoundManager.Instance.Play("LevelCompleteNiceButtonClicked");
                ShowAnimatedQuestProgress();
            }
            else if (animCounter == 2)
            {
                SoundManager.Instance.Play("LevelCompleteNextButtonClicked");
                if (GameConfiguration.Instance.IsGameCompleted())
                {
                    ShowLevelCompleteDissappearAnimation(() =>
                    {
                        PopupManager.Instance.Show("GameCompleted");
                       // LoadingManager.Instance.LoadScene(LoadingManager.StartScreen);
                    });
                }
                else if (questCompleted)
                {
                    PopupManager.Instance.Show("QuestScreen", null, onQuestScreenClosed);
                    questCompleted = false;
                    nextLevelButtonText.text = hasPopup() ? LeanLocalization.GetTranslationText("Next")
                : (LeanLocalization.GetTranslationText("Lvl") + " ") + UserDataManager.Instance.GetData("current_level");
                }
                else if (hasPopup())
                {
                    ShowGiftScreens();
                }
                else if (!questCompleted)
                {
                    ShowLevelCompleteDissappearAnimation(goToNextLevel);
                }
            }
        }

        public void OnGreyStarAppearSound()
        {
            SoundManager.Instance.Play("LevelRetryPopup");
        }

        public void OnThreeStarTextSound()
        {
            if (GameManager.Instance.Grid.GetNumberOfStars() > 2)
            {
                SoundManager.Instance.Play("LevelRetryPopup");
            }
        }
        private void onQuestScreenClosed(bool cancelled, object[] data)
        {
            SetQuestProgress();
            QuestProgressBar.SetValue(questProgress);
        }
        public void ShowLevelCompleteDissappearAnimation(Action callBack = null)
        {
            cancelHomeButtonAnimation = true;
            //Utils.DoSwipeVerticalAnimation(QuestProgress, QuestProgress.anchoredPosition.y, containerInitPos, 1f, 0f);
            Utils.DoSwipeVerticalAnimation(nextLevelButton, nextLevelButton.anchoredPosition.y, containerInitPos, 1f, 0f);
            Utils.DoSwipeVerticalAnimation(homeButton, homeButton.anchoredPosition.y, containerInitPos, 1f, 0f);
            GetComponent<Animator>().SetBool("reset", true);
            StartCoroutine(Utils.ExecuteAfterDelay(1f, (args) =>
            {
                //Utils.DoZoomAnimation(QuestProgress, 0f, 1f, 0f);
                Utils.DoSwipeVerticalAnimation(LollipopProgress, LollipopProgress.anchoredPosition.y, containerInitPos, 0f, 0f);
                Utils.DoSwipeHorizontalAnimation(nextLevelButton, nextLevelButton.anchoredPosition.x, 0f, 1f, 0f);

                //Utils.DoZoomAnimation(QuestProgress, 0f, 0f, 1f);
                Utils.DoZoomAnimation(LollipopProgress, 0f, 0f, 1f);
                stars[0].transform.parent.gameObject.GetComponent<Animator>().enabled = false;
                stars[1].transform.parent.gameObject.GetComponent<Animator>().enabled = false;
                stars[2].transform.parent.gameObject.GetComponent<Animator>().enabled = false;
                stars[0].transform.GetChild(0).gameObject.SetActive(false);
                stars[1].transform.GetChild(0).gameObject.SetActive(false);
                stars[2].transform.GetChild(0).gameObject.SetActive(false);
                lollipopImage.transform.GetChild(0).gameObject.SetActive(false);

                GameManager.Instance.PlayGameScreenBGM();
                Hide(true);
                callBack();

            }));
        }
        private void goToNextLevel()
        {
            foreach (ParticleSystem p in confetti)
            {
                p.gameObject.SetActive(false);
            }
            mainScreen.GetComponent<MainScreen>().SetCurrentChapterBackground();
            GameManager.Instance.NextLevel();
        }
        public void ShowHomeButtonAnimation()
        {
            StartCoroutine(Utils.ExecuteAfterDelay(5f, (args) =>
            {
                if (cancelHomeButtonAnimation) { return; }
                // SoundManager.Instance.Play("LevelCompleteHomeButtonAppears");
                Utils.DoSwipeVerticalAnimation(homeButton, homeButtonInitPos, 300f, 1f, 0f);
                Utils.DoSwipeHorizontalAnimation(nextLevelButton, nextLevelButton.anchoredPosition.x, 300f, 1f, 0f);
            }));
        }
        public void ShowStarSparks(int index)
        {
            GameGrid grid = GameManager.Instance.Grid;
            if (grid.GetNumberOfStars() < index + 1)
            {
                stars[index].transform.parent.GetChild(0).gameObject.SetActive(false);
                //SoundManager.Instance.Play("LevelCompleteGreyStar");
                stars[index].transform.parent.gameObject.GetComponent<Animator>().enabled = false;
            }
            else
            {
                stars[index].transform.parent.GetChild(0).gameObject.SetActive(true);
                SoundManager.Instance.Play("LevelCompleteYellowStar" + (index + 1));
                //starParticles[index].Play();
                stars[index].transform.parent.gameObject.GetComponent<Animator>().enabled = true;
            }
        }
        public void SetLollipopImage()
        {
            lollipopImage.color = (GameManager.Instance.Grid.lollipop) ? new Color(1f, 1f, 1f, 1f)
                : new Color(0.05f, 0.05f, 0.05f);
        }
        public void onLevelCompleteAppearAnimationComplete()
        {
            /*showLollipopProgress = GameManager.Instance.Grid.lollipop;

            if (showLollipopProgress)
            {
                SoundManager.Instance.Play("LevelCompleteLollipopProgress");
                LollipopProgressBar.SetValue(lollipopProgress);

                stars[0].transform.GetChild(0).gameObject.SetActive(true);
                stars[1].transform.GetChild(0).gameObject.SetActive(true);
                stars[2].transform.GetChild(0).gameObject.SetActive(true);
                lollipopImage.transform.GetChild(0).gameObject.SetActive(true);
                nextLevelButtonText.text = "Nice";
                nextLevelButtonText.fontSize = 90;

                Utils.DoSwipeVerticalAnimation(LollipopProgress, containerInitPos, containerInitPos + progressOffset, 1f, 0f);
                animCounter = 1;
                if (lollipopRewards != null && lollipopRewards.Count > 0)
                {
                    StartCoroutine(Utils.ExecuteAfterDelay(1f, (args) =>
                    {
                        UserDataManager.Instance.SetUserGiftReward(lollipopRewards,
                            GameConfiguration.Instance.LollipopCollectReward.giftType, "LevelCompletePopup");
                        PopupManager.Instance.Show("GiftScreen");
                    }));
                }
            }
            else
            {*/
            ShowAnimatedQuestProgress();
            //}

            Utils.DoSwipeVerticalAnimation(nextLevelButton, containerInitPos, containerInitPos + buttonOffset, 1f, 1f);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowAnimatedQuestProgress()
        {
            SoundManager.Instance.Play("LevelCompleteQuestProgress");
            GameManager.Instance.Grid.IncrementLevelNumber();

            QuestManager.Instance.waitForClaim = questComplete;
            QuestProgressBar.SetValue(questProgress);

            StartCoroutine(Utils.ExecuteAfterDelay(0.5f, (args) =>
            {
                nextLevelButtonText.text = questCompleted ? LeanLocalization.GetTranslationText("Claim")
                    : hasPopup() ? LeanLocalization.GetTranslationText("Next")
                    : (LeanLocalization.GetTranslationText("Lvl") + " ") + UserDataManager.Instance.GetData("current_level");

            }));
            float swipeUpAnimDuration = showLollipopProgress ? 0f : 1f;
            float showGiftDelay = showLollipopProgress ? 1.0f : 1.5f;
            if (showLollipopProgress) { Utils.DoZoomAnimation(LollipopProgress, 0.5f, 1f, 0f, 0f); }
            //if (!hasPopup())
            //{
            //    homeButton.gameObject.SetActive(true);
            //    NextLevelButtonAnim.gameObject.GetComponent<Image>().enabled = true;
            //    NextLevelButtonAnim.gameObject.GetComponentInChildren<Text>().enabled = true;
            //    NextLevelButtonAnim.SetBool("focus", true);
            //}
            //else
            //{
            //    NextLevelButtonAnim.gameObject.GetComponent<Image>().enabled = false;
            //    NextLevelButtonAnim.gameObject.GetComponentInChildren<Text>().enabled = false;
            //    homeButton.gameObject.SetActive(false);
            //}
            //Utils.DoSwipeVerticalAnimation(QuestProgress, containerInitPos, containerInitPos + progressOffset, swipeUpAnimDuration, 0f);
            if (showLollipopProgress)
            { //Utils.DoZoomAnimation(QuestProgress, 0.5f, 0f, 1f, 0.5f);
            }

            //Stop Automatic gift popup
            //StartCoroutine(Utils.ExecuteAfterDelay(showGiftDelay, (args) => { GameManager.Instance.Grid.ShowGiftScreens(); }));

            animCounter = 2;
            ShowHomeButtonAnimation();
            if (GameConfiguration.Instance.IsGameCompleted())
            {
                nextLevelButtonText.text = "Completed";
            }
        }

        private void SetQuestProgress()
        {
            UserQuest user_quest = UserDataManager.Instance.LoadUserQuest();
            string quest_message = "";

            if (user_quest.type == QuestType.collectable_type.COLLECT_STARS)
            {
                quest_message = "Collect {0} stars";
            }
            else if (user_quest.type == QuestType.collectable_type.COLLLECT_LOLLIPOP)
            {
                quest_message = "Collect {0} lollipops";
            }
            else if (user_quest.type == QuestType.collectable_type.PAIR_CANDIES)
            {
                quest_message = "Pair {0} candies";
            }

            if (user_quest.amount_collected >= user_quest.max_amount) { UserDataManager.Instance.SetData("notification_questscreen", 1); }

            collection_message.GetComponent<Text>().text = String.Format(quest_message, user_quest.max_amount);
            collection_text.GetComponent<Text>().text = Math.Min(user_quest.amount_collected, user_quest.max_amount) + "/" + user_quest.max_amount;
            questProgress = Math.Min(user_quest.amount_collected, user_quest.max_amount) / (float)user_quest.max_amount;
            collection_move_reward_text.GetComponent<Text>().text = "x" + user_quest.moves_rewarded;
            questCompleted = user_quest.amount_collected >= user_quest.max_amount;
            if (user_quest.amount_collected >= user_quest.max_amount) { QuestProgressBarBG.sprite = questComplete; }
            else { QuestProgressBarBG.sprite = questIncomplete; }

            if (UserDataManager.Instance.IsDarkModeOn())
            {
                GameConfiguration.Instance.SetDarkModeOnCards(null, new Image[] { QuestProgressBarBG });
            }
            if (questCompleted) { QuestManager.Instance.SetUserGiftReward(); }
            Sprite slider_bg = null;
            switch (user_quest.type)
            {
                case QuestType.collectable_type.COLLECT_STARS:
                    questIcon.sprite = StarsSpriteSmall;
                    questIcon.transform.localScale = Vector3.one;
                    slider_bg = StarSlider;
                    break;
                case QuestType.collectable_type.COLLLECT_LOLLIPOP:
                    questIcon.sprite = LollipopSpriteSmall;
                    questIcon.transform.localScale = Vector3.one;
                    slider_bg = LollipopSlider;
                    break;
                case QuestType.collectable_type.PAIR_CANDIES:
                    questIcon.sprite = PairedCandySpriteSmall;
                    questIcon.transform.localScale = Vector3.one * 0.8f;
                    slider_bg = PairedCandySlider;
                    break;
            }
            if (slider_bg != null) { QuestSlider.sprite = slider_bg; }
        }

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);

            bool isLastLevel = (bool)inData[0];
            int numMoves = (int)inData[1];
            int numMoveForStar = (int)inData[2];
            bool earnedStar = (bool)inData[3];
            bool alreadyEarnedStar = (bool)inData[4];
            bool giftProgressed = (bool)inData[5];
            bool giftAwarded = (bool)inData[6];
            int fromGiftProgress = (int)inData[7];
            int toGiftProgress = (int)inData[8];
            int numLevelsForGift = (int)inData[9];

            PlayerPrefs.SetInt("moves_done", 0);
        }

        private void CheckDarkModeSettings()
        {
            if (DateTime.Now.Hour >= 18 && !UserDataManager.Instance.IsDarkModeOn())
            {
                PopupManager.Instance.Show("ChangeToDarkmode", null, onChangeToDarkmode);
            }
            else if (DateTime.Now.Hour < 18 && UserDataManager.Instance.IsDarkModeOn())
            {
                PopupManager.Instance.Show("ChangeFromDarkmode", null, onChangeFromDarkmode);
            }
        }


        public void onChangeToDarkmode(bool cancelled, object[] data)
        {
            if (data != null && data.Length > 0)
            {
                string action = (string)data[0];
                switch (action)
                {
                    case "YES":
                        UserDataManager.Instance.SetData("dark_mode_enabled", 1);
                        LoadingManager.Instance.ReloadScene();
                        break;
                }
            }
        }


        public void onChangeFromDarkmode(bool cancelled, object[] data)
        {
            if (data != null && data.Length > 0)
            {
                string action = (string)data[0];
                switch (action)
                {
                    case "YES":
                        UserDataManager.Instance.SetData("dark_mode_enabled", 0);
                        LoadingManager.Instance.ReloadScene();
                        break;
                }
            }
        }


        public void ShowGiftScreens()
        {
            if (GameManager.Instance.Grid.allAcheivementData.Count == 0)
            {
                UserDataManager.Instance.SetUserGift(InventoryManager.Key_InventoryTrophies, null);
                UserDataManager.Instance.SetUserGift(InventoryManager.Key_InventoryStars, null);
                UserDataManager.Instance.SetUserGift(InventoryManager.Key_InventoryLollipops, null);
                ShowSubscriptionPopup();
            }
            else
            {
                PopupManager.Instance.Show("AcheivementsScreen", new object[] { GameManager.Instance.Grid.allAcheivementData[0].isChapter, GameManager.Instance.Grid.allAcheivementData[0].acheivement_type }, OnAcheivementsScreenClosed);
            }
        }

        bool subscriptionShowed = false;
        private void ShowSubscriptionPopup()
        {
            if (UserDataManager.Instance.GetData("mini_game_stars") >= GameConfiguration.Instance.MiniGameMaxStars && !subscriptionShowed)
            {
                subscriptionShowed = true;
                PopupManager.Instance.Show("MiniGamePopup", null, OnSubscriptionScreenClosed);
            }
            else
            {
                if (!hasPopup())
                {
                    nextLevelButtonText.text = (LeanLocalization.GetTranslationText("Lvl") + " ") + UserDataManager.Instance.GetData("current_level");
                }
                ShowInviteScreen();
            }
        }
        private void OnSubscriptionScreenClosed(bool cancelled, object[] data)
        {
            if (!hasPopup())
            {
                nextLevelButtonText.text = (LeanLocalization.GetTranslationText("Lvl") + " ") + UserDataManager.Instance.GetData("current_level");
            }
        }

        private void OnInviteScreenClosed(bool cancelled, object[] data)
        {
            if (!hasPopup())
            {
                nextLevelButtonText.text = (LeanLocalization.GetTranslationText("Lvl") + " ") + UserDataManager.Instance.GetData("current_level");
            }
        }

        bool inviteShown = false;
        public void ShowInviteScreen()
        {
            inviteShown = true;
            PopupManager.Instance.Show("InviteScreen", null, OnInviteScreenClosed);

            if (!hasPopup())
            {
                nextLevelButtonText.text = (LeanLocalization.GetTranslationText("Lvl") + " ") + UserDataManager.Instance.GetData("current_level");
            }
        }
        /// <summary>
        /// Show next gift after acheivement is closed
        /// </summary>
        private void OnAcheivementsScreenClosed(bool cancelled, object[] data)
        {
            StartCoroutine(Utils.ExecuteAfterDelay(0.5f, (args) =>
            {
                GameManager.Instance.Grid.allAcheivementData.RemoveAt(0);
                UserDataManager.Instance.RemoveAchivement(0);
                if (!hasPopup())
                {
                    nextLevelButtonText.text = (LeanLocalization.GetTranslationText("Lvl") + " ") + UserDataManager.Instance.GetData("current_level");
                }
            }));
        }

        public bool hasPopup()
        {
            if (GameManager.Instance.Grid.allAcheivementData.Count > 0)
                return true;
            if (UserDataManager.Instance.GetData("mini_game_stars") >= GameConfiguration.Instance.MiniGameMaxStars && !subscriptionShowed)
                return true;
            int current_level = UserDataManager.Instance.GetData("current_level");
            if ((current_level) % GameConfiguration.Instance.intervalToShowInviteScreen == 0 && !inviteShown)
                return true;
            return false;

        }

        public void StartFirework()
        {
            return;
            if (GameManager.Instance.Grid.one_star || GameManager.Instance.Grid.two_stars || GameManager.Instance.Grid.three_stars)
            {
                fireworks2[0].Play();
                StartCoroutine(Fireworks(0));
            }

            if (GameManager.Instance.Grid.two_stars || GameManager.Instance.Grid.three_stars && !stopFirework)

                StartCoroutine(Utils.ExecuteAfterDelay(4f, (args) =>
                {
                    fireworks1[0].Play();
                    StartCoroutine(Fireworks(1));
                    if (GameManager.Instance.Grid.three_stars && !stopFirework)

                        StartCoroutine(Utils.ExecuteAfterDelay(4f, (args1) =>
                    {
                        fireworks3[0].Play();
                        StartCoroutine(Fireworks(2));
                    }));
                }));
        }

        #endregion

        #region Private Methods

        int currentFirework1 = 1;
        int currentFirework2 = 1;
        int currentFirework3 = 1;
        IEnumerator Fireworks(int i)
        {

            yield return new WaitForSeconds(12f);
            if (!stopFirework)
            {
                switch (i)
                {
                    case 1:
                        fireworks1[currentFirework1].Play();

                        currentFirework1++;
                        if (currentFirework1 >= fireworks1.Count)
                            currentFirework1 = 0;
                        break;
                    case 0:
                        fireworks2[currentFirework2].Play();
                        currentFirework2++;
                        if (currentFirework2 >= fireworks2.Count)
                            currentFirework2 = 0;
                        break;
                    case 2:
                        fireworks3[currentFirework3].Play();
                        currentFirework3++;
                        if (currentFirework3 >= fireworks3.Count)
                            currentFirework3 = 0;
                        break;
                }
                StartCoroutine(Fireworks(i));
            }
        }
        #endregion
    }
}
