using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpriteLibControl : MonoBehaviour
{
    public static SpriteLibControl Instance;

    //[SerializeField] 
    //private List<Sprite> _sprite;
    //readonly private Dictionary<string, Sprite> spriteDict = new();

    private void Awake()
    {
        Instance = this;
    }
    private void Start ()
    {
        //foreach(var sprite in _sprite)
        //{
        //    //Debug.Log(sprite.name.ToString ());
        //    //Debug.Log(sprite.name);
        //    spriteDict.Add(sprite.name, sprite);
        //}
    }

    //public Sprite GetSpriteByName(string name)
    //{
    //    //Debug.Log($"GetSpriteByName{name}");
    //    //if (spriteDict.ContainsKey(name)) return spriteDict[name];
    //    else
    //    {
    //        //Debug.Log($"GetSpriteByName:{name} == null");
    //       return null;
    //    //}
    //}
    public string GetCircleSpriteName(int type ,int id)
    {
        string name;
        int idType =  type -3;
        switch (idType)
        {
            case 1: //SLICE SPRITE
                 name = GetSliceSprite(id);
                return name;
            case 2: //FRUIT SPRITE
                 name = GetFruitSprite(id);
                return name;
            case 3: //DONUTS
                name = GetDonutSprite(id);
                Debug.Log(name);
                return name;
            case 4: //COOKIES
                 name = GetCookies(id);
                return name;
            case 5: //CANDY
                name = GetCandySkins(id);
                return name;
            case 6: //FASTFOOD
                name = GetFastFoodSkins(id);
                return name;
            case 7: //JAPANESE FOOD
                name = GetJapanFoodSkins(id);
                return name; 
            case 8: //CHINESE FOOD
                name = GetChineseFoodSkins(id);
                return name;
            default: return null;
        }
    }
    private string GetChineseFoodSkins(int id)
    {
        id++;
        string name = $"{id}_Chinese food";
        if (id > 0 && id <= 11)
        {
            return name;
        }
        else return null;
    }
    private string GetJapanFoodSkins(int id)
    {
        id++;
        string name = $"{id}_Japanfood";
        if (id > 0 && id <= 11)
        {
            return name;
        }
        else return null;
    }
    private string GetCandySkins(int id)
    {
        id++;
        string name = $"{id}_Candy";
        if (id > 0 && id <= 11)
        {
            return name;
        }
        else return null;
    }
    private string GetFastFoodSkins(int id)
    {
        id++;
        string name = $"{id}_Fastfood";
        if (id > 0 && id <= 11)
        {
            return name;
        }
        else return null;
    }
    private string GetCookies(int id)
    {
        id++;
        string name =  $"{id}_Cookie";
        if (id > 0 && id <= 11)
        {
            return name;
        }
        else return null;
    }
    private string GetDonutSprite(int id)
    {
        id++;
        string name = $"{id}_Donut";
        if (id >0 && id <=11)
        {

            Debug.Log("Name" + name);
            return name;
        }
        else return null;
       
    }

    public string GetSliceSprite(int id)
    {
        id++;
        string name = $"{id}_Slice";
        if (id > 0 && id <= 11)
        {

            //Debug.Log("Name" + name);
            return name;
        }
        else return null;
    }
    public string GetFruitSprite(int id)
    {
        id++;
        string name = $"{id}_Fruit";
        if (id > 0 && id <= 11)
        {

            //Debug.Log("Name" + name);
            return name;
        }
        else return null;
    }
}
