using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class GameConfiguration : SingletonComponent<GameConfiguration>
    {
        // Global Settings
        [Space(5)]
        [Header("Global Settings")]
        public int NoOfChallenges = 0;

        // Fresh User
        [Space(5)]
        [Header("Free rewards")]
        public int FirstTimeFreeMoves = 15;
        public int FirstTimeFreeResets = 15;
        public int FirstTimeFreeHints = 15;
        public int FirstTimeFreeUndos = 15;
        public int FirstTimeFreeCoins = 15;
        public int LevelFailMovesAdder = 0;

        // Moves Offer
        [Space(5)]
        [Header("Moves Offer")]
        public int MiniOfferCoins = 0;
        public int MiniOfferMoves = 0;
        public int MegaOfferCoins = 0;
        public int MegaOfferMoves = 0;

        // Gameplay
        [Space(5)]
        [Header("Gameplay")]
        public int WinTrophyNormal = 0;
        public int WinTrophyChallenge = 0;
        public int FailTrophyChallenge = 0;

        // ProfileScreen
        [Space(5)]
        [Header("Profile Screen")]
        public List<ProfileTier> ProfileTrophies;
        public List<ProfileTier> ProfileStars;
        public List<ProfileTier> ProfileLollipops;
        public Sprite BronzeBadge = null;
        public Sprite SilverBadge = null;
        public Sprite GoldBadge = null;
        public int BrainActivityLevelGap = 20;

        // ChapterScreen
        [Header("Chapter Screen")]
        [Space(5)]
        public List<ChapterTier> ChapterTiers;

        // QuestScreen
        [Header("Quest Screen")]
        [Space(5)]
        public TextAsset QuestData;
        public int MovesLowerThreshold = 15;

        // DailyRewardScreen
        [Header("Daily Reward Screen")]
        [Space(5)]
        public List<DailyRewardConfig> DailyRewardConfig;
        public float DailyRewardTimeInHrs;

        //LevelCompletePopup
        [Header("Level Complete Popup")]
        [Space(5)]
        public int LollipopCollectCount = 25;
        public ProbableRewards LollipopCollectReward;
        public string[] threeStarMessage;
        public string[] twoStarMessage;
        public string[] oneStarMessage;
        public int intervalToShowInviteScreen = 0;
        public int LevelMessageOfferContinousLevel = 0;
        public List<LevelMessageOffers> LevelMessageOffer = new List<LevelMessageOffers>();
        public int questTimerVisibilityMininmumMinutes = 15;

        // Mini Game
        [Header("Mini Game")]
        [Space(5)]
        public int MiniGameMaxStars = 30;
        public int MaxScoreInMiniGame = 100;
        public float MiniGameTimeInSeconds = 180;
        public List<ProbableRewards> RewardsForMiniGameScore;
        public int ScoreWonForCorrectMatch = 100;
        public int ScoreLostForWrongMatch = 50;
        public int ChapterGapRateUsPopup = 0;

        // Moves Recharge
        [Header("Moves Recharge")]
        [Space(5)]
        public int MovesRechargeMaximumFreeMoves = 0;
        public int MovesRechargedPerTimeGap = 0;
        public int MovesRechargeTimeGapInMinutes = 0;
        public int InstantRechargeCostPerMove = 50;

        [Header("Level Fail Purchase")]
        [Space(5)]
        public int[] LevelFailPrice = new int[3];

        [Header("Gift Images")]
        [Space(5)]
        public Sprite PinkGift;
        public Sprite BlueGift;
        public Sprite RedGift;
        public Sprite YellowGift;
        public Sprite GreenGift;

        [Header("Booster Images")]
        [Space(5)]
        public Sprite TrophyIcon;
        public Sprite MovesIcon;
        public Sprite UndoIcon;
        public Sprite ResetIcon;
        public Sprite HintIcon;
        public Sprite CoinsIcon;

        [Header("Subscription Popup")]
        [Space(5)]
        public DefiniteRewards SubscriptionRewards = null;

        [Header("DarkMode Assets")]
        [Space(5)]
        public Sprite MediumDarkCard;
        public Sprite SmallDarkCard;
        public Sprite CloseDarkButton;
        public Sprite TitleDarkHeader;
        public Sprite PopupBackgroundDark;

        [Header("Debug")]
        [Space(5)]
        public bool isCustomLevel = false;
        public int customLevel = 0;

        public float QuestAllottedTime { get; set; }
        protected override void Awake()
        {
            if (Exists())
            {
                Destroy(gameObject);
            }
            else
            {
                base.Awake();
                DontDestroyOnLoad(this);
            }
        }

        public Sprite GetGiftSprite(GiftType type)
        {
            Sprite tex = null;
            switch (type)
            {
                case GiftType.BLUE:
                    tex = BlueGift;
                    break;
                case GiftType.GREEN:
                    tex = GreenGift;
                    break;
                case GiftType.RED:
                    tex = RedGift;
                    break;
                case GiftType.YELLOW:
                    tex = YellowGift;
                    break;
                case GiftType.PINK:
                    tex = PinkGift;
                    break;
            }
            return tex;
        }

        public Sprite GetMedalImage(ProfileTier.badge_type badge)
        {
            Sprite tex = null;
            switch (badge)
            {
                case ProfileTier.badge_type.GOLD:
                    tex = GoldBadge;
                    break;
                case ProfileTier.badge_type.SILVER:
                    tex = SilverBadge;
                    break;
                case ProfileTier.badge_type.BRONZE:
                    tex = BronzeBadge;
                    break;
            }
            return tex;
        }
        public Sprite GetSpriteForReward(RewardTypes type)
        {
            Sprite tex = null;
            switch (type)
            {
                case RewardTypes.TROPHIES:
                    tex = TrophyIcon;
                    break;

                case RewardTypes.MOVES:
                    tex = MovesIcon;
                    break;

                case RewardTypes.UNDO:
                    tex = UndoIcon;
                    break;

                case RewardTypes.COINS:
                    tex = CoinsIcon;
                    break;

                case RewardTypes.RESET:
                    tex = ResetIcon;
                    break;

                case RewardTypes.HINT:
                    tex = HintIcon;
                    break;
            }
            return tex;
        }
        public bool IsGameCompleted()
        {
            return (UserDataManager.Instance.GetData("current_level") > NoOfChallenges);
        }

        public void SetDarkModeOnPopups(Image CloseButtonImg, Image TitleHeaderImg, Image BackgroundCard)
        {
            if (CloseButtonImg != null) { CloseButtonImg.sprite = CloseDarkButton; }
            if (TitleHeaderImg != null) { TitleHeaderImg.sprite = TitleDarkHeader; }
            if (BackgroundCard != null) { BackgroundCard.sprite = PopupBackgroundDark; }
        }
        public void SetDarkModeOnCards(Image[] MediumCards, Image[] SmallCards)
        {
            if (UserDataManager.Instance.IsDarkModeOn())
            {
                if (MediumCards != null)
                {
                    foreach (Image card in MediumCards)
                    {
                        card.sprite = MediumDarkCard;
                    }
                }

                if (SmallCards != null)
                {
                    foreach (Image card in SmallCards)
                    {
                        card.sprite = SmallDarkCard;
                    }
                }
            }
        }
    }
}
