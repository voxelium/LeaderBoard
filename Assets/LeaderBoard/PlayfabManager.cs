using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    [Header("Windows")]
    [SerializeField] private GameObject nameWindow;
    [SerializeField] private GameObject leaderboardWindow;

    [Header("Display name window")]
    [SerializeField] private GameObject nameError;
    [SerializeField] private TMP_InputField nameInput;
    

    [Header("LeaderBoard")]
    [SerializeField] private GameObject leadRow;
    [SerializeField] private Transform leadRowsParent;

    [Header("Input datas")]
    [SerializeField] private TextMeshProUGUI clicksText;
    [SerializeField] private TextMeshProUGUI leadersText;
    [SerializeField] private string tempName;
    private int clicksNum = 0;

    private void Start()
    {
        Login();
    }


    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            TitleId = tempName,
            //CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }



    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Successfull login/account create");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }

        if (name == null)
        {
            nameWindow.SetActive(true);
        }
        else
            leaderboardWindow.SetActive(true);
        
    }

    public void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        leaderboardWindow.SetActive(true);
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
            MaxResultsCount = 500
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError);
    }


    private void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue );

            GameObject newGo = Instantiate(leadRow,leadRowsParent);
            TextMeshProUGUI[] texts = newGo.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

        }
        
    }


    public void Clicker()
    {
        clicksNum++;
        clicksText.text = clicksNum.ToString();
    }

}
