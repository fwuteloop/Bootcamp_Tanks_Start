using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections;
using ExitGames.Client.Photon;
namespace Tanks
{
    public class PlayerLobbyEntry : MonoBehaviour
    {
        [SerializeField] private Button readyButton;
        [SerializeField] private GameObject readyText;
        [SerializeField] private Button waitingButton;
        [SerializeField] private GameObject waitingText;

        [SerializeField] private TMP_Text playerName;
        [SerializeField] private Button changeTeamButton;
        [SerializeField] private Image teamHolder;
        [SerializeField] private List<Sprite> teamBackgrounds;

        private Player player;

        // Update player team to other clients
        public int PlayerTeam 
        { get => player.CustomProperties.ContainsKey("Team") ? (int)player.CustomProperties["Team"] : 0;
            set
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable { { "Team", value } };
                player.SetCustomProperties(hash);
           } 
  }  
        


        public bool IsPlayerReady 
        { 
            get => player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"];
            set 
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable { { "IsReady", value } };
                player.SetCustomProperties(hash);
            } 
        
        }  // TODO: Update player ready status to other clients

        private bool IsLocalPlayer => Equals(player, PhotonNetwork.LocalPlayer); // TODO: Get if this entry belongs to the local player

        public void Setup(Player entryPlayer)
        {
            // TODO: Store and update player information
            player = entryPlayer;
            if(IsLocalPlayer)
            {
                //update player team to others
                PlayerTeam = (player.ActorNumber - 1) % PhotonNetwork.CurrentRoom.MaxPlayers;
            }

            playerName.text = PlayerPrefs.GetString("PlayerName");

            if (!IsLocalPlayer)
                Destroy(changeTeamButton);

            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            
            teamHolder.sprite = teamBackgrounds[PlayerTeam];

            waitingText.SetActive(!IsPlayerReady);
            readyText.SetActive(IsPlayerReady);
        }

        private void Start()
        {
            waitingButton.onClick.AddListener(() => OnReadyButtonClick(true));
            readyButton.onClick.AddListener(() => OnReadyButtonClick(false));
            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);

            waitingButton.gameObject.SetActive(IsLocalPlayer);
            readyButton.gameObject.SetActive(false);
        }

        private void OnChangeTeamButtonClicked()
        {
            // TODO: Change player team
            PlayerTeam = (PlayerTeam + 1) % PhotonNetwork.CurrentRoom.MaxPlayers;
        }

        private void OnReadyButtonClick(bool isReady)
        {
            waitingButton.gameObject.SetActive(!isReady);
            waitingText.SetActive(!isReady);
            readyButton.gameObject.SetActive(isReady);
            readyText.SetActive(isReady);

            IsPlayerReady = isReady;
        }
    }
}