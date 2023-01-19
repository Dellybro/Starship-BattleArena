
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class ShipDisplay : NetworkBehaviour {
    private Settings settings;

    [SerializeField] TMP_Text displayNameText;
    [SerializeField] SpriteRenderer shipRenderer;
    [SerializeField] Slider healthBar;
    [SerializeField] SpriteRenderer mapIcon;
    [SerializeField] Sprite friendlyMapIcon;

    [SyncVar(hook = nameof(HandleDisplayArchetype))]
    private string archetype;

    [SyncVar(hook = nameof(HandleDirectionUpdated))]
    private int direction;

    [SyncVar(hook = nameof(HandleDisplayNameUpdated))]
    private string displayName;

    private void Awake() {
        settings = FindObjectOfType<Settings>();
    }

    private void Start() {
        displayNameText.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);

        if(!hasAuthority) return;
        mapIcon.sprite = friendlyMapIcon;
    }

    private void Update() {
        if(!isClient) return;

        if(hasAuthority) {
            Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if(movement.y > 0) {
                if(direction != 1) CmdSetDirection(1);
            } else if(movement.y < 0) {
                if(direction != -1) CmdSetDirection(-1);
            } else {
                if(direction != 0) CmdSetDirection(0);
            }
        }

        if(!settings.displayShipInfo.isOn) {
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                displayNameText.gameObject.SetActive(true);
                healthBar.gameObject.SetActive(true);
            }

            if(Input.GetKeyUp(KeyCode.LeftShift)) {
                displayNameText.gameObject.SetActive(false);
                healthBar.gameObject.SetActive(false);
            }
        }
        else {
            if(!displayNameText.gameObject.activeInHierarchy) {
                displayNameText.gameObject.SetActive(true);
            }
            if(!healthBar.gameObject.activeInHierarchy) {
                healthBar.gameObject.SetActive(true);
            }
        }

    }

    #region server

    [Server]
    public void SetArchetype(string newArchetype) {
        archetype = newArchetype;
    }

    [Server]
    public void SetDisplayName(string newDisplayName) {
        displayName = newDisplayName;
    }

    [Server]
    private void SetDirection(int _direction){
        direction = _direction;
    }

    [Command]
    private void CmdSetDirection(int _direction) {
        SetDirection(_direction);
    }

    #endregion

    #region client

    public override void OnStartClient() {
        gameObject.GetComponent<Health>().ClientOnHealthUpdated += ClientHandleHealthUpdated;
    }

    public override void OnStopClient() {
        gameObject.GetComponent<Health>().ClientOnHealthUpdated -= ClientHandleHealthUpdated;
    }


    private void ClientHandleHealthUpdated(int currentHealth, int maxHealth) {
        healthBar.value = (float)currentHealth / maxHealth;
    }

    private void HandleDisplayNameUpdated(string oldName, string newName) {
        displayNameText.text = newName;
    }

    private void HandleDirectionUpdated(int oldDirection, int newDirection) {
        string _direction = "";
        if(newDirection == 1) _direction = "_Forward";
        if(newDirection == -1) _direction = "_Reverse";
        Sprite sp = Resources.Load<Sprite>($"Ships/Arena_{archetype}{_direction}");
        shipRenderer.sprite = sp;
    }

    private void HandleDisplayArchetype(string oldArch, string newArch) {
        Sprite sp = Resources.Load<Sprite>($"Ships/Arena_{newArch}");
        shipRenderer.sprite = sp;
    }

    #endregion
}
