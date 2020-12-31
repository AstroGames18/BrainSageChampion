using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioButton : MonoBehaviour
{
    [SerializeField] GameObject SelectedImage;

    public void SetValue(bool enable)
    {
        SelectedImage.SetActive(enable);
    }
}
