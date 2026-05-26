using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : MonoBehaviour
{
    // 프리팹 오브젝트 받아오기
    [SerializeField] private GameObject Prefab_SampleObject;
    // 키는 = 2
    [SerializeField] int FindTestKey = 2;
    // 삭제할 오브젝트 
    [SerializeField] private GameObject GObj_RemoveTargetC;

    // 딕셔너리 변수 저장
    private Dictionary<int, GameObject> _objectDictionary = new Dictionary<int, GameObject>();

    // 딕셔너리에 저장될때 붙일 번호 변수
    private int _generatedInstanceIdx = 0;


    // 붙여진 번호에 따라 이름 지어주기 함수
    private string GetGeneratedName(int curIdx)
    {
        int currentNum = (curIdx % 3); 
        if (currentNum == 0)
        {
            return "감자";
        }
        else if (currentNum == 1)
        {
            return "만두";
        }
        else if (currentNum == 2)
        {
            return "순대";
        }
        else
        {
            return "피자";
        }
    }

    private void Update()
    {
        // 스페이스바를 누르면 객체를 동적 생성
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _generatedInstanceIdx++; // 동적 생성될때 마다 번호 순차적 번호 붙여주기

            // 받아온 프리팹으로 동적생성된 녀석을 내 자식으로 넣어라(하이라키 창에서)
            var instantiatedObj = Instantiate(Prefab_SampleObject, this.gameObject.transform);
            if (instantiatedObj != null)
            {
                // 실체화된 게임오브젝트의 이름을 바꿔보자 (에디터 하이어라키 뷰에서 확인용)
                instantiatedObj.name = $"{instantiatedObj.name}(Id_{_generatedInstanceIdx})";

                // 실체화된 게임오브젝트에서 인터페이스를 가져와보자!
                var objectable = instantiatedObj.GetComponent<IObjectable>();
                objectable.ObjectNumber = _generatedInstanceIdx;
                objectable.ObjectName = GetGeneratedName(_generatedInstanceIdx);
                objectable.SetTextMeshNameOnInit();

                // 유니티의 게임오브젝트를 담을때는 인터페이스 같은 순수 C#클래스가 아니라 GameObject를 넣는게 좋다!
                _objectDictionary.Add(objectable.ObjectNumber, instantiatedObj);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            // 오브젝트가 잘 있는지 찾아보자!

            if (_objectDictionary.ContainsKey(FindTestKey) == true)
            {
                var gObj = _objectDictionary[FindTestKey];
                if (gObj != null)
                {
                    var objectable = gObj.GetComponent<IObjectable>();
                    Debug.LogWarning($"인덱스 {FindTestKey}인 오브젝트 : {objectable.ObjectName} 찾아짐!");
                }
            }
            else
            {
                Debug.LogError($"인덱스 {FindTestKey}인 오브젝트가 없습니다!");
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GObj_RemoveTargetC.RemoveComponent<ObjectC>();
        }
    }
}
