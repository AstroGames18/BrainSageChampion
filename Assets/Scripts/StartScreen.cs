using BizzyBeeGames.DotConnect;
using Lean.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BizzyBeeGames.DotConnect.GameGrid;
using static UserData;

namespace BizzyBeeGames
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] GameObject ChapterNumberText, TextCapsule, BackgroundImage, DailyRewardsButton, ChapterPackIcon, MovesIconHUD;
        [SerializeField] GameObject NotificationDailyRewardsText, NotificationQuestScreenText, NotificationProfileScreenText;
        [SerializeField] GameObject NotificationDailyRewards, NotificationQuestScreen, NotificationProfileScreen;
        [SerializeField] Text ChapterPackTimer;
        [SerializeField] RectTransform ChapterPackTimerRect;
        [SerializeField] RectTransform[] SocialMediaIcons;
        [SerializeField] Image[] textBackgroundCapsules;
        [SerializeField] Text[] textBackgroundTexts;
        [SerializeField] private Animator PlayButtonAnim = null;
        [SerializeField] private LeanToken level;
        [SerializeField] private Text chapterBannerText;

        // DarkMode Image Changes
        [SerializeField] Image BottomHudBarBG = null, ChapterCapsule = null, ChapterBannerBG = null, DailyRewardNotifImage = null, QuestScreenNotifImage = null, ProfileScreenNotifImage = null;
        [SerializeField] Sprite BottomHudBarDarkBG = null, DarkCapsule = null, DarkChapterBanner = null, DarkNotif = null, DarkChapterBG = null;

        private string dataPrefix = "chapter_pack_";


        private List<AcheivementData> allAcheivementData = null;

        private void OnEnable()
        {
            DateTime next_daily_reward_time = UserDataManager.Instance.GetDateTime("daily_reward_time");
            if (DateTime.Compare(next_daily_reward_time, DateTime.Now) < 0)
            {
                UserDataManager.Instance.SetData("notification_dailyrewards", 1);
            }
            else { DisableDailyRewards(); }

            CheckDarkMode();
            DisplayStartScreenData();
            DisplayNotificationsData();
            ChapterPackIcon.SetActive(false);

            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_UserDataUpdated, onUserDataUpdated);
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_DailyRewardSelected, onDailyRewardSelected);
            allAcheivementData = UserDataManager.Instance.GetAchivements();
        }

        public void ShowGiftScreens()
        {
            if (allAcheivementData.Count == 0)
            {
                CheckDarkModeSettings();
            }
            else
            {
                PopupManager.Instance.Show("AcheivementsScreen", new object[] { allAcheivementData[0].isChapter, allAcheivementData[0].acheivement_type }, OnAcheivementsScreenClosed);
            }
        }

        private void OnAcheivementsScreenClosed(bool cancelled, object[] data)
        {
            StartCoroutine(Utils.ExecuteAfterDelay(0.5f, (args) =>
            {
                allAcheivementData.RemoveAt(0);
                UserDataManager.Instance.RemoveAchivement(0);
                ShowGiftScreens();
            }));
        }

        private void onUserNameClosed(bool cancelled, object[] data)
        {
            CheckDarkModeSettings();
        }

        private void CheckDarkModeSettings()
        {
            if (UserDataManager.Instance.dark_mode_popup_shown || true)
                return;
            UserDataManager.Instance.dark_mode_popup_shown = true;
            if (UserDataManager.Instance.GetData("reloaded") > 0)
            {
                UserDataManager.Instance.SetData("reloaded", 0);
                return;
            }
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


        public void CheckDarkMode()
        {
            if (UserDataManager.Instance.IsDarkModeOn())
            {
                BottomHudBarBG.sprite = BottomHudBarDarkBG;
                ChapterCapsule.sprite = DarkCapsule;
                ChapterBannerBG.sprite = DarkChapterBanner;
                DailyRewardNotifImage.sprite = DarkNotif;
                ProfileScreenNotifImage.sprite = DarkNotif;
                QuestScreenNotifImage.sprite = DarkNotif;

                ChapterNumberText.GetComponent<Text>().color = Color.white;

                for (int i = 0; i < textBackgroundCapsules.Length; i++)
                {
                    textBackgroundCapsules[i].sprite = DarkCapsule;
                }
                for (int i = 0; i < textBackgroundTexts.Length; i++)
                {
                    textBackgroundTexts[i].color = Color.white;
                }
            }
        }
        public void ShowChapterPack()
        {
            UserDataManager udm = UserDataManager.Instance;
            object[] tierInfo = getCurrentChapterTierInfo();

            if (InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryMoves) <= 0)
            {
                int tierLevel = (int)tierInfo[1];
                if (udm.GetDateTime(dataPrefix + tierLevel) == new DateTime(1970, 1, 1, 0, 0, 0))
                {
                    ChapterTier tier = (ChapterTier)tierInfo[0];
                    udm.SetData(dataPrefix + tierLevel, DateTime.Now.AddHours(tier.chapter_pack.time));
                    udm.SetCurrentChapterPack(tier.chapter_pack);
                    ChapterPackIcon.SetActive(true);
                }
            }
            else if (udm.GetCurrentChapterPack() != null)
            {
                ChapterPackIcon.SetActive(true);
            }
        }
        private void Update()
        {
            UserDataManager udm = UserDataManager.Instance;
            MovesIconHUD.GetComponent<Animator>().SetBool("recharged", InventoryManager.Instance.movesRecharged);
            if (udm.GetCurrentChapterPack() != null)
            {
                if (!ChapterPackTimerRect.gameObject.activeSelf) { return; }
                int remaining_time = (int)(UserDataManager.Instance.GetDateTime(dataPrefix + (int)getCurrentChapterTierInfo()[1]) - DateTime.Now).TotalSeconds;
                ChapterPackTimer.text = Utils.ConvertToTime(remaining_time);
                ChapterPackIcon.SetActive(remaining_time > 0);
                if (remaining_time <= 0)
                {
                    udm.SetCurrentChapterPack(null);
                }
            }
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
        private void DisableDailyRewards()
        {
            DailyRewardsButton.SetActive(false);
        }
        public void AnimateSocialMediaIcons()
        {
            float delayTime = 5f;
            if (DailyRewardsButton.activeSelf || LoadingManager.Instance.isLoading) { return; }
            for (int i = 0; i < SocialMediaIcons.Length; i++)
            {
                if (LoadingManager.Instance.isLoading) { return; }

                RectTransform icon = SocialMediaIcons[i];
                StartCoroutine(Utils.ExecuteAfterDelay(i * delayTime, (args) =>
                {
                    if (LoadingManager.Instance.isLoading) { return; }
                    int j = (int)args[0];
                    Utils.DoSwipeHorizontalAnimation(icon, 220f, -160f, 1f, 0f);
                    StartCoroutine(Utils.ExecuteAfterDelay(delayTime, (args1) =>
                    {
                        if (LoadingManager.Instance.isLoading) { return; }
                        int index = (int)args1[0];
                        Utils.DoSwipeHorizontalAnimation(icon, -160f, 220f, 1f, 0f);
                        if (index == SocialMediaIcons.Length - 1)
                        {
                            AnimateSocialMediaIcons();
                        }
                    }, new object[] { j }));
                }, new object[] { i }));
            }
        }
        public void ResetSocialMediaIcons()
        {
            for (int i = 0; i < SocialMediaIcons.Length; i++)
            {
                RectTransform icon = SocialMediaIcons[i];
                Utils.DoSwipeHorizontalAnimation(icon, 220f, 220f, 0f, 0f);
            }
        }
        public void OpenSocialMedia()
        {
            PopupManager.Instance.Show("SocialPopup");
        }
        private void OnDisable()
        {
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_DailyRewardSelected, onDailyRewardSelected);
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_UserDataUpdated, onUserDataUpdated);
        }
        private void onUserDataUpdated(string eventId, object[] data)
        {
            DisplayNotificationsData();
        }
        private void onDailyRewardSelected(string eventId, object[] data)
        {
            DisableDailyRewards();
        }
        public void DisplayNotificationsData()
        {
            int n_dailyrewards = UserDataManager.Instance.GetData("notification_dailyrewards");
            int n_questscreen = UserDataManager.Instance.GetData("notification_questscreen");
            int n_profilescreen = UserDataManager.Instance.GetData("notification_profilescreen");

            NotificationDailyRewards.SetActive(n_dailyrewards > 0);
            NotificationQuestScreen.SetActive(n_questscreen > 0);
            NotificationProfileScreen.SetActive(n_profilescreen > 0);

            NotificationDailyRewardsText.GetComponent<Text>().text = "!";
            NotificationQuestScreenText.GetComponent<Text>().text = n_questscreen.ToString();
            NotificationProfileScreenText.GetComponent<Text>().text = n_profilescreen.ToString();
        }
        public void DisplayStartScreenData()
        {
            DisplayLevelNumber();
            Debug.Log("Chapter");
            Debug.Log("------------------------");
            ChapterTier data;
            int curr_level = UserDataManager.Instance.GetData("current_level");
            String message = "Sweet Brain";
            for (int i = 0; i < GameConfiguration.Instance.ChapterTiers.Count; i++)
            {
                data = GameConfiguration.Instance.ChapterTiers[i];
                if (curr_level >= data.min_level && curr_level < data.max_level)
                {
                    Debug.Log("1");
                    Debug.Log(data.banner_message);
                    message = data.banner_message;
                    BackgroundImage.GetComponent<Image>().sprite = UserDataManager.Instance.IsDarkModeOn() ? DarkChapterBG : data.chapter_image;
                    ChapterNumberText.GetComponent<Text>().text = curr_level.ToString() + "/" + data.max_level;
                    break;
                }
            }

            data = GameConfiguration.Instance.ChapterTiers[GameConfiguration.Instance.ChapterTiers.Count - 1];
            if (curr_level >= data.max_level)
            {
                Debug.Log("2");
                Debug.Log(data.banner_message);
                ChapterNumberText.GetComponent<Text>().text = curr_level.ToString();
                message = data.banner_message;
            }
            StartCoroutine(Utils.ExecuteAfterDelay(0.2f, (args) =>
            {
                Debug.Log(message);
                chapterBannerText.text = LeanLocalization.GetTranslationText(message);
            }));
        }

        public void onPlayButton()
        {
            SoundManager.Instance.Play("PlayButtonPressed");
            if (GameConfiguration.Instance.isCustomLevel)
            {
                UserDataManager.Instance.SetData("current_level", GameConfiguration.Instance.customLevel);
            }
            else if (GameConfiguration.Instance.IsGameCompleted())
            {
                PopupManager.Instance.Show("GameCompleted");
                return;
            }
            ResetSocialMediaIcons();
            LoadingManager.Instance.LoadScene(LoadingManager.GameScreen);
        }
        public void DisplayLevelNumber()
        {
            StartCoroutine(Utils.ExecuteAfterDelay(5f, (args) => { PlayButtonAnim.SetBool("focus", true); }));
            if (GameConfiguration.Instance.IsGameCompleted())
            {
                //LevelButtonText.GetComponent<Text>().text = "Completed";
                level.SetValue("Completed");
            }
            else
            {

                int l = UserDataManager.Instance.GetData("current_level");
                level.SetValue("Level: " + l.ToString());
                //LevelButtonText.GetComponent<Text>().text = "Level: " + level.ToString();
            }
        }
    }
}
