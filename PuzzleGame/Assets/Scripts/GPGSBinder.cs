using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi.Events;


public class GPGSBinder
{
    static GPGSBinder inst = new GPGSBinder();
    public static GPGSBinder Inst => inst;

    public bool isLogin;

    // ISavedGameClient SavedGame => 
    //     PlayGamesPlatform.Instance.SavedGame;

    // IEventsClient Events =>
    //     PlayGamesPlatform.Instance.Events;

    void Init()
    {
        var config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public void Login(Action<bool, UnityEngine.SocialPlatforms.ILocalUser> onLoginSuccess = null)
    {
        Init();
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (success) =>
        {
            onLoginSuccess?.Invoke(success == SignInStatus.Success, Social.localUser);
        });
    }

    public void Logout()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    public void ShowAllLeaderboardUI() =>
        Social.ShowLeaderboardUI();

    public void ShowTargetLeaderboardUI(string gpgsId) => 
        ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(gpgsId);

    public void ReportLeaderboard(string gpgsId, long score, Action<bool> onReported = null) =>
        Social.ReportScore(score, gpgsId, success => onReported?.Invoke(success));

	public void LoadAllLeaderboardArray(string gpgsId, Action<UnityEngine.SocialPlatforms.IScore[]> onloaded = null) => 
        Social.LoadScores(gpgsId, onloaded);

    public void LoadCustomLeaderboardArray(string gpgsId, int rowCount, LeaderboardStart leaderboardStart, 
        LeaderboardTimeSpan leaderboardTimeSpan, Action<bool, LeaderboardScoreData> onloaded = null)
    {
        PlayGamesPlatform.Instance.LoadScores(gpgsId, leaderboardStart, rowCount, LeaderboardCollection.Public, leaderboardTimeSpan, data =>
        {
            onloaded?.Invoke(data.Status == ResponseStatus.Success, data);
        });
    }

}