using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimatedText : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] bool isTextMeshPro = false;

    private int currentValue = 0;
    private float lerpDuration = 1f;
    private float valueToLerp;
    private string preValue = "";
    private string postValue = "";

    private System.Action callbackAction = null;
    public void SetValue(int valTo, string preVal = "", string postVal = "", System.Action action = null)
    {
        preValue = preVal;
        postValue = postVal;
        StartCoroutine(UpdateText(valTo));
        callbackAction = action;
    }

    public void ResetValue()
    {
        currentValue = 0;
        gameObject.GetComponent<Text>().text = "0";
    }

    IEnumerator UpdateText(int valTo)
    {
        float timeElapsed = 0;
        float startValue = currentValue;
        float endValue = valTo;
        Text labelText = gameObject.GetComponent<Text>();
        TextMeshProUGUI labelTextPro = gameObject.GetComponent<TextMeshProUGUI>();
        setContainerAnim(true);

        if (endValue <= 0) { timeElapsed = lerpDuration; }
        while (timeElapsed < lerpDuration)
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            if (isTextMeshPro) { labelTextPro.text = preValue + Mathf.FloorToInt(valueToLerp).ToString() + postValue; }
            else { labelText.text = preValue + Mathf.FloorToInt(valueToLerp).ToString() + postValue; }
            yield return null;
        }

        valueToLerp = endValue;
        if (isTextMeshPro) { labelTextPro.text = preValue + ((int)valueToLerp).ToString() + postValue; }
        else { labelText.text = preValue + ((int)valueToLerp).ToString() + postValue; }
        if (valueToLerp == endValue && callbackAction != null) { callbackAction(); }
        currentValue = (int)endValue;
    }
    public void setContainerAnim(bool status)
    {
        if (container != null)
        {
            Animator anim = container.GetComponent<Animator>();
            if (anim != null) { anim.SetBool("appear", status); }
        }
    }
}
