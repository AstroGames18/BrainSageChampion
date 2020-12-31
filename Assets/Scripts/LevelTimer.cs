using BizzyBeeGames.DotConnect;
using UnityEngine;

namespace BizzyBeeGames
{
    public class LevelTimer : MonoBehaviour
    {
        public void onLevelTimerAnimationComplete()
        {
            SoundManager.Instance.Play("LevelTimerFocus");
            FocusOnTimer(3f);
        }

        public void FocusOnTimer(float delay_time, bool low_on_time = false)
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null) { anim.SetBool("focus", true); }

            StartCoroutine(Utils.ExecuteAfterDelay(delay_time, (args) =>
            {
                SoundManager.Instance.Stop("LevelTimerFocus");
                anim.SetBool("focus", false);
            }));
        }
    }
}