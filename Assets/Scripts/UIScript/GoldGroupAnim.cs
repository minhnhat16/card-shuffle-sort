using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GoldGroupAnim : MonoBehaviour
{
    [SerializeField] private RectTransform goldLb; 
    [SerializeField] private Vector3 target_Position;
    [SerializeField] Animator animator;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private int radius;

    [HideInInspector] public UnityEvent<int> goldClaimEvent = new();
    private void OnEnable()
    {
        goldClaimEvent = IngameController.instance.onDealerClaimGold;
        goldClaimEvent.AddListener(GroupGoldSpawn);
    }
    public void SetTargetPosition(Vector3 target)
    {
        target_Position = target;
    }
    public void GroupGoldSpawn(int amountGold)
    {
        StartCoroutine(SpawnGoldByTime(amountGold));
    }
    public int FixAmoutnGoldSpawn(int amountGold)
    {
        int scale = amountGold / 100;
        if (scale < 1) return (int)SizeAmoutGold.S;
        else if (scale > 1 && scale < 5) return (int)SizeAmoutGold.M;
        else if (scale > 5 && scale < 10) return (int)SizeAmoutGold.L;
        else if (scale > 10 && scale < 20) return (int)SizeAmoutGold.XL;
        else return 2;
    }
    public IEnumerator SpawnGoldByTime(int amountGold)
    {
        bool isSpawn = false;
        for (int i = 0; i < amountGold;)
        {
            isSpawn = true;
            SpawGoldUI(() =>
            {
                isSpawn = true;
            });
            yield return new WaitUntil(()=>isSpawn == true);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
    }
    public void SpawGoldUI(Action callback)
    {
        Vector3 randomPos = RandomUIPositionAround(radius);
        GameObject goldUI = Instantiate(goldPrefab, randomPos, Quaternion.identity, transform);
        goldUI.GetComponent<GoldUI>().DoScaleUp(Vector3.zero,Vector3.one);
        goldUI.GetComponent<GoldUI>().DoRotation();
        goldUI.GetComponent<GoldUI>().DoMoveToTarget(goldLb.transform.position);
        callback?.Invoke();
    }
    Vector3 RandomUIPositionAround( float radius)
    {
        Vector3 rootPosition = transform.position;

        // Generate random angles for polar coordinates
        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(0f, radius);

        // Convert polar coordinates to Cartesian coordinates
        float x = rootPosition.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = rootPosition.y + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        // Return the random position
        return new Vector3(x, y, rootPosition.z);
    }

}
