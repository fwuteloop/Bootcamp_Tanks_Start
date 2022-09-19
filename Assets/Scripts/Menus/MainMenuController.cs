using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Photon.Pun;
using Photon.Realtime;

namespace Tanks
{
    public class MainMenuController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button lobbyButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private SettingsController settingsPopup;
        private Action pendingAction;

        private void Start()
        {
            // TODO: Connect to photon server
            if(!PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
           
            playButton.onClick.AddListener(JoinRandomRoom);
            playButton.onClick.AddListener(() => OnConnectionDependentActionClicked(JoinRandomRoom));
            lobbyButton.onClick.AddListener(GoToLobbyList);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            settingsPopup.gameObject.SetActive(false);
            settingsPopup.Setup();

            if (!PlayerPrefs.HasKey("PlayerName"))
                PlayerPrefs.SetString("PlayerName", "Player #" + Random.Range(0, 9999));
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("Connected to Master");
            pendingAction?.Invoke();
            PhotonNetwork.AutomaticallySyncScene = false;
        }
        private void OnSettingsButtonClicked()
        {
            settingsPopup.gameObject.SetActive(true);
        }

        public void JoinRandomRoom()
        {
            // TODO: Connect to a random room
            RoomOptions roomOptions = new RoomOptions { IsOpen = true, MaxPlayers = 4 };
            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            SceneManager.LoadScene("RoomLobby");
        }

        private void OnConnectionDependentActionClicked(Action action)
        {
            if(pendingAction != null)
            {
                return;
            }
            pendingAction = action;

            if(PhotonNetwork.IsConnectedAndReady)
            {
                action();
            }
        }

        private void GoToLobbyList()
        {
            SceneManager.LoadSceneAsync("LobbyList");
        }
    }
}