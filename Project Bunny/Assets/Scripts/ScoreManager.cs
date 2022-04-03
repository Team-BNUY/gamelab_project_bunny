using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;

    public static ScoreManager Instance {
        get {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScoreManager>();
            }

            return _instance;
        }
    }
    [Header("PROPERTY KEYS")]
    public const string DEATHS_KEY = "deaths";
    public const string REBEL_KEY = "rebelHits";
    public const string BULLY_KEY = "bullyHits";
    public const string HARD_WORKER_KEY = "hitsLanded";
    public const string TEACHERS_PET_KEY = "ballsThrown";
    public const string MEET_IN_OFFICE_KEY = "caughtByTeacher";
    public const string GLACE_FOLIE_KEY = "iceHitsLanded";
    public const string SHOVELER_KEY = "snowballsDug";
    public const string AVALANCHE_KEY = "giantRollballHits";

    [Header("Match Information")]
    public bool isFirstMatch = true;
    public int winningTeamCode = 0;

    public string[] scores = new string[8];
    // rebel - 0, bully - 1, hardWorker - 2, teachersPet - 3, office - 4, glaceFolie - 5, shoveler - 6, avalance - 7
    public string rebel = string.Empty;
    public string bully = string.Empty;
    public string hardWorker = string.Empty;
    public string teachersPet = string.Empty;
    public string meetMeInMyOffice = string.Empty;
    public string glaceFolie = string.Empty;
    public string shoveler = string.Empty;
    public string avalanche = string.Empty;

    public PhotonView _view;

    #region PublicMethods
    public void CalculateScore()
    {
        isFirstMatch = false;

        int blueDeaths = 0;
        int redDeaths = 0;
        int mostBullyHits = 0;
        int mostRascalHits = 0;
        int mostHits = 0;
        int leastThrows = 9999;
        int mostCaught = 0;
        int mostIceHits = 0;
        int mostSnowballsDug = 0;
        int mostRollballsHits = 0;

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {

            //WIN CONDITION
            if (player.CustomProperties.ContainsKey(DEATHS_KEY))
            {
                PhotonTeam team = player.GetPhotonTeam();
                if (team != null)
                {
                    int deaths = (int)player.CustomProperties[DEATHS_KEY];

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
            if (player.CustomProperties.ContainsKey(HARD_WORKER_KEY))
            {
                int hits = (int)player.CustomProperties[HARD_WORKER_KEY];

                if (hits > mostHits)
                {
                    mostHits = hits;
                    hardWorker = player.NickName;
                    scores[2] = player.NickName;
                }
            }

            //TEACHER'S PET
            if (player.CustomProperties.ContainsKey(TEACHERS_PET_KEY))
            {
                int throws = (int)player.CustomProperties[TEACHERS_PET_KEY];

                if (throws < leastThrows)
                {
                    leastThrows = throws;
                    teachersPet = player.NickName;
                    scores[3] = player.NickName;
                }
            }

            //BULLY
            if (player.CustomProperties.ContainsKey(BULLY_KEY))
            {
                int hits = (int)player.CustomProperties[BULLY_KEY];

                if (hits > mostBullyHits)
                {
                    mostBullyHits = hits;
                    bully = player.NickName;
                    scores[1] = player.NickName;
                }
            }

            //RASCAL
            if (player.CustomProperties.ContainsKey(REBEL_KEY))
            {
                int hits = (int)player.CustomProperties[REBEL_KEY];

                if (hits > mostRascalHits)
                {
                    mostRascalHits = hits;
                    rebel = player.NickName;
                    scores[0] = player.NickName;
                }
            }

            //MEET ME IN MY OFFICE
            if (player.CustomProperties.ContainsKey(MEET_IN_OFFICE_KEY))
            {
                int caught = (int)player.CustomProperties[MEET_IN_OFFICE_KEY];

                if (caught > mostCaught)
                {
                    mostRascalHits = mostCaught;
                    meetMeInMyOffice = player.NickName;
                    scores[4] = player.NickName;
                }
            }

            //GLACE FOLIE
            if (player.CustomProperties.ContainsKey(GLACE_FOLIE_KEY))
            {
                int iceHits = (int)player.CustomProperties[GLACE_FOLIE_KEY];

                if (iceHits > mostIceHits)
                {
                    mostIceHits = iceHits;
                    glaceFolie = player.NickName;
                    scores[5] = player.NickName;
                }
            }

            //SHOVELER
            if (player.CustomProperties.ContainsKey(SHOVELER_KEY))
            {
                int snowballsDug = (int)player.CustomProperties[SHOVELER_KEY];

                if (snowballsDug > mostSnowballsDug)
                {
                    mostIceHits = snowballsDug;
                    shoveler = player.NickName;
                    scores[6] = player.NickName;
                }
            }

            //AVALANCHE
            if (player.CustomProperties.ContainsKey(AVALANCHE_KEY))
            {
                int rollballHits = (int)player.CustomProperties[AVALANCHE_KEY];

                if (rollballHits > mostRollballsHits)
                {
                    mostRollballsHits = rollballHits;
                    avalanche = player.NickName;
                    scores[7] = player.NickName;
                }
            }
        }

        if (blueDeaths < redDeaths)
            winningTeamCode = 1;
        else if (redDeaths < blueDeaths)
            winningTeamCode = 2;
        else
            Debug.Log("how do we handle exact ties? TO BE SOLVED LATER");

        _view.RPC(nameof(SyncMatchInformation), RpcTarget.AllBuffered, isFirstMatch, winningTeamCode, bully, rebel, hardWorker, teachersPet, glaceFolie, shoveler, avalanche, meetMeInMyOffice);
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

        this.glaceFolie = string.Empty;
        this.shoveler = string.Empty;
        this.avalanche = string.Empty;
        this.meetMeInMyOffice = string.Empty;
        this.scores = new string[8];
        string[] keys = new string[] { DEATHS_KEY, REBEL_KEY, BULLY_KEY, HARD_WORKER_KEY, TEACHERS_PET_KEY, MEET_IN_OFFICE_KEY, GLACE_FOLIE_KEY, SHOVELER_KEY, AVALANCHE_KEY };

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

    private void ResetStats()
    {
        this.winningTeamCode = 0;
        this.bully = string.Empty;
        this.rebel = string.Empty;
        this.hardWorker = string.Empty;
        this.teachersPet = string.Empty;
        this.glaceFolie = string.Empty;
        this.shoveler = string.Empty;
        this.avalanche = string.Empty;
        this.meetMeInMyOffice = string.Empty;
        this.scores = new string[8];
    }

    #endregion

    #region RPC

    [PunRPC]
    public void SyncMatchInformation(bool isFirstMatch, int winningTeamCode, string bully, string rebel, string hardWorker, string teachersPet, string glaceFolie, string shoveler, string avalanche, string meetMeInMyOffice)
    {
        this.isFirstMatch = isFirstMatch;
        this.winningTeamCode = winningTeamCode;
        this.bully = bully;
        this.rebel = rebel;
        this.hardWorker = hardWorker;
        this.teachersPet = teachersPet;
        this.glaceFolie = glaceFolie;
        this.shoveler = shoveler;
        this.avalanche = avalanche;
        this.meetMeInMyOffice = meetMeInMyOffice;
        this.scores = new string[] { rebel, bully, hardWorker, teachersPet, meetMeInMyOffice, glaceFolie, shoveler, avalanche };
    }
    #endregion
}
