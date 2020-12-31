using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BizzyBeeGames
{
    public class GiftCard : MonoBehaviour
    {
        [SerializeField] Image RewardIcon = null, CardBackground = null;
        [SerializeField] TextMeshProUGUI RewardText = null;

        private float speed = 50.0f;
        private float time = 0.5f;
        private float waitTime = 0.5f;

        private Transform target;
        public bool reachedTraget;
        public void SetCard(Sprite tex, int amount, int index, string sound)
        {
            RewardText.text = "x " + amount;
            RewardIcon.sprite = tex;
            Utils.DoZoomAnimation(gameObject.transform as RectTransform, 0f, 1f, 0f);
            StartCoroutine(Utils.ExecuteAfterDelay(/*index * 0.35f*/0f, (args) =>
            {
                Utils.DoZoomAnimation(gameObject.transform as RectTransform, 0.5f, 0f, 1f);
                SoundManager.Instance.Play(sound);
            }));
            reachedTraget = false;
        }
        public void HideCardBG()
        {
            CardBackground.color = new Color(255f, 255f, 255f, 0f);
            RewardText.text = "";
        }
        public void HideIcon()
        {
            RewardIcon.color = new Color(255f, 255f, 255f, 0f);
            reachedTraget = true;
        }
        public void SetTarget(Transform t)
        {
            float distance = Vector3.Distance(transform.position, t.position);
            speed = distance / time;
            target = t;
        }
        void Update()
        {
            if (target == null) { return; }
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                return;
            }
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            if (Vector3.Distance(transform.position, target.position) < 0.001f)
            {
                target = null;
                HideIcon();
            }
        }
    }

}

