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
    private void Start()
    {
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        yield return new WaitUntil(()=> DataAPIController.instance.isInitDone && ConfigFileManager.Instance.isDone);
        cardType = DataAPIController.instance.GetCurrentCardType();
        cardColorConfig = ConfigFileManager.Instance.ColorConfig;
        var colors = cardColorConfig.GetAllRecord();
        listCardColor = DataAPIController.instance.GetDataColorByType(cardType);
        foreach (var c in colors)
        {
            int i = colors.IndexOf(c);
            CollectionItem newItem = Instantiate(Resources.Load("Prefabs/UIPrefab/CollectionItem", typeof(CollectionItem)),content) as CollectionItem;
            Sprite sprite = SpriteLibControl.Instance.GetCardSpriteByCategory(cardType, i);
            if (listCardColor.color.Contains(c.Name))
            {
                newItem.CardImg.sprite = sprite;
                newItem.LockSprite(false);
                collections.Add(newItem);

            }
            else
            {
                newItem.CardImg.sprite = sprite;
                newItem.LockSprite(true);
                collections.Add(newItem);
            }
        }
        CardTotalCount();
        FillCount(listCardColor.color.Count, colors.Count);
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
