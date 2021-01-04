using BizzyBeeGames;
using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class ProfileScreen : Popup
    {
        [Header("Popup Settings")]
        [SerializeField] GameObject[] medal_images;
        [SerializeField] GameObject profile_trophies, profile_stars, profile_lollipop, user_trophies, user_stars, user_lollipops, user_name;
        [SerializeField] GameObject[] medal_tag;
        [SerializeField] GameObject[] numbers;
        [SerializeField] Image[] medalImage;
        [SerializeField] GameObject StarProgressContainer, StarProgressCompleteContainer, TrophyProgressContainer, TrophyProgressCompleteContainer, LollipopProgressContainer, LollipopProgressCompleteContainer;
        [SerializeField] AnimatedScrollbar trophyProgressBar, starProgressBar, lollipopProgressBar, brainActivityProgressBar;
        [SerializeField] Image TrophyGiftImage, StarGiftImage, LollipopGiftImage;
        [SerializeField] Text QuestLevel, brain_activity_percentage_text;
        private List<GiftToReward> allProfileGifts = new List<GiftToReward>();

        //DarkMode Assets
        [SerializeField] Image[] MediumCards, SmallerCards;

        void OnEnable()
        {
            allProfileGifts.Clear();
            display();
            UserDataManager.Instance.SetData("notification_profilescreen", 0);
            GameConfiguration.Instance.SetDarkModeOnCards(MediumCards, SmallerCards);
        }

        public void display()
        {
            int inventory_trophies = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryTrophies);
            int inventory_stars = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryStars);
            int inventory_lollipops = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryLollipops);

            user_trophies.GetComponent<Text>().text = inventory_trophies.ToString();
            user_stars.GetComponent<Text>().text = inventory_stars.ToString();
            user_lollipops.GetComponent<Text>().text = inventory_lollipops.ToString();
            user_name.SetActive(PlayerPrefs.HasKey("username"));
            user_name.GetComponent<Text>().text = PlayerPrefs.GetString("username");

            SetProphileTrophies(inventory_trophies);
            SetProphileStars(inventory_stars);
            SetProphileLoliipops(inventory_lollipops);
            SetBrainActivity();
            QuestLevel.text = /*LeanLocalization.GetTranslationText("QuestLevel") + " - " +*/ (UserDataManager.Instance.GetData("quest_level") + 1).ToString();
        }
        void SetProphileTrophies(int inventory_trophies)
        {
            ProfileTier profile_tier;
            int max = 0;
            bool hasMedal = false;
            for (int i = 0; i < GameConfiguration.Instance.ProfileTrophies.Count; i++)
            {
                profile_tier = GameConfiguration.Instance.ProfileTrophies[i];
                if (inventory_trophies >= profile_tier.min_value && inventory_trophies < profile_tier.max_value)
                {
                    float progress_size = inventory_trophies / (float)profile_tier.max_value;
                    medal_images[0].GetComponent<Image>().sprite = GameConfiguration.Instance.GetMedalImage(profile_tier.badge);
                    profile_trophies.GetComponent<Text>().text = inventory_trophies.ToString() + "/" + profile_tier.max_value.ToString();
                    trophyProgressBar.SetValue(progress_size);
                    TrophyGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
                }

                if (profile_tier.max_value > max && inventory_trophies >= profile_tier.max_value)
                {
                    hasMedal = true;
                    max = profile_tier.max_value;
                    numbers[0].GetComponent<Text>().text = profile_tier.medal_number.ToString();
                    //  medal_tag[0].GetComponent<Text>().text = Lean.Localization.LeanLocalization.GetTranslationText(profile_tier.medal_tag);
                }
            }
            if (!hasMedal)
            {
                numbers[0].SetActive(false);
                medal_tag[0].SetActive(false);
                var tempColor = medalImage[0].color;
                tempColor.r = 0.4f;
                tempColor.g = 0.4f;
                tempColor.b = 0.4f;
                tempColor.a = 0.4f;
                medalImage[0].color = tempColor;
                //medalImage[0].GetComponent<Image>().color = new Color(100, 100, 100, 100);
            }
            TrophyProgressCompleteContainer.SetActive(inventory_trophies >= GameConfiguration.Instance.ProfileTrophies[GameConfiguration.Instance.ProfileTrophies.Count - 1].max_value);
            TrophyProgressContainer.SetActive(inventory_trophies < GameConfiguration.Instance.ProfileTrophies[GameConfiguration.Instance.ProfileTrophies.Count - 1].max_value);
        }
        void SetProphileStars(int inventory_stars)
        {
            ProfileTier profile_tier;

            int max = 0;
            bool hasMedal = false;
            for (int i = 0; i < GameConfiguration.Instance.ProfileStars.Count; i++)
            {
                profile_tier = GameConfiguration.Instance.ProfileStars[i];
                if (inventory_stars >= profile_tier.min_value && inventory_stars < profile_tier.max_value)
                {
                    float progress_size = inventory_stars / (float)profile_tier.max_value;

                    medal_images[1].GetComponent<Image>().sprite = GameConfiguration.Instance.GetMedalImage(profile_tier.badge);
                    profile_stars.GetComponent<Text>().text = inventory_stars.ToString() + "/" + profile_tier.max_value.ToString();
                    numbers[1].GetComponent<Text>().text = profile_tier.medal_number.ToString();
                    // medal_tag[1].GetComponent<Text>().text = Lean.Localization.LeanLocalization.GetTranslationText(profile_tier.medal_tag);
                    starProgressBar.SetValue(progress_size);
                    StarGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
                }
                if (profile_tier.max_value > max && inventory_stars >= profile_tier.max_value)
                {
                    hasMedal = true;
                    max = profile_tier.max_value;
                    numbers[1].GetComponent<Text>().text = profile_tier.medal_number.ToString();
                    // medal_tag[1].GetComponent<Text>().text = Lean.Localization.LeanLocalization.GetTranslationText(profile_tier.medal_tag);
                }
            }
            if (!hasMedal)
            {
                numbers[1].SetActive(false);
                medal_tag[1].SetActive(false);
                var tempColor = medalImage[1].color;
                tempColor.r = 0.4f;
                tempColor.g = 0.4f;
                tempColor.b = 0.4f;
                tempColor.a = 0.4f;
                medalImage[1].color = tempColor;
                //medalImage[1].GetComponent<Image>().color = new Color(100, 100, 100, 100);
            }
            StarProgressCompleteContainer.SetActive(inventory_stars >= GameConfiguration.Instance.ProfileStars[GameConfiguration.Instance.ProfileStars.Count - 1].max_value);
            StarProgressContainer.SetActive(inventory_stars < GameConfiguration.Instance.ProfileStars[GameConfiguration.Instance.ProfileStars.Count - 1].max_value);
        }
        void SetProphileLoliipops(int inventory_lollipop)
        {
            ProfileTier profile_tier;

            int max = 0;
            bool hasMedal = false;
            for (int i = 0; i < GameConfiguration.Instance.ProfileLollipops.Count; i++)
            {
                profile_tier = GameConfiguration.Instance.ProfileLollipops[i];
                if (inventory_lollipop >= profile_tier.min_value && inventory_lollipop < profile_tier.max_value)
                {
                    float progress_size = inventory_lollipop / (float)profile_tier.max_value;

                    medal_images[2].GetComponent<Image>().sprite = GameConfiguration.Instance.GetMedalImage(profile_tier.badge);
                    profile_lollipop.GetComponent<Text>().text = inventory_lollipop.ToString() + "/" + profile_tier.max_value.ToString();
                    numbers[2].GetComponent<Text>().text = profile_tier.medal_number.ToString();
                    // medal_tag[2].GetComponent<Text>().text = Lean.Localization.LeanLocalization.GetTranslationText(profile_tier.medal_tag);
                    lollipopProgressBar.SetValue(progress_size);
                    LollipopGiftImage.sprite = GameConfiguration.Instance.GetGiftSprite(profile_tier.reward.giftType);
                }
                if (profile_tier.max_value > max && inventory_lollipop >= profile_tier.max_value)
                {
                    hasMedal = true;
                    max = profile_tier.max_value;
                    numbers[2].GetComponent<Text>().text = profile_tier.medal_number.ToString();
                    // medal_tag[2].GetComponent<Text>().text = Lean.Localization.LeanLocalization.GetTranslationText(profile_tier.medal_tag);
                }
            }
            if (!hasMedal)
            {
                numbers[2].SetActive(false);
                medal_tag[2].SetActive(false);
                var tempColor = medalImage[2].color;
                tempColor.r = 0.4f;
                tempColor.g = 0.4f;
                tempColor.b = 0.4f;
                tempColor.a = 0.4f;
                medalImage[2].color = tempColor;
                //medalImage[2].GetComponent<Image>().color = new Color(100, 100, 100, 100);
            }
            LollipopProgressCompleteContainer.SetActive(inventory_lollipop >= GameConfiguration.Instance.ProfileLollipops[GameConfiguration.Instance.ProfileLollipops.Count - 1].max_value);
            LollipopProgressContainer.SetActive(inventory_lollipop < GameConfiguration.Instance.ProfileLollipops[GameConfiguration.Instance.ProfileLollipops.Count - 1].max_value);
        }
        void SetBrainActivity()
        {
            int curr_level = UserDataManager.Instance.GetData("current_level");
            int range = GameConfiguration.Instance.BrainActivityLevelGap;
            int min_level = curr_level <= range ? 1 : curr_level - range;

            int total_stars = GetTotalStarsinLevelRange(min_level, curr_level);
            float fill_perc = total_stars / (range * 3.0f);

            brainActivityProgressBar.SetValue(fill_perc);
            brain_activity_percentage_text.text = Mathf.Round(fill_perc * 100).ToString() + "%";
        }
        private int GetTotalStarsinLevelRange(int min_level, int max_level)
        {
            int stars = 0;
            for (int i = min_level; i <= max_level; i++)
            {
                PuzzleData data = UserDataManager.Instance.LoadPuzzleData(i);
                if (data != null)
                {
                    stars += data.stars;
                }
            }
            return stars;
        }

        public void ShowSettingsPopup()
        {
            PopupManager.Instance.Show("SettingsPopup", null, onSettingsPopupClosed);
        }
        public void onSettingsPopupClosed(bool cancelled, object[] data)
        {
            if (data != null && data.Length > 0)
            {
                string action = (string)data[0];
                if (action == "Reload")
                {
                    StartCoroutine(Utils.ExecuteAfterDelay(0.5f, (args) =>
                    {
                        Hide(true);
                        LoadingManager.Instance.ReloadScene();
                    }));

                }
            }
        }
    }
}


