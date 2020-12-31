using BizzyBeeGames.DotConnect;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BizzyBeeGames
{
    public class ChallengeRetryScreen : Popup
    {
        [SerializeField] private Animator RetryButtonAnim = null;

        private void OnEnable()
        {
            SoundManager.Instance.Play("LevelRetryPopup");
            InventoryManager.Instance.UpdateInventory(InventoryManager.Key_InventoryTrophies, GameConfiguration.Instance.FailTrophyChallenge);
            RetryButtonAnim.SetBool("focus", true);
        }
        public void onRetryButton()
        {
            Hide(true);
            GameManager.Instance.NextLevel();
        }
        public void ClosePopup()
        {
            Hide(true);
            LoadingManager.Instance.LoadScene(LoadingManager.StartScreen);
        }
    }
}
