using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainHUD : MonoBehaviour {
    public static string HOW_TO_PLAY = "how-to-play";

    [Header("HUD Elements")]
    [SerializeField] Image selectedShip;
    [SerializeField] Text hpText;
    [SerializeField] Text power;
    [SerializeField] Text speed;
    [SerializeField] Text steering;
    [SerializeField] Text killCount;
    [SerializeField] Text deathCount;
    [SerializeField] Text rank;
    [SerializeField] Slider healthBar;
    [SerializeField] Text ConnectedText;
    [SerializeField] Image ConnectedImage;
    [SerializeField] Sprite[] ConnectedSprites;

    [Header("Dialogs")]
    [SerializeField] GameObject infoDialog;

    [Header("Panels")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject web3Panel;
    [SerializeField] GameObject selectShipPanel;
    [SerializeField] GameObject selectShipPanelContent;
    [SerializeField] GameObject howToPlay; 

    [Header("Prefabs")]
    [SerializeField] GameObject shipDisplayImagePrefab;

    [Header("Web3")]
    Helpers.ChainInfo connectedChain;

    ShipModel currentShip;
    List<ShipModel> ships;
    List<GameObject> shipSelectors = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        #if UNITY_EDITOR 
        if(!Connection.singleton.isNetworkActive) Connection.singleton.StartHost();
        #endif
        
        ConnectWallet.AccountConnected += HandleAccountConnected;

        ShipItem.ShipSelected += HandleShipSelected;
        web3Panel.gameObject.SetActive(true);
        settingsPanel.SetActive(false);
        if(PlayerPrefs.GetInt(MainHUD.HOW_TO_PLAY, 0) == 0) {
            howToPlay.SetActive(true);
        }

    }

    void OnDestroy() {
        ConnectWallet.AccountConnected -= HandleAccountConnected;
        ShipItem.ShipSelected -= HandleShipSelected;
    }

    public void toggleSelectionPanel() {
        selectShipPanel.gameObject.SetActive(!selectShipPanel.gameObject.activeInHierarchy);
    }

    public void toggleSettingsPanel() {
        settingsPanel.gameObject.SetActive(!settingsPanel.gameObject.activeInHierarchy);
    }

    private void HandleShipSelected(ShipItem gObject) {
        if(NetworkClient.isConnected) {
            toggleSelectionPanel();
            ArenaPlayer player = NetworkClient.connection.identity.GetComponent<ArenaPlayer>();
            if(player.shipData != null) {
                infoDialog.SetActive(true);
                FindObjectOfType<EventLogs>().AddLog("Your next ship has been queued.");
            }
            player.SetNextShip(gObject.shipData);
        }
    }

    public async void HandleShipSpawned(ShipModel shipData) {
        if(currentShip?.name == shipData?.name) return;

        currentShip = shipData;
        power.text = $"{shipData.FirePower()}";
        steering.text = $"{shipData.SteerPower()}";
        speed.text = $"{shipData.Acceleration()}";
        hpText.text = $"{shipData.GetMaxHealth()} / {shipData.GetMaxHealth()}";
        selectedShip.sprite = await shipData.GetImage();
    }

    public void HandleHealthUpdated(int currentHealth, int maxHealth) {
        healthBar.value = (float)currentHealth / maxHealth;
        hpText.text = $"{currentHealth} / {maxHealth}";
    }

    public void HandleKillCountUpdated(int _killCount) {
        killCount.text = $"{_killCount}";
    }

    public void HandleDeathCountUpdated(int _killCount) {
        deathCount.text = $"{_killCount}";
    }

    public void HandleRankUpdated(int _rank) {
        rank.text = $"{_rank}";
    }

    public void ReturnHome() {
        Connection.singleton.StopClient();
    }

    public void HandleCloseHowToPlay() {
        PlayerPrefs.SetInt("how-to-play", 1);
        howToPlay.SetActive(false);
    }

    private async void HandleAccountConnected(string account, int chainId) {
        connectedChain = Helpers.GetChainInfo(chainId);
        ConnectedText.text = $"Connected to {connectedChain.display}";
        ConnectedImage.sprite = ConnectedSprites[1];

        toggleSelectionPanel();
        List<ShipMetadata> metas = await ReaderContract.GetStarships(connectedChain.chain, connectedChain.network, connectedChain.readerContract, account, connectedChain.rpc);

        ships = new List<ShipModel>();
        foreach(Transform child in selectShipPanelContent.transform) {
            Destroy(child.gameObject);
        }
        ships.Clear();

        GameObject baseInstance = Instantiate(
            shipDisplayImagePrefab, 
            new Vector3(0,0,0), 
            Quaternion.identity, 
            selectShipPanelContent.transform);
        shipSelectors.Add(baseInstance.gameObject);
        baseInstance.GetComponent<ShipItem>().SetShipData(ShipModel.BasicModel());

        for(int i = 0; i < metas.Count; i++) {
            GameObject instance = Instantiate(
                shipDisplayImagePrefab, 
                new Vector3(0,0,0), 
                Quaternion.identity, 
                selectShipPanelContent.transform);
            shipSelectors.Add(instance.gameObject);
            instance.GetComponent<ShipItem>().SetShipData(new ShipModel(metas[i]));
        }
    }
    
    public bool windowsOpen() {
        if(web3Panel.activeInHierarchy || selectShipPanel.activeInHierarchy || settingsPanel.activeInHierarchy) {
            return true;
        }

        return false;
    }
}
