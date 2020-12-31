using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BizzyBeeGames.DotConnect
{
    public class MainScreen : Screen
    {
        #region Inspector Variables

        [SerializeField] private Image BackgroundImage, MovesCounterBG, MovesCounterProgress, TopHUDBG, PauseButton, FillCapsule, PairedCandyCapsule;
        [SerializeField] private Image HintBubble, ResetBubble, UndoBubble, Reset, Hint, Undo;
        [SerializeField] private Sprite DarkBackground, DarkMovesCounterBG, DarkTopHUDBG, DarkPauseButton, DarkCapsule, DarkBubble, DarkHint, DarkReset, DarkUndo;
        #endregion

        #region Unity Methods

        protected override void Start()
        {
            base.Start();
            SetCurrentChapterBackground();
        }
        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ShowPauseScreen();
            }
        }

        #endregion

        #region Public Methods
        public void SetCurrentChapterBackground()
        {
            ChapterTier data;
            int curr_level = UserDataManager.Instance.GetData("current_level");
            bool isDarkModeOn = UserDataManager.Instance.IsDarkModeOn();

            for (int i = 0; i < GameConfiguration.Instance.ChapterTiers.Count; i++)
            {
                data = GameConfiguration.Instance.ChapterTiers[i];
                if (curr_level >= data.min_level && curr_level < data.max_level)
                {
                    BackgroundImage.sprite = isDarkModeOn ? DarkBackground : data.chapter_image;
                    MovesCounterBG.sprite = isDarkModeOn ? DarkMovesCounterBG : data.chapter_moves_counter_bg;
                    MovesCounterProgress.sprite = data.chapter_moves_progress;
                    TopHUDBG.sprite = isDarkModeOn ? DarkTopHUDBG : data.chapter_top_hud_bg;
                    PauseButton.sprite = isDarkModeOn ? DarkPauseButton : data.chapter_pause_image;
                }
            }

            if (isDarkModeOn)
            {
                FillCapsule.sprite = DarkCapsule;
                PairedCandyCapsule.sprite = DarkCapsule;
                HintBubble.sprite = DarkBubble;
                ResetBubble.sprite = DarkBubble;
                UndoBubble.sprite = DarkBubble;
                Reset.sprite = DarkReset;
                Hint.sprite = DarkHint;
                Undo.sprite = DarkUndo;
            }
        }
        public void GoToStartScreen()
        {
            LoadingManager.Instance.LoadScene(LoadingManager.StartScreen);
        }
        public void ShowPauseScreen()
        {
            PopupManager.Instance.Show("PauseScreen", null, OnPauseScreenClosed);
            GameManager.Instance.SetPause(true);
        }
        #endregion Public Methods

        #region Private Methods
        private void OnPauseScreenClosed(bool cancelled, object[] data)
        {
            if (data != null && data.Length > 0)
            {
                string action = data[0] as string;
                GameManager.Instance.SetPause(false);

                if (action == "home")
                {
                    GoToStartScreen();
                }
            }
        }
        #endregion Private Methods
    }
}
