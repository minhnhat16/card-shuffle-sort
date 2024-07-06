using UnityEngine;
using UnityEngine.UI;

public class TooglePool : MonoBehaviour
{
    public static TooglePool Instance;
    public BY_Local_Pool<Toggle> pool;
    public Toggle prefab;
    public int total;

    private void Awake()
    {
        Instance = this;
        pool = new BY_Local_Pool<Toggle>(prefab, total, this.transform);
    }
}
