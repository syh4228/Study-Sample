using UnityEngine;

// 유니티 생명주기(Lifecycle) 순서 테스트
public class MonoClass : MonoBehaviour
{
    // 변수 선언
    // !! => MonoBehaviour를 상속받은 스크립트는 절대 new 키워드로 생성하면 안 됩
    // 유니티의 컴포넌트는 오직 Instantiate나 AddComponent로만 생성해야 됨
    public MonoClass _emptyCodeNonMono = new MonoClass();
    bool _isFirstFixedUpdate = false;
    bool _isFirstUpdate = false;
    bool _isFirstLateUpdate = false;


    private void Awake() // 가장 먼저 무조건 실행
    {
        Debug.Log("저는 Awake 입니다");
    }

    // '켜질 때마다' 실행. 처음 태어날 때 켜져 있으니 Awake 직후에 바로 불립
    private void OnEnable()
    {
        Debug.Log("저는 OnEnable 입니다");
    }

    // 게임이 본격적으로 시작되고 첫 프레임(Update)이 돌기 직전에 실행
    // Awake에서 준비를 마칠 때까지 기다렸다가, 서로 연결하거나 초기 스탯을 세팅할 때 씀
    void Start()
    {
        Debug.Log("저는 Start 입니다");

        StaticClass.PrintStaticText();
    }

    // 유니티의 물리 엔진이 돌아가는 시간(보통 0.02초)마다 일정하게 불림
    // 캐릭터를 밀거나(AddForce) 물리 충돌을 계산할 때 무조건 여기서 처리해야 안정적
    private void FixedUpdate()
    {
        if (_isFirstFixedUpdate == true)
        {
            return;
        }

        _isFirstFixedUpdate = true;
        Debug.Log("저는 FixedUpdate 입니다");

    }

    // 매 프레임마다 불림(컴퓨터 성능에 따라 1초에 60번~144번 등 다름)
    // 사용자의 키보드 입력을 받거나 타이머를 잴 때 가장 많이 쓰는 핵심 함수
    void Update()
    {
        // 한번만 부르기 위해서 예외처리
        if (_isFirstUpdate == true)
        {
            return;
        }

        _isFirstUpdate = true;
        Debug.Log("저는 Update 입니다");

        // _emptyCodeNonMono.PrintSomeText();
        // Debug.Log("월드 틱-");
    }

    // 모든 Update가 다 끝나고 나서 제일 마지막에 불림
    // 주로 플레이어가 Update에서 이동을 끝내면,
    // 카메라가 그 뒤를 쫓아갈 때(LateUpdate) 이 함수를 씀
    private void LateUpdate()
    {
        // 한번만 부르기 위해서 예외처리
        if (_isFirstLateUpdate == true)
        {
            return;
        }

        _isFirstLateUpdate = true;
        Debug.Log("저는 LateUpdate 입니다");

        Destroy(this.gameObject);
    }

    // 오브젝트가 파괴되거나 체크박스가 '꺼지는 순간'에 실행
    private void OnDisable()
    {
        Debug.Log("저는 OnDisable 입니다");
    }

    // 오브젝트가 메모리에서 완전히 삭제되기 직전 마지막으로 불림.
    // 주로 연결해 놨던 이벤트나 데이터를 깔끔하게 해제하고 지울 때 씀
    private void OnDestroy()
    {
        Debug.Log("저는 OnDestroy 입니다");
    }
}
