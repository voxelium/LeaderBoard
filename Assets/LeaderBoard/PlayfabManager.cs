using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clicksText;
    [SerializeField] private TextMeshProUGUI leadersText;
    private int clicksNum = 0;

    private void Start()
    {
        Login();
    }


    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {CustomId = SystemInfo.deviceUniqueIdentifier,CreateAccount = true};

        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }



    private void OnSuccess(LoginResult result)
    {
        Debug.Log("Successfull login/account create");
    }


    private void OnError(PlayFabError error)
    {
        Debug.Log("Error while login/creating account");
        Debug.Log(error.GenerateErrorReport());
    }


    public void SendClicks()
    {
        SendLeaderBoard(clicksNum);
    }

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "BestScore", Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
    }


    private void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfull leaderboard sent");
    }


    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "BestScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError);
    }

    private void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            leadersText.text = item.Position + " " + item.PlayFabId + " " + item.StatValue;
            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue );
        }
        
    }


    public void Clicker()
    {
        clicksNum++;
        clicksText.text = clicksNum.ToString();
    }

}
