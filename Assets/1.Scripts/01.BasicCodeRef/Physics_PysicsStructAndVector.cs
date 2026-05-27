using UnityEngine;

public class Physics_PysicsStructAndVector : MonoBehaviour
{
    public GameObject TestObject; // 제어 대상 오브젝트
    public GameObject TestTargetObject; // 회전이나 LookAt 시 타겟 오브젝트 연결
    public GameObject TestLookAtObject; // 바라보기 테스트용 오브젝트 연결
    public Rigidbody TestObjectRigidBody; // 물리 효과를 적용할 리지드바디 컴포넌트 연결

    public int RotationSpeed = 10; // 회전 속도 조절용 변수

    private Quaternion _rotationSlerp; // 부드러운 회전을 위한 쿼터니언 변수
    private bool _toggleSlerpRotation = false; // 회전 토글 상태값

    private bool _movePlayer = false; // 이동 상태 플래그
    private bool _movePlayerByRigidBody = false; // 물리 이동 상태 플래그
    private bool _isMovePlayerRigidBodyLeft = false; // 물리 이동 방향 플래그


    // 벡터의 기초 함수
    void DaniTechRefCode_VectorBasic()
    {
        // X, Y를 멤버변수로 갖는 Vector2
        var vector2 = new Vector2(0, 0);
        vector2.x = 10;
        vector2.y = 10;
        Debug.Log($"{vector2.x} {vector2.y}");

        // X, Y, Z를 멤버변수로 갖는 Vector3
        var vector3 = new Vector3(0, 0, 0);
        vector3.x = 10;
        vector3.y = 10;
        vector3.z = 10;
        Debug.Log($"{vector3.x} {vector3.y} {vector3.z}");
    }

    // 회전 제어 함수
    void DaniTechRefCode_QuaternionBasicOnUpdate()
    {
        if (TestObject == null)
        {
            return;
        }

        // 이렇게 절대 Rotation을 직접 수정하지 않는다!
        // 절대 주의: rotation 값을 직접 x=45 쓰면 =>
        // this.transform.rotation.x = 45.0f; -> 짐벌락(Gimbal Lock) 오류 발생함

        // 회전 초기화. Euler(0,0,0)과 identity는 같은 상태값.
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            // 쿼터니언 1) 원하는 각도 대입하기
            TestObject.transform.rotation = Quaternion.Euler(0, 0.0f, 0);
            TestObject.transform.rotation = Quaternion.identity; // 위에 Euler (0,0,0과 같다)
        }
        // 현재 각도에 15도 추가(곱셈 연산).
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            // 쿼터니언 2) 현재 회전에서 추가 회전하기
            TestObject.transform.rotation *= Quaternion.Euler(0, 15.0f, 0);
        }
        // 타겟의 위치를 바라보게 설정
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            // 쿼터니언 3) 특정 방향(dir)을 바라보게 만드는 회전 값을 구할 때
            TestObject.transform.rotation = Quaternion.LookRotation(TestTargetObject.transform.position);
        }
        // Slerp 회전 모드 끄고 켜기
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {

            _toggleSlerpRotation = (!_toggleSlerpRotation);
        }
        // 특정 축(Vector3.up)을 기준으로 45도 회전
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            // 쿼터니언 5) 특정 축을 기준으로 몇 도 회전시킬 때
            TestObject.transform.rotation = Quaternion.AngleAxis(45.0f, Vector3.up);
        }

        // Slerp 모드가 켜져있다면 부드럽게 45도만큼 회전
        if (_toggleSlerpRotation == true)
        {
            var startRotation = TestObject.transform.rotation;
            _rotationSlerp = TestObject.transform.rotation * Quaternion.Euler(0, 45, 0);

            // 쿼터니언 4) A에서 B로 부드럽게 회전시킬 때 Slerp 사용
            TestObject.transform.rotation = Quaternion.Slerp(startRotation, _rotationSlerp, Time.deltaTime * RotationSpeed);
        }
    }

    // 직접 위치 제어(Kinematic) 함수 ( 키보드 이동)
    // Transform.Translate를 사용하여 물리 엔진을 타지 않고 직접 좌표를 변경하는 방식
    void DaniTechRefCode_TranslateOnUpdate()
    {
        if (TestObject == null)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            _movePlayerByRigidBody = false;

            TestObject.transform.rotation = Quaternion.Euler(0, -90.0f, 0);
            _movePlayer = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _movePlayerByRigidBody = false;

            TestObject.transform.rotation = Quaternion.Euler(0, 90.0f, 0);
            _movePlayer = true;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            _movePlayerByRigidBody = false;

            _movePlayer = false;
            // 월드 좌표의 위쪽 방향으로 1만큼 이동 [ Space.World (절대 좌표) ]
            // Translate => 벽을 뚤고 지나감
            TestObject.transform.Translate(Vector3.up * 2, Space.World);
        }

        if (_movePlayer == true)
        {
            _movePlayerByRigidBody = false;

            // 오브젝트의 로컬 앞 뒤 방향으로 이동 [ Space.Self (로컬 좌표) ]
            // Vector3.back으로 넣으면 뒤로 간다
            // Translate(Vector3.forward * Time.deltaTime): 프레임마다 조금씩 앞으로 이동
            // 컴퓨터 성능(프레임)에 관계없이 일정한 속도로 이동
            TestObject.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
        }
    }

    // 물리 엔진 제어(Dynamic) 함수
    // Rigidbody를 사용하여 물리 법칙을 적용한 이동
    void DaniTechRefCode_AddForceAndVelocityOnUpdate()
    {
        if (TestObjectRigidBody == null)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            _movePlayer = false;
            _movePlayerByRigidBody = false;

            float jumpPower = 5;

            // 속도 초기화 후 점프 (더블점프 버그 방지)
            // linearVelocity = Vector2.zero: => 새로운 힘을 주기 전에 이전의 속도를 0으로 초기화하면,
            // 점프 중 또 점프하는 '공중 점프' 같은 버그를 방지하고 쾌적한 조작감을 줌
            TestObjectRigidBody.linearVelocity = Vector2.zero;

            // 질량이 적용되는 ForceMode.Impulse는 점프를 거의 하지 못하게 된다!
            // TestObjectRigidBody.mass = 100;
            // ForceMode.VelocityChange => 질량을 무시하고 즉시 속도를 적용
            // 즉각적인 반응"이 필요할 때
            TestObjectRigidBody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            // Velocity, AddForce => 벽에 부딪히면 멈추거나 튕겨 나감
            TestObjectRigidBody.linearVelocity = Vector2.zero;
            TestObject.transform.rotation = Quaternion.Euler(0, 90.0f, 0);

            _movePlayer = false;
            _movePlayerByRigidBody = true;
            _isMovePlayerRigidBodyLeft = false;
            //TestObjectRigidBody.AddForce(1, ForceMode.VelocityChange);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            TestObjectRigidBody.linearVelocity = Vector2.zero;
            TestObject.transform.rotation = Quaternion.Euler(0, -90.0f, 0);

            _movePlayer = false;
            _movePlayerByRigidBody = true;
            _isMovePlayerRigidBodyLeft = true;
            //TestObjectRigidBody.AddForce(Vector2.right * 1, ForceMode.VelocityChange);
        }

        if (_movePlayerByRigidBody == true)
        {
            int moveSpeed = 1;
            float currentYVelocity = TestObjectRigidBody.linearVelocity.y;
            float targetXVelocity = _isMovePlayerRigidBodyLeft ? -moveSpeed : moveSpeed;

            TestObjectRigidBody.linearVelocity = new Vector2(targetXVelocity, currentYVelocity);
        }

    }

    // 구조체를 통한 메모리 관리 함수
    // class는 생성할 때마다 힙 메모리에 쌓이고 GC가 치워야 하지만,
    // struct는 스택(Stack)에 저장되어 메서드가 끝나면 즉시 사라짐
    // 게임 성능을 위해 '메모리 할당'을 최소화
    void DaniTechRefCode_WhyVectorIsStruct()
    {
        // 구조체인 이유 1) 값복사를 통한 원본 값 안정성 확보
        // 이렇게 받아서 쓸 때, 원본의 값에 영향주지 않도록 함
        Vector3 currentPosition = this.gameObject.transform.position;
        currentPosition.x = 20; // 내 오브젝트의 위치가 X가 수정되지 않는다!!


        // 구조체인 이유 2) 불필요한 힙 메모리 방지
        // C#에서 값형인 구조체의 new는 힙 메모리 저장을 의미하지 않습니다! -> 생성자를 부른다는 의미에서만 new
        // 즉 다음 코드는 스택에 만들어집니다.
        // 특정 메서드에서 사용되고 나서 알아서 제거됩니다. (GC 발생하지 않음)
        Vector3 spawnPoint = new Vector3(500, 10, 20);

        // 구조체인 이유 3) 값복사를 통해 확장성 유지 (기존 기능에는 영향 없고 바리에이션 된 값만 추가로 변형)
        Vector3 spawnPointBasicCreature = spawnPoint;
        Vector3 spawnPointFlyCreature = spawnPoint;

        // 3==) 나는 몬스터는 더 높은 곳에서 스폰되어야 해서 100을 더해줌
        // Vector3가 참조형인 class였다면 모든 크리처들이 하늘 위에서 나오게 됨
        spawnPointFlyCreature.y = (spawnPointFlyCreature.y + 100);
    }

    // 방향과 시선제어 함수
    void DaniTechRefCode_LookAtAndRotateOnUpdate()
    {
        if (TestObject == null || TestLookAtObject == null)
        {
            return;
        }

        // 주석 해제 후 테스트 (아이템 회전 뷰 느낌 낼 때, 선풍기나 회전목마 같은 것도)
        // TestObject.transform.Rotate(Vector3.up * 45f * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            // 바라본다
            // LookAt => 대상의 위치(position)를 주면 자동으로 그쪽을 바라보게 회전
            TestObject.transform.LookAt(TestLookAtObject.transform.position);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            // Y축 기준으로 회전
            // Rotate => 현재 각도에서 상대적으로 더 회전합니다. Space.World를 쓰면 맵 기준
            // 안 쓰면 나(오브젝트)를 기준으로 회전
            TestObject.transform.Rotate(Vector3.up * 90f);
        }
    }

    void OnEnable()
    {
        DaniTechRefCode_WhyVectorIsStruct();
        DaniTechRefCode_VectorBasic();
    }

    // Update is called once per frame
    void Update()
    {
        DaniTechRefCode_QuaternionBasicOnUpdate();
        DaniTechRefCode_TranslateOnUpdate();
        DaniTechRefCode_AddForceAndVelocityOnUpdate();
        DaniTechRefCode_LookAtAndRotateOnUpdate();
    }
}
