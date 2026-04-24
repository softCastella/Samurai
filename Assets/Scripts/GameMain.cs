using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{

    public Button attackBtn;
    public Button hitBtn;
    public Button deathBtn;
    public Samurai samurai;
    public Demon demon;
    public Transform fxSlashPoint; // 슬래시 이펙트 생성 위치 (Samurai 하위 오브젝트)
    [SerializeField] private Slider demonGauge; // DemonGauge 등 (0~1 슬라이더)
    [SerializeField] private GameObject winText; // 비워두면 씬에서 이름 "WinText"로 자동 검색
    [SerializeField] private int damagePerHit = 10; // 공격/히트 버튼당 데미지

    private const float WinDelaySeconds = 1f;
    private const float WinShowSeconds = 1.5f;
    private const string WinTextObjectName = "WinText";

    private void Awake()
    {
        ResolveWinText();
        HideWinTextNow();
    }

    void Start()
    {
        if (demon == null)
        {
            Debug.LogError("GameMain: demon이 비어 있습니다.");
            return;
        }
        if (samurai == null)
        {
            Debug.LogError("GameMain: samurai가 비어 있습니다.");
            return;
        }

        demon.onHpChanged += RefreshDemonGauge;
        demon.onBeforeDestroy += OnDemonBeforeDestroy;

        if (attackBtn != null)
        {
            attackBtn.onClick.AddListener(() =>
            {
                Debug.Log("클릭");
                samurai.Attack();
                if (demon != null) demon.TakeDamage(damagePerHit);
            });
        }
        else
            Debug.LogError("GameMain: attackBtn이 비어 있습니다.");

        // 히트 버튼 → 데미지 + 데몬 Hit 애니메이션
        if (hitBtn != null)
            hitBtn.onClick.AddListener(() =>
            {
                if (demon == null) return;
                demon.TakeDamage(damagePerHit);
                demon.Hit();
            });
        else
            Debug.LogError("hitBtn이 연결되지 않았습니다. GameMain Inspector에서 연결하세요.");

        // 데스 버튼 → 데몬 Death 애니메이션
        if (deathBtn != null)
            deathBtn.onClick.AddListener(() =>
            {
                if (demon != null) demon.Death();
            });
        else
            Debug.LogError("deathBtn이 연결되지 않았습니다. GameMain Inspector에서 연결하세요.");

        samurai.onAttackStart = () =>
        {
            if (FxManager.Instance == null)
            {
                Debug.LogError("FxManager가 씬에 없습니다. Hierarchy에 FxManager 오브젝트를 추가하세요.");
                return;
            }
            if (fxSlashPoint == null)
            {
                Debug.LogError("fxSlashPoint가 연결되지 않았습니다. GameMain Inspector에서 연결하세요.");
                return;
            }
            // 싱글톤 FxManager를 통해 슬래시 이펙트를 fxSlashPoint 위치에 생성
            FxManager.Instance.SpawnSlash(fxSlashPoint);
        };

        samurai.onAttackComplete = () =>
        {
            Debug.Log("공격 종료");
        };
        // 시작 시 버튼 비활성화
        if (attackBtn != null) attackBtn.gameObject.SetActive(false);
        if (hitBtn != null) hitBtn.gameObject.SetActive(false);
        if (deathBtn != null) deathBtn.gameObject.SetActive(false);

        samurai.onMoveComplete = () =>
        {
            // 사무라이가 목표지점에 도착하면 버튼 활성화
            if (attackBtn != null) attackBtn.gameObject.SetActive(true);
            if (hitBtn != null) hitBtn.gameObject.SetActive(true);
            if (deathBtn != null) deathBtn.gameObject.SetActive(true);
        };

        // 현재 위치에서 X=0 까지 X축 직선 이동 (Y, Z 유지)
        Vector3 destination = new Vector3(0f, samurai.transform.position.y, samurai.transform.position.z);
        samurai.MoveTo(destination);

        // Demon.Start보다 먼저 돌면 이벤트를 놓칠 수 있어 한 번 더 동기화
        RefreshDemonGauge(demon.CurrentHp, demon.MaxHp);
    }

    private void OnDestroy()
    {
        UnsubscribeDemonEvents();
    }

    private void UnsubscribeDemonEvents()
    {
        if (demon == null) return;
        demon.onHpChanged -= RefreshDemonGauge;
        demon.onBeforeDestroy -= OnDemonBeforeDestroy;
    }

    /// <summary>데몬 제거 직전: HP 게이지 UI 제거, 데몬 전용 버튼 비활성화</summary>
    private void OnDemonBeforeDestroy()
    {
        UnsubscribeDemonEvents();

        if (demonGauge != null && winText != null && winText.transform.IsChildOf(demonGauge.transform))
        {
            Debug.LogError("WinText가 DemonGauge 안에 있습니다. WinText를 Canvas의 다른 자식으로 옮기세요. 그렇지 않으면 게이지 삭제 시 WinText도 함께 제거됩니다.");
        }

        if (demonGauge != null)
            Destroy(demonGauge.gameObject);
        demonGauge = null;
        demon = null; // 이후 TakeDamage 등 호출 방지

        if (hitBtn != null) hitBtn.interactable = false;
        if (deathBtn != null) deathBtn.interactable = false;
        if (attackBtn != null) attackBtn.interactable = false;

        StartCoroutine(ShowWinTextRoutine());
    }

    private IEnumerator ShowWinTextRoutine()
    {
        ResolveWinText();
        if (winText == null)
        {
            Debug.LogWarning($"GameMain: '{WinTextObjectName}' 오브젝트를 찾지 못했습니다. Canvas 아래 이름을 맞추거나 Win Text 슬롯에 연결하세요.");
            yield break;
        }

        // Time.timeScale == 0 이면 WaitForSeconds는 멈춤 → 실시간 기준 대기
        yield return new WaitForSecondsRealtime(WinDelaySeconds);

        winText.SetActive(true);

        yield return new WaitForSecondsRealtime(WinShowSeconds);

        winText.SetActive(false);
    }

    private void ResolveWinText()
    {
        if (winText != null) return;

        foreach (var canvas in Resources.FindObjectsOfTypeAll<Canvas>())
        {
            if (canvas == null || !canvas.gameObject.scene.IsValid())
                continue;
            Transform found = FindDeepChild(canvas.transform, WinTextObjectName);
            if (found != null)
            {
                winText = found.gameObject;
                return;
            }
        }
    }

    private static Transform FindDeepChild(Transform parent, string objectName)
    {
        if (parent.name == objectName)
            return parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindDeepChild(parent.GetChild(i), objectName);
            if (found != null)
                return found;
        }
        return null;
    }

    private void HideWinTextNow()
    {
        if (winText != null)
            winText.SetActive(false);
    }

    private void RefreshDemonGauge(int current, int max)
    {
        if (demonGauge == null)
        {
            Debug.LogWarning("GameMain: Demon Gauge(Slider)가 비어 있습니다. Inspector에서 DemonGauge의 Slider를 연결하세요.");
            return;
        }

        // Whole Numbers + max=1 이면 값이 0/1만 되어 게이지가 안 움직이는 것처럼 보일 수 있음
        demonGauge.wholeNumbers = false;
        demonGauge.minValue = 0f;
        demonGauge.maxValue = 1f;
        demonGauge.value = max > 0 ? (float)current / max : 0f;
    }
}
