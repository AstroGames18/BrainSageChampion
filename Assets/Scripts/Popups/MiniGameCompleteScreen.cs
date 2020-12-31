using System;
using System.Collections;
using System.Collections.Generic;
using TwoOfAKindGame;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class MiniGameCompleteScreen : Popup
    {

        public Text title;
        public Text subtitle;
        public GameObject Divider;
        public GameObject DividerLayout;
        public GameObject close;
        public GameObject claim;
        [Tooltip("The progress bar for mini game")]
        public GameObject ScrollBar;
        public TOKGameController gameController;

        float scoreCount = 0;
        int currentLevel = 0;
        private List<List<QuestReward>> multipleGifts;
        private List<GiftType> multipleGiftTypes;

        public void ClosePopup()
        {
            Hide(true);
        }

        void UpdateScore()
        {
            //Update the score text
            int current_score = Mathf.CeilToInt(scoreCount);
            ScrollBar.GetComponent<Scrollbar>().size = current_score / (float)GameConfiguration.Instance.MaxScoreInMiniGame;
        }
        void SetupProgressBar()
        {
            List<GameObject> divider_list = new List<GameObject>();
            List<ProbableRewards> RewardsForScore = GameConfiguration.Instance.RewardsForMiniGameScore;
            GameObject start_divider = Instantiate(Divider, DividerLayout.transform);

            for (int i = 0; i < RewardsForScore.Count; i++)
            {
                GameObject divider = Instantiate(Divider, DividerLayout.transform);
                GameObject gift = divider.transform.GetChild(0).gameObject;
                gift.GetComponent<Image>().sprite = GameConfiguration.Instance.GetGiftSprite(RewardsForScore[i].giftType);
                divider_list.Add(divider);
            }

            start_divider.GetComponent<Image>().color = new Color(0.0F, 0.0F, 0.0F, 0.0F);
            start_divider.transform.GetChild(0).gameObject.SetActive(false);
            UpdateScore();
        }
        private void MainMenu()
        {
            gameController.SetHideableObjects(false);
            LoadingManager.Instance.LoadScene(LoadingManager.StartScreen);
        }
        public void Close()
        {
            ClosePopup();
            MainMenu();
        }

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            scoreCount = (float)inData[0];
            currentLevel = (int)inData[1];
            multipleGifts = (List<List<QuestReward>>)inData[2];
            multipleGiftTypes = (List<GiftType>)inData[3];
            SetupUi();
        }

        private void SetupUi()
        {
            if (multipleGifts == null)
            {
                multipleGifts = new List<List<QuestReward>>();
            }
            close.SetActive(multipleGifts.Count <= 0);
            claim.SetActive(multipleGifts.Count > 0);
            if (multipleGifts.Count > 3)
            {
                SoundManager.Instance.Play("ChapterAcheivementAppear");
                title.text = Lean.Localization.LeanLocalization.GetTranslationText("Wonderfull");
                subtitle.text = Lean.Localization.LeanLocalization.GetTranslationText("You have achived all gifts");
            }
            else if (multipleGifts.Count > 0 && multipleGifts.Count <= 3)
            {
                SoundManager.Instance.Play("LevelFailedPopup");
                title.text = Lean.Localization.LeanLocalization.GetTranslationText("Time out");
                subtitle.text = Lean.Localization.LeanLocalization.GetTranslationText("You've collected")+" " + multipleGifts.Count + " " + Lean.Localization.LeanLocalization.GetTranslationText("Gifts")+ "!";

            }
            else
            {
                SoundManager.Instance.Play("LevelFailedPopup");
                title.text = Lean.Localization.LeanLocalization.GetTranslationText("Time out");
                subtitle.text = Lean.Localization.LeanLocalization.GetTranslationText("Failed to achive all gift, Better luck next time!");

            }
            SetupProgressBar();
        }
    }
}
