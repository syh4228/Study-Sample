using UnityEngine;

// 2D 게임에서 좌우 중 한 방향을 무작위로 골라 계속 걸어가는
// 아주 기본적인 적(Enemy)의 AI를 구현한 코드
// 특히 외부의 '매니저(Manager)' 스크립트가 이 적을 소환하고 관리할 수 있도록
// 고유 ID(번호표)를 부여받는 기능이 포함되어 있는 것이 특징
public class Enemy_2D : MonoBehaviour
{
    // 적의 이미지(스프라이트)를 좌우로 뒤집기 위해 SpriteRenderer 컴포넌트를 연결
    [SerializeField] private SpriteRenderer SpriteRenderer_Enemy;

    // GameObject 매니저에서 이 객체를 동적생성하고 등록할때 부여되는,
    // 충돌시 상호작용 요청에 쓰이는 인스턴스 Id (적의 '주민등록번호' 같은 고유 번호표)
    // 외부에서 번호를 '읽을 수(get)'는 있지만,
    // '수정(set)'은 이 스크립트 내부에서만 가능하도록 막아둠
    public int EntityInstancId { get; private set; }

    // // 적이 현재 이동하고 있는 방향(좌 또는 우)을 저장하는 변수
    private Vector3 _moveDirection;

    void Start() // 시작시 1번만
    {
        // 시작하자마자 왼쪽으로 갈지 오른쪽으로 갈지 랜덤으로 정하는 함수 호출
        RandomPickDirection();
    }

    void Update() // 매 프레임 마다 실행
    {
        // 정해진 방향으로 계속 걸어가게 하는 함수 호출
        SimpleEnemyMoveOnUpdate();
    }

    // 매니저 스크립트가 이 적을 소환(Instantiate)한 직후에 부르는 초기화 함수
    public void InitEnemyInfo(int instanceId)
    {
        // 매니저가 넘겨준 고유 번호표(instanceId)를 내 번호표(EntityInstancId)로 저장
        EntityInstancId = instanceId;

        // (추후 확장성) 나중에 적의 최대 체력, 공격력, 스피드 등을
        // 매니저가 여기서 한 번에 정해주도록 코드를 추가
    }

    // 이동 방향을 무작위로 뽑는 함수
    void RandomPickDirection()
    {
        // 변칙성을 위해서 0 또는 1 중 하나를 뽑아 왼쪽(-1) 혹은 오른쪽(1) 결정
        // 삼항 연산자, Random.Range(0, 2)는 0 아니면 1을 뽑음
        float randomX = Random.Range(0, 2) == 0 ? -1f : 1f;
        // 위에서 정해진 좌/우 값을 방향(Vector3) 데이터로 만듬 (y와 z는 0이므로 상하 이동은 안 함)
        _moveDirection = new Vector3(randomX, 0, 0);
        // 이동 방향에 맞춰서 캐릭터가 바라보는 이미지의 방향도 돌려줌
        SetMeshDirectionByMoveDirection((int)_moveDirection.x);
    }

    // 캐릭터의 이미지를 이동 방향에 맞게 좌우 반전시키는 함수
    void SetMeshDirectionByMoveDirection(int x)
    {
        // x < 0 이면 true(참)가 되어 스프라이트가 뒤집힘(flipX = true). 즉 왼쪽을 보게 됨
        // x > 0 이면 false(거짓)가 되어 원본 이미지 그대로(오른쪽)를 보게 됨
        SpriteRenderer_Enemy.flipX = (x < 0);
    }

    // 적을 실제로 이동시키는 함수
    void SimpleEnemyMoveOnUpdate()
    {
        // 내 현재 위치(transform.position)에 지정된 방향(_moveDirection)으로 이동값을 더함
        // * 5.0f: 이동 속도 (1초에 5만큼 이동)
        // * Time.deltaTime: 컴퓨터 성능에 상관없이 똑같은 속도로 걷게 해주는 시간 보정 값
        transform.position += _moveDirection * 5.0f * Time.deltaTime;
    }
}
