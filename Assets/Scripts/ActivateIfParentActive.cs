using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActivateIfParentActive : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.gameObject.GetComponent<Image>().enabled)
        {
            this.gameObject.GetComponent<Image>().enabled = true;
         
        }
        else {
            this.gameObject.GetComponent<Image>().enabled = false;
        }
    }
}
