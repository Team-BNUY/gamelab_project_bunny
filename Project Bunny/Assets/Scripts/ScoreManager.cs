using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

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

    [Header("Match Information")]
    public bool isFirstMatch = true;
    public int winningTeamCode = 0;
    public string rascal = string.Empty;
    public string bully = string.Empty;
    public string hardWorker = string.Empty;
    public string teachersPet = string.Empty;

    public PhotonView _view;

    #region PublicMethods
    public void CalculateScore()
    {
        _isFirstMatch = false;

        int blueDeaths = 0;
        int redDeaths = 0;
        int mostBullyHits = 0;
        int mostRascalHits = 0;
        int mostHits = 0;
        int leastThrows = 9999;

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {

            //WIN CONDITION
            if (player.CustomProperties.ContainsKey("deaths"))
            {
                PhotonTeam team = player.GetPhotonTeam();
                if (team != null)
                {
                    int deaths = (int)player.CustomProperties["deaths"];

                    if (team.Code == 1)
                    {
                        blueDeaths += deaths;
                    }
                    else if (team.Code == 2)
                    {
                        redDeaths += deaths;
                    }
                }
            }

            //HARD WORKER
            if (player.CustomProperties.ContainsKey("hitsLanded"))
            {
                int hits = (int)player.CustomProperties["hitsLanded"];

                if (hits > mostHits)
                {
                    mostHits = hits;
                    hardWorker = player.NickName;
                }
            }

            //TEACHER'S PET
            if (player.CustomProperties.ContainsKey("ballsThrown"))
            {
                int throws = (int)player.CustomProperties["ballsThrown"];

                if (throws < leastThrows)
                {
                    leastThrows = throws;
                    teachersPet = player.NickName;
                }
            }

            //BULLY
            if (player.CustomProperties.ContainsKey("bullyHits"))
            {
                int hits = (int)player.CustomProperties["bullyHits"];

                if (hits > mostBullyHits)
                {
                    mostBullyHits = hits;
                    bully = player.NickName;
                }
            }

            //RASCAL
            if (player.CustomProperties.ContainsKey("rascalHits"))
            {
                int hits = (int)player.CustomProperties["rascalHits"];

                if (hits > mostRascalHits)
                {
                    mostRascalHits = hits;
                    rascal = player.NickName;
                }
            }
        }

        if (blueDeaths < redDeaths)
            winningTeamCode = 1;
        else if (redDeaths < blueDeaths)
            winningTeamCode = 2;
        else
            Debug.Log("how do we handle exact ties? TO BE SOLVED LATER");

        _view.RPC(nameof(SyncMatchInformation), RpcTarget.AllBuffered, isFirstMatch, winningTeamCode, bully, rascal, hardWorker, teachersPet);
    }

    public void IncrementPropertyCounter(Photon.Realtime.Player player, string code)
    {
        ExitGames.Client.Photon.Hashtable props = player.CustomProperties;
        if (props.ContainsKey(code))
        {
            int count = (int)props[code];
            props[code] = ++count;
        }
        else
        {
            props.Add(code, 1); //if property does not exist, we assume it's zero.
        }
        player.SetCustomProperties(props);

    }

    public void ClearPropertyCounters()
    {
        ExitGames.Client.Photon.Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;

        string[] keys = new string[] { "deaths", "hitsLanded", "ballsThrown", "bullyHits", "rascalHits" };

        foreach (string key in keys)
        {
            if (props.ContainsKey(key))
                props[key] = 0;
            else
                props.Add(key, 0);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        ResetStats();
    }

    private void ResetStats() {
        this.winningTeamCode = 0;
        this.bully = string.Empty;
        this.rascal = string.Empty;
        this.hardWorker = string.Empty;
        this.teachersPet = string.Empty;
    }

    #endregion

    #region RPC

    [PunRPC]
    public void SyncMatchInformation(bool isFirstMatch, int winningTeamCode, string bully, string rascal, string hardWorker, string teachersPet)
    {
        this.isFirstMatch = isFirstMatch;
        this.winningTeamCode = winningTeamCode;
        this.bully = bully;
        this.rascal = rascal;
        this.hardWorker = hardWorker;
        this.teachersPet = teachersPet;
    }
    #endregion
}
