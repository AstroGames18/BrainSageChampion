using BizzyBeeGames.DotConnect;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class LevelIntroduction : Popup
    {
        [SerializeField] Text LevelTitle;
        [SerializeField] AnimatedText LevelProgress;
        [SerializeField] AnimatedScrollbar progressFill;

        private void OnEnable()
        {
            int level_hardness = GameManager.Instance.ActiveLevelData.level_hardness;
            LevelTitle.text = "Level - " + (GameManager.Instance.ActiveLevelData.LevelIndex + 1);
            LevelProgress.SetValue(level_hardness, "", "%");
            progressFill.SetValue(level_hardness / 100f);
            StartCoroutine(Utils.ExecuteAfterDelay(1.5f, (args1) =>
            {
                closePopup();
            }));
            SoundManager.Instance.Play("LevelMessageAppear");
        }
        private void closePopup()
        {
            SoundManager.Instance.Play("LevelMessageDissappear");
            Hide(true);
            GameManager.Instance.StartGamePlayAnimation();
        }
    }
}
