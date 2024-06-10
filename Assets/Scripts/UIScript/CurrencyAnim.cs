using DG.Tweening;
using System;
using UnityEngine;

public abstract class CurrencyAnim : MonoBehaviour
{
    public virtual void Start()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void DoScaleUp(Vector3 fromScale, Vector3 toScale, Action callback)
    {
        transform.localScale = fromScale;
        Tween t = transform.DOScale(toScale, 0.25f).SetEase(Ease.OutBounce);
        t.OnComplete(() =>
        {
            callback?.Invoke();
            t.Kill();
        });
    }
    public virtual void DoRotation(Action callback)
    {
        Tween t = transform.DORotateQuaternion(new Quaternion(0, 0, 0, 360f), 4f);
        t.OnComplete(() => 
        {
            callback?.Invoke();
            t.Kill();
            });

    }
    public virtual void DoMoveToTarget(Vector3 to)
    {
        Tween t = GetComponent<RectTransform>().DOAnchorPos3D(to, 1f).SetEase(Ease.InQuad);
        t.OnComplete(() =>
        {
            t.Kill(true);
            Destroy(gameObject);
        });
    }
    public virtual void DoPathTo(Vector3 to)
    {
        int segments = 100;
        float curveIntensity = 100f;
        Vector3[] pathPoints = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float curveFactor = Mathf.Sin(t * Mathf.PI); // Hàm s? ?? t?ng ?? cong (?ây ch? là m?t ví d?)
            Vector3 curvePoint = Vector3.Lerp(transform.position, to, t) + transform.right * curveFactor * curveIntensity;
            pathPoints[i] = curvePoint;
        }

        transform.DOPath(pathPoints, 0.75f, PathType.CatmullRom);
    }
}
