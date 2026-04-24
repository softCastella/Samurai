using UnityEngine;

public class FxManager : MonoBehaviour
{
    public static FxManager Instance { get; private set; }

    [SerializeField] private GameObject fxSamuraiSlashPrefab; // 슬래시 이펙트 프리팹

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("FxManager가 씬에 여러 개입니다. 하나만 두세요.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 슬래시 이펙트를 지정한 위치/회전으로 생성합니다.
    /// </summary>
    /// <param name="point">생성할 기준 Transform (fxSlashPoint)</param>
    public void SpawnSlash(Transform point)
    {
        if (fxSamuraiSlashPrefab == null)
        {
            Debug.LogError("FxManager: fxSamuraiSlashPrefab이 비어 있습니다.");
            return;
        }
        if (point == null)
        {
            Debug.LogError("FxManager: 슬래시 생성 위치 Transform이 null입니다.");
            return;
        }
        Instantiate(fxSamuraiSlashPrefab, point.position, point.rotation);
    }
}
