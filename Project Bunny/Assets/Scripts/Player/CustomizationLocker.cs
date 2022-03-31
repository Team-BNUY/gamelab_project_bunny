using System;
using Interfaces;
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

    [SerializeField] private Color[] skinColors;
    private int skinColorIndex;

    private bool isActive;

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
        _customizationPanel.SetActive(!isActive);
        isActive = !isActive;

        
        currentPlayer.CharacterControllerComponent.enabled = !isActive;
        
        _changeHatButton.onClick.AddListener(() => currentPlayer.SwitchHat_RPC());
        _changeCoatButton.onClick.AddListener(() => currentPlayer.SwitchCoat_RPC());
        _changePantsButton.onClick.AddListener(() => currentPlayer.SwitchPants_RPC());
        _changeHairButton.onClick.AddListener(() => currentPlayer.SwitchHair_RPC());
        
        _changeSkinColorButton.onClick.AddListener(() => currentPlayer.SwitchSkinColor_RPC());
        _changeCoatColorButton.onClick.AddListener(() => currentPlayer.SwitchCoatColor_RPC());
        
        _changePantColorButton.onClick.AddListener(() => currentPlayer.SwitchPantsColor_RPC());
        _changeHairColorButton.onClick.AddListener(() => currentPlayer.SwitchHairColor_RPC());
        
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

