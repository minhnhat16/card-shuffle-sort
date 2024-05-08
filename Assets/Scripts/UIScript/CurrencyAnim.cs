using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurrencyAnim:MonoBehaviour
{
    public virtual void Start()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void DoScaleUp(Vector3 fromScale, Vector3 toScale)
    {
        transform.localScale = fromScale;
        Tween t = transform.DOScale(toScale, 0.1f).SetEase(Ease.OutBounce);
        t.OnComplete(() => t.Kill());

    }
    public virtual void DoRotation()
    {
        Tween t = transform.DORotateQuaternion(new Quaternion(0, 0, 0, 360f), 10f);
        t.OnComplete(() => t.Kill());
    }
    public virtual void DoMoveToTarget(Vector3 target)
    {
        Tween t = transform.DOMove(target, 0.75f).SetEase(Ease.InQuad);
        t.OnComplete(() =>
        {
            t.Kill();
            Destroy(gameObject);
        });
    }
}
