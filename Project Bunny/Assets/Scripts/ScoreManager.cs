using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoreManager : MonoBehaviour
{



    private static ScoreManager _instance;

    public static ScoreManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScoreManager>();
            }

            return _instance;
        }
    }

    [Header("Death Count")]
    [SerializeField] private int _blueDeaths = 0;
    [SerializeField] private int _redDeaths = 0;

    [Header("Match Information")]
    public bool _isFirstMatch = true;
    public int _winningTeamCode = 0;

    public PhotonView _view;

    #region PublicMethods
    public void CalculateScore()
    {
        _isFirstMatch = false;

        if (_blueDeaths < _redDeaths)
            _winningTeamCode = 1;
        else if (_blueDeaths > _redDeaths)
            _winningTeamCode = 2;
        else
            Debug.Log("how do we handle exact ties? TO BE SOLVED LATER");

        _view.RPC(nameof(SyncMatchInformation), RpcTarget.AllBuffered, _isFirstMatch, _winningTeamCode);
    }

    public void IncrementTeamDeaths(int teamCode)
    {
        _view.RPC(nameof(IncrementTeamDeathsRPC), RpcTarget.AllBuffered, teamCode);
    }

    public void ResetTeamDeaths()
    {
        _view.RPC(nameof(ResetTeamDeathsRPC), RpcTarget.AllBuffered);
    }
    #endregion

    #region RPC
    [PunRPC]
    private void IncrementTeamDeathsRPC(int teamCode)
    {
        switch (teamCode)
        {
            case 1:
                _blueDeaths++;
                break;
            case 2:
                _redDeaths++;
                break;
            default:
                Debug.LogError("Invalid team number");
                break;
        }
    }

    [PunRPC]
    public void ResetTeamDeathsRPC()
    {
        _blueDeaths = 0;
        _redDeaths = 0;
    }

    [PunRPC]
    public void SyncMatchInformation(bool isFirstMatch, int winningTeamCode)
    {
        this._isFirstMatch = isFirstMatch;
        this._winningTeamCode = winningTeamCode;
    }
    #endregion
}
