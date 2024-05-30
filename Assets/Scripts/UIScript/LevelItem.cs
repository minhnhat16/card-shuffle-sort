using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    [SerializeField] private CardType cardType;
    [SerializeField] private Image cardImage;
    [SerializeField] private Text lb_CardName;
    [SerializeField] private Text lb_Percent;
    [SerializeField] private Slider percentSlider;
    [SerializeField] private float percent;


}
