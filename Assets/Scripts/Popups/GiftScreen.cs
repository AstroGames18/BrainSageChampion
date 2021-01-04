using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using BizzyBeeGames.DotConnect;
using Lean.Localization;

namespace BizzyBeeGames
{
    public class GiftScreen : Popup
    {
        [SerializeField] RectTransform GiftTop, RewardCard, RewardCardFront, RewardCardShadow, GiftBase, GiftMask, ClaimButton, InventoryBag;
        [SerializeField] Image CardImage, RewardIcon, GiftTopImage, GiftBaseImage, GiftMaskImage;
        [SerializeField] Sprite CardFront, CardBack, TrophyIcon, MovesIcon, UndoIcon, CoinsIcon, ResetIcon, HintIcon;
        [SerializeField] float resetPositionsCard, resetPositionsTop, resetPositionButton;
        [SerializeField] TextMeshProUGUI RewardText;
        [SerializeField] GameObject giftCard = null, DisplayContainer = null, BackGround = null, TrophyHUD = null, InventoryHUD = null, TapInfo = null, TrophyObject = null, InventoryObject = null;
        [SerializeField] List<QuestReward> giftRewards = new List<QuestReward>();
        [SerializeField] Sprite[] Tops, Bases, Masks;
        [SerializeField] Animator giftAnimator;
        [SerializeField] bool customTargetRect;
        [SerializeField] GameObject AnimatedShine;
        [SerializeField] Text titleText;
        [SerializeField] GiftTopHUD topHUD;
        [Header("Particle")]
        [SerializeField] ParticleSystem shootParticle;
        [SerializeField] ParticleSystem[] fireworkParticles;
        [SerializeField] FireworkParticle[] fireworkParticlesColors;

        private bool giftBoxOpened = false;
        private int giftOpenedCounter = 0;
        private bool cardShowing = false;
        private List<GiftCard> allCards = new List<GiftCard>();
        private bool rewardsClaimed = false;
        private GiftType giftType = GiftType.BLUE;
        private GiftToReward userGift = null;
        private Popup currentPopup = null;
        private Action<object[]> giftScreenCallBack;
        private bool overrideCustomTargetRect = false;
        private int movesToReward = 0;
        private bool isUnclaimed = false;
        private string title = "";

        private void OnEnable()
        {
            // All the variables and objects are reset to their
            // original positions, sizes and values here
            userGift = UserDataManager.Instance.GetUserGift();
            SoundManager.Instance.PlayScreenBGM("GiftScreenBGM");
            currentPopup = PopupManager.Instance.GetPopupById(userGift.id);
            giftRewards = userGift.rewardsToGive;
            giftType = userGift.rewardType;
            SetGiftImages(giftType);
            movesToReward = userGift.moves;
            isUnclaimed = userGift.isUnclaimed;
            title = Lean.Localization.LeanLocalization.GetTranslationText(userGift.title != null && userGift.title != "" ? userGift.title : "Lollipops");
            userGift = null;
            UserDataManager.Instance.SetUserGift();
            AnimatedShine.SetActive(true);

            giftOpenedCounter = 0;
            cardShowing = false;
            giftBoxOpened = false;
            TapInfo.SetActive(true);

            CardImage.sprite = CardFront;
            RewardIcon.gameObject.SetActive(false);
            RewardText.gameObject.SetActive(false);
            giftScreenCallBack = null;

            titleText.gameObject.SetActive(isUnclaimed);
            if (isUnclaimed)
            {
                titleText.text = Lean.Localization.LeanLocalization.GetTranslationText("Reward for collecting") + " " + title;
            }

            Utils.DoZoomAnimation(GiftBase, 0f, 0f, 1f);
            Utils.DoZoomAnimation(GiftTop, 0f, 0f, 1f);
            Utils.DoZoomAnimation(GiftMask, 0f, 0f, 1f);
            //Utils.DoZoomAnimation(RewardCard, 0f, 0f, 0f);
            Utils.DoSwipeVerticalAnimation(ClaimButton, ClaimButton.anchoredPosition.y, resetPositionButton, 0f);
            Utils.DoSwipeVerticalAnimation(InventoryBag, InventoryBag.anchoredPosition.y, resetPositionButton, 0f);

            Utils.DoZoomAnimation(ClaimButton, 0f, 0f, 1f);

            BackGround.SetActive(true);
        }
        private void OnDisable()
        {
            // start the current screens bgm when gift screen is closed
            if (LoadingManager.Instance.currentScreen == LoadingManager.StartScreen)
            {
                SoundManager.Instance.PlayScreenBGM("StartScreenBGM");
            }
            else if (LoadingManager.Instance.currentScreen == LoadingManager.GameScreen)
            {
                GameManager.Instance.PlayGameScreenBGM();
            }
            else if (LoadingManager.Instance.currentScreen == LoadingManager.TOKGameScreen)
            {
                SoundManager.Instance.PlayScreenBGM("MiniGameBGM");
            }
        }
        private string getRewardSound(QuestReward.reward_types type)
        {
            string sfx = "";
            switch (type)
            {
                case QuestReward.reward_types.COINS:
                    sfx = "GiftCoins";
                    break;
                case QuestReward.reward_types.HINT:
                    sfx = "GiftHints";

                    break;
                case QuestReward.reward_types.TROPHIES:
                    sfx = "GiftTrophies";

                    break;
                case QuestReward.reward_types.RESET:
                    sfx = "GiftResets";

                    break;
                case QuestReward.reward_types.UNDO:
                    sfx = "GiftUndos";

                    break;
                case QuestReward.reward_types.MOVES:
                    sfx = "GiftMoves";

                    break;
            }
            return sfx;
        }
        public void onGiftShaking()
        {
            SoundManager.Instance.Play("GiftShake");
        }

        public void OpenGiftBox(QuestReward reward)
        {
            lastReward = reward;
            cardShowing = true;

            if (!giftBoxOpened)
            {
                // if gift is being opened for the first time, open the lid
                SoundManager.Instance.Play("GiftLidOpened");
                giftAnimator.SetBool("OpenLid", true);
                //Utils.DoSwipeVerticalAnimation(GiftTop, GiftTop.anchoredPosition.y, GiftTop.anchoredPosition.y + 2000f, 1f, 0f);
            }
            else
            {
                // if gift is already opened show the reward card
                SoundManager.Instance.Play("RewardCardGoesUp");
            }

            StartCoroutine(Utils.ExecuteAfterDelay(0.5f, (args3) =>
            {
                SoundManager.Instance.Play("RewardCardShown");
            }));
        }

        public void onCardAnimComplete()
        {
            cardShowing = false;
            StartCoroutine(Utils.ExecuteAfterDelay(0.2f, (args) =>
            {
                cardShowing = false;
                ShowNextGift();
            }));
        }
        QuestReward lastReward;
        public void SetCardRewardType()
        {
            SoundManager.Instance.Play(getRewardSound(lastReward.type));

            RewardText.text = "x " + lastReward.amount;
            RewardIcon.sprite = getSpriteForId(lastReward.type);
        }

        public void ShowNextGift()
        {
            // if card is being shown ignore input
            if (cardShowing) { return; }
            titleText.gameObject.SetActive(false);

            if (!giftAnimator.GetBool("tap"))
                giftAnimator.SetBool("tap", true);

            //if there is more gifts to be shown, show
            // else show all the gifts we received one final time
            if (giftOpenedCounter < giftRewards.Count)
            {
                AnimatedShine.SetActive(false);
                OpenGiftBox(giftRewards[giftOpenedCounter]);
                giftBoxOpened = true;
                giftAnimator.SetTrigger("shoot");
                giftOpenedCounter += 1;
            }
            else
            {
                cardShowing = true;

                giftAnimator.SetBool("showAllGift", true);
                //Utils.DoSwipeVerticalAnimation(GiftBase, GiftBase.anchoredPosition.y, GiftBase.anchoredPosition.y - 1000f, 1f, 0f);
                //Utils.DoSwipeVerticalAnimation(GiftMask, GiftMask.anchoredPosition.y, GiftMask.anchoredPosition.y - 1000f, 1f, 0f);
                //Utils.DoSwipeVerticalAnimation(RewardCard, resetPositionsCard + 750f, resetPositionsCard + 1750f, 0.5f);
                //Utils.DoSwipeVerticalAnimation(ClaimButton, resetPositionButton, resetPositionButton + 500f, 1f, 1f);

                ShowAllRewards();
            }
        }
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            if (inData != null && inData.Length > 0)
            {
                giftScreenCallBack = (Action<object[]>)inData[0];
                overrideCustomTargetRect = (bool)inData[1];
            }
        }

        public void PlayShootParticle()
        {
            shootParticle.Play();
        }

        /// <summary>
        /// Show what kind of gift to open
        /// </summary>
        private void SetGiftImages(GiftType type)
        {
            Sprite topSprite = null, baseSprite = null, maskSprite = null;
            string giftMusic = "";
            switch (type)
            {
                case GiftType.BLUE:
                    topSprite = Tops[0];
                    baseSprite = Bases[0];
                    maskSprite = Masks[0];
                    giftMusic = "BlueGift";

                    break;
                case GiftType.GREEN:
                    topSprite = Tops[1];
                    baseSprite = Bases[1];
                    maskSprite = Masks[1];
                    giftMusic = "GreenGift";

                    break;
                case GiftType.RED:
                    topSprite = Tops[2];
                    baseSprite = Bases[2];
                    maskSprite = Masks[2];
                    giftMusic = "RedGift";

                    break;
                case GiftType.YELLOW:
                    topSprite = Tops[3];
                    baseSprite = Bases[3];
                    maskSprite = Masks[3];
                    giftMusic = "YellowGift";

                    break;
                case GiftType.PINK:
                    topSprite = Tops[4];
                    baseSprite = Bases[4];
                    maskSprite = Masks[4];
                    giftMusic = "PinkGift";

                    break;
            }
            GiftTopImage.sprite = topSprite;
            GiftBaseImage.sprite = baseSprite;
            GiftMaskImage.sprite = maskSprite;
            SoundManager.Instance.Play(giftMusic);
        }

        /// <summary>
        /// Displaying all the gifts received after 
        /// showing one by one
        /// </summary>
        /// 
        bool cardsAdded = false;
        private void ShowAllRewards()
        {
            if (cardsAdded)
                return;
            cardsAdded = true;
            TapInfo.SetActive(false);
            allCards = new List<GiftCard>();
            for (int i = 0; i < giftRewards.Count; i++)
            {

                GameObject card = Instantiate(giftCard, DisplayContainer.transform);

                GiftCard cardScript = card.GetComponent<GiftCard>();
                if (cardScript != null)
                {
                    cardScript.SetCard(getSpriteForId(giftRewards[i].type), giftRewards[i].amount, i, getRewardSound(giftRewards[i].type));
                    allCards.Add(cardScript);
                }
            }
        }
        /// <summary>
        /// Reset all animation position
        /// </summary>
        private void ResetPositions(bool end = false)
        {
            if (end)
            {
                cardsAdded = false;
                //Utils.DoSwipeVerticalAnimation(GiftTop, GiftTop.anchoredPosition.y, GiftTop.anchoredPosition.y - 2000f, 0f, 0f);
                //Utils.DoSwipeVerticalAnimation(GiftBase, GiftBase.anchoredPosition.y, GiftBase.anchoredPosition.y + 1000f, 0f, 0f);
                //Utils.DoSwipeVerticalAnimation(GiftMask, GiftMask.anchoredPosition.y, GiftMask.anchoredPosition.y + 1000f, 0f, 0f);
                //Utils.DoSwipeVerticalAnimation(RewardCard, RewardCard.anchoredPosition.y, resetPositionsCard, 0f, 0f);
            }
            //Utils.DoSwipeVerticalAnimation(RewardCard, RewardCard.anchoredPosition.y, resetPositionsCard, 0f, 0f);
        }

        /// <summary>
        /// Check if there is rewards other than trophy
        /// </summary>
        private bool getIfHasBesidesTrophy()
        {
            bool hasBesidesTrophy = false;

            for (int i = 0; i < giftRewards.Count; i++)
            {
                if (giftRewards[i].type != QuestReward.reward_types.TROPHIES)
                {
                    hasBesidesTrophy = true;
                    break;
                }
            }
            return hasBesidesTrophy;
        }

        /// <summary>
        /// Claim all gifts. This closes the gift screen with animation
        /// </summary>
        public void OnClaimButtonPressed()
        {
            float delayTime = 0f;
            rewardsClaimed = true;
            SoundManager.Instance.Play("GiftClaimed");

            Utils.DoZoomAnimation(ClaimButton, 0.5f, 1f, 0f);

            // if animation is not being sent to a custom target
            if (!customTargetRect || overrideCustomTargetRect)
            {
                // if there is rewards other than trophy than show inventory bag
                // and increase delay to show inventory bag
                if (getIfHasBesidesTrophy()) { Utils.DoSwipeVerticalAnimation(InventoryBag, resetPositionButton, resetPositionButton + 300f, 0.5f); }
                delayTime = 0.5f;
            }
            else
            {
                // if rewards are being moved to custom target then hide
                // background and current popup 
                BackGround.SetActive(false);
                if (currentPopup != null) { currentPopup.Hide(true); }
            }
            StartCoroutine(Utils.ExecuteAfterDelay(delayTime, (args) =>
            {
                // set target to move towards in every card
                for (int i = 0; i < giftRewards.Count; i++)
                {
                    allCards[i].HideCardBG();
                    Transform targetTransform;
                    if (giftRewards[i].type == QuestReward.reward_types.TROPHIES)
                    {
                        giftAnimator.SetTrigger("TopOpen");
                        topHUD.RefreshHUD();
                        targetTransform = overrideCustomTargetRect ? TrophyObject.transform : TrophyHUD.gameObject.transform;
                    }
                    else
                    {
                        //giftAnimator.SetBool("BottomOpen", true);
                        targetTransform = overrideCustomTargetRect ? InventoryObject.transform : InventoryHUD.gameObject.transform;
                    }
                    allCards[i].SetTarget(targetTransform);
                }
            }));
            if (movesToReward > 0)
                InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryMoves, movesToReward);
            InventoryManager.Instance.UpdateAllQuestRewards(giftRewards);

            // giftAnimator.SetBool("BottomOpen", false);

        }
        private void Update()
        {
#if !UNITY_EDITOR
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == 0)
                {
                    ShowNextGift();
                }
            }
#endif
#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                ShowNextGift();
            }
#endif
            if (!rewardsClaimed) { return; }
            bool allCardsReachedTarget = true;
            for (int i = 0; i < allCards.Count; i++)
            {
                if (!allCards[i].reachedTraget) { return; }
            }

            // once all cards reached their target
            // 1. Destro all the children in DisplayContainer
            // 2. call the call back if there is one.
            // 3. Close popup after reseting all positions
            if (allCardsReachedTarget)
            {
                rewardsClaimed = false;
                foreach (Transform child in DisplayContainer.transform)
                {
                    Destroy(child.gameObject);
                }
                if (giftScreenCallBack != null) { giftScreenCallBack(null); }
                if (!customTargetRect || overrideCustomTargetRect)
                {
                    if (getIfHasBesidesTrophy()) { Utils.DoSwipeVerticalAnimation(InventoryBag, resetPositionButton + 300f, resetPositionButton, 0.5f); }
                    StartCoroutine(Utils.ExecuteAfterDelay(1.0f, (args) =>
                    {
                        ClosePopup();
                    }));
                }
                else { ClosePopup(); }
            }
        }
        private void ClosePopup()
        {
            ResetPositions(true);
            Hide(true);
        }
        /// <summary>
        /// Get the sprite for each type of reward to put on card
        /// </summary>
        private Sprite getSpriteForId(QuestReward.reward_types type)
        {
            Sprite tex = null;
            switch (type)
            {
                case QuestReward.reward_types.TROPHIES:
                    tex = TrophyIcon;
                    break;

                case QuestReward.reward_types.MOVES:
                    tex = MovesIcon;
                    break;

                case QuestReward.reward_types.UNDO:
                    tex = UndoIcon;
                    break;

                case QuestReward.reward_types.COINS:
                    tex = CoinsIcon;
                    break;

                case QuestReward.reward_types.RESET:
                    tex = ResetIcon;
                    break;

                case QuestReward.reward_types.HINT:
                    tex = HintIcon;
                    break;
            }
            return tex;
        }

        public void PlayCardBgParticle()
        {
            for (int i = 0; i < fireworkParticles.Length; i++)
            {
                var col = fireworkParticles[i].colorOverLifetime;
                col.enabled = true;

                col.color = fireworkParticlesColors[GetGradientIndex()].gradients[i];
            }
            fireworkParticles[0].Play();
        }

        private int GetGradientIndex()
        {
            return 0;
        }

        [Serializable]
        public class FireworkParticle
        {
            public Gradient[] gradients;
        }
    }
}
