using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ShipItem : MonoBehaviour, IPointerClickHandler {
    public ShipModel shipData;
    [SerializeField] Image shipImage;
    [SerializeField] Text itemName;
    [SerializeField] Text power;
    [SerializeField] Text speed;
    [SerializeField] Text maxHP;
    [SerializeField] Text steering;
    public static Action<ShipItem> ShipSelected;

    public async void SetShipData(ShipModel _shipData) {
        shipData = _shipData;
        itemName.text = shipData.name;
        power.text = $"{shipData.FirePower()}";
        steering.text = $"{shipData.SteerPower()}";
        speed.text = $"{shipData.Acceleration()}";
        maxHP.text = $"{shipData.GetMaxHealth()}";
        shipImage.sprite = await shipData.GetImage();
    }

    public void OnPointerClick(PointerEventData eventData) {
        ShipSelected?.Invoke(this);
    }
}
