using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ConnectWallet : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    public static Action<string, int> AccountConnected;

    [SerializeField] Image web3Panel;


    private int expirationTime;
    private string account; 
    private int chainId; 

    public void OnLogin() {
        #if UNITY_WEBGL && !UNITY_EDITOR
        Web3Connect();
        #endif
        OnConnected();
    }

    #pragma warning disable
    async private void OnConnected() {
        #if UNITY_WEBGL && !UNITY_EDITOR
        account = ConnectAccount();
        while (account == "") {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };
        chainId = Web3GL.Network();
        #else
        account = "0x1567c5ACC88af853125Bf1b4c418341f6Fc3D4A5";
        chainId = 137;
        #endif
        // save account for next scene
        PlayerPrefs.SetString("Account", account);
        PlayerPrefs.SetInt("ChainId", chainId);
        // reset login message
        // SetConnectAccount("");
        AccountConnected?.Invoke(account, chainId);

        web3Panel.gameObject.SetActive(false);
    }

}
