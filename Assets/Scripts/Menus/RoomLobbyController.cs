using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
namespace Tanks
{
    public class RoomLobbyController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text username;
        [SerializeField] private PlayerLobbyEntry playerLobbyEntryPrefab;
        [SerializeField] private RectTransform entriesHolder;

        // TODO: Create and Delete player entries
        private Dictionary<Player, PlayerLobbyEntry> lobbyEntries;

        private bool IsEveryPlayerReady => lobbyEntries.Values.ToList().TrueForAll(entry => entry.IsPlayerReady);
        private void AddLobbyEntry(Player player)
        {
            var entry = Instantiate(playerLobbyEntryPrefab, entriesHolder);
            entry.Setup(player);

            // TODO: track created player lobby entries
            lobbyEntries.Add(player, entry);
        }

        private void Start()
        {
            LoadingGraphics.Disable();
            DestroyHolderChildren();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
            startButton.gameObject.SetActive(false);

            PhotonNetwork.AutomaticallySyncScene = true;

            lobbyEntries = new Dictionary<Player, PlayerLobbyEntry>(PhotonNetwork.CurrentRoom.MaxPlayers);
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                AddLobbyEntry(player);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddLobbyEntry(newPlayer);
            UpdateStartButton();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(lobbyEntries[otherPlayer].gameObject);
            lobbyEntries.Remove(otherPlayer);

            UpdateStartButton();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            lobbyEntries[targetPlayer].UpdateVisuals();

            UpdateStartButton();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            UpdateStartButton();
        }
        // Show start button only to the master client and when all players are ready
        private void UpdateStartButton()
        {
            startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && IsEveryPlayerReady);
        }
        //Load gameplay level for all clients
        private void OnStartButtonClicked()
        {
           if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("You're not master client, dummy.");
                return;
            }

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("Gameplay");
        }

        /// <summary>
        /// leave room and go to main menu
        /// </summary>

        private void OnCloseButtonClicked()
        {
            // TODO: Leave room
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }

        private void DestroyHolderChildren()
        {
            for (var i = entriesHolder.childCount - 1; i >= 0; i--) {
                Destroy(entriesHolder.GetChild(i).gameObject);
            }
        }
    }
}