using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GemGroup : MonoBehaviour
{
    [SerializeField] private Dealer dealer;
    [SerializeField] private RectTransform gemlb;
    [SerializeField] private Vector3 target_Position;
    [SerializeField] Animator animator;
    [SerializeField] private GameObject gemPrefab;
    [SerializeField] private int radius;

    [HideInInspector] public UnityEvent<int> gemClaimEvent = new();
    private void OnEnable()
    {
        gemClaimEvent = dealer.dealSlot.goldCollected;
        gemClaimEvent.AddListener(GroupGemSpawn);
    }
    private void Start()
    {
        StartCoroutine(GetGemLb());
    }
    IEnumerator GetGemLb()
    {
        yield return new WaitUntil(() =>
        {
            if (ViewManager.Instance.dicView.TryGetValue(ViewIndex.GamePlayView, out BaseView gamePlayViewObj))
            {
                if (gamePlayViewObj.TryGetComponent(out GamePlayView gamePlayView))
                {
                    gemlb = gamePlayView.GemLB.rectTransform;
                    return true; // Exit the loop once the condition is met
                }
            }
            return false; // Continue waiting
        });

        // Timeout handling
        if (gemlb == null)
        {
            Debug.LogError("Timed out waiting for GamePlayView or GoldLb.");
            // Handle the timeout gracefully, e.g., display an error message or fallback behavior
        }
    }
    public void SetPositionWithParent(GameObject parent)
    {
        transform.position = parent.transform.position;
    }
    public void SetTargetPosition(Vector3 target)
    {
        target_Position = target;
    }
    public void GroupGemSpawn(int amountGem)
    {
        Debug.Log("GroupGoldSpawn");
        StartCoroutine(SpawnByTime(amountGem));
    }
    public int FixAmountSpawn(int amountGold)
    {
        int scale = amountGold / 100;
        if (scale < 1) return (int)SizeAmoutGold.S;
        else if (scale > 1 && scale < 5) return (int)SizeAmoutGold.M;
        else if (scale > 5 && scale < 10) return (int)SizeAmoutGold.L;
        else if (scale > 10 && scale < 20) return (int)SizeAmoutGold.XL;
        else return 2;
    }
    public IEnumerator SpawnByTime(int amountGold)
    {
        bool isSpawn = false;
        for (int i = 0; i < amountGold;)
        {
            isSpawn = true;
            SpawGoldUI(() =>
            {
                isSpawn = true;
            });
            yield return new WaitUntil(() => isSpawn == true);
            yield return new WaitForSeconds(0.05f);
            i++;
        }
    }
    public void SpawGoldUI(Action callback)
    {
        Vector3 randomPos = RandomUIPositionAround(radius);
        GameObject goldUI = Instantiate(gemPrefab, randomPos, Quaternion.identity, transform);
        goldUI.GetComponent<GoldUI>().DoScaleUp(Vector3.zero, Vector3.one);
        goldUI.GetComponent<GoldUI>().DoMoveToTarget(gemlb.transform.position);
        callback?.Invoke();
    }
    Vector3 RandomUIPositionAround(float radius)
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
