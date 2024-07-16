using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GoldGroupAnim : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private Dealer dealer;
    [SerializeField] private RectTransform goldLb;
    [SerializeField] private Vector3 target_Position;
    [SerializeField] Animator animator;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private float radius;
    [SerializeField] Vector3 anchor3D;
    private int addedGold;
    [HideInInspector] public UnityEvent<int> goldClaimEvent = new();
    private void OnEnable()
    {
        goldClaimEvent = dealer.dealSlot.goldCollected;
        goldClaimEvent.AddListener(GroupGoldSpawn);
    }
    private void OnDisable()
    {
        goldClaimEvent.RemoveListener(GroupGoldSpawn);
    }
    private void Start()
    {
        StartCoroutine(GetGoldLb());
        rect = GetComponent<RectTransform>();
    
    }
    IEnumerator GetGoldLb()
    {
        ViewManager.Instance.dicView.TryGetValue(ViewIndex.GamePlayView, out BaseView view);
        yield return new WaitUntil(() => view != null);
        goldLb = view.GetComponent<GamePlayView>().GoldParent;
        //transform.SetParent(view.transform);
        ScreenToWorld.Instance.SetWorldToAnchorView(transform.position, rect);
        anchor3D = rect.anchoredPosition3D;
        target_Position = goldLb.anchoredPosition3D - anchor3D;
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
        addedGold = amountGold;
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
            yield return new WaitUntil(() => isSpawn == true);
            //yield return new WaitForSeconds(0.1f); 
            i++;
        }
        DataAPIController.instance.AddGold(addedGold, (isDone)=>isSpawn = false);
    }
    public void SpawGoldUI(Action callback)
    {
        Vector3 randomPos = RandomUIPositionAround(radius);
        //Debug.LogWarning($"random post  {randomPos}");

        //GameObject goldUI = Instantiate(goldPrefab, Vector3.zero, Quaternion.identity, transform.parent);
        GoldUI goldUI = GoldPool.Instance.pool.SpawnNonGravity();
        goldUI.Transf.SetParent(transform);
        goldUI.Rect.anchoredPosition3D = randomPos;
        goldUI.DoScaleUp(Vector3.zero, Vector3.one, () =>
        {
            goldUI.DoMoveToTarget(target_Position, () =>
            {
                if (goldUI != null)
                {
                    goldUI.Transf.SetParent(GoldPool.Instance.gameObject.transform);
                    GoldPool.Instance.pool.DeSpawnNonGravity(goldUI);
                }
                SoundManager.instance.PlaySFX(SoundManager.SFX.CoinSFX);
                callback?.Invoke();
            });
        });

    }
    Vector3 RandomUIPositionAround(float radius)
    {
        Vector3 rootPosition = anchor3D;

        //Debug.Log($"RandomUIPositionAround {rootPosition}");

        // Generate random angles for polar coordinates
        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(0f, radius);

        // Convert polar coordinates to Cartesian coordinates
        float x = rootPosition.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float y = rootPosition.y + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        // Return the random position
        //Debug.Log($"RandomUIPositionAround {new Vector3(x, y, 0)}" );
        return new Vector3(x, y, 0);
    }

}
