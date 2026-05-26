using UnityEngine;

public interface IObjectable
{
    // 이 인터페이스를 가져다 쓰는 녀석은 무조건 ObjectName이라는 이름표와 ObjectNumber라는 번호표를 가지고 있어야 한다
    // { get; set; }은 이 값을 읽고 쓸 수 있게 열어두라는 뜻
    string ObjectName { get; set; }
    int ObjectNumber { get; set; }

    // 이 함수들이 구체적으로 어떻게 작동할지는 나도 모르겠지만,
    // 이 인터페이스를 쓰는 녀석은 이 이름표를 단 함수 2개를 무조건 만들어라! 안 만들면 에러를 띄울 거다!"
    // 밑에 함수는 예시 꼭 밑에 두개 함수를 따라 할 필요는 없음

    void SetTextMeshNameOnInit();

    void PrintSomthing();
}
