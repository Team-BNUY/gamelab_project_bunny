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
    public int[] scoreValues = new int[8];
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
                    scores[2] = player.NickName;
                    this.hardWorker = player.NickName;
                    scoreValues[2] = mostHits;
                }
            }

            //TEACHER'S PET
            if (player.CustomProperties.ContainsKey(TEACHERS_PET_KEY))
            {
                int throws = (int)player.CustomProperties[TEACHERS_PET_KEY];

                if (throws < leastThrows)
                {
                    leastThrows = throws;
                    scores[3] = player.NickName;
                    this.teachersPet = player.NickName;
                    scoreValues[3] = leastThrows;
                }
            }

            //BULLY
            if (player.CustomProperties.ContainsKey(BULLY_KEY))
            {
                int hits = (int)player.CustomProperties[BULLY_KEY];

                if (hits > mostBullyHits)
                {
                    mostBullyHits = hits;
                    scores[1] = player.NickName;
                    this.bully = player.NickName;
                    scoreValues[1] = mostBullyHits;
                }
            }

            //RASCAL
            if (player.CustomProperties.ContainsKey(REBEL_KEY))
            {
                int hits = (int)player.CustomProperties[REBEL_KEY];

                if (hits > mostRascalHits)
                {
                    mostRascalHits = hits;
                    scores[0] = player.NickName;
                    this.rebel = player.NickName; 
                    scoreValues[0] = mostRascalHits;
                }
            }

            //MEET ME IN MY OFFICE
            if (player.CustomProperties.ContainsKey(MEET_IN_OFFICE_KEY))
            {
                int caught = (int)player.CustomProperties[MEET_IN_OFFICE_KEY];

                if (caught > mostCaught)
                {
                    mostCaught = caught;
                    scores[4] = player.NickName;
                    this.meetMeInMyOffice = player.NickName;
                    scoreValues[4] = mostCaught;
                }
            }

            //GLACE FOLIE
            if (player.CustomProperties.ContainsKey(GLACE_FOLIE_KEY))
            {
                int iceHits = (int)player.CustomProperties[GLACE_FOLIE_KEY];

                if (iceHits > mostIceHits)
                {
                    mostIceHits = iceHits;
                    scores[5] = player.NickName;
                    this.glaceFolie = player.NickName;
                    scoreValues[6] = mostIceHits;
                }
            }

            //SHOVELER
            if (player.CustomProperties.ContainsKey(SHOVELER_KEY))
            {
                int snowballsDug = (int)player.CustomProperties[SHOVELER_KEY];

                if (snowballsDug > mostSnowballsDug)
                {
                    mostSnowballsDug = snowballsDug;
                    scores[6] = player.NickName;
                    this.shoveler = player.NickName;
                    scoreValues[6] = mostSnowballsDug;
                }
            }

            //AVALANCHE
            if (player.CustomProperties.ContainsKey(AVALANCHE_KEY))
            {
                int rollballHits = (int)player.CustomProperties[AVALANCHE_KEY];

                if (rollballHits > mostRollballsHits)
                {
                    mostRollballsHits = rollballHits;
                    scores[7] = player.NickName;
                    this.avalanche = player.NickName;
                    scoreValues[7] = mostRollballsHits;
                }
            }
        }

        if (blueDeaths < redDeaths)
            winningTeamCode = 1;
        else if (redDeaths < blueDeaths)
            winningTeamCode = 2;
        else
            winningTeamCode = 0;

        _view.RPC(nameof(SyncMatchInformation), RpcTarget.AllBuffered, isFirstMatch, winningTeamCode, bully, rebel, hardWorker, teachersPet, glaceFolie, shoveler, avalanche, meetMeInMyOffice, (object)scoreValues);
    }

    public int GetLeadingTeam()
    {
        var blueDeaths = 0;
        var redDeaths = 0;
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            //WIN CONDITION
            if (!player.CustomProperties.ContainsKey(DEATHS_KEY)) continue;
            
            var team = player.GetPhotonTeam();
            if (team == null) continue;
            
            var deaths = (int) player.CustomProperties[DEATHS_KEY];

            switch (team.Code)
            {
                case 1:
                    blueDeaths += deaths;
                    break;
                case 2:
                    redDeaths += deaths;
                    break;
            }
        }

        if (blueDeaths < redDeaths)
        {
            winningTeamCode = 1;
        }
        else if (redDeaths < blueDeaths)
        {
            winningTeamCode = 2;
        }
        else
        {
            winningTeamCode = 0;
            //Debug.Log("how do we handle exact ties? TO BE SOLVED LATER");
        }

        _view.RPC(nameof(SyncLeadingTeamScore), RpcTarget.AllBuffered, winningTeamCode);
        return winningTeamCode;
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
        this.scoreValues = new int[8];
    }

    #endregion

    #region RPC

    [PunRPC]
    public void SyncMatchInformation(bool isFirst, int winningTeam, string bullyGuy, string rebelGuy, string hardWorkerGuy, string teacherPet, string glaceFolieGuy, string shovelerGuy, string avalancheGuy, string meetMeInMyOfficeGuy, int[] scoreVals)
    {
        isFirstMatch = isFirst;
        winningTeamCode = winningTeam;
        bully = bullyGuy;
        rebel = rebelGuy;
        hardWorker = hardWorkerGuy;
        teachersPet = teacherPet;
        glaceFolie = glaceFolieGuy;
        shoveler = shovelerGuy;
        avalanche = avalancheGuy;
        meetMeInMyOffice = meetMeInMyOfficeGuy;
        scores = new [] { rebelGuy, bullyGuy, hardWorkerGuy, teacherPet, meetMeInMyOfficeGuy, glaceFolieGuy, shovelerGuy, avalancheGuy };
        scoreValues = scoreVals;
    }

    [PunRPC]
    public void SyncLeadingTeamScore(int leadingTeamCode)
    {
        winningTeamCode = leadingTeamCode;
    }
    #endregion
}