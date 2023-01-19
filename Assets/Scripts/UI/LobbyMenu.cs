using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour {
    [SerializeField] string connectTo = "game-server.warp.bond";
    [SerializeField] Connection networkManager;

    [Header("UI")]
    [SerializeField] GameObject joinLobbyPopup;
    [SerializeField] InputField ipAddressInputField;
    [SerializeField] GameObject settingsPanel;

    [SerializeField] GameObject createBtn;
    [SerializeField] GameObject joinBtn;

    private void Awake() {
        #if !UNITY_EDITOR
        createBtn.SetActive(false);
        joinBtn.SetActive(false);
        #endif
    }

    void Update() {
        if(joinLobbyPopup.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.KeypadEnter)) {
            JoinLobby();
        }    
    }

    public void HostLobby() {
        networkManager.StartHost();
    }

    public void OpenJoinLobby() {
        joinLobbyPopup.SetActive(true);
    }

    public void CloseJoinLobby() {
        joinLobbyPopup.SetActive(false);
    }

    public void ToggleSettingsPanel() {
        settingsPanel.SetActive(!settingsPanel.activeInHierarchy);
    }

    public void JoinLobby() {
        // string ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = "localhost";

        networkManager.StartClient();
    }

    public void JoinServer() {
        networkManager.networkAddress = connectTo;

        networkManager.StartClient();
    }
}
