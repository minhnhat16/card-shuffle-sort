using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardColorPallet cardColor;
    public SpriteRenderer sprite;
    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    public Tween PlayAnimation(Slot targetSlot, float duration, float height, Ease e, float offsetY, float offsetZ, float delay)
    {
        Debug.Log("targetSlot pos " + targetSlot.transform.position);
        var rotationVector = new Vector3();
        var currentRotation = transform.rotation.eulerAngles;

        rotationVector = GetMovementDirection(targetSlot.transform) switch
        {
            Direction.Up => new Vector3(currentRotation.x + 180, 0, 0),
            Direction.Down => new Vector3(currentRotation.x - 180, 0,0 ),
            Direction.Right => new Vector3(0, 0, currentRotation.z - 180 ),
            Direction.Left => new Vector3(0, 0, currentRotation.z + 180),
            Direction.UpperRight => new Vector3(currentRotation.x + 180, 0, 0 ),
            Direction.UpperLeft => new Vector3(currentRotation.x + 180, 0, 0),
            Direction.DownRight => new Vector3(currentRotation.x - 180, 0, 0),
            Direction.DownLeft => new Vector3(currentRotation.x - 180, 0, 0),

            _ => rotationVector
        };

        var position = targetSlot.transform.position;
        var p = new Vector3(position.x, offsetY, offsetZ );

        //Debug.Log("Position " + p);
        Vector3 addZ = new Vector3(0, 0, -5);
        Tween tween = transform.DOJump(p + addZ, height, 1, duration).SetEase(e).SetDelay(delay);
        transform.DORotate(rotationVector, duration).SetEase(e).SetDelay(delay).OnComplete(() =>
        {
            transform.position -= addZ;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            tween.Kill();
        });

        return tween;

    }

    private Direction GetMovementDirection(Transform target)
    {
        var dir = target.position - transform.position;

        var result = dir.x switch
        {
            0 when dir.z > 0 => Direction.Up,
            0 when dir.z < 0 => Direction.Down,
            > 0 when dir.z == 0 => Direction.Right,
            < 0 when dir.z == 0 => Direction.Left,
            > 0 when dir.z > 0 => Direction.UpperRight,
            < 0 when dir.z > 0 => Direction.UpperLeft,
            > 0 when dir.z < 0 => Direction.DownRight,
            < 0 when dir.z < 0 => Direction.DownLeft,
            _ => Direction.Unknown
        };

        return result;
    }
    internal void ColorSetBy(CardColorPallet cardColor,CardType currentType)
    {
        this.cardColor = cardColor;
        //sprite.color = color;
        sprite.sprite = SpriteLibControl.Instance.GetCardSpriteByCategory(currentType,(int)cardColor);
    }
}
