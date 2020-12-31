using UnityEngine;

public class QuitScreen : MonoBehaviour
{
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
