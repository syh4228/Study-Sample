using UnityEngine;

// C# 을 이용한 프로젝트 전체의 공용 함수 샘플
// 유니티 엔진의 물리법칙이나 화면 랜더링이 전혀 필요 없는
// '순수한 데이터 보관함'이나 '계산기'를 만들 때 씁니다.
// 예를 들어 아이템의 이름과 가격 정보만 담아두는 ItemData 클래스,
// 혹은 데미지 계산 공식만 모아둔 DamageCalculator 클래스를 만들 때
// 이렇게 : MonoBehaviour를 떼버리고 가볍게 만들어서 사용합니다
public static class StaticClass
{
    // 프린트 출력 함수
    public static void PrintStaticText()
    {
        Debug.Log("스태틱 클래스로 스태틱 메서드인 출력함수도 호출해보기");
    }
}
