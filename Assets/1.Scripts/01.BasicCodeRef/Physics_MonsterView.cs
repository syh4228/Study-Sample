using UnityEngine;

public class Physics_MonsterView : MonoBehaviour
{
    [SerializeField] private float _rayCastDistance = 10.0f; // 레이캐스트 거리
    Collider[] _overlapHitResults = new Collider[10]; // 감지 갯수

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // z 키 눌리면
        {
            // 주석 해제 - 관통형
            // PerformPenetrationRaycast();
            //ShotRaycast();
            DetectEnemies(); 
            DetectEnemiesNonAlloc();
        }
    }

    // 오버랩 방식(영역, 공간) 탐지 함수 (최적화)
    // Physics.OverlapSphereNonAlloc을 사용, 미리 만들어둔 배열(_overlapHitResults)을 재사용
    // 메모리 할당(Alloc)이 없으므로 성능 면에서 매우 유리
    void DetectEnemiesNonAlloc()
    {
        // 중심점, 반지름, 탐색할 레이어 설정
        Vector3 center = transform.position;
        // 탐지 범위 반지름 5.0f 구체 영역 설정 위한 변수
        float radius = 5.0f;
        // "Dummy"라는 이름을 가진 레이어만 탐지하도록 필터를 생성
        int layerMask = LayerMask.GetMask("Dummy");

        // 중심점(center)에서 반지름(radius)만큼의 영역 안에 있는 모든 물체를 찾아서 
        // 결과물(_overlapHitResults)에 저장
        // hitColliderSize에는 실제로 감지된 물체의 개수 담기
        var hitColliderSize = Physics.OverlapSphereNonAlloc(center, radius, _overlapHitResults, layerMask);

        // 감지된 물체 갯수만큼 반복
        for (int i = 0; i < hitColliderSize; i++)
        {
            var hitCollider = _overlapHitResults[i];
            // 감지된 물체 이름 출력
            Debug.Log(hitCollider.name + " 감지됨!");
            Destroy(hitCollider.gameObject); // 오브젝트 파괴
        }
    }

    // 오버랩 방식(영역, 공간) 탐지 함수
    // Physics.OverlapSphere를 사용, 매번 새로운 배열(Collider[])을 생성하여 반환
    // 코드는 깔끔하지만, 호출할 때마다 새로운 배열을 만들기 때문에 가비지 컬렉션(메모리 정리)을 유발하여 성능 저하
    void DetectEnemies()
    {
        // 중심점, 반지름, 탐색할 레이어 설정
        Vector3 center = transform.position;
        float radius = 5.0f;
        int layerMask = LayerMask.GetMask("Dummy");

        // 영역 내의 모든 콜라이더 검출
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, layerMask);

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.name + " 감지됨!");
            Destroy(hitCollider.gameObject);
        }
    }

    // 시선(광선)으로 탐지하기 , 단일 감지
    // 정면으로 광선을 딱 하나 쏩니다. 가장 먼저 닿는 물체 하나만 즉시 반환
    // FPS 총기 발사, 칼 휘두르기, 상호작용 가능한 문/상자 클릭에 사용 적절
    void ShotRaycast()
    {
        // 정보를 저장할 변수 선언 - 구조체!
        RaycastHit hit;

        // 2. 레이캐스트 발사
        // Physics.Raycast(시작점, 방향, 결과저장, 거리)
        if (Physics.Raycast(transform.position, transform.forward, out hit, _rayCastDistance))
        {
            // 3. 감지된 오브젝트의 정보 확인
            Debug.Log($"감지된 물체: {hit.collider.gameObject.name}");

            // 4. 감지된 오브젝트 삭제
            // hit.collider.gameObject를 통해 실제 게임 오브젝트에 접근합니다.
            Destroy(hit.collider.gameObject);
        }
    }

    // 시선(광선)으로 탐지하기 , 관통 감지
    // RaycastAll을 사용 광선 경로상에 있는 모든 물체를 배열 형태로 가져옴
    // 관통형 스나이퍼 탄환, 레이저 공격, 여러 물체를 동시에 뚫고 지나가는 마법 사용 적절
    void PerformPenetrationRaycast()
    {
        // 1. RaycastAll은 RaycastHit의 '배열'을 반환합니다.
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, _rayCastDistance);

        // 2. 루프를 돌며 맞은 모든 물체를 처리합니다.
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Debug.Log($"{i}번째 관통된 물체: {hit.collider.name}");

            // 맞은 물체 삭제
            Destroy(hit.collider.gameObject);
        }

        Debug.Log($"총 {hits.Length}개의 물체를 관통했습니다.");
    }

    // 에디터 뷰에서 레이를 시각적으로 확인하기 위함
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 색깔 빨강 지정
        // 정면을 향해 레이가 어디로 나가는지 보여줌
        Gizmos.DrawRay(transform.position, transform.forward * _rayCastDistance);
        // 반지름 5.0f인 구(Sphere) 영역이 어디에 위치하는지 보여줌
        Gizmos.DrawWireSphere(transform.position, 5.0f);
    }

    // OnCollision(Enter/Stay/Exit) 물리충돌
    // 물리적으로 '부딪히는' 경우 튕겨 나가거나 밀리는 물리 연산이 필요할 때 사용

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning($"{collision.gameObject.name}와 충돌!");

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Map"))
        {
            return;
        }

        Debug.Log($"{collision.gameObject.name}와 충돌 중이다!");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.LogWarning($"{collision.gameObject.name}와 충돌 끝");
    }

    // OnTrigger(Enter/Stay/Exit) 물리충돌
    // IsTrigger가 체크된 상태에서 물체와 부딪히지 않고 '통과'할 때 사용
    // 용도: 아이템 획득(Gold 태그), 구역 입장 이벤트, 함정 발동 등

    // 콜라이더에 다른 콜라이더가 들어오면
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gold")) // gold 태그면 파괴
        {
            Destroy(other.gameObject);
        }
    }

    // 콜라이더안에 있던 콜라이더가 나가면
    private void OnTriggerExit(Collider other)
    {
        Debug.LogWarning($"{other.name}를 빠져나갔다!");
    }

    // 콜라이더가 콜라이더 안에 있으면
    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"{other.name}에 머무는 중이다");
    }
}
