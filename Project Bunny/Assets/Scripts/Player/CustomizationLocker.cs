using Interfaces;
using Networking;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationLocker : MonoBehaviour, INetworkTriggerable
{
    [Header("Components")] 
    [SerializeField] private GameObject _customizationPanel;

    [SerializeField] private Button _changeHatButton;
    [SerializeField] private Button _changeCoatButton;
    [SerializeField] private Button _changePantsButton;
    [SerializeField] private Button _changeHairButton;
    [SerializeField] private Button _changeSkinColorButton;
    [SerializeField] private Button _changeCoatColorButton;
    [SerializeField] private Button _changePantColorButton;
    [SerializeField] private Button _changeHairColorButton;
    [SerializeField] private Button _changeBootsColorButton;
    [SerializeField] private Button _doneButton;

    [SerializeField] private AudioClip _interact;

    [SerializeField] private Color[] skinColors;
    private int skinColorIndex;

    private bool isActive;
    
    [SerializeField] public Animator hoverEButtonUI;

    private void Awake()
    {
        isActive = false;
        skinColorIndex = 0;
    }

    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this customization menu
    /// </summary>
    public void Trigger(NetworkStudentController currentPlayer)
    {
        if (isActive)
        {
            AudioManager.Instance.PlayOneShot(_interact, 0.5f);

            return;
        }
        
        _customizationPanel.SetActive(true);
        isActive = true;
        
        currentPlayer.CharacterControllerComponent.enabled = false;
        
        _changeHatButton.onClick.AddListener(() => currentPlayer.SwitchHat_RPC());
        _changeCoatButton.onClick.AddListener(() => currentPlayer.SwitchCoat_RPC());
        _changePantsButton.onClick.AddListener(() => currentPlayer.SwitchPants_RPC());
        _changeHairButton.onClick.AddListener(() => currentPlayer.SwitchHair_RPC());
        
        _changeSkinColorButton.onClick.AddListener(() => currentPlayer.SwitchSkinColor_RPC());
        _changeCoatColorButton.onClick.AddListener(() => currentPlayer.SwitchCoatColor_RPC());
        
        _changePantColorButton.onClick.AddListener(() => currentPlayer.SwitchPantsColor_RPC());
        _changeHairColorButton.onClick.AddListener(() => currentPlayer.SwitchHairColor_RPC());
        _changeBootsColorButton.onClick.AddListener(() => currentPlayer.SwitchShoesColor_RPC());
        _doneButton.onClick.AddListener(Leave);
        
        AudioManager.Instance.PlayOneShot(_interact, 0.5f);
    }

    public void Leave()
    {
        RoomManager.Instance.LocalStudentController.CharacterControllerComponent.enabled = true;
        _customizationPanel.SetActive(false);
        isActive = false;
    }

    public void Enter()
    {
        hoverEButtonUI.enabled = true;
        hoverEButtonUI.StartPlayback();
        hoverEButtonUI.gameObject.SetActive(true);
    }

    public void Exit()
    {
        hoverEButtonUI.StopPlayback();
        hoverEButtonUI.enabled = false;
        hoverEButtonUI.gameObject.SetActive(false);
    }
    
    #endregion
    
    #region Utilities

    private Color GenerateSkinColor()
    {
        if (skinColorIndex + 1 >= skinColors.Length) skinColorIndex = 0;
        else skinColorIndex++;

        return skinColors[skinColorIndex];
    }
    
    
    #endregion
}

