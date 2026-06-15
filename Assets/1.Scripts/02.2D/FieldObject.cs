using UnityEngine;
using System.Collections.Generic;

public class FieldObject : Entity
{
    [Header("에디터 확인용 데이터 - 따로 지정하는 용도는 아닙니다")]
    // 오브젝트 생성시 부여받는 고유번호 저장 (ex) 100번째 생성된 사과
    [SerializeField] private int _fieldObjectInstanceId;
    // '종류'를 나타내는 도감 번호 저장 (ex) "item_apple_01"
    [SerializeField] private string _fieldObjectDataId;
    [SerializeField] private string _fieldObjectName; // 에디터에서 이름이 뭔지 쉽게 보려고 놔둔 변수

    // 초기화 함수: 매니저가 이 오브젝트를 맵에 소환할 때 제일 먼저 부르는 함수
    public void InitFieldObjectInfoOnCreated(int instanceId, string fieldObjectDataId)
    {
        // 넘겨받은 도감 번호(fieldObjectDataId)를 데이터 매니저에게 주면서
        // "이 번호에 해당하는 아이템 정보 좀 찾아줘!" 라고 요청
        var fieldObjectData = GameDataManager.Instance.GetDNFieldObjectData(fieldObjectDataId);
        
        if (fieldObjectData == null) // 만약 데이터 매니저에 도감번호가 없으면 반환
        {
            Debug.LogWarning($"유효하지 않은 필드 오브젝트 데이터 입니다! {fieldObjectDataId}");
            return;
        }

        // 정상적으로 데이터를 찾았다면, 넘겨받은 고유 번호와 도감 번호를 내 변수에 저장
        _fieldObjectInstanceId = instanceId;
        _fieldObjectDataId = fieldObjectDataId;
    }

    // 외부에서 내 도감 번호(DataId)가 궁금할 때 알려주는 함수
    public string GetFieldObjectDataId()
    {
        return _fieldObjectDataId;
    }

    // 3D 충돌 감지: 콜라이더(Is Trigger 체크됨)에 무언가 닿았을 때 실행
    private void OnTriggerEnter(Collider other)
    {
        // 닿은 녀석의 태그가 "Player"라면
        if (other.CompareTag("Player") == true)
        {
            OnPlayerTriggerEntered(); // 상호작용 함수 호출
        }
    }

    // 2D 충돌 감지: 콜라이더에 무언가 닿았을 때 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 녀석의 태그가 "Player"라면
        if (collision.CompareTag("Player") == true)
        {
            OnPlayerTriggerEntered(); // 상호작용 함수 호출
        }
    }

    // 플레이어와 닿았을 때 일어나는 핵심 상호작용 로직
    private void OnPlayerTriggerEntered()
    {
        // 플레이어와 충돌했을때 이제 GameManager에 아이템을 저장해준다던가 등등 처리
        // 필요에 따라 역할을 다하면 지우거나 비활성화 해주자 = 아래 둘 중 하나 사용
        // this.gameObject.SetActive(false);
        // Destroy(this.gameObject);

        // 채집과 드랍 1-0) 내가 상호작용한 필드 오브젝트의 타입에 따라 처리를 추가해봅시다

        // 내 도감 번호(DataId)로 데이터 매니저에게 다시 한번 상세 정보를 가져오기
        var fieldObjectData = GameDataManager.Instance.GetDNFieldObjectData(_fieldObjectDataId);
        
        if (fieldObjectData == null) // 도감번호가 없으면 반환
        {
            Debug.LogWarning($"유효하지 않은 필드 오브젝트 데이터 입니다! {_fieldObjectDataId}");
            return;
        }

        // 채집과 드랍 1-1) 내가 상호작용한 필드 오브젝트가 채집물이거나 드랍아이템 유형인지 확인
        // (Enum으로 바꿔서 쓰면 오타로 인한 버그를 막을 수 있음)

        // 내 타입이 '채집물(Harvest)' 이거나 '드랍 아이템(DropItem)'인지 확인
        if (fieldObjectData.FieldObjectType == "Harvest" || fieldObjectData.FieldObjectType == "DropItem")
        {
            // 이 채집물을 캤을 때 나오는 '결과물 아이템 ID(DropItemDataId)'가 비어있는지 확인
            if (string.IsNullOrEmpty(fieldObjectData.DropItemDataId))
            {
                // 드랍 아이템이 없다면 더이상 처리하지 않는다
                return;
            }

            // 채집과 드랍 1-2) 채집물이나 드랍이 맞으면 "아이템 정보를 찾아서" 인벤토리에 추가해주자

            // 결과물 아이템 ID를 가지고, 데이터 매니저에게 "이 아이템이 진짜 존재하는지" 한 번 더 검사
            var itemData = GameDataManager.Instance.GetDNItemData(fieldObjectData.DropItemDataId);
            
            if (itemData == null) // 아이템 데이터가 없으면 반환
            {
                Debug.LogWarning($"유효하지 않은 아이템 데이터 입니다! {_fieldObjectDataId}");
                return;
            }

            // 채집과 드랍 1-3) 아이템 드랍 수를 FieldObject 데이터의 DropCountRange 범위 내에서 랜덤으로 가져오자

            // 몇 개를 드랍할지 결정하기 위해, 데이터에 적힌 최소~최대 개수 범위(List)를 가져옴
            List<int> dropCountRange = fieldObjectData.DropCountRange;
            int finalDropItemCount = 1; // 기본값은 1개

            // 리스트에 숫자가 제대로 들어있다면
            if (dropCountRange != null && dropCountRange.Count > 0)
            {
                // 삼항 연산자를 사용
                // 리스트에 숫자가 2개 이상 들어있다면(ex 1, 3) -> 첫번째(1)와 두번째 수(3)를 최소, 최대로 랜덤으로 뽑고,
                // 숫자가 1개밖에 없다면(예: 5) -> 무조건 그 숫자(5개)를 확정 드랍합니다.
                finalDropItemCount = dropCountRange.Count > 1 ? Random.Range(dropCountRange[0], dropCountRange[1]) : dropCountRange[0];
            }

            // 채집과 드랍 1-4) 게임 매니저에게 "어디서든"(현재는 채집물이 충돌된 시점) 편하게 아이템 추가를 요청한다!

            // 게임 매니저(인벤토리 관리자)에게 이 아이템을 계산된 개수만큼 획득 처리 알림
            GameManager.Inst.AddItem(itemData.Id, finalDropItemCount);

            // 채집과 드랍 1-5) 추가 완료 되었다면 이 오브젝트를 비활성화 또는 제거하자 (우리는 제거를 선택)

            // 아이템을 다 줬으면 직접 Destroy하지 않고,
            // 오브젝트 매니저에게 내 고유 번호(InstanceId)를 주면서 "나 좀 지워줘"라고 안전하게 요청
            GameObjectManager.Inst.RequestDestroyFieldObject(_fieldObjectInstanceId);
        }

    }
}
