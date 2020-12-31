using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class AnimatedScrollbar : MonoBehaviour
    {
        [SerializeField] Scrollbar ProgressFill = null;

        private float curr_fill_value = 0.0f;

        public void ResetValue()
        {
            curr_fill_value = 0.0f;
            ProgressFill.size = curr_fill_value;
        }

        public void SetValue(float value)
        {
            SoundManager.Instance.Play("ScrollBarUpdate");
            curr_fill_value = value;
        }

        public float GetFillValue()
        {
            return curr_fill_value;
        }

        void UpdateFill()
        {
            ProgressFill.size = Mathf.Lerp(
                ProgressFill.size,
                curr_fill_value,
                6 * Time.deltaTime);
        }

        void Update()
        {
            if (curr_fill_value != ProgressFill.size)
            {
                if (curr_fill_value - ProgressFill.size < 0.01)
                {
                    SoundManager.Instance.Play("ScrollBarStop");
                    ProgressFill.size = curr_fill_value;
                }
                else
                {
                    UpdateFill();
                }
            }
        }
    }
}

