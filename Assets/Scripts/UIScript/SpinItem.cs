using UnityEngine;
using UnityEngine.UI;

public class SpinItem : MonoBehaviour
{

    [SerializeField] int id;
    [SerializeField] SpinEnum itemType;
    [SerializeField] Currency currencyType;
    [SerializeField] int amount;
    [SerializeField] Image itemImg;
    [SerializeField] Text amount_lb;
    [SerializeField] float rotateValue;
    // Start is called before the first frame update

    public int ID {  get { return id; } }
    public SpinEnum Type { get { return itemType; } }
    public int Amount {  get { return amount; } }

    public float RotateValue { get => rotateValue; set => rotateValue = value; }

    void Start()
    {
        rotateValue = transform.localRotation.z;
    }

    // Update is called once per frame
    public void InitItem(SpinConfigRecord record)
    {
        Debug.Log(record.Type + " " + record.Id);
        id = record.Id;
        itemType = record.Type;
        amount = record.Amount;
        amount_lb = GetComponentInChildren<Text>();
        amount_lb.text = $"{amount}";
        itemImg.sprite = SpriteLibControl.Instance.GetSpriteByName(record.ItemImg);
        itemImg.SetNativeSize();
    }
    public void OnRewardItem()
    {
        //if (type == ItemType.GOLD)
        //{
        //    DataAPIController.instance.AddGold(amount);
        //}
        //else if( type  ==ItemType.CHANGE  && type == ItemType.HAMMER && type == ItemType.ROTATE)
        //{
        //    Debug.Log($"Added to data {amount} item {type} ");

        //    DataAPIController.instance.AddItemTotal(type.ToString(), amount);   
        //}
        //else if(type == ItemType.FRUITSKIN)
        //{
        //    Debug.Log($"Added to data {amount} item {type} ");
        //    DataAPIController.instance.SaveFruitSkin(amount);
        //}
    }
}
