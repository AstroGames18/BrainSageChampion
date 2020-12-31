using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace BizzyBeeGames
{
    public class LoadingManager : SingletonComponent<LoadingManager>
    {
        public const int StartScreen = 0;
        public const int GameScreen = 1;
        public const int TOKGameScreen = 2;
        public bool isLoading = false;
        public int currentScreen = 0;
        [SerializeField] bool isOnEnterAnimated;
        [SerializeField] bool isOnExitAnimated;
        [SerializeField] List<RectTransform> SwipeUpList = new List<RectTransform>();
        [SerializeField] List<RectTransform> SwipeLeftList = new List<RectTransform>();
        [SerializeField] List<RectTransform> SwipeRightList = new List<RectTransform>();
        [SerializeField] List<RectTransform> SwipeDownList = new List<RectTransform>();
        [SerializeField] List<GameObject> objectsToHideOnExit = new List<GameObject>();
        [SerializeField] float animEnterDuraion = 1.5f;
        [SerializeField] float animExitDuraion = 1.5f;
        [SerializeField] GameObject LoadingIcon = null;

        float verticalOffset = 2000f;
        float horizontalOffset = 400f;

        protected override void Awake()
        {
            base.Awake();
            LoadingIcon.SetActive(false);
            isLoading = false;
            if (isOnEnterAnimated)
            {
                // Animate all game objects as it enters 
                for (int i = 0; i < SwipeUpList.Count; i++)
                {
                    float yAxis = SwipeUpList[i].anchoredPosition.y;
                    Utils.DoSwipeVerticalAnimation(SwipeUpList[i], verticalOffset, yAxis, animEnterDuraion, 0f);
                }
                for (int i = 0; i < SwipeDownList.Count; i++)
                {
                    float yAxis = SwipeDownList[i].anchoredPosition.y;
                    Utils.DoSwipeVerticalAnimation(SwipeDownList[i], -verticalOffset, yAxis, animEnterDuraion, 0f);
                }
                for (int i = 0; i < SwipeLeftList.Count; i++)
                {
                    float xAxis = SwipeLeftList[i].anchoredPosition.x;
                    Utils.DoSwipeHorizontalAnimation(SwipeLeftList[i], -horizontalOffset, xAxis, animEnterDuraion, 0f);
                }
                for (int i = 0; i < SwipeRightList.Count; i++)
                {
                    float xAxis = SwipeRightList[i].anchoredPosition.x;
                    Utils.DoSwipeHorizontalAnimation(SwipeRightList[i], horizontalOffset, xAxis, animEnterDuraion, 0f);
                }
            }
        }
        public void ReloadScene()
        {
            UserDataManager.Instance.SetData("reloaded", 1);
            LoadScene(currentScreen);
        }
        public void LoadScene(int scene_index)
        {
            PopupManager.Instance.CloseAllActivePopup();
            currentScreen = scene_index;
            LoadingIcon.SetActive(true);
            isLoading = true;
            if (isOnExitAnimated)
            {
                // Animate all game objects as it exits 
                for (int i = 0; i < SwipeUpList.Count; i++)
                {
                    float yAxis = SwipeUpList[i].anchoredPosition.y;
                    Utils.DoSwipeVerticalAnimation(SwipeUpList[i], 0f, yAxis + verticalOffset, animExitDuraion, 0f);
                }
                for (int i = 0; i < SwipeDownList.Count; i++)
                {
                    float yAxis = SwipeDownList[i].anchoredPosition.y;
                    Utils.DoSwipeVerticalAnimation(SwipeDownList[i], 0f, yAxis - verticalOffset, animExitDuraion, 0f);
                }
                for (int i = 0; i < SwipeLeftList.Count; i++)
                {
                    float xAxis = SwipeLeftList[i].anchoredPosition.x;
                    Utils.DoSwipeHorizontalAnimation(SwipeLeftList[i], 0f, xAxis - horizontalOffset, animEnterDuraion, 0f);
                }
                for (int i = 0; i < SwipeRightList.Count; i++)
                {
                    float xAxis = SwipeRightList[i].anchoredPosition.x;
                    Utils.DoSwipeHorizontalAnimation(SwipeRightList[i], 0f, xAxis + horizontalOffset, animEnterDuraion, 0f);
                }
                for (int i = 0; i < objectsToHideOnExit.Count; i++)
                {
                    objectsToHideOnExit[i].SetActive(false);
                }
            }

            StartCoroutine(LoadAsynchronously(scene_index));
        }

        IEnumerator LoadAsynchronously(int scene_index)
        {
            yield return new WaitForSeconds(2.0f);
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene_index);
        }
    }
}
