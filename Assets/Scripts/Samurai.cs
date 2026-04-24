using System.Collections;
using UnityEngine;
using System;

public class Samurai : MonoBehaviour
{
    // 애니메이터 파라미터 int형식 State 0:idle 1:Attack 2:Run
    private static readonly int State = Animator.StringToHash("State");

    private Animator animator;
    [SerializeField] private float moveSpeed = 5f; // 초당 이동 거리 (유닛/초)

    private bool isAttacking = false; // 공격 중복 실행 방지 플래그

    public Action onMoveComplete;
    public Action onAttackStart;    // 공격 애니메이션 시작 시 호출
    public Action onAttackComplete; // 공격 애니메이션 종료 시 호출

    private void Awake()
    {
        // 같은 게임오브젝트에서 Animator 자동으로 가져오기
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// 공격 애니메이션을 재생하고 종료 후 Idle로 복귀합니다.
    /// </summary>
    public void Attack()
    {
        // 공격 중이면 무시
        if (isAttacking) return;

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        animator.SetInteger(State, 1); // Attack 애니메이션 시작
        onAttackStart?.Invoke();       // 공격 시작 콜백 호출

        // 현재 애니메이션이 Attack 상태로 전환될 때까지 대기
        yield return null;

        // Attack 애니메이션이 끝날 때까지 대기
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        animator.SetInteger(State, 0); // 애니메이션 종료 후 Idle 복귀
        isAttacking = false;
        onAttackComplete?.Invoke();    // 공격 종료 콜백 호출
    }

    /// <summary>
    /// 목표 위치(tpos)까지 이동하며 애니메이션을 전환합니다.
    /// </summary>
    /// <param name="tpos">이동할 목표 월드 좌표</param>
    public void MoveTo(Vector3 tpos)
    {
        StartCoroutine(MoveCoroutine(tpos));
    }

    private IEnumerator MoveCoroutine(Vector3 tpos)
    {
        animator.SetInteger(State, 2); // Run 애니메이션 시작

        // 목표 지점에 도달할 때까지 매 프레임 이동
        while (Vector3.Distance(transform.position, tpos) > 0.05f)
        {
            // 거리 / 속도 = 이동에 걸리는 시간(초) 기반으로 등속 이동
            transform.position = Vector3.MoveTowards(transform.position, tpos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = tpos;          // 목표 위치로 정확히 고정
        animator.SetInteger(State, 0);      // 도착 후 Idle 복귀
        onMoveComplete?.Invoke();           // 도착 콜백 호출
    }
}
