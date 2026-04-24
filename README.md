# Samurai - Unity 2D 액션 게임

Unity로 제작한 2D 사무라이 대 데몬 액션 프로토타입입니다.

## 게임 흐름

1. **사무라이**가 씬에 배치된 현재 위치에서 **X = 0**까지 수평 이동합니다 (Y·Z 유지).
2. 도착 후 **Attack / Hit / Death** 버튼이 활성화됩니다.
3. **Attack**: 사무라이 공격 애니 + 슬래시 이펙트 + 데몬에게 데미지.
4. **Hit**: 데몬 피격 애니 + 데미지 (연타 시 히트 애니는 처음부터 재생).
5. **Death**: 데몬 즉시 사망 연출 (디버그용).
6. 데몬 HP가 0이 되면 사망 애니 재생 후 오브젝트 제거 → HP 게이지 UI 삭제 → **1초 후 WinText 표시 → 1.5초 후 숨김** (`WaitForSecondsRealtime`).

## 씬 / Inspector 연결 (GameMain)

| 필드 | 설명 |
|------|------|
| Attack Btn / Hit Btn / Death Btn | UI Button |
| Samurai / Demon | 각 스크립트가 붙은 캐릭터 오브젝트 |
| Fx Slash Point | 슬래시 생성 위치 (보통 Samurai 자식) |
| Demon Gauge | DemonGauge 등 **Slider** 컴포넌트 |
| Win Text | 승리 문구 루트 `GameObject` (비우면 씬에서 이름 `WinText` 자동 검색) |
| Damage Per Hit | Attack·Hit 한 번당 데몬 데미지 |

**주의**

- **WinText**는 **DemonGauge 자식이 아니어야** 합니다. 게이지 삭제 시 함께 파괴됩니다.
- 씬에 **FxManager** 오브젝트 + 슬래시 프리팹 연결 필요.

## 스크립트 요약

| 파일 | 역할 |
|------|------|
| `Samurai.cs` | `MoveTo`, `Attack` 코루틴, Animator `State`, 콜백 |
| `Demon.cs` | HP, `TakeDamage`, `Hit`, `Death` 트리거, `onHpChanged`, `onBeforeDestroy` |
| `GameMain.cs` | 버튼, 게이지, 승리 텍스트 연출, Fx 연동 |
| `FxManager.cs` | 싱글톤, `SpawnSlash` |

## 애니메이터 파라미터

### Samurai

| 파라미터 | 타입 | 값 |
|----------|------|-----|
| State | int | 0 Idle, 1 Attack, 2 Run |

### Demon

| 파라미터 | 타입 | 설명 |
|----------|------|------|
| State | int | 0 Idle, 1 Hit |
| Death | Trigger | 사망 전환 (Any State 등에서 조건 필수, 빈 조건 금지) |

히트 재시작 시 `Animator.Play("Demon_Hit", …)` 사용.

## 기술 메모

- 이동: `Vector3.MoveTowards` 코루틴.
- 슬래시: DOTween 미사용 (의존성 없음).
- 승리 텍스트: `Time.timeScale == 0`에서도 동작하도록 **Realtime** 대기.
- 저장소: [github.com/softCastella/Samurai](https://github.com/softCastella/Samurai)

## 개발 환경

- Unity 2022 이상 권장
- C#
