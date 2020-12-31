using BizzyBeeGames.DotConnect;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class LevelCompleteMessage : Popup
    {
        [SerializeField] private Text LevelMessage = null;
        [SerializeField] private RectTransform LevelCompleteContainer = null, LevelOfferContainer = null, OfferButton = null;
        [SerializeField] private Image FirstOfferIcon = null, SecondOfferIcon = null;
        [SerializeField] private Text FirstOfferAmount = null, SecondOfferAmount = null, ButtonPrice = null;
        [SerializeField] private TextMeshProUGUI TrophyAmount = null;
        [SerializeField] private RectTransform closeButton = null;

        private object[] popupData;
        private string messagePrefix = "";
        private int messageIndex = 0;
        bool showLevelCompleteOffer = false;
        int offerIndex = 0;
        int levelCounter = 0;
        GameConfiguration config = null;

        private void OnEnable()
        {
            Utils.DoSwipeVerticalAnimation(LevelCompleteContainer, 0, 0, 0f);
            Utils.DoSwipeVerticalAnimation(LevelOfferContainer, -2000f, -2000f, 0f);

            config = GameConfiguration.Instance;
            levelCounter += 1;
            showLevelCompleteOffer = levelCounter >= config.LevelMessageOfferContinousLevel;
            ShowLevelMessageContents();
            LevelOfferContainer.gameObject.SetActive(showLevelCompleteOffer);
        }

        public void ShowTrophyAmount()
        {
            int add_trophies = GameManager.Instance.Grid.ActiveLevel.is_challenge_library ? GameConfiguration.Instance.WinTrophyChallenge :
                GameConfiguration.Instance.WinTrophyNormal;

            SoundManager.Instance.Play("LevelCompleteTrophyAmount");
            TrophyAmount.text = "x " + add_trophies;
        }
        public void ShowingTrophy()
        {
            SoundManager.Instance.Play("LevelCompleteTrophy");
        }

        public void ShowLevelCompleteOffer()
        {
            if (showLevelCompleteOffer)
            {
                LevelMessageOffers offer = config.LevelMessageOffer[offerIndex];
                FirstOfferIcon.sprite = config.GetSpriteForReward(offer.OfferOne.type);
                SecondOfferIcon.sprite = config.GetSpriteForReward(offer.OfferTwo.type);

                FirstOfferAmount.text = "x " + offer.OfferOne.amount;
                SecondOfferAmount.text = "x " + offer.OfferTwo.amount;
                ButtonPrice.text = "$ " + offer.PriceOfOffer;

                Utils.DoSwipeVerticalAnimation(LevelCompleteContainer, LevelCompleteContainer.anchoredPosition.y, LevelCompleteContainer.anchoredPosition.y + 500f, 1f);
                Utils.DoSwipeVerticalAnimation(LevelOfferContainer, -1500f, LevelOfferContainer.anchoredPosition.y + 1500f, 1f);
                closeButton.gameObject.SetActive(true);
                Utils.DoSwipeHorizontalAnimation(closeButton, 300, closeButton.anchoredPosition.y, 1f, 0f);

                offerIndex = (offerIndex + 1) % config.LevelMessageOffer.Count;
            }
            else
            {
                closeButton.gameObject.SetActive(false);
                StartCoroutine(Utils.ExecuteAfterDelay(1f, (args) => { onTouchInput(); })); ;
            }
        }

        private void ShowLevelMessageContents()
        {
            if (GameManager.Instance.Grid.three_stars)
            {
                LevelMessage.text = messagePrefix + config.threeStarMessage[messageIndex] + "!";
            }
            else if (GameManager.Instance.Grid.two_stars)
            {
                LevelMessage.text = messagePrefix + config.twoStarMessage[messageIndex] + "!";
            }
            else if (GameManager.Instance.Grid.one_star)
            {
                LevelMessage.text = messagePrefix + config.oneStarMessage[messageIndex] + "!";
            }
            SoundManager.Instance.Play("LevelCompleteMessage");
            messageIndex = (messageIndex + 1) % config.threeStarMessage.Length;
        }

        public void ShowLevelCompleteScreen()
        {
            Hide(true);
            PopupManager.Instance.Show("LevelCompletePopup", popupData);
        }

        private void Update()
        {
#if !UNITY_EDITOR
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == 0)
                {
                    onTouchInput();
                }
            }
#endif
#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                onTouchInput();
            }
#endif
        }

        public void onTouchInput()
        {
            if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.name != OfferButton.gameObject.name)
            {
                GetComponent<Animator>().SetBool("dissappear", true);
                Utils.DoSwipeVerticalAnimation(LevelOfferContainer, LevelOfferContainer.anchoredPosition.y, -1500f, 1f);
                Utils.DoSwipeHorizontalAnimation(closeButton, closeButton.anchoredPosition.y,300f, 1f, 0f);
               
            }
        }
        public void OnPurchaseButtonClicked()
        {

        }
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            popupData = inData;
        }

        public void HideGamePlayScreen()
        {
            GameManager.Instance.ResetAllGamePlayAnimation();
        }
    }
}

