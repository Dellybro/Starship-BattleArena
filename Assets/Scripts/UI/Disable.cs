using System.Collections;
using UnityEngine;

public class Disable : MonoBehaviour {
    [SerializeField] float disableAfter = 5f;
    [SerializeField] CanvasGroup group;
    [SerializeField] bool destroy;
    bool startFade;

    private void OnEnable() {
        group.alpha = 1;
        StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate() {
        yield return new WaitForSeconds(disableAfter);
        startFade = true;
    }

    private void Update() {
        if(startFade && group.alpha > 0) {
            group.alpha -= Time.deltaTime;
        }
        else if(group.alpha == 0 && gameObject.activeInHierarchy) {
            gameObject.SetActive(false);
            if(destroy) Destroy(gameObject);
        }
    }
}
