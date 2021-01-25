using BizzyBeeGames;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MiniGameChecker : MonoBehaviour
{
    [SerializeField] AnimatedScrollbar StarProgressBar;
    [SerializeField] GameObject display_collected_stars;
    [SerializeField] GameObject reward_image, CardAnimatedShine;
    [SerializeField] Sprite CompleteProgressBG, IncompleteProgressBG;
    [SerializeField] Image Background;

    // DarkMode Image Changes
    [SerializeField] Sprite DarkBackground = null;
    private int max_stars;

    private void Start()
    {
        max_stars = GameConfiguration.Instance.MiniGameMaxStars;
        UpdateStarsInfoDisplay();
    }
    public void claim()
    {
        PopupManager.Instance.Show("MiniGamePopup");
    }
    void UpdateStarsInfoDisplay()
    {
        int stars = Math.Min(UserDataManager.Instance.GetData("mini_game_stars"), max_stars);
        if (StarProgressBar)
            StarProgressBar.SetValue(stars / (float)max_stars);
        CardAnimatedShine.SetActive(stars >= max_stars);
        if (stars >= max_stars) { Background.sprite = CompleteProgressBG; }
        else { Background.sprite = IncompleteProgressBG; }
        if (UserDataManager.Instance.IsDarkModeOn()) { Background.sprite = DarkBackground; }
        display_collected_stars.GetComponent<Text>().text = stars.ToString() + "/" + max_stars.ToString();
    }
}


