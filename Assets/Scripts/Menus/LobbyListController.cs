using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tanks
{
    public class LobbyListController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button createNewLobbyButton;
        [SerializeField] private Button joinPrivateLobbyButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private LobbyListEntry lobbyListEntryPrefab;
        [SerializeField] private RectTransform entriesHolder;

        [SerializeField] private GameObject createLobbyPopup;
        [SerializeField] private GameObject joinPrivateLobbyPopup;

        private Dictionary<string, LobbyListEntry> entries;

        private void OnNewLobbyButtonClicked()
        {
            createLobbyPopup.SetActive(true);
        }

        private void OnJoinPrivateLobbyButtonClicked()
        {
            joinPrivateLobbyPopup.SetActive(true);
        }

        private void OnCloseButtonClicked()
        {
            SceneManager.LoadScene("MainMenu");
        }

        // TODO: Create, Update and Remove room entries
        private void DeleteRoomEntry(RoomInfo info)
        {
            Destroy(entries[info.Name].gameObject);
            entries.Remove(info.Name);
        }

        private bool IsRoomUnlisted(RoomInfo roomInfo) => !roomInfo.IsVisible || roomInfo.PlayerCount == 0 || !roomInfo.IsOpen;

        private void AddNewLobbyEntry(RoomInfo info)
        {
            var entry = Instantiate(lobbyListEntryPrefab, entriesHolder);
            entry.Setup(info);
            entries.Add(info.Name, entry);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.LogWarning("Room list updated.");
            foreach(RoomInfo roomInfo in roomList)
            {
                if(roomInfo.RemovedFromList)
                {
                    DeleteRoomEntry(roomInfo);
                    continue;
                }

                if(IsRoomUnlisted(roomInfo))
                {
                    if(entries.ContainsKey(roomInfo.Name))
                    {
                        DeleteRoomEntry(roomInfo);
                        continue;
                    }
                }

                if(entries.ContainsKey(roomInfo.Name))
                {
                    entries[roomInfo.Name].Setup(roomInfo);
                }
                else
                {
                    AddNewLobbyEntry(roomInfo);
                }
            }
        }

        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene("RoomLobby");
        }

        private void Start()
        {
            LoadingGraphics.Disable();

            entries = new Dictionary<string, LobbyListEntry>();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            createNewLobbyButton.onClick.AddListener(OnNewLobbyButtonClicked);
            joinPrivateLobbyButton.onClick.AddListener(OnJoinPrivateLobbyButtonClicked);

            DestroyHolderChildren();

            createLobbyPopup.SetActive(false);
            joinPrivateLobbyPopup.SetActive(false);

            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }

        private void DestroyHolderChildren()
        {
            for (var i = entriesHolder.childCount - 1; i >= 0; i--) {
                Destroy(entriesHolder.GetChild(i).gameObject);
            }
        }
    }
}