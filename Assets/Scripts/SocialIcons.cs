using BizzyBeeGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialIcons : MonoBehaviour
{

    int index = 0;
    public List<GameObject> objects;

    public void OnFlip()
    {
        if (LoadingManager.Instance.isLoading)
            return;
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(index == i);
        }
        index++;
        if (index >= objects.Count)
            index = 0;
    }
}
