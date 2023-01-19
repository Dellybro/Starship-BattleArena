using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;


//Used by metadata class for storing attributes
public class ShipAttributes
{

    public static string Archetype {get { return "Archetype"; } }
    public static string Brand {get { return "Brand"; } }
    public static string Origin {get { return "Origin"; } }
    public static string Planet {get { return "Docked Planet"; } }
    public static string CurrentIntegrity {get { return "Current Integrity"; } }
    public static string ShipIntegrity {get { return "Ship Integrity"; } }
    public static string BridgeIntegrity {get { return "Bridge Integrity"; } }
    public static string HullIntegrity {get { return "Hull Integrity"; } }
    public static string EngineIntegrity {get { return "Engine Integrity"; } }
    public static string Fuel {get { return "Fuel"; } }
    public static string BridgeDmg {get { return "Bridge Damage"; } }
    public static string HullDmg {get { return "Hull Damage"; } }
    public static string EngineDmg {get { return "Engine Damage"; } }

    //The type or name of a given trait
    public string trait_type;
    //The value associated with the trait_type
    public string value;
}

//Used for storing NFT metadata from standard NFT json files
public class ShipMetadata
{
    // Token id
    public string tokenId { get; set; }
    //List storing attributes of the NFT
    public List<ShipAttributes> attributes { get; set; }
    //Description of the NFT
    public string description { get; set; }
    //An external_url related to the NFT (often a website)
    public string external_url { get; set; }
    //image stores the NFTs URI for image NFTs
    public string image { get; set; }
    //Name of the NFT
    public string name { get; set; }
    // owner of the nft
    public string owner { get; set; }
}

public class ReaderReturn {
    public string uri {get; set;}
}

public class ReaderContract {
    public static async Task<List<ShipMetadata>> GetStarships(string _chain, string _network, string _contract, string _account, string _rpc="") {
        List<ShipMetadata> metadatas = new List<ShipMetadata>();

        try {
            string method = "getStarships";
            string[] obj = { _account };
            string args = JsonConvert.SerializeObject(obj);
            string response = await EVM.Call(_chain, _network, _contract, ABI.STARSHIP_READER_ABI, method, args, _rpc);

            string[] result = JsonConvert.DeserializeObject<string[]>(response);

            for(int i = 0; i < result.Length; i++) {
                string URI = result[i];
                URI = URI.Substring(URI.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(URI);
                string decodedText = Encoding.UTF8.GetString(data);
                ShipMetadata metadata = JsonConvert.DeserializeObject<ShipMetadata>(decodedText);
                metadata.owner = _account;
                metadatas.Add(metadata);
            }

        }
        catch {
            Debug.LogError("An Error occured");
            throw;
        }

        return metadatas;
    }

}

public class ShipContract : ERC721
{
    public static async Task<List<ShipMetadata>> TokensOfOwner(string _chain, string _network, string _contract, string _account, string _rpc="", string _multicall="") {
        List<ShipMetadata> metadatas = new List<ShipMetadata>();

        int balance = await ERC721.BalanceOf(_chain, _network, _contract, _account, _rpc);
        string method = "tokenOfOwnerByIndex";
        string[][] obj = new string[balance][];
        for (int i = 0; i < balance; i++) {
            obj[i] = new string[2] { _account, i.ToString() };
        };
        string args = JsonConvert.SerializeObject(obj);
        string response = await EVM.MultiCall(_chain, _network, _contract, ABI.STARSHIP_ABI, method, args, _multicall, _rpc);

        try {
            string[] responses = JsonConvert.DeserializeObject<string[]>(response);
            for (int i = 0; i < responses.Length; i++) {
                // clean up address
                string tokenId = responses[i];
                string URI = await ERC721.URI(_chain, _network, _contract, tokenId, _rpc);
                URI = URI.Substring(URI.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(URI);
                string decodedText = Encoding.UTF8.GetString(data);
                ShipMetadata metadata = JsonConvert.DeserializeObject<ShipMetadata>(decodedText);
                metadata.tokenId = tokenId;
                metadata.owner = _account;
                metadatas.Add(metadata);
            }
        }
        catch {
            Debug.LogError(response);
            throw;
        }

        return metadatas;
    }
}
