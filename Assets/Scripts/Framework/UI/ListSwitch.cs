using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListSwitch : MonoBehaviour
{
    [System.Serializable]
     class ListInfo
    {
        public string ListName = "";
    }
    public ScrollRect ListView;
    public Text ListName;
    public Button LeftButton;
    public Button RightButton;
    [SerializeField] private List<ListInfo> listInfos = new List<ListInfo>();
    // Start is called before the first frame update

    int ListPos = 0;
    void Start()
    {
        ShowList(ListPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LeftButtonClick()
    {

        ListPos--;
        if (ListPos < 0)
        {
            ListPos = listInfos.Count-1;
        }
        //Debug.LogError("Position is " + ListPos);
        ShowList(ListPos);
    }
    public void RightButtonClick()
    {
        ListPos++;

        if (ListPos >= listInfos.Count)
        {
            ListPos =0 ;
        }
        ShowList(ListPos);
    }
     void ShowList(int pos)
    {
        ListName.text = listInfos[pos].ListName;
    }
}
