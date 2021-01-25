using System;
using System.Collections.Generic;
using BizzyBeeGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DarkModeTexthandler : MonoBehaviour
{
    [SerializeField] List<TextColor> textColors = new List<TextColor>();
    [SerializeField] List<Text> removeOutline = new List<Text>();

    [SerializeField] List<Image> addBlackOverlay = new List<Image>();

    private void Start()
    {

        if (UserDataManager.Instance.IsDarkModeOn())
        {
            if (textColors != null && textColors.Count > 0)
                foreach (TextColor textColor in textColors)
                {
                    if (textColor != null && textColor.texts != null && textColor.texts.Count > 0)
                        foreach (Text text in textColor.texts)
                        {
                            if (text)
                            {
                                Outline outline = text.gameObject.GetComponent<Outline>();
                                if (outline)
                                    outline.enabled = false;
                                text.color = textColor.color;
                            }
                        }
                    if (textColor != null && textColor.textMeshes != null && textColor.textMeshes.Count > 0)
                        foreach (TextMeshProUGUI text in textColor.textMeshes)
                        {
                            text.color = textColor.color;
                        }
                }

            if (removeOutline != null && removeOutline.Count > 0)
                foreach (Text text in removeOutline)
                {
                    Outline outline = text.gameObject.GetComponent<Outline>();
                    if (outline)
                        outline.enabled = false;
                }

            if (addBlackOverlay != null && addBlackOverlay.Count > 0)
                foreach (Image image in addBlackOverlay)
                {
                    image.color = new Color(0.1f, 0.1f, 0.1f);
                }
        }
    }

    [Serializable]
    class TextColor
    {
        public Color color = Color.white;
        public List<Text> texts = new List<Text>();
        public List<TextMeshProUGUI> textMeshes = new List<TextMeshProUGUI>();
    }
}
