using System;
using System.Collections;
using System.Collections.Generic;
using BizzyBeeGames.DotConnect;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace BizzyBeeGames
{
    public class SocialManager : SingletonComponent<SocialManager>
    {
        public bool isLoggedIn
        {
            get
            {
#if UNITY_EDITOR
                return true;
#endif
                if (!isPlatformActivated || platform == null)
                    return false;
                return platform.localUser.authenticated;
            }
            private set { }
        }

        private PlayGamesPlatform platform;
        private bool isPlatformActivated = false;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);

            if (platform == null)
            {

                PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                       // enables saving game progress.
                       //.EnableSavedGames()
                       // requests the email address of the player be available.
                       // Will bring up a prompt for consent.
                       //.RequestEmail()
                       // requests a server auth code be generated so it can be passed to an
                       //  associated back end server application and exchanged for an OAuth token.
                       //.RequestServerAuthCode(false)
                       // requests an ID token be generated.  This OAuth token can be used to
                       //  identify the player to other services such as Firebase.
                       //.RequestIdToken()
                       .Build();

                PlayGamesPlatform.InitializeInstance(config);
                // recommended for debugging:
                PlayGamesPlatform.DebugLogEnabled = true;
                // Activate the Google Play Games platform
                platform = PlayGamesPlatform.Activate();
                isPlatformActivated = true;
            }

            if (PlayerPrefs.GetInt("signed_out", 0) == 0)
            {
                SignInPlayer();
            }
        }

        private void Start()
        {
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.RegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }

        private void OnDestroy()
        {
            UpdateLatestScoreToLeaderboard(null);
            if (GameEventManager.Instance == null) { return; }
            GameEventManager.Instance.UnRegisterEventHandler(GameEventManager.EventId_InventoryUpdated, OnInventoryUpdate);
        }

        public void UpdateLatestScoreToLeaderboard(Action<bool> callback)
        {
            InventoryManager inventory = InventoryManager.Instance;

            Social.ReportScore(inventory.GetInventory(InventoryManager.Key_InventoryTrophies), PlayServiceIds.leaderboard_trophy, (bool s1) =>
               {
                   Social.ReportScore(inventory.GetInventory(InventoryManager.Key_InventoryStars), PlayServiceIds.leaderboard_stars, (bool s2) =>
                      {
                          Social.ReportScore(inventory.GetInventory(InventoryManager.Key_InventoryLollipops), PlayServiceIds.leaderboard_lollipop,
                          (bool s3) =>
                          {
                              callback.Invoke(s1 && s2 && s3);
                          });
                      });
               });
        }

        public void SignInPlayer(Action<bool> callback = null)
        {
            PlayerPrefs.SetInt("signed_out", 0);
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
            {
                if (callback != null)
                    callback.Invoke(result == SignInStatus.Success);
                if (result == SignInStatus.Success)
                {
                    ((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.TOP);
                    PlayerPrefs.SetString("username", platform.localUser.userName);
                }
            });
        }

        public void UnlockAchievement(string id, float percent = 100f)
        {
            if (!isLoggedIn)
                return;
            Social.ReportProgress(id, percent, (bool success) =>
            {
            });
        }
        public void IncrementAchievement(string id, int increment = 1)
        {
            if (!isLoggedIn)
                return;
            PlayGamesPlatform.Instance.IncrementAchievement(id, increment, (bool success) => { });
        }

        private void OnInventoryUpdate(string eventId, object[] data)
        {
            if (!isLoggedIn)
                return;
            switch ((string)data[0])
            {
                case InventoryManager.Key_InventoryTrophies:
                    CheckTrophyAchiement();
                    break;
                case InventoryManager.Key_InventoryStars:
                    CheckStarAchiement();
                    break;
                case InventoryManager.Key_InventoryLollipops:
                    CheckLollipopAchiement();
                    break;
            }
        }

        private void CheckLollipopAchiement()
        {
            if (!isLoggedIn)
                return;
            int lollipopCount = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryLollipops);
            IncrementAchievement(PlayServiceIds.achievement_collect_500_lollipops, lollipopCount);
        }

        private void CheckStarAchiement()
        {
            if (!isLoggedIn)
                return;
            int starCount = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryStars);
            IncrementAchievement(PlayServiceIds.achievement_collect_1000_stars, starCount);
        }

        private void CheckTrophyAchiement()
        {
            if (!isLoggedIn)
                return;
            int trophyCount = InventoryManager.Instance.GetInventory(InventoryManager.Key_InventoryTrophies);
            if (trophyCount >= 50)
                UnlockAchievement(PlayServiceIds.achievement_collect_50_trophy);
            if (trophyCount >= 100)
                UnlockAchievement(PlayServiceIds.achievement_collect_100_trophy);
            if (trophyCount >= 200)
                UnlockAchievement(PlayServiceIds.achievement_collect_200_trophy);
            if (trophyCount >= 500)
                UnlockAchievement(PlayServiceIds.achievement_collect_500_trophy);
            if (trophyCount >= 750)
                UnlockAchievement(PlayServiceIds.achievement_collect_750_trophy);
            if (trophyCount >= 1000)
                UnlockAchievement(PlayServiceIds.achievement_collect_1000_trophy);
            if (trophyCount >= 2000)
                UnlockAchievement(PlayServiceIds.achievement_collect_2000_trophy);
            if (trophyCount >= 5000)
                UnlockAchievement(PlayServiceIds.achievement_collect_5000_trophy);
        }


        public void ReportScoreToLeadeboard(string id, int score, Action<bool> callback)
        {
            Social.ReportScore(score, id, callback != null ? callback : (bool success) =>
               {
                   Toast.Show("ScoreReport: " + success);
               });
        }

        public void GetLeaderboard(string id, Action<LeaderboardScoreData> callback)
        {
            platform.LoadScores(
           id,
           LeaderboardStart.PlayerCentered,
           10,
           LeaderboardCollection.Public,
           LeaderboardTimeSpan.AllTime,
           callback);
        }
        public void GetLeaderboard(ScorePageToken token, Action<LeaderboardScoreData> callback)
        {
            platform.LoadMoreScores(token, 10, callback);
        }

        public void LoadFriends(Action<IUserProfile[]> callback)
        {
            Social.localUser.LoadFriends(success =>
            {
                if (success)
                {
                    callback.Invoke(Social.localUser.friends);
                }
                else
                {
                    if (PlayGamesPlatform.Instance.GetLastLoadFriendsStatus() == LoadFriendsStatus.ResolutionRequired)
                    {
                        PlayGamesPlatform.Instance.AskForLoadFriendsResolution((result) =>
                        {
                            if (result == UIStatus.Valid)
                            {
                                Social.localUser.LoadFriends(
                                    (s) =>
                                    {
                                        if (s)
                                        {
                                            callback.Invoke(Social.localUser.friends);
                                        }
                                        else
                                        {
                                            Toast.Show("Unable to load friends");
                                            callback.Invoke(null);
                                        }
                                    });
                            }
                            else
                            {
                                Toast.Show("Permission denied");
                                callback.Invoke(null);
                            }
                        });
                    }
                    else
                    {
                        Toast.Show("Unable to load friends");
                        callback.Invoke(null);
                    }
                }
            });
        }

        public void SignOut()
        {
            PlayGamesPlatform.Instance.SignOut();
            isLoggedIn = false;
            PlayerPrefs.SetInt("signed_out", 1);
        }

    }

}
