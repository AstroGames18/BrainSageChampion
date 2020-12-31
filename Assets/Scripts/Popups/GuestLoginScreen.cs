using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class GuestLoginScreen : Popup
    {
        [SerializeField] GameObject user_name, button;
        [SerializeField] Sprite grey_button, green_button;
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            if (user_name.GetComponent<InputField>().text.Length > 3 && user_name.GetComponent<InputField>().text.Length < 9)
            {
                button.GetComponent<Image>().sprite = green_button;
                button.GetComponent<Button>().enabled = true;
            }
            else
            {
                button.GetComponent<Image>().sprite = grey_button;
                button.GetComponent<Button>().enabled = false;
            }
        }
        public void close_popup()
        {
            if (!PlayerPrefs.HasKey("username"))
            {
                PlayerPrefs.SetString("username", user_name.GetComponent<InputField>().text);
            }
            Hide(true);
        }
    }
}
