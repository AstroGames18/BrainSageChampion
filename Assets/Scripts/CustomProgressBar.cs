using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class CustomProgressBar : MonoBehaviour
    {
        public float progress_full = 1.0f;
        public float progress_high = 0.55f;
        public float progress_middle = 0.38f;
        public float progress_low = 0.181f;
        public float progress_empty = 0.0f;

        private Image progress_image;
        private float curr_fill_value = 0.0f;

        void Start()
        {
            progress_image = this.gameObject.GetComponent<Image>();
            progress_image.fillAmount = curr_fill_value;
            SetFillValue(0.0f, progress_low, progress_empty);
        }

        public void ResetValue()
        {
            curr_fill_value = 0.0f;
            progress_image.fillAmount = curr_fill_value;
        }

        public void SetFillValue(float value, float range_high, float range_low)
        {
            float range_gap = 1.0f - range_low;
            float val = range_high - value * range_gap;

            val = Mathf.Max(range_low, val);
            val = Mathf.Min(range_high, val);

            curr_fill_value = val;
        }

        public float GetFillValue()
        {
            return curr_fill_value;
        }

        void UpdateFill()
        {
            progress_image.fillAmount = Mathf.Lerp(
                progress_image.fillAmount,
                curr_fill_value,
                3 * Time.deltaTime);
        }

        void Update()
        {
            if (curr_fill_value != progress_image.fillAmount)
            {
                UpdateFill();
            }
        }
    }
}
