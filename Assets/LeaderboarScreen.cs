using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class LeaderboarScreen : Popup
    {

        public ScrollRect scrollRect;
        public Transform container;
        public GameObject loading;
        public PlayerCard player;
        public PlayerCard cardPrefab;
        public GameObject errorContainer;
        public Text errorMessage;
        public GameObject retryButton;


        public Text userTypeText;

        private bool completed = false;
        private LeaderboardType leaderboard;
        private UserType userType;

        private ScorePageToken nextPageToken;

        private List<string> friendsIds = new List<string>();

        private int userCount = 0;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            scrollRect.onValueChanged.RemoveAllListeners();
            scrollRect.onValueChanged.AddListener(OnScroll);
            leaderboard = (LeaderboardType)inData[0];
            player.SetLoading(true);
            Reset();
            SocialManager.Instance.UpdateLatestScoreToLeaderboard(success =>
            {
                SetUserType(UserType.GLOBAL);
            });
        }

        private void Reset()
        {
            foreach (Transform child in container)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void ChangeUserType()
        {
            Reset();
            SetUserType(userType == UserType.FIRENDS ? UserType.GLOBAL : UserType.FIRENDS);
        }

        private void SetUserType(UserType type)
        {
            SetError(false);
            userType = type;
            SetLoading(true);
            completed = false;
            nextPageToken = null;
            player.SetLoading(true);
            if (userType == UserType.GLOBAL)
            {
                userTypeText.text = "Global";
                GetUsers();
            }
            else
            {
                userTypeText.text = "Friends";
                GetFriends();
            }
        }

        private void GetFriends()
        {
            friendsIds.Clear();
            SocialManager.Instance.LoadFriends(
                users =>
                {
                    if (users != null && users.Length > 0)
                    {
                        for (int i = 0; i < users.Length; i++)
                        {
                            friendsIds.Add(users[i].id);
                        }
                    }
                    GetUsers();
                }
            );
        }


        private void GetUsers()
        {
            if (nextPageToken == null)
            {
                SocialManager.Instance.GetLeaderboard(
                    GetId(),
                    (data) =>
                    {
                        ProcessUserData(data);
                    });
            }
            else
            {
                LoadMoreUser();
            }
        }

        private void ProcessUserData(LeaderboardScoreData data)
        {
            if (data.Valid && data.Status == ResponseStatus.SuccessWithStale || data.Status == ResponseStatus.Success)
            {
                if (data.Scores == null || data.Scores.Length < 1)
                {
                    completed = true;
                    nextPageToken = null;
                    SetError(true, "No Users Found!", false);
                    SetLoading(false);
                    return;
                }
                nextPageToken = data.NextPageToken;
                if (data.Scores.Length < 10)
                    nextPageToken = null;

                if (data.PlayerScore != null)
                {
                    InitializePlayer(data.PlayerScore);
                }
                Social.LoadUsers(CreateUserIdFromIScore(data.Scores), users =>
                {
                    if (users == null || users.Length < 1)
                    {
                        completed = true;
                        nextPageToken = null;
                        SetError(true, "Failed to get users info!");
                        SetLoading(false);
                        return;
                    }

                    Reset();
                    userCount = 0;
                    for (int i = 0; i < users.Length; i++)
                    {
                        if ((userType == UserType.GLOBAL) || (userType == UserType.FIRENDS && friendsIds.Contains(users[i].id)))
                        {
                            userCount++;
                            PlayerCard instance = Instantiate(cardPrefab);

                            instance.Initialize(leaderboard, users[i].userName, data.Scores[i].rank, data.Scores[i].formattedValue, users[i].image);
                            instance.SetFriend(userType == UserType.FIRENDS);
                            instance.transform.SetParent(container);
                            instance.transform.localScale = Vector3.one * 0.9f;
                        }

                    }
                    if (userCount == 0)
                    {
                        completed = true;
                        nextPageToken = null;
                        SetError(true, "No users found!");
                        SetLoading(false);
                        return;
                    }
                    if (data.Scores.Length >= 10 && userCount < 10 && nextPageToken != null)
                    {
                        GetUsers();
                        return;
                    }
                    SetLoading(false);
                });
            }
            else
            {
                SetError(true, "Failed to load leaderbord!");
                SetLoading(false);
            }
        }

        private void InitializePlayer(IScore playerData)
        {
            player.SetLoading(true);
            Social.LoadUsers(new string[] { playerData.userID },
                      users =>
                      {
                          if (users == null || users.Length < 1)
                          {
                              nextPageToken = null;
                              player.SetError(true, "Player info not found!");
                              return;
                          }

                          player.Initialize(leaderboard, users[0].userName, playerData.rank, playerData.formattedValue, users[0].image);
                          player.SetPlayer(true);
                      });
        }

        private void OnScroll(Vector2 vector)
        {
            Debug.Log(vector);
            if (!loading.activeSelf && vector.y < 0.1 && userType == UserType.GLOBAL && nextPageToken != null)
            {
                SetLoading(true);
                GetUsers();
            }
        }

        private void LoadMoreUser()
        {
            SocialManager.Instance.GetLeaderboard(nextPageToken,
            data =>
            {
                ProcessUserData(data);
            });
        }

        private void SetLoading(bool enable)
        {
            loading.SetActive(enable);
        }


        private string[] CreateUserIdFromIScore(IScore[] scores)
        {
            string[] userIds = new string[scores.Length];
            for (int i = 0; i < scores.Length; i++)
            {
                userIds[i] = scores[i].userID;
            }
            return userIds;
        }

        private string GetId()
        {
            switch (leaderboard)
            {
                case LeaderboardType.STAR:
                    return PlayServiceIds.leaderboard_stars;
                case LeaderboardType.TROPHY:
                    return PlayServiceIds.leaderboard_trophy;
                case LeaderboardType.LOLLIPOP:
                    return PlayServiceIds.leaderboard_lollipop;
            }
            return "";
        }

        public void SetError(bool enable, String message = "Something went wrong", bool showButton = true)
        {
            errorContainer.SetActive(enable);
            errorMessage.text = message;
            retryButton.SetActive(showButton);
        }
        public void Retry()
        {
            SetUserType(userType);
        }

    }



    public enum LeaderboardType
    {
        STAR,
        TROPHY,
        LOLLIPOP
    }

    public enum UserType
    {
        GLOBAL,
        FIRENDS
    }
}
