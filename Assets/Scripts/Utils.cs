using BizzyBeeGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static string ConvertSecondsToString(int seconds, bool hours_needed = true)
    {
        int hours = Math.Max(0, TimeSpan.FromSeconds(seconds).Hours);
        int mins = Math.Max(0, TimeSpan.FromSeconds(seconds).Minutes);
        int secs = Math.Max(0, TimeSpan.FromSeconds(seconds).Seconds);
        string hh = hours < 10 ? "0" + hours.ToString() : hours.ToString();
        string mm = mins < 10 ? "0" + mins.ToString() : mins.ToString();
        string ss = secs < 10 ? "0" + secs.ToString() : secs.ToString();

        string final_result = hours_needed ? hh + ":" + mm + ":" + ss :
            mm + ":" + ss;

        return final_result;
    }
    public static string ConvertToTime(double timeSeconds)
    {
        int mySeconds = Convert.ToInt32(timeSeconds);

        int myHours = mySeconds / 3600; // 3600 Seconds in 1 hour
        mySeconds %= 3600;

        int myMinutes = mySeconds / 60; // 60 Seconds in a minute
        mySeconds %= 60;
        string mySec = mySeconds.ToString(),
        myMin = myMinutes.ToString(),
        myHou = myHours.ToString();

        if (myHours < 10) { myHou = myHou.Insert(0, "0"); }
        if (myMinutes < 10) { myMin = myMin.Insert(0, "0"); }
        if (mySeconds < 10) { mySec = mySec.Insert(0, "0"); }

        return myHou + ":" + myMin + ":" + mySec;
    }

    public static ProbabilityResult GetRandomNumberInWeights(List<ProbabilityData> all_data)
    {
        float total_value = 0;
        all_data = SortArrays(all_data);
        List<ProbabilityResult> all_result = new List<ProbabilityResult>();

        for (int i = 0; i < all_data.Count; i++)
        {
            ProbabilityData data = all_data[i];
            ProbabilityResult prev_result = i == 0 ? null : all_result[i - 1];
            total_value += data.percentatage;
            float min_value = i == 0 ? 0 : prev_result.max_value + 1;
            float max_value = i == 0 ? data.percentatage : prev_result.max_value + data.percentatage;
            all_result.Add(new ProbabilityResult(min_value, max_value, data.item));
        }

        int random_value = (int)UnityEngine.Random.Range(1, total_value + 1);
        random_value = (int)Math.Min(total_value, random_value);
        for (int j = 0; j < all_result.Count; j++)
        {
            ProbabilityResult result = all_result[j];
            if (random_value >= result.min_value && random_value <= result.max_value)
            {
                return result;
            }
        }
        return null;
    }

    public static int GetNonRepeatedRandomNumber(int from, int to, List<int> list)
    {
        int random = UnityEngine.Random.Range(from, to);
        while (list.Contains(random))
        {
            random = UnityEngine.Random.Range(from, to);
        }
        return random;
    }

    public static List<ProbabilityData> SortArrays(List<ProbabilityData> data)
    {
        ProbabilityData temp;
        for (int j = 0; j <= data.Count - 2; j++)
        {
            for (int i = 0; i <= data.Count - 2; i++)
            {
                if (data[i].percentatage > data[i + 1].percentatage)
                {
                    temp = data[i + 1];
                    data[i + 1] = data[i];
                    data[i] = temp;
                }
            }
        }
        return data;
    }

    public static void DoZoomAnimation(RectTransform animContainer, float animDuration, float fromVal = 0f, float toVal = 1f, float delayTime = 0f, AnimationCurve animCurve = null)
    {
        UIAnimation anim = UIAnimation.ScaleX(animContainer, fromVal, toVal, animDuration);
        if (animCurve != null)
        {
            anim.style = UIAnimation.Style.Custom;
            anim.animationCurve = animCurve;
        }
        else { anim.style = UIAnimation.Style.EaseOut; }
        anim.startOnFirstFrame = true;
        anim.startDelay = delayTime;
        anim.Play();

        anim = UIAnimation.ScaleY(animContainer, fromVal, toVal, animDuration);
        if (animCurve != null)
        {
            anim.style = UIAnimation.Style.Custom;
            anim.animationCurve = animCurve;
        }
        else { anim.style = UIAnimation.Style.EaseOut; }
        anim.startOnFirstFrame = true;
        anim.startDelay = delayTime;

        anim.Play();
    }
    public static void DoSwipeVerticalAnimation(RectTransform animContainer, float valFrom, float valTo, float animDuration, float delayTime = 0f)
    {
        UIAnimation anim = UIAnimation.PositionY(animContainer, valFrom, valTo, animDuration);
        anim.style = UIAnimation.Style.EaseOut;
        anim.startOnFirstFrame = true;
        anim.startDelay = delayTime;
        anim.Play();
    }
    public static void DoSwipeHorizontalAnimation(RectTransform animContainer, float valFrom, float valTo, float animDuration, float delayTime)
    {
        UIAnimation anim = UIAnimation.PositionX(animContainer, valFrom, valTo, animDuration);
        anim.style = UIAnimation.Style.EaseOut;
        anim.startOnFirstFrame = true;
        anim.startDelay = delayTime;
        anim.Play();
    }
    public static void DoRotateAnimation(RectTransform animContainer, float valFrom, float valTo, float animDuration, float delayTime = 0f)
    {
        UIAnimation anim = UIAnimation.RotationZ(animContainer, valFrom, valTo, animDuration);
        anim.style = UIAnimation.Style.EaseOut;
        anim.startOnFirstFrame = true;
        anim.startDelay = delayTime;
        anim.Play();
    }
    public static void DoScaleAnimation(RectTransform animContainer, float valFrom, float valTo, float animDuration, float delayTime = 0)
    {
        UIAnimation anim = UIAnimation.ScaleX(animContainer, valFrom, valTo, animDuration);
        anim.style = UIAnimation.Style.EaseOut;
        anim.startOnFirstFrame = true;
        anim.startDelay = delayTime;
        anim.Play();
    }
    public static void DoFlipAnimation(RectTransform animContainer, float valFrom, float valTo, float animDuration, float delayTime = 0)
    {
        UIAnimation anim = UIAnimation.RotationY(animContainer, valFrom, valTo, animDuration);
        anim.style = UIAnimation.Style.EaseOut;
        anim.startOnFirstFrame = true;
        anim.startDelay = delayTime;
        anim.Play();
    }
    public static IEnumerator ExecuteAfterDelay(float time, Action<object[]> action, object[] args = null)
    {
        yield return new WaitForSeconds(time);
        action(args);
    }
}
