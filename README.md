# Samurai - Unity 2D 액션 게임

Unity로 제작한 2D 사무라이 액션 게임 프로젝트입니다.

## 게임 흐름

1. 사무라이가 화면 왼쪽 끝에서 `(0, 0, 0)` 지점까지 자동으로 달려옵니다.
2. 도착 후 **Attack 버튼**이 등장합니다.
3. 버튼을 누르면 공격 애니메이션과 함께 슬래시 이펙트가 발생합니다.

## 주요 구조

| 스크립트 | 역할 |
|---|---|
| `Samurai.cs` | 이동(MoveTo), 공격(Attack) 로직 및 애니메이션 전환 |
| `GameMain.cs` | 씬 초기화, 버튼 이벤트, 사무라이 콜백 연결 |
| `FxManager.cs` | 싱글톤 이펙트 매니저, 슬래시 이펙트 생성 |

## 애니메이션 파라미터

Animator Controller의 `State` (int) 파라미터로 상태를 제어합니다.

| 값 | 상태 |
|---|---|
| 0 | Idle |
| 1 | Attack |
| 2 | Run |

## 주요 기능

- **코루틴 기반 이동**: `Vector3.MoveTowards`로 등속 이동
- **공격 중복 방지**: `isAttacking` 플래그로 연속 입력 차단
- **콜백 시스템**: `onMoveComplete`, `onAttackStart`, `onAttackComplete`
- **싱글톤 FxManager**: 어디서든 `FxManager.Instance.SpawnSlash()` 호출 가능

## 개발 환경

- Unity 2022 이상
- C#
