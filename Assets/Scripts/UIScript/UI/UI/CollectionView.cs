using System;
using UnityEngine;
using UnityEngine.UI;

public class CollectionView : BaseView
{
    [SerializeField] private Image bgImg;
    [SerializeField] private Image fill_collection;
    [SerializeField] private Button returnBtn;
    [SerializeField] private CollectionCards collection;

    public override void OnInit(Action callback)
    {   
        base.OnInit(callback);
        collection = GetComponentInChildren<CollectionCards>();
        collection.Init();
    }
    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
    }
}

