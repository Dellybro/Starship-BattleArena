using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventLogs : MonoBehaviour
{
    [SerializeField] Text textPrefab;  
    [SerializeField] GameObject content;


    public void AddLog(string _text) {
        Text instance = Instantiate(textPrefab,Vector3.zero, Quaternion.identity, content.transform);
        instance.text = _text;
    }

    public void AddKillLog(string killed, string killer) {
        Text instance = Instantiate(textPrefab,Vector3.zero, Quaternion.identity, content.transform);
        instance.text = $"{killed} was destroyed by {killer}";
    }
}
