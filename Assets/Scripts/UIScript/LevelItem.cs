using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    [SerializeField] private float percent;
    [SerializeField] private bool isUnlocked;
    [SerializeField] private CardType cardType;
    [SerializeField] private Image cardImage;
    [SerializeField] private Text lb_CardName;
    [SerializeField] private Text lb_Percent;
    [SerializeField] private Slider percentSlider;
    [SerializeField] private List<CardColorPallet> _listCardColor;


    public Image CardImage { get => cardImage; set => cardImage = value; }
    public Text Lb_CardName { get => lb_CardName; set => lb_CardName = value; }
    public Text Lb_Percent { get => lb_Percent; set => lb_Percent = value; }
    public Slider PercentSlider { get => percentSlider; set => percentSlider = value; }
    public float Percent { get => percent; set => percent = value; }
    public CardType CardType { get => cardType; set => cardType = value; }
    public List<CardColorPallet> ListCardColor { get => _listCardColor; set => _listCardColor = value; }

    public LevelItem(CardType cardType, Image cardImage, Text lb_CardName, Text lb_Percent, Slider percentSlider, float percent, List<CardColorPallet> listCardColor)
    {
        this.CardType = cardType;
        this.CardImage = cardImage;
        this.Lb_CardName = lb_CardName;
        this.Lb_Percent = lb_Percent;
        this.PercentSlider = percentSlider;
        this.Percent = percent;
        this._listCardColor = listCardColor;
    }
    public void Init()
    {
        cardImage.sprite = SpriteLibControl.Instance.GetSpriteByName($"{(int)CardType+1}_card_{cardType.ToString().ToLower()}");
        lb_CardName.text = $"{(int)CardType + 1}. {CardType}";
        isUnlocked = CheckUnlock();
        percent = PercentCalculator();
        percentSlider.value = percent /100;
        lb_Percent.text = $"{percent}%";
    }
    public bool CheckUnlock()
    {
        if (_listCardColor is null || _listCardColor is null) return false;
        else return true;
    }
    public float PercentCalculator()
    {
        var colorConfig = ConfigFileManager.Instance.ColorConfig.GetAllRecord();
        if (_listCardColor is null) return 0;
        int totalCardOwn = _listCardColor.Count;
        int totalCardColor = colorConfig.Count;
        float percent = Mathf.Round(((float)totalCardOwn / (float)totalCardColor) * 100) ;
        ////Debug.Log("percent" + percent);
        return percent;
    }
}
