using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace BizzyBeeGames
{
    public class MyTimer : SingletonComponent<MyTimer>
    {
        public Text moveAmountText;

        public Text TimeAmountText;
        public int maxMovePerLevel;
        public int CurrentMoveCount;
        public AudioSource countdownAudioArray;
        public float elapsed;
        public float startTime;
        public int countDown;
        public int minTime;
        public bool isCountDown = true;
        public List<Image> ProgressTimerStars;
        public Sprite grayStar;
        public Sprite goldStar;
        public int StarsEarned;
        public FailedType failedType;
        public System.Action<bool> OnTimerComplete;
        bool SoundStarted = false;

        void Start()
        {

        }

        void Update()
        {
            if (!isCountDown) return;

            elapsed -= Time.deltaTime;
            countDown = Mathf.CeilToInt(elapsed);
            if (countDown <= minTime && SoundStarted==false)
            {
                Debug.Log("playing sound");
                SoundManager.Instance.Play("Time-Tic");
                SoundStarted = true;
            }

            // Debug.LogError("Elapsed Time ::" + elapsed);
            string r = "";
            r += ((int)countDown / 60).ToString("00") + ":";

            //Seconds

            r += (countDown % 60).ToString("00") + "";

           
            if (elapsed <= 0)
            {
                isCountDown = false;
                failedType = FailedType.Time;
                if (OnTimerComplete != null)
                {
                    OnTimerComplete(isCountDown);
                }
                StopSound();
            }
            if (countDown != 0)
            {
                if (countDown / startTime >= .60f)
                {
                    ProgressTimerStars[0].sprite = goldStar;
                    ProgressTimerStars[1].sprite = goldStar;
                    ProgressTimerStars[2].sprite = goldStar;
                    StarsEarned = 3;
                }
                else if (countDown / startTime < .60f && countDown / startTime >= .4f)
                {
                    ProgressTimerStars[0].sprite = grayStar;
                    ProgressTimerStars[1].sprite = goldStar;
                    ProgressTimerStars[2].sprite = goldStar;
                    StarsEarned = 2;
                }
                else if (countDown / startTime < .4f && countDown / startTime >= .05f)
                {
                    ProgressTimerStars[0].sprite = grayStar;
                    ProgressTimerStars[1].sprite = grayStar;
                    ProgressTimerStars[2].sprite = goldStar;
                    StarsEarned = 1;
                }
                else
                {
                    ProgressTimerStars[0].sprite = grayStar;
                    ProgressTimerStars[1].sprite = grayStar;
                    ProgressTimerStars[2].sprite = grayStar;

                    StarsEarned = 0;
                }
            }
        }

        public ulong CurrentTimeInMilliSeconds()
        {
            return (ulong)DateTime.Now.Ticks;
        }
        public void StopSound()
        {

            SoundManager.Instance.Stop("Time-Tic");
            SoundStarted = false;
        }
        public void AddTimer(int Amount)
        {
            elapsed += Amount;
            StopSound();
        }
        public void addMovesAgain(int amt)
        {
            CurrentMoveCount -= amt;
            failedType = FailedType.None;
            moveAmountText.text = (maxMovePerLevel - CurrentMoveCount).ToString();
            MyTimer.Instance.isCountDown = true;
        }
        public void ResetMoves(int MaxVAl)
        {
            CurrentMoveCount = 0;
            maxMovePerLevel = MaxVAl;
            moveAmountText.text = (maxMovePerLevel - CurrentMoveCount).ToString();
        }
        public void ReduceNumMoves()
        {
            if (CurrentMoveCount < maxMovePerLevel-1)
            {
                CurrentMoveCount++;
                moveAmountText.text = (maxMovePerLevel - CurrentMoveCount).ToString();
            }
            else
            {
                failedType = FailedType.Move;
                CurrentMoveCount++;
                moveAmountText.text = (maxMovePerLevel - CurrentMoveCount).ToString();

                isCountDown = false;

                if (OnTimerComplete != null)
                {
                    OnTimerComplete(isCountDown);
                }
            }
        }
    }
}
public enum FailedType
{
    Move,
    Time,
    None
}