using BizzyBeeGames.DotConnect;
using UnityEngine;

namespace BizzyBeeGames
{
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] GameManager manager;
        public void onLevelTimerAnimationComplete()
        {
            SoundManager.Instance.Play("LevelTimerFocus");
            if (manager)
                manager.isPlaying = true;

            Debug.Log(manager.isPlaying);
            FocusOnTimer(3f);
        }

        public void FocusOnTimer(float delay_time, bool low_on_time = false)
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null) { anim.SetBool("focus", true); }

            StartCoroutine(Utils.ExecuteAfterDelay(delay_time, (args) =>
            {
                if (manager)
                    manager.isPlaying = false;
                SoundManager.Instance.Stop("LevelTimerFocus");
                anim.SetBool("focus", false);
            }));
        }
    }
}