using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CollectionCards : MonoBehaviour
{

    [SerializeField] private List<CollectionItem>collections = new List<CollectionItem>();
    [SerializeField] private RectTransform content;
    [SerializeField] private Slider cardTotalSlider;
    [SerializeField] private Text cardTotalLb;

    private ColorConfig cardColorConfig;
    private ListCardColor listCardColor;
    private CardType cardType;
    public void InitCoroutine()
    {
        Debug.Log("Collection coroutine start");
        StartCoroutine(Init());

    }
    IEnumerator Init()
    {
        // Wait for both initializations to complete
        yield return new WaitUntil(() => DataAPIController.instance.isInitDone && ConfigFileManager.Instance.isDone);

        // Cache references to frequently accessed properties and methods
        var dataAPI = DataAPIController.instance;
        var configFileManager = ConfigFileManager.Instance;
        cardType = dataAPI.GetCurrentCardType();
        cardColorConfig = configFileManager.ColorConfig;

        var colors = cardColorConfig.GetAllRecord();
        listCardColor = dataAPI.GetDataColorByType(cardType);

        // Load the prefab once and cache it
        var collectionItemPrefab = Resources.Load("Prefabs/UIPrefab/CollectionItem", typeof(CollectionItem)) as CollectionItem;

        for (int i = 0; i < colors.Count; i++)
        {
            yield return StartCoroutine(InitializeCollectionItem(collectionItemPrefab, colors[i], i));
        }

        // Update card count and fill count
        CardTotalCount();
        FillCount(listCardColor.color.Count, colors.Count);
    }

    IEnumerator InitializeCollectionItem(CollectionItem prefab, ColorConfigRecord color, int index)
    {
        // Instantiate a new CollectionItem and set its properties
        CollectionItem newItem = Instantiate(prefab, content) as CollectionItem;
        Sprite sprite = SpriteLibControl.Instance.GetCardSpriteByCategory(cardType, index);

        newItem.CardImg.sprite = sprite;
        bool isColorPresent = listCardColor.color.Contains(color.Name);
        newItem.LockSprite(!isColorPresent);

        // Add the new item to the collections list
        collections.Add(newItem);

        // Optionally wait for a frame to ensure this item is initialized before proceeding
        yield return null;
    }

    private void CardTotalCount()
    {
        cardTotalLb.text = $"{listCardColor.color.Count}/{cardColorConfig.GetAllRecord().Count}";
    }
    private void FillCount(int dataOwnCard,int totalCard)
    {
        cardTotalSlider.value =(float)dataOwnCard / (float)totalCard;
    }
}
