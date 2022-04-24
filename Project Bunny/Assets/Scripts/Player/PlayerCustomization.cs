using System.Collections.Generic;
using Networking;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Player
{
    public class PlayerCustomization : MonoBehaviourPunCallbacks
    {
        // Jersey/shirt
        [SerializeField] private GameObject _teamShirt;
        [SerializeField] private SkinnedMeshRenderer _jersey;
        private bool _isJerseyNull;

        // Hat
        [SerializeField] private GameObject[] _playerHats;
        private GameObject _currentHat;
        private int _hatIndex;
        private List<Renderer> _playerHatRenderers;
    
        // Boots
        [SerializeField] private GameObject _playerBoots;
        private int _shoesColorIndex;
    
        // Pants
        [SerializeField] private GameObject[] _playerPants;
        private GameObject _currentPants;
        private int _pantIndex;
        private int _pantColorIndex;
        private Color _pantColor;
    
        // Hair
        [SerializeField] private GameObject[] _playerHairStyles;
        private GameObject _currentHairStyle;
        private int _hairStyleIndex;
        private int _hairColorIndex;
        private Color _hairColor;
    
        // Coat
        [SerializeField] private GameObject[] _playerCoats;
        private GameObject _currentCoat;
        private int _coatIndex;
        private int _coatColorIndex;
        private Color _coatColor;
    
        // Skin
        [SerializeField] private GameObject _playerSkin;
        [SerializeField] private Color[] skinColors;
        private int _skinColorIndex;
    
        // Colors
        [SerializeField] private Color[] _colors;
    
        // UI
        [Header("UI")]
        [SerializeField] private TMPro.TMP_Text _nickNameText;
        [SerializeField] private AudioClip _uiInteractSound;

        private void Awake()
        {
            _playerHatRenderers = new List<Renderer>();

            foreach (var playerHat in _playerHats)
            {
                _playerHatRenderers.Add(playerHat.GetComponent<Renderer>());
            }

            _currentHat = _playerHats[0];
            _currentCoat = _playerCoats[0];
            _currentHairStyle = _playerHairStyles[0];
            _currentPants = _playerPants[0];

            _hatIndex = 0;
            _coatIndex = 0;
            _hairStyleIndex = 0;
            _pantIndex = 0;
            _shoesColorIndex = 0;

            _pantColorIndex = 0;
            _hairColorIndex = 0;
            _coatColorIndex = 0;
            _skinColorIndex = 0;

            _hairColor = Color.black;
            _pantColor = Color.white;
            _coatColor = Color.white;
        }

        private void Start()
        {
            _isJerseyNull = _jersey == null;
            UpdateTeamColorVisuals();
            SetNameText();
        }
    
        public void SwitchHat()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchHat_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchCoat()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchCoat_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchPants()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchPants_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchHair()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchHair_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchHairColor()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchHairColor_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchPantsColor()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchPantsColor_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchCoatColor()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchCoatColor_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchSkinColor()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchSkinColor_RPC), RpcTarget.AllBuffered);
        }

        public void SwitchShoesColor()
        {
            PlayUIInteractSound();
            photonView.RPC(nameof(SwitchShoesColor_RPC), RpcTarget.AllBuffered);
        }

        public void SetVisualCustomProperties()
        {
            var playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

            if (playerProperties.ContainsKey("hatIndex"))
            {
                photonView.RPC(nameof(SetHat_RPC), RpcTarget.AllBuffered, (int)playerProperties["hatIndex"]);
            }

            if (playerProperties.ContainsKey("hairIndex"))
            {
                if (playerProperties.ContainsKey("hairColorIndex"))
                {
                    photonView.RPC(nameof(SetHair_RPC), RpcTarget.AllBuffered, (int)playerProperties["hairIndex"], (int)playerProperties["hairColorIndex"]);
                }
                else
                {
                    photonView.RPC(nameof(SetHair_RPC), RpcTarget.AllBuffered, (int)playerProperties["hairIndex"], -1);
                }
            }

            if (playerProperties.ContainsKey("pantIndex"))
            {
                if (playerProperties.ContainsKey("pantColorIndex"))
                {
                    photonView.RPC(nameof(SetPants_RPC), RpcTarget.AllBuffered, (int)playerProperties["pantIndex"], (int)playerProperties["pantColorIndex"]);
                }
                else
                {
                    photonView.RPC(nameof(SetPants_RPC), RpcTarget.AllBuffered, (int)playerProperties["pantIndex"], -1);
                }
            }

            if (playerProperties.ContainsKey("coatIndex"))
            {
                if (playerProperties.ContainsKey("coatColorIndex"))
                {
                    photonView.RPC(nameof(SetCoat_RPC), RpcTarget.AllBuffered, (int)playerProperties["coatIndex"], (int)playerProperties["coatColorIndex"]);
                }
                else
                {
                    photonView.RPC(nameof(SetCoat_RPC), RpcTarget.AllBuffered, (int)playerProperties["coatIndex"], -1);
                }
            }

            if (playerProperties.ContainsKey("skinColorIndex"))
            {
                photonView.RPC(nameof(SetSkinColor_RPC), RpcTarget.AllBuffered, (int)playerProperties["skinColorIndex"]);
            }
            
            if (playerProperties.ContainsKey("shoesColorIndex"))
            {
                photonView.RPC(nameof(SetShoesColor_RPC), RpcTarget.AllBuffered, (int)playerProperties["shoesColorIndex"]);
            }
        }

        public void UpdateTeamColorVisuals()
        {
            photonView.RPC(nameof(UpdateTeamColorVisuals_RPC), RpcTarget.AllBuffered);
        }
        
        public void RestoreTeamlessColors()
        {
            photonView.RPC(nameof(RestoreTeamlessColors_RPC), RpcTarget.AllBuffered);
        }
    
        private void SetNameText()
        {
            _nickNameText.text = photonView.Owner.NickName;
        }

        private void PlayUIInteractSound()
        {
            AudioManager.Instance.PlayOneShot(_uiInteractSound, 0.5f);
        }

        #region RPCs

        /// <summary>
        /// Temporary functionality for updating visuals like mesh object and name text colors
        /// Functionality will still be kept for later, but more refined
        /// </summary>
        [PunRPC]
        private void UpdateTeamColorVisuals_RPC()
        {
            if (_isJerseyNull)
            {
                Debug.LogError("Missing reference to player jersey");
                return;
            }

            if (!photonView.Owner.CustomProperties.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out var teamId) 
                || !PhotonTeamsManager.Instance.TryGetTeamByCode((byte) teamId, out var team)) return;

            if (!_teamShirt.activeSelf)
            {
                _teamShirt.SetActive(true);
            }
        
            switch (team.Code)
            {
                //Temporary color changing code
                case 1:
                    _teamShirt.GetComponent<Renderer>().material.color = Color.blue;
                    foreach (var playerHatRenderer in _playerHatRenderers)
                    {
                        playerHatRenderer.material.color = Color.blue;
                    }
                    _nickNameText.color = Color.blue;
                    break;
                case 2:
                    _teamShirt.GetComponent<Renderer>().material.color = Color.red;
                    foreach (var playerHatRenderer in _playerHatRenderers)
                    {
                        playerHatRenderer.material.color = Color.red;
                    }
                    _nickNameText.color = Color.red;
                    break;
            }
        }
        
        /// <summary>
        /// Temporary function for restoring a player's colors to all white to show they are teamless
        /// </summary>
        [PunRPC]
        private void RestoreTeamlessColors_RPC()
        {
            foreach (var playerHatRenderer in _playerHatRenderers)
            {
                playerHatRenderer.material.color = Color.white;
            }
            _nickNameText.color = Color.white;
            _teamShirt.SetActive(false);
        }

        [PunRPC]
        private void SwitchHat_RPC()
        {
            _currentHat.SetActive(false);
            if (_hatIndex + 1 >= _playerHats.Length)
            {
                _hatIndex = 0;
            }
            else _hatIndex++;

            _currentHat = _playerHats[_hatIndex];
            _currentHat.SetActive(true);

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("hatIndex", _hatIndex);
            }
        }


        [PunRPC]
        private void SwitchPants_RPC()
        {
            _currentPants.SetActive(false);
            if (_pantIndex + 1 >= _playerPants.Length)
            {
                _pantIndex = 0;
            }
            else _pantIndex++;

            _currentPants = _playerPants[_pantIndex];
            _currentPants.SetActive(true);
            _currentPants.GetComponent<Renderer>().material.color = _pantColor;

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("pantIndex", _pantIndex);
            }
        }

        [PunRPC]
        private void SwitchPantsColor_RPC()
        {
            if (_pantColorIndex + 1 >= _colors.Length) _pantColorIndex = 0;
            else _pantColorIndex++;
            _pantColor = _colors[_pantColorIndex];
            _currentPants.GetComponent<Renderer>().material.color = _pantColor;

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("pantColorIndex", _pantColorIndex);
            }
        }

        [PunRPC]
        private void SwitchCoat_RPC()
        {
            _currentCoat.SetActive(false);
            if (_coatIndex + 1 >= _playerCoats.Length)
            {
                _coatIndex = 0;
            }
            else _coatIndex++;
            _currentCoat = _playerCoats[_coatIndex];
            _currentCoat.SetActive(true);
            _currentCoat.GetComponent<Renderer>().material.color = _coatColor;

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("coatIndex", _coatIndex);
            }
        }

        [PunRPC]
        private void SwitchCoatColor_RPC()
        {
            if (_coatColorIndex + 1 >= _colors.Length)
            {
                _coatColorIndex = 0;
            }
            else
            {
                _coatColorIndex++;
            }
            _coatColor = _colors[_coatColorIndex];
            _currentCoat.GetComponent<Renderer>().material.color = _coatColor;

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("coatColorIndex", _coatColorIndex);
            }
        }

        [PunRPC]
        private void SwitchHair_RPC()
        {
            _currentHairStyle.SetActive(false);
            if (_hairStyleIndex + 1 >= _playerHairStyles.Length)
            {
                _hairStyleIndex = 0;
            }
            else
            {
                _hairStyleIndex++;
            }
            _currentHairStyle = _playerHairStyles[_hairStyleIndex];
            _currentHairStyle.SetActive(true);
            _currentHairStyle.GetComponent<Renderer>().material.color = _hairColor;

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("hairIndex", _hairStyleIndex);
            }
        }

        [PunRPC]
        private void SwitchHairColor_RPC()
        {
            if (_hairColorIndex + 1 >= _colors.Length) _hairColorIndex = 0;
            else _hairColorIndex++;
            _hairColor = _colors[_hairColorIndex];
            _currentHairStyle.GetComponent<Renderer>().material.color = _hairColor;

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("hairColorIndex", _hairColorIndex);
            }
        }

        [PunRPC]
        private void SwitchSkinColor_RPC()
        {
            if (_skinColorIndex + 1 >= skinColors.Length)
            {
                _skinColorIndex = 0;
            }
            else
            {
                _skinColorIndex++;
            }

            _playerSkin.GetComponent<Renderer>().material.color = skinColors[_skinColorIndex];

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("skinColorIndex", _skinColorIndex);
            }
        }
    
        [PunRPC]
        private void SwitchShoesColor_RPC()
        {
            if (_shoesColorIndex + 1 >= _colors.Length)
            {
                _shoesColorIndex = 0;
            }
            else
            {
                _shoesColorIndex++;
            }

            _playerBoots.GetComponent<Renderer>().material.color = _colors[_shoesColorIndex];

            if (photonView.IsMine)
            {
                RoomManager.Instance.SetCustomProperty("shoesColorIndex", _shoesColorIndex);
            }
        }
    
        [PunRPC]
        private void SetHat_RPC(int index)
        {
            _currentHat.SetActive(false);
            _currentHat = _playerHats[index];
            _currentHat.SetActive(true);
        }

        [PunRPC]
        private void SetCoat_RPC(int index, int colorIndex)
        {
            _currentCoat.SetActive(false);
            _currentCoat = _playerCoats[index];
            _currentCoat.SetActive(true);
            if (colorIndex < 0) return;
            _currentCoat.GetComponent<Renderer>().material.color = _colors[colorIndex];
            
        }

        [PunRPC]
        private void SetHair_RPC(int index, int colorIndex)
        {
            _currentHairStyle.SetActive(false);
            _currentHairStyle = _playerHairStyles[index];
            _currentHairStyle.SetActive(true);
            if (colorIndex < 0) return;
            _currentHairStyle.GetComponent<Renderer>().material.color = _colors[colorIndex];
            
        }

        [PunRPC]
        private void SetPants_RPC(int index, int colorIndex)
        {
            _currentPants.SetActive(false);
            _currentPants = _playerPants[index];
            _currentPants.SetActive(true);
            if (colorIndex < 0) return;
            _currentPants.GetComponent<Renderer>().material.color = _colors[colorIndex];
        }

        [PunRPC]
        private void SetSkinColor_RPC(int colorIndex)
        {
            if (colorIndex < 0) return;
            _playerSkin.GetComponent<Renderer>().material.color = skinColors[colorIndex];
        }
        
        [PunRPC]
        private void SetShoesColor_RPC(int colorIndex)
        {
            if (colorIndex < 0) return;
            _playerBoots.GetComponent<Renderer>().material.color = _colors[colorIndex];
        }

        #endregion
    }
}
