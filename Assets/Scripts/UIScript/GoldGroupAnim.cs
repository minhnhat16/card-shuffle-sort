using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class GoldGroupAnim : MonoBehaviour
{
    [SerializeField] private Dealer dealer;
    [SerializeField] private RectTransform goldLb; 
    [SerializeField] private Vector3 target_Position;
    [SerializeField] Animator animator;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private float radius;

    [HideInInspector] public UnityEvent<int> goldClaimEvent = new();
    private void OnEnable()
    {
        goldClaimEvent = dealer.dealSlot.goldCollected;
        goldClaimEvent.AddListener(GroupGoldSpawn);
    }
    private void Start()
    {
        StartCoroutine(GetGoldLb());
    }
    IEnumerator GetGoldLb()
    {
        yield return new WaitUntil(() =>
        {
            if (ViewManager.Instance.dicView.TryGetValue(ViewIndex.GamePlayView, out BaseView gamePlayViewObj))
            {
                if (gamePlayViewObj.TryGetComponent(out GamePlayView gamePlayView))
                {
                    goldLb = gamePlayView.GoldLb.rectTransform;
                    target_Position = ScreenToWorld.Instance.ConvertPositionNew(goldLb.anchoredPosition3D);
                    Debug.Log($"target post = {target_Position}");
                    return true; // Exit the loop once the condition is met
                }
            }
            return false; // Continue waiting
        });

        // Timeout handling
        if (goldLb == null)
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
    public void GroupGoldSpawn(int amountGold)
    {
        //Debug.Log("GroupGoldSpawn");
        int fixedGold = FixAmountGoldSpawn(amountGold);
        StartCoroutine(SpawnGoldByTime(fixedGold));
    }
    public int FixAmountGoldSpawn(int amountGold)
    {
        int scale = amountGold / 1000;
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
            yield return new WaitForSeconds(0.1f);
            i++;
        }
    }
    public void SpawGoldUI(Action callback)
    {
        Vector3 randomPos = RandomUIPositionAround(radius);
        Debug.LogWarning($"random post  {randomPos}");

        GameObject goldUI = Instantiate(goldPrefab, Vector3.zero, Quaternion.identity, transform.parent);
        goldUI.GetComponent<RectTransform>().anchoredPosition3D = randomPos;
        //goldUI.GetComponent<RectTransform>().anchoredPosition = ScreenToWorld.Instance.ConvertPosition(randomPos);
        goldUI.GetComponent<GoldUI>().DoScaleUp(Vector3.zero,Vector3.one);
        Debug.LogWarning($"gold lb post {goldLb.rect.position}");
        goldUI.GetComponent<GoldUI>().DoMoveToTarget(target_Position);
        SoundManager.instance.PlaySFX(SoundManager.SFX.CoinSFX);
        callback?.Invoke();
    }
    Vector3 RandomUIPositionAround(float radius)
    {
        Vector3 rootPosition = GetComponent<RectTransform>().anchoredPosition3D;

        Debug.Log($"RandomUIPositionAround {rootPosition}");

        // Generate random angles for polar coordinates
        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(0f, radius);

        // Convert polar coordinates to Cartesian coordinates
        float x = rootPosition.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = rootPosition.y + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        // Return the random position
        Debug.Log($"RandomUIPositionAround {new Vector3(x, y, 0)}" );
        return new Vector3(x, y,0);
    }

}
