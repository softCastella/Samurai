using System;
using System.Collections;
using UnityEngine;

public class Demon : MonoBehaviour
{
    // 애니메이터 파라미터 정보
    // - int  State : 0 = Idle, 1 = Hit
    // - trigger Death : false = 생존, true = 사망

    private static readonly int StateParam = Animator.StringToHash("State");
    private static readonly int DeathParam = Animator.StringToHash("Death"); // Trigger
    private static readonly int HitStateHash = Animator.StringToHash("Demon_Hit"); // Play()용 상태 이름 해시

    private const string DeathStateName = "Demon_Death"; // Animator 상태 이름(컨트롤러와 동일해야 함)

    private Animator animator;
    private bool isDead = false;       // 사망 후 중복 처리 방지
    private Coroutine hitCoroutine;    // 진행 중인 Hit 코루틴 참조

    [SerializeField] private int maxHp = 100;
    private int hp;

    public int CurrentHp => hp;
    public int MaxHp => maxHp;
    public bool IsDead => isDead;

    /// <summary>체력 변경 시 (현재 HP, 최대 HP)</summary>
    public event Action<int, int> onHpChanged;

    /// <summary>Destroy(gameObject) 직전에 호출됩니다.</summary>
    public event Action onBeforeDestroy;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Demon: Animator 컴포넌트가 없습니다.");
        if (maxHp < 1) maxHp = 100;
        hp = maxHp;
    }

    private void Start()
    {
        onHpChanged?.Invoke(hp, maxHp);
    }

    /// <summary>피해를 입힙니다. HP가 0 이하면 Death가 호출됩니다.</summary>
    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0) return;

        hp = Mathf.Max(0, hp - amount);
        onHpChanged?.Invoke(hp, maxHp);

        if (hp <= 0)
            Death();
    }

    /// <summary>
    /// Hit 애니메이션 재생 후 Idle로 복귀합니다.
    /// 재생 중 다시 호출하면 처음부터 재시작합니다.
    /// </summary>
    public void Hit()
    {
        if (isDead) return; // 사망 상태에선 무시

        // 진행 중인 Hit 코루틴이 있으면 중단 후 재시작
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        // Play()는 '상태 이름' 해시를 써야 함 (파라미터 이름 State와 다름)
        animator.Play(HitStateHash, 0, 0f);
        animator.SetInteger(StateParam, 1); // Hit 상태 유지

        // Hit 상태로 전환될 때까지 대기
        yield return null;

        // Hit 애니메이션이 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        animator.SetInteger(StateParam, 0); // Idle 복귀
        hitCoroutine = null;
    }

    /// <summary>
    /// Death 애니메이션 재생 후 오브젝트를 제거합니다.
    /// </summary>
    public void Death()
    {
        if (isDead) return; // 중복 방지
        isDead = true;
        hp = 0;
        onHpChanged?.Invoke(hp, maxHp);

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        animator.SetTrigger(DeathParam);
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        // Death 상태로 전환될 때까지 대기
        yield return null;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(DeathStateName))
        {
            yield return null;
        }

        // Death 애니메이션 종료까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        onBeforeDestroy?.Invoke();
        Destroy(gameObject);
    }
}
