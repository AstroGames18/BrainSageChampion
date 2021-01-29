using System;
using System.Collections;
using System.Collections.Generic;
using BizzyBeeGames;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Image background;
    public Image profileImage;
    public Text playerName;
    public Text playerTag;
    public Text position;
    public Image positionIcon;
    public Text score;
    public Image scoreIcon;
    public Image google;

    public Sprite backroundNormal;
    public Sprite backroundDark;
    public Sprite backroundGold;
    public Sprite backroundSilver;
    public Sprite backroundBronze;
    public Sprite posIcon1;
    public Sprite posIcon2;
    public Sprite posIcon3;
    public Sprite iconStar;
    public Sprite iconTrophy;
    public Sprite iconLollipop;
    public Sprite googleNonFriend;
    public Sprite googleFriend;

    public GameObject contents;
    public GameObject loading;
    public GameObject errorContent;
    public Text errorMessage;


    private bool isPlayer = false;

    private bool isFriend = false;

    private void OnEnable()
    {


        if (UserDataManager.Instance.IsDarkModeOn())
        {
            background.sprite = backroundDark;

            if (playerTag)
            {
                Outline outline = playerTag.gameObject.GetComponent<Outline>();
                if (outline)
                    outline.enabled = false;
                playerTag.color = Color.white;
            }
            if (score)
            {
                score.color = Color.white;
            }
        }
    }

    public void Initialize(
        LeaderboardType type,
        string name,
        int pos,
        string scoreValue,
        Texture2D image)
    {
        SetBackground(pos);
        playerName.text = name;
        if (image)
            profileImage.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        score.text = scoreValue;
        SetScoreIcon(type);
        SetLoading(false);
    }

    private void SetScoreIcon(LeaderboardType type)
    {

        switch (type)
        {
            case LeaderboardType.STAR:
                scoreIcon.sprite = iconStar;
                break;
            case LeaderboardType.TROPHY:
                scoreIcon.sprite = iconTrophy;
                break;
            case LeaderboardType.LOLLIPOP:
                scoreIcon.sprite = iconLollipop;
                break;
        }
    }

    private void SetBackground(int pos)
    {
        background.sprite = backroundNormal;
        position.text = "#" + pos;
        position.gameObject.SetActive(true);
        positionIcon.gameObject.SetActive(false);
        switch (pos)
        {
            case 1:
                if (!UserDataManager.Instance.IsDarkModeOn())
                    background.sprite = backroundGold;
                position.gameObject.SetActive(false);
                positionIcon.gameObject.SetActive(true);
                positionIcon.sprite = posIcon1;
                break;
            case 2:
                if (!UserDataManager.Instance.IsDarkModeOn())
                    background.sprite = backroundSilver;
                position.gameObject.SetActive(false);
                positionIcon.gameObject.SetActive(true);
                positionIcon.sprite = posIcon2;
                break;
            case 3:
                if (!UserDataManager.Instance.IsDarkModeOn())
                    background.sprite = backroundBronze;
                position.gameObject.SetActive(false);
                positionIcon.gameObject.SetActive(true);
                positionIcon.sprite = posIcon3;
                break;
        }

        if (UserDataManager.Instance.IsDarkModeOn())
        {

            background.sprite = backroundDark;

            if (playerTag)
            {
                Outline outline = playerTag.gameObject.GetComponent<Outline>();
                if (outline)
                    outline.enabled = false;
                playerTag.color = Color.white;
            }
        }

    }

    public void SetPlayer(bool player)
    {
        isPlayer = player;
        if (isPlayer)
        {
            google.gameObject.SetActive(false);
            return;
        }
    }

    public void SetFriend(bool friend)
    {
        isFriend = friend;
        if (isFriend)
        {
            google.sprite = googleFriend;
        }
        else
        {
            google.sprite = googleNonFriend;
        }
    }

    public void SetLoading(bool loading)
    {
        SetError(false);
        contents.SetActive(!loading);
        this.loading.SetActive(loading);
    }

    public void SetError(bool enable, string message = "Error!")
    {
        errorContent.SetActive(enable);
        errorMessage.text = message;
    }
}
