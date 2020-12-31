using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Background : MonoBehaviour
{
    [SerializeField] Sprite background;

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.gameObject.GetComponent<Image>().enabled)
        {
            this.gameObject.GetComponent<Image>().enabled = true;
            this.gameObject.GetComponent<Image>().sprite = background;
        }
        else
        {
            this.gameObject.GetComponent<Image>().enabled = false;
        }
    }
}