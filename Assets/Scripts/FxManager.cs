using UnityEngine;

public class FxManager : MonoBehaviour
{
    public static FxManager Instance { get; private set; }

    [SerializeField] private GameObject fxSamuraiSlashPrefab; // 슬래시 이펙트 프리팹

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 슬래시 이펙트를 지정한 위치/회전으로 생성합니다.
    /// </summary>
    /// <param name="point">생성할 기준 Transform (fxSlashPoint)</param>
    public void SpawnSlash(Transform point)
    {
        Instantiate(fxSamuraiSlashPrefab, point.position, point.rotation);
    }
}
