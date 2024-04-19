using System.Collections.Generic;
using UnityEngine;

public class SkinLibControl : MonoBehaviour
{
    public static SkinLibControl Instance;

    //[SerializeField]
    //private List<int> fruitSkinId = new List<int>();
    //[SerializeField]
    //private List<string> fruitSkins = new List<string>();
    [SerializeField]
    private List<int> boxSkinID = new List<int>();
    [SerializeField]
    private List<string> boxSkins = new List<string>();
    private Dictionary<int, string> fruitSkinDic = new Dictionary<int, string>();
    private Dictionary<int, string> boxSkinDic = new Dictionary<int, string>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < boxSkins.Count; i++)
        {
            boxSkinDic.Add(boxSkinID[i], boxSkins[i]);
        }
    }

    public void InitFruitSkin()
    {
        Debug.Log("INIT FRUIT SKIN");
        var ownedSkins = DataAPIController.instance.GetAllFruitSkinOwned();
        var skinConfig = ConfigFileManager.Instance.ItemConfig.GetAllRecord();
        if (ownedSkins != null && skinConfig != null)
        {
            foreach (var id in ownedSkins)
            {
                string skinName = skinConfig[id].SpriteName;
                Debug.Log($"id owned skin {id} skin name {skinName}");
                fruitSkinDic.Add(id, skinName);
            }
        }
    }
    public string GetFruitSkinName(int id)
    {
        return fruitSkinDic[id];
    }

    public string GetDinoSkinById(int id)
    {
        return boxSkinDic[id];
    }
}
