using BizzyBeeGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EscapeHandler : MonoBehaviour
{

    public UnityEvent espcapeEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PopupManager.Instance.Showing())
            {
                PopupManager.Instance.CloseActivePopup();
                return;
            }

            espcapeEvent.Invoke();
        }
    }
}