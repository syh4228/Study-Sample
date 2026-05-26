using UnityEngine;

// Mono~ 뒤에 IOb~를 하나더 붙임으로 IOb~의 인터페이스를 상속받고, 그 규칙을 지키겠다고 선언
public class ObjectA : MonoBehaviour, IObjectable
{
    // 인터페이스를 상속받기 위해서, 실제 데이터를 담을 수 있게 변수 선언
    // [Ser~]를 써서 인스펙터 창에서 무엇을 상속받고 있는지를 볼수 있게 함
    [SerializeField] private string _objectName;
    //  get, set 규칙
    public string ObjectName { get { return _objectName; } set { _objectName = value; } }
    [SerializeField] private int _objectNumber;
    public int ObjectNumber { get { return _objectNumber; } set { _objectNumber = value; } }


    [SerializeField] private TextMesh TextMesh_ObjectName;

    // 시작할때 원래 이름과 번호를 따로 가지고 있었다고 해도,
    // 내가 여기서 지정한 이름과 번호로 바꾸고 시작하겠다.
    private void Awake() // 시작할때 단 한번 불려오는 함수
    {
        // 1) 씬에서 미리 설정했더라도, Awake에 들어오면서 덮어 씌워진다.
        ObjectName = "감자";
        ObjectNumber = 777;

        // 2) 텍스트 메쉬에 이름 표시!
        TextMesh_ObjectName.text = this.ObjectName;
    }

    // 상속 받은 부모의 함수를 불러와야 하는 규칙
    // 3) 이 메서드는 다른 객체가 부를 것이다!
    public void PrintSomthing()
    {
        Debug.Log("오브젝트 A에서 아무거나 출력");
    }

    public void SetTextMeshNameOnInit()
    {
        TextMesh_ObjectName.text = this.ObjectName;
    }
}
