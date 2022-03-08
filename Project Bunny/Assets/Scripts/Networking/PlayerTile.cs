using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

public class PlayerTile : MonoBehaviour
{
    public Photon.Realtime.Player player;

    [SerializeField] private TMP_Text playerNameDisplay;
    [SerializeField] private Image readyIndicator;
    [SerializeField] private Image teamIndicator;

    [SerializeField] private Sprite masterIndicator;
    [SerializeField] private Sprite regularIndicator;

    public void SetReadyIndicator(bool isActive)
    {
        if (player.IsMasterClient)
        {
            readyIndicator.color = Color.blue;
            readyIndicator.sprite = masterIndicator;
        }
        else
        {
            readyIndicator.sprite = regularIndicator;
            readyIndicator.color = isActive ? Color.green : Color.grey;
        }
    }

    public void SetTeamIndicator(byte team)
    {
        switch (team) {
            case 1:
                teamIndicator.color = Color.blue;
                break;
            case 2:
                teamIndicator.color = Color.red;
                break;
            default:
                Debug.LogError("Error: Invalid Team");
                break;
        }
    }

    public void SetPlayer(Photon.Realtime.Player player)
    {
        this.player = player;
        SetPlayerNameDisplay(player.NickName);
        if (player.CustomProperties.ContainsKey("ready"))
        {
            SetReadyIndicator((bool)player.CustomProperties["ready"]);
        }
        else
        {
            SetReadyIndicator(false);
        }

        PhotonTeam team = PhotonTeamExtensions.GetPhotonTeam(player);
        if (team != null)
        {
            SetTeamIndicator(team.Code);
        }

    }

    public void SetPlayerNameDisplay(string name)
    {
        this.playerNameDisplay.text = name;
    }

    public void UpdateView(Hashtable changedProps)
    {
        if (changedProps.ContainsKey("ready"))
        {
            SetReadyIndicator((bool)changedProps["ready"]);
        }
        SetPlayerNameDisplay(player.NickName);
    }
}
