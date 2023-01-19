using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class RespawnTimer : MonoBehaviour
{
    [SerializeField] Image respawnPanel;
    [SerializeField] TMP_Text killedByText;
    [SerializeField] TMP_Text respawnText;

    private double respawnTime;

    // Update is called once per frame
    void Update() {
        UpdateTimeDisplay();
    }

    public void UpdateTimeDisplay() {
        if(respawnPanel.gameObject.activeInHierarchy) {
            respawnTime -= Time.deltaTime;
            if(respawnTime <= 0f) respawnTime = 0f;
            respawnText.text = $"You will respawn in {Mathf.FloorToInt((float)respawnTime)}";
            if(respawnTime == 0) ShowDisplay(false);
        }
    }

    public void SetRespawnTime(double time, string killedBy) {
        respawnTime = time;
        respawnPanel.gameObject.SetActive(true);

        killedByText.text = $"Killed by {killedBy}";
    }

    private void ShowDisplay(bool active) {
        respawnPanel.gameObject.SetActive(active);
    }
}
