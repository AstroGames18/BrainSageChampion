using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UserData;
using System;
using Lean.Localization;

namespace BizzyBeeGames
{
    public class AcheivementsScreen : Popup
    {
        [SerializeField] RectTransform PrevAchievementImageRect = null, PrevAcheivementTagRect = null, NextAcheivementImageRect = null, NextAcheivementTagRect = null;
        [SerializeField] Text AcheivementText = null, PrevAcheivementTag = null, NextAcheivementTag = null, ButtonText = null;
        [SerializeField] TextMeshProUGUI PrevMedalNumber = null, NextMedalNumber = null;
        [SerializeField] Image PrevAchievementImage = null, NextAcheivementImage = null;
        [SerializeField] Sprite BookIcon = null;
        [SerializeField] AnimatedScrollbar trophyProgressBar, starProgressBar, lollipopProgressBar;
        [SerializeField] Image TrophyGiftImage, StarGiftImage, LollipopGiftImage, starBg, lollipopBg, trophyBg;
        [SerializeField] Sprite completedBg, inCompletedBg, darkmodeBg;
        [SerializeField]
        GameObject profile_trophies, profile_stars, profile_lollipop;
        [SerializeField]
        GameObject trophiesMainContainer, starsMainContainer, lollipopMainContainer;
        [SerializeField] GameObject StarProgressContainer, StarProgressCompleteContainer, TrophyProgressContainer, TrophyProgressCompleteContainer, LollipopProgressContainer, LollipopProgressCompleteContainer;
        [SerializeField] RectTransform prevMedalObj, nextmedalObj, giftObj;
        [SerializeField] GameObject[] giftObjItems;
        [SerializeField] LeanToken achievementType;

        [SerializeField] Vector3 medalSmallSize = new Vector3(0.5f, 0.5f, 1f);

        private float prevChapterImageReset = 0f;
        private float nextChapterImageReset = 1000f;
        private float chapterImageOffset = 2500f;
        private bool isChapter = false;
        private string keyInventory = "";
        private bool isRewardClaimed = false;
        private List<QuestReward> allRewards = null;
        private GiftType allRewardsGiftType;
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);

            ResetAcheivementScreen();
            if (inData != null && inData.Length > 0)
            {
                isChapter = (bool)inData[0];
                keyInventory = (string)inData[1];
                if (isChapter) { ShowChapterProgression(); }
                else { ShowProfileAcheivement(keyInventory); }
            }
            ButtonText.text = "Claim";
        }
        private void ResetAcheivementScreen()
        {
            isRewardClaimed = false;
            isChapter = false;
            keyInventory = "";
            allRewards = new List<QuestReward>();
            allRewardsGiftType = GiftType.BLUE;

            PrevAchievementImageRect.gameObject.SetActive(true);
            PrevAcheivementTagRect.gameObject.SetActive(true);

            NextAcheivementImageRect.gameObject.SetActive(false);
            NextAcheivementTagRect.gameObject.SetActive(false);

            Utils.DoSwipeHorizontalAnimation(PrevAchievementImageRect, prevChapterImageReset, prevChapterImageReset, 0f, 0f);
            Utils.DoSwipeHorizontalAnimation(PrevAcheivementTagRect, prevChapterImageReset, prevChapterImageReset, 0f, 0f);

            Utils.DoSwipeHorizontalAnimation(NextAcheivementImageRect, nextChapterImageReset, prevChapterImageReset, 0f, 0f);
            Utils.DoSwipeHorizontalAnimation(NextAcheivementTagRect, nextChapterImageReset, prevChapterImageReset, 0f, 0f);

            Utils.DoZoomAnimation(NextAcheivementImageRect, 0f, 1f, 1f);
            Utils.DoZoomAnimation(NextAcheivementTagRect, 0f, 1f, 1f);

            nextmedalObj.position = prevMedalObj.position;
            nextmedalObj.localScale = Vector3.one;
            foreach (GameObject giftObjItem in giftObjItems)
            {
                giftObjItem.SetActive(true);
            }
        }
        private void ShowProfileAcheivement(string keyInventory)
        {
            SoundManager.Instance.Play("MedalAcheivementAppear");
            achievementType.SetValue(GetConfigProfileTitle(keyInventory));
            AcheivementText.text = LeanLocalization.GetTranslationText("Medal target completed on x");
            int inventory_amount = InventoryManager.Instance.GetInventory(keyInventory);
            PrevMedalNumber.gameObject.SetActive(true);
            NextMedalNumber.gameObject.SetActive(true);

            PrevAchievementImageRect.gameObject.SetActive(true);
            PrevAcheivementTagRect.gameObject.SetActive(true);

            NextAcheivementImageRect.gameObject.SetActive(false);
            NextAcheivementTagRect.gameObject.SetActive(false);
            NextAcheivementTag.gameObject.SetActive(false);

            object[] profTiers = GetProphileTiers(inventory_amount, GetConfigProfileTiers(keyInventory));

            ProfileTier nextTier = (ProfileTier)profTiers[0];
            ProfileTier prevTier = (ProfileTier)profTiers[1];

            NextAcheivementImage.sprite = GameConfiguration.Instance.GetMedalImage(nextTier.badge);
            NextAcheivementTag.text = Lean.Localization.LeanLocalization.GetTranslationText(nextTier.medal_tag);
            NextMedalNumber.text = nextTier.medal_number.ToString();

            PrevAchievementImage.sprite = GameConfiguration.Instance.GetMedalImage(prevTier.badge);
            // PrevAcheivementTag.text = Lean.Localization.LeanLocalization.GetTranslationText(prevTier.medal_tag);
            PrevAcheivementTag.text = GetConfigProfileTitle(keyInventory);
            PrevMedalNumber.text = prevTier.medal_number.ToString();


            allRewards = InventoryManager.Instance.GetProbableRewards(prevTier.reward);
            allRewardsGiftType = prevTier.reward.giftType;
            Gift gift = new Gift();
            gift.gifts = allRewards;
            gift.giftType = allRewardsGiftType;
            SetProgress(true, prevTier);
            //InventoryManager.Instance.UpdateAllQuestRewards(allRewards);
        }

        private string GetConfigProfileTitle(string keyInventory)
        {
            string title = "";
            switch (keyInventory)
            {
                case InventoryManager.Key_InventoryStars:
                    title = Lean.Localization.LeanLocalization.GetTranslationText("Stars");
                    break;

                case InventoryManager.Key_InventoryTrophies:
                    title = Lean.Localization.LeanLocalization.GetTranslationText("Trophies");
                    break;
                case InventoryManager.Key_InventoryLollipops:
                    title = Lean.Localization.LeanLocalization.GetTranslationText("Lollipops");
                    break;
            }
            return title;
        }

        private List<ProfileTier> GetConfigProfileTiers(string keyInventory)
        {
            List<ProfileTier> tiers = null;
            switch (keyInventory)
            {
                case InventoryManager.Key_InventoryStars:
                    tiers = GameConfiguration.Instance.ProfileStars;
                    break;

                case InventoryManager.Key_InventoryTrophies:
                    tiers = GameConfiguration.Instance.ProfileTrophies;
                    break;
                case InventoryManager.Key_InventoryLollipops:
                    tiers = GameConfiguration.Instance.ProfileLollipops;
                    break;


            }
            return tiers;
        }


        private void SetProgress(bool first, ProfileTier profileTier)
        {
            switch (keyInventory)
            {
                case InventoryManager.Key_InventoryStars:
                    if (first)
                        SetProphileStars(profileTier);
                    else
                    {

                        int inventory_stars = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryStars);
                        SetProphileStars(inventory_stars);
                    }
                    break;

                case InventoryManager.Key_InventoryTrophies:
                    if (first)
                        SetProphileTrophies(profileTier);
                    else
                    {
                        int inventory_trophies = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryTrophies);
                        SetProphileTrophies(inventory_trophies);
                    }
                    break;
                case InventoryManager.Key_InventoryLollipops:
                    if (first)
                        SetProphileLoliipops(profileTier);
                    else
                    {
                        int inventory_lollipops = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryLollipops);
                        SetProphileLoliipops(inventory_lollipops);
                    }
                    break;


            }
        }
        private void ShowChapterProgression()
        {
            trophiesMainContainer.SetActive(false);
            starsMainContainer.SetActive(false);
            lollipopMainContainer.SetActive(false);
            AcheivementText.text = "Chapter Complete";
            int curr_level = UserDataManager.Instance.GetData("current_level");
            PrevMedalNumber.gameObject.SetActive(false);
            NextMedalNumber.gameObject.SetActive(false);
            SoundManager.Instance.Play("ChapterAcheivementAppear");

            object[] chapTiers = GetChapterTiers(curr_level);
            ChapterTier nextTier = (ChapterTier)chapTiers[0];
            ChapterTier prevTier = (ChapterTier)chapTiers[1];

            NextAcheivementImage.sprite = BookIcon;
            NextAcheivementTag.text = nextTier.banner_message;

            PrevAchievementImage.sprite = BookIcon;
            PrevAcheivementTag.text = prevTier.banner_message;

            allRewards = InventoryManager.Instance.GetProbableRewards(prevTier.reward);
            allRewardsGiftType = prevTier.reward.giftType;
            Gift gift = new Gift();
            gift.gifts = allRewards;
            gift.giftType = allRewardsGiftType;
            //InventoryManager.Instance.UpdateAllQuestRewards(allRewards);
        }
        private void DoChapterAnimation()
        {
            AcheivementText.text = "New Chapter Unlocked";
            NextAcheivementImageRect.gameObject.SetActive(true);
            NextAcheivementTagRect.gameObject.SetActive(true);
            SoundManager.Instance.Play("ChapterAcheivementUpdate");

            bool landscape = UnityEngine.Screen.width > UnityEngine.Screen.height;
            Utils.DoSwipeHorizontalAnimation(PrevAchievementImageRect, prevChapterImageReset, prevChapterImageReset - chapterImageOffset, landscape ? 0.4f : 1f, 0f);
            Utils.DoSwipeHorizontalAnimation(PrevAcheivementTagRect, prevChapterImageReset, prevChapterImageReset - chapterImageOffset, landscape ? 0.4f : 1f, 0f);

            Utils.DoSwipeHorizontalAnimation(NextAcheivementImageRect, nextChapterImageReset, prevChapterImageReset, landscape ? 0.4f : 1f, 1f);
            Utils.DoSwipeHorizontalAnimation(NextAcheivementTagRect, nextChapterImageReset, prevChapterImageReset, landscape ? 0.4f : 1f, 1f);
        }
        private void DoProfileAnimation()
        {
            AcheivementText.text = "New Challenge Unlocked";

            SoundManager.Instance.Play("MedalAcheivementUpdate");

            PrevAchievementImageRect.gameObject.SetActive(false);
            PrevAcheivementTagRect.gameObject.SetActive(false);

            NextAcheivementImageRect.gameObject.SetActive(true);
            NextAcheivementTagRect.gameObject.SetActive(true);
            NextAcheivementTag.gameObject.SetActive(false);

            nextmedalObj.position = giftObj.position;
            nextmedalObj.localScale = medalSmallSize;
            foreach (GameObject giftObjItem in giftObjItems)
            {
                giftObjItem.SetActive(false);
            }
            SetProgress(false, null);
            Utils.DoZoomAnimation(NextAcheivementImageRect, 0.2f, 3f, 1f);
            Utils.DoZoomAnimation(NextAcheivementTagRect, 0.2f, 3f, 1f);
        }
        public object[] GetChapterTiers(int curr_level)
        {
            ChapterTier data;
            for (int i = 0; i < GameConfiguration.Instance.ChapterTiers.Count; i++)
            {
                data = GameConfiguration.Instance.ChapterTiers[i];
                if (curr_level >= data.min_level && curr_level < data.max_level)
                {
                    return new object[] { data, GameConfiguration.Instance.ChapterTiers[i - 1] };
                }
            }
            return null;
        }
        public object[] GetProphileTiers(int inventory_amount, List<ProfileTier> ProfileTierList)
        {
            ProfileTier profile_tier;

            for (int i = 0; i < ProfileTierList.Count; i++)
            {
                profile_tier = ProfileTierList[i];
                if (inventory_amount >= profile_tier.min_value && inventory_amount <= profile_tier.max_value)
                {
                    object[] test = new object[] { ProfileTierList[i + 1], profile_tier };
                    return test;
                }
            }
            return null;
        }

        public void OnClaimButtonPressed()
        {
            if (!isRewardClaimed)
            {
                UserDataManager.Instance.SetUserGiftReward(allRewards,
                    allRewardsGiftType, "AcheivementsScreen");
                SoundManager.Instance.Play("AcheivementClaimButton");
                isRewardClaimed = true;
                ButtonText.text = "Nice";
                PopupManager.Instance.Show("GiftScreen", new object[] { null, true }, onGiftScreenClosed);
            }
            else
            {
                SoundManager.Instance.Play("AcheivementNiceButton");
                Hide(true);
            }
        }

        private void onGiftScreenClosed(bool cancelled, object[] data)
        {
            if (isChapter) { DoChapterAnimation(); }
            else { DoProfileAnimation(); }
        }
        void SetProphileTrophies(ProfileTier profile_tier)
        {
            trophiesMainContainer.SetActive(true);
            starsMainContainer.SetActive(false);
            lollipopMainContainer.SetActive(false);

            trophyBg.sprite = UserDataManager.Instance.IsDarkModeOn() ? darkmodeBg : completedBg;

            float progress_size = (float)profile_tier.max_value / (float)profile_tier.max_value;
            profile_trophies.GetComponent<Text>().text = profile_tier.max_value.ToString() + "/" + profile_tier.max_value.ToString();
            // profile_trophies.gameObject.SetActive(false);
            trophyProgressBar.SetValue(progress_size);
            TrophyGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
            TrophyProgressCompleteContainer.SetActive(false);
            TrophyProgressContainer.SetActive(true);
        }
        void SetProphileTrophies(int inventory_trophies)
        {
            trophiesMainContainer.SetActive(true);
            starsMainContainer.SetActive(false);
            lollipopMainContainer.SetActive(false);
            ProfileTier profile_tier;

            trophyBg.sprite = UserDataManager.Instance.IsDarkModeOn() ? darkmodeBg : inCompletedBg;
            for (int i = 0; i < GameConfiguration.Instance.ProfileTrophies.Count; i++)
            {
                profile_tier = GameConfiguration.Instance.ProfileTrophies[i];
                if (inventory_trophies >= profile_tier.min_value && inventory_trophies < profile_tier.max_value)
                {
                    float progress_size = inventory_trophies / (float)profile_tier.max_value;
                    profile_trophies.GetComponent<Text>().text = inventory_trophies.ToString() + "/" + profile_tier.max_value.ToString();
                    trophyProgressBar.SetValue(progress_size);
                    TrophyGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
                }
            }
            TrophyProgressCompleteContainer.SetActive(inventory_trophies >= GameConfiguration.Instance.ProfileTrophies[GameConfiguration.Instance.ProfileTrophies.Count - 1].max_value);
            TrophyProgressContainer.SetActive(inventory_trophies < GameConfiguration.Instance.ProfileTrophies[GameConfiguration.Instance.ProfileTrophies.Count - 1].max_value);
        }

        void SetProphileStars(ProfileTier profile_tier)
        {
            trophiesMainContainer.SetActive(false);
            starsMainContainer.SetActive(true);
            lollipopMainContainer.SetActive(false);

            starBg.sprite = UserDataManager.Instance.IsDarkModeOn() ? darkmodeBg : completedBg;
            float progress_size = profile_tier.max_value / (float)profile_tier.max_value;

            profile_stars.GetComponent<Text>().text = profile_tier.max_value.ToString() + "/" + profile_tier.max_value.ToString();
            // profile_stars.gameObject.SetActive(false);
            starProgressBar.SetValue(progress_size);
            StarGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
            StarProgressCompleteContainer.SetActive(false);
            StarProgressContainer.SetActive(true);
        }
        void SetProphileStars(int inventory_stars)
        {
            trophiesMainContainer.SetActive(false);
            starsMainContainer.SetActive(true);
            lollipopMainContainer.SetActive(false);
            ProfileTier profile_tier;
            starBg.sprite = UserDataManager.Instance.IsDarkModeOn() ? darkmodeBg : inCompletedBg;

            for (int i = 0; i < GameConfiguration.Instance.ProfileStars.Count; i++)
            {
                profile_tier = GameConfiguration.Instance.ProfileStars[i];
                if (inventory_stars >= profile_tier.min_value && inventory_stars < profile_tier.max_value)
                {
                    float progress_size = inventory_stars / (float)profile_tier.max_value;
                    profile_stars.GetComponent<Text>().text = inventory_stars.ToString() + "/" + profile_tier.max_value.ToString();
                    starProgressBar.SetValue(progress_size);
                    StarGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
                }
            }
            StarProgressCompleteContainer.SetActive(inventory_stars >= GameConfiguration.Instance.ProfileStars[GameConfiguration.Instance.ProfileStars.Count - 1].max_value);
            StarProgressContainer.SetActive(inventory_stars < GameConfiguration.Instance.ProfileStars[GameConfiguration.Instance.ProfileStars.Count - 1].max_value);
        }

        void SetProphileLoliipops(ProfileTier profile_tier)
        {
            trophiesMainContainer.SetActive(false);
            starsMainContainer.SetActive(false);
            lollipopMainContainer.SetActive(true);

            lollipopBg.sprite = UserDataManager.Instance.IsDarkModeOn() ? darkmodeBg : completedBg;

            float progress_size = profile_tier.max_value / (float)profile_tier.max_value;
            profile_lollipop.GetComponent<Text>().text = profile_tier.max_value.ToString() + "/" + profile_tier.max_value.ToString();
            // profile_lollipop.gameObject.SetActive(false);
            lollipopProgressBar.SetValue(progress_size);
            LollipopGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
            LollipopProgressCompleteContainer.SetActive(false);
            LollipopProgressContainer.SetActive(true);
        }
        void SetProphileLoliipops(int inventory_lollipop)
        {
            trophiesMainContainer.SetActive(false);
            starsMainContainer.SetActive(false);
            lollipopMainContainer.SetActive(true);
            ProfileTier profile_tier;

            lollipopBg.sprite = UserDataManager.Instance.IsDarkModeOn() ? darkmodeBg : inCompletedBg;
            for (int i = 0; i < GameConfiguration.Instance.ProfileLollipops.Count; i++)
            {
                profile_tier = GameConfiguration.Instance.ProfileLollipops[i];
                if (inventory_lollipop >= profile_tier.min_value && inventory_lollipop < profile_tier.max_value)
                {
                    float progress_size = inventory_lollipop / (float)profile_tier.max_value;
                    profile_lollipop.GetComponent<Text>().text = inventory_lollipop.ToString() + "/" + profile_tier.max_value.ToString();
                    lollipopProgressBar.SetValue(progress_size);
                    LollipopGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
                }
            }
            LollipopProgressCompleteContainer.SetActive(inventory_lollipop >= GameConfiguration.Instance.ProfileLollipops[GameConfiguration.Instance.ProfileLollipops.Count - 1].max_value);
            LollipopProgressContainer.SetActive(inventory_lollipop < GameConfiguration.Instance.ProfileLollipops[GameConfiguration.Instance.ProfileLollipops.Count - 1].max_value);
        }
    }
}