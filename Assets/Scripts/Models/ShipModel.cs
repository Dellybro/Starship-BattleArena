using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class ShipModel {
    public string owner;
    public string name;
    public string image;
    public string archetype;
    public string brand;
    public string origin;
    public string planet;
    public int bridgeIntegrity;
    public int hullIntegrity;
    public int engineIntegrity;
    public int fuel;
    public int bridgeDmg;
    public int hullDmg;
    public int engineDmg;
    public int currentIntegrity;
    public int shipIntegrity;

    public ShipModel() {}

    public int GetMaxHealth() {
        // max integrity = 15000 (10% stronger at most)
        float multiplier = 1f + (currentIntegrity / 150000f);
        return Mathf.FloorToInt(multiplier * 1500f);
    }

    public int FirePower() {
        // max integrity = 15000 (10% stronger at most)
        float multiplier = 1f + (currentIntegrity / 150000f);
        return Mathf.FloorToInt(multiplier * 50f);
    }

    public float SteerPower() {
        var multiplier = 1 + (bridgeIntegrity / 250 * 0.05);
        return (float)multiplier * 300f;
    }

    public float Acceleration() {
        var multiplier = 1 + (engineIntegrity / 250 * 0.075);
        return (float)multiplier * 10;
    }

    async public Task<Sprite> GetImage() {
        //Performs another web request to collect the image related to the NFT
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(this.image))
        {
            //Sends webrequest
            await webRequest.SendWebRequest();
            //Gets the image from the web request and stores it as a texture
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1000, 1000), new Vector2(0.5f, 0.5f));
            return sprite;
        }
    }

    public static ShipModel BasicModel() {
        ShipModel model = new ShipModel();

        model.owner = "0x000";
        model.name = "Basic Model";
        model.image = "https://warp-token-public.s3.amazonaws.com/starships/base/6930.jpg";
        model.archetype = "Javelin";
        model.brand = "Game";
        model.origin = "Void";
        model.planet = "Earth";
        model.bridgeIntegrity = 250;
        model.hullIntegrity = 250;
        model.engineIntegrity = 250;
        model.fuel = 250;
        model.bridgeDmg = 250;
        model.hullDmg = 250;
        model.engineDmg = 250;
        model.currentIntegrity = 250;
        model.shipIntegrity = 250;

        return model;
    }

    public ShipModel(ShipMetadata data) {
        name = data.name;
        owner = data.owner;
        image = data.image;

        for(int i = 0; i < data.attributes.Count; i++) {
            if(data.attributes[i].trait_type == ShipAttributes.Archetype) {
                archetype = data.attributes[i].value;
            }
            else if(data.attributes[i].trait_type == ShipAttributes.Brand) {
                brand = data.attributes[i].value;
            }
            else if(data.attributes[i].trait_type == ShipAttributes.Origin) {
                origin = data.attributes[i].value;
            }
            else if(data.attributes[i].trait_type == ShipAttributes.Planet) {
                planet = data.attributes[i].value;
            }
            else if(data.attributes[i].trait_type == ShipAttributes.BridgeIntegrity) {
                bridgeIntegrity = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.BridgeDmg) {
                bridgeDmg = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.EngineIntegrity) {
                engineIntegrity = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.EngineDmg) {
                engineDmg = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.HullIntegrity) {
                hullIntegrity = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.HullDmg) {
                hullDmg = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.Fuel) {
                fuel = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.CurrentIntegrity) {
                currentIntegrity = int.Parse(data.attributes[i].value);
            }
            else if(data.attributes[i].trait_type == ShipAttributes.ShipIntegrity) {
                shipIntegrity = int.Parse(data.attributes[i].value);
            }
        }
    }
}