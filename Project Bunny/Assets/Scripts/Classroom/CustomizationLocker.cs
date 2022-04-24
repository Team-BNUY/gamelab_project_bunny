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
    [SerializeField] private AudioClip _stopInteract;

    [SerializeField] private Color[] skinColors;
    private int skinColorIndex;

    private bool _isActive;
    
    [SerializeField] public Animator hoverEButtonUI;

    private void Awake()
    {
        _isActive = false;
        skinColorIndex = 0;
    }

    #region InterfaceMethods
    
    /// <summary>
    /// Method that runs when you trigger this customization menu
    /// </summary>
    public void TriggerableTrigger(NetworkStudentController currentStudentController)
    {
        if (_isActive) return;
        _isActive = true;
        
        currentStudentController.CharacterControllerComponent.enabled = false;
        _customizationPanel.SetActive(true);

        var playerCustomization = currentStudentController.GetComponent<PlayerCustomization>();
        _changeHatButton.onClick.AddListener(() => playerCustomization.SwitchHat());
        _changeCoatButton.onClick.AddListener(() => playerCustomization.SwitchCoat());
        _changePantsButton.onClick.AddListener(() => playerCustomization.SwitchPants());
        _changeHairButton.onClick.AddListener(() => playerCustomization.SwitchHair());
        
        _changeSkinColorButton.onClick.AddListener(() => playerCustomization.SwitchSkinColor());
        _changeCoatColorButton.onClick.AddListener(() => playerCustomization.SwitchCoatColor());
        
        _changePantColorButton.onClick.AddListener(() => playerCustomization.SwitchPantsColor());
        _changeHairColorButton.onClick.AddListener(() => playerCustomization.SwitchHairColor());
        _changeBootsColorButton.onClick.AddListener(() => playerCustomization.SwitchShoesColor());
        _doneButton.onClick.AddListener(Leave);
        
        AudioManager.Instance.PlayOneShot(_interact, 0.5f);
    }

    public void Leave()
    {
        RoomManager.Instance.LocalStudentController.CharacterControllerComponent.enabled = true;
        _customizationPanel.SetActive(false);
        _isActive = false;
        
        AudioManager.Instance.PlayOneShot(_stopInteract, 0.5f);
    }

    public void TriggerableEnter()
    {
        hoverEButtonUI.enabled = true;
        hoverEButtonUI.gameObject.SetActive(true);
        hoverEButtonUI.Play("EInteract");
    }

    public void TriggerableExit()
    {
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

