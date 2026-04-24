using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{

    public Button attackBtn;
    public Samurai samurai;
    public Transform fxSlashPoint; // 슬래시 이펙트 생성 위치 (Samurai 하위 오브젝트)
    
    void Start()
    {
        attackBtn.onClick.AddListener(()=>
        {
            Debug.Log("클릭");
            samurai.Attack();
        });

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
        attackBtn.gameObject.SetActive(false);

        samurai.onMoveComplete = () =>
        {
            // 사무라이가 목표지점에 도착하면 버튼 활성화
            attackBtn.gameObject.SetActive(true);
        };

        // 왼쪽 끝에서 시작해서 (0,0,0)으로 이동
        samurai.transform.position = new Vector3(-10f, 0f, 0f);
        samurai.MoveTo(Vector3.zero);
    }    
}
