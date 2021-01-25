using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] string title = "English";
    [SerializeField] Text titleText;
    [SerializeField] Image mark;

    private void Start()
    {
        titleText.text = title;
        Select();
    }
    public void Select(bool selected = false)
    {
        if (selected)
        {
            mark.enabled = true;
        }
        else
        {
            mark.enabled = false;
        }
    }

}
