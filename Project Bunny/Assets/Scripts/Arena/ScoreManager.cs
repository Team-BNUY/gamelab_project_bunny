using System.Globalization;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScoreManager>();
            }

            return _instance;
        }
    }
    
    [Header("PROPERTY KEYS")]
    public const string DeathsKey = "deaths";
    public const string RebelKey = "rebelHits";
    public const string BullyKey = "bullyHits";
    public const string HardWorkerKey = "hitsLanded";
    public const string TeachersPetKey = "ballsThrown";
    public const string MeetInOfficeKey = "caughtByTeacher";
    public const string GlaceFolieKey = "iceHitsLanded";
    public const string ShovelerKey = "snowballsDug";
    public const string AvalancheKey = "giantRollballHits";

    [Header("Match Information")]
    public bool isFirstMatch = true;
    public int winningTeamCode;
    
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

    public PhotonView view;

    public int[] RandomNumbers { get; } = new int[100];

    #region PublicMethods
    
    public void CalculateScore()
    {
        // Syncing random numbers to all clients so that they can all have the same board after a match
        if (PhotonNetwork.IsMasterClient)
        {
            var randomNumbers = GenerateRandomNumbers();
            view.RPC(nameof(SyncRandomNumbers), RpcTarget.AllBuffered, randomNumbers);
        }
        
        isFirstMatch = false;

        var blueDeaths = 0;
        var redDeaths = 0;
        var mostBullyHits = 0;
        var mostRascalHits = 0;
        var mostHits = 0;
        var leastThrows = 9999;
        var mostCaught = 0;
        var mostIceHits = 0;
        var mostSnowballsDug = 0;
        var mostRollballsHits = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            //WIN CONDITION
            if (player.CustomProperties.ContainsKey(DeathsKey))
            {
                var team = player.GetPhotonTeam(); // TODO Check why we are retrieving a player's team right after leaving it
                if (team != null)
                {
                    var deaths = (int)player.CustomProperties[DeathsKey];

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
            if (player.CustomProperties.ContainsKey(HardWorkerKey))
            {
                var hits = (int)player.CustomProperties[HardWorkerKey];

                if (hits > mostHits)
                {
                    mostHits = hits;
                    scores[2] = player.NickName;
                    hardWorker = player.NickName;
                    scoreValues[2] = mostHits;
                }
            }

            //TEACHER'S PET
            if (player.CustomProperties.ContainsKey(TeachersPetKey))
            {
                var throws = (int)player.CustomProperties[TeachersPetKey];

                if (throws < leastThrows)
                {
                    leastThrows = throws;
                    scores[3] = player.NickName;
                    teachersPet = player.NickName;
                    scoreValues[3] = leastThrows;
                }
            }

            //BULLY
            if (player.CustomProperties.ContainsKey(BullyKey))
            {
                var hits = (int)player.CustomProperties[BullyKey];

                if (hits > mostBullyHits)
                {
                    mostBullyHits = hits;
                    scores[1] = player.NickName;
                    bully = player.NickName;
                    scoreValues[1] = mostBullyHits;
                }
            }

            //RASCAL
            if (player.CustomProperties.ContainsKey(RebelKey))
            {
                var hits = (int)player.CustomProperties[RebelKey];

                if (hits > mostRascalHits)
                {
                    mostRascalHits = hits;
                    scores[0] = player.NickName;
                    rebel = player.NickName; 
                    scoreValues[0] = mostRascalHits;
                }
            }

            //MEET ME IN MY OFFICE
            if (player.CustomProperties.ContainsKey(MeetInOfficeKey))
            {
                var caught = (int)player.CustomProperties[MeetInOfficeKey];

                if (caught > mostCaught)
                {
                    mostCaught = caught;
                    scores[4] = player.NickName;
                    meetMeInMyOffice = player.NickName;
                    scoreValues[4] = mostCaught;
                }
            }

            //GLACE FOLIE
            if (player.CustomProperties.ContainsKey(GlaceFolieKey))
            {
                var iceHits = (int)player.CustomProperties[GlaceFolieKey];

                if (iceHits > mostIceHits)
                {
                    mostIceHits = iceHits;
                    scores[5] = player.NickName;
                    glaceFolie = player.NickName;
                    scoreValues[6] = mostIceHits;
                }
            }

            //SHOVELER
            if (player.CustomProperties.ContainsKey(ShovelerKey))
            {
                var snowballsDug = (int)player.CustomProperties[ShovelerKey];

                if (snowballsDug > mostSnowballsDug)
                {
                    mostSnowballsDug = snowballsDug;
                    scores[6] = player.NickName;
                    shoveler = player.NickName;
                    scoreValues[6] = mostSnowballsDug;
                }
            }

            //AVALANCHE
            if (player.CustomProperties.ContainsKey(AvalancheKey))
            {
                var rollballHits = (int)player.CustomProperties[AvalancheKey];

                if (rollballHits > mostRollballsHits)
                {
                    mostRollballsHits = rollballHits;
                    scores[7] = player.NickName;
                    avalanche = player.NickName;
                    scoreValues[7] = mostRollballsHits;
                }
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
        }

        view.RPC(nameof(SyncMatchInformation), RpcTarget.AllBuffered, isFirstMatch, winningTeamCode, bully, rebel, hardWorker, teachersPet, glaceFolie, shoveler, avalanche, meetMeInMyOffice, scoreValues);
    }

    public int GetLeadingTeam()
    {
        var blueDeaths = 0;
        var redDeaths = 0;
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            //WIN CONDITION
            if (!player.CustomProperties.ContainsKey(DeathsKey)) continue;
            
            var team = player.GetPhotonTeam();
            if (team == null) continue;
            
            var deaths = (int) player.CustomProperties[DeathsKey];

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

        view.RPC(nameof(SyncLeadingTeamScore), RpcTarget.AllBuffered, winningTeamCode);
        return winningTeamCode;
    }
    
    public static void IncrementPropertyCounter(Photon.Realtime.Player player, string code)
    {
        var props = player.CustomProperties;
        if (props.ContainsKey(code))
        {
            var count = (int)props[code];
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
        var props = PhotonNetwork.LocalPlayer.CustomProperties;
        var keys = new [] { DeathsKey, RebelKey, BullyKey, HardWorkerKey, TeachersPetKey, MeetInOfficeKey, GlaceFolieKey, ShovelerKey, AvalancheKey };

        foreach (var key in keys)
        {
            if (props.ContainsKey(key))
            {
                props[key] = 0;
            }
            else
            {
                props.Add(key, 0);
            }
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        ResetStats();
    }

    private void ResetStats()
    {
        winningTeamCode = 0;
        bully = string.Empty;
        rebel = string.Empty;
        hardWorker = string.Empty;
        teachersPet = string.Empty;
        glaceFolie = string.Empty;
        shoveler = string.Empty;
        avalanche = string.Empty;
        meetMeInMyOffice = string.Empty;
        scores = new string[8];
        scoreValues = new int[8];
    }

    private string GenerateRandomNumbers()
    {
        var randomNumbers = string.Empty;
        for (var i = 0; i < 100; i++)
        {
            var rand = Random.Range(0, scores.Length);
            randomNumbers += i == 0 ? rand.ToString(CultureInfo.InvariantCulture) : " " + rand.ToString(CultureInfo.InvariantCulture);
        }
        
        return randomNumbers;
    }

    #endregion

    #region RPC

    [PunRPC]
    private void SyncMatchInformation(bool isFirst, int winningTeam, string bullyGuy, string rebelGuy, string hardWorkerGuy, string teacherPet, string glaceFolieGuy, string shovelerGuy, string avalancheGuy, string meetMeInMyOfficeGuy, int[] scoreVals)
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
    private void SyncLeadingTeamScore(int leadingTeamCode)
    {
        winningTeamCode = leadingTeamCode;
    }

    [PunRPC]
    private void SyncRandomNumbers(string randomNumbers)
    {
        for (var i = 0; i < randomNumbers.Split(' ').Length; i++)
        {
            var randomNumber = randomNumbers.Split(' ')[i];
            RandomNumbers[i] = int.Parse(randomNumber);
        }
    }

    #endregion
}
