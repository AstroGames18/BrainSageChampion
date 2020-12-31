using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class ProgressHealthBar : MonoBehaviour
    {
        public Image ProgressBar;
        //public RectTransform 
        public float HealthValue = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float amount = ((MyTimer.Instance.countDown / 30.0f) * 100.0f);
            ProgressBar.fillAmount = (0.1f +  (((100.0f - amount) / 10.0f) / 10.0f));
            //Debug.LogError("countDown :" + MyTimer.Instance.countDown + "    Amount:   " + amount + " fill Amount :"+ (0.1f +  ((100.0f - amount) / 10.0f) / 10.0f));
        }
    }
}
