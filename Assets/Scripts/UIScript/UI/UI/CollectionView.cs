using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectionView : BaseView
{
    [SerializeField] private Image bgImg;
    [SerializeField] private Image fill_collection;
    [SerializeField] private Button returnBtn;
    [SerializeField] private CollectionCards collection;

    public override void OnInit()
    {   
        base.OnInit();
        collection = GetComponentInChildren<CollectionCards>();
        
    }
    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
        collection.InitCoroutine();
    }
}

