using UnityEngine;

// 캐릭터가 가질 수 있는 상태를 열거형으로 정리
public enum DaniTech_EntityAnimState
{
    None = 0, // 기본값
    Idle, // 대기 상태
    Walk, // 걷기
    Atk, // 공격
    Positive, // 긍정적 반응
    Negative, // 부정적 반응
    InteractionStart // 상호작용
}

// 2D 애니메이션 컨틀롤러
public class AnimatorController_2D : MonoBehaviour
{
    // 캐릭터의 애니메이터를 연결
    [SerializeField] private Animator Animator_Entity;

    // 현재 캐릭터가 어떤 상태인지 기억해두는 변수
    private DaniTech_EntityAnimState _currentAnimState;

    // 외부에서 쉽게 변경을 요청하려고 만든 퍼블릭 함수
    // 이 상태에 따른 애니메이션 재생을 여기서만 모아서 해줄려고
    public void SetState(DaniTech_EntityAnimState newState) // 새로운 상태
    {
        // 만약 새로 요청받은 상태가 대기이거나, 현재 캐릭터 상태가 대기이면 (방어코드)
        if (newState == DaniTech_EntityAnimState.Idle && _currentAnimState == DaniTech_EntityAnimState.Idle)
        {
            return; // 반환  (매 프레임 Idle이 반복 호출되는 낭비 방지)
        }

        // 비교를 했는데, 같은 값이 아니고, 이제 동작을 바꿔도 된다면 이렇게 대입
        // 새로 요청 받은 상태를 현재 상태로 저장
        _currentAnimState = newState;

        // 갱신된 현재 상태가 무엇인지 스위치문으로 검사
        switch (_currentAnimState)
        {
            case DaniTech_EntityAnimState.Idle: // 대기 상태면
                ResetAllAnimParameters(); // 다른 상태 끄는 함수 호출
                break;
            case DaniTech_EntityAnimState.Walk: // 걷기 상태면
                // Animator의 "IsWalk" 스위치를(Bool) 켜서 걷는 애니메이션을 반복 재생
                Animator_Entity.SetBool("IsWalk", true);
                break;
            case DaniTech_EntityAnimState.Atk: // 공격 상태면
                // 1회성 동작이므로 방아쇠(Trigger)를 당깁
                Animator_Entity.SetTrigger("IsAtk");
                break;
            case DaniTech_EntityAnimState.Positive:
                Animator_Entity.SetTrigger("IsPositive");
                break;
            case DaniTech_EntityAnimState.Negative:
                Animator_Entity.SetTrigger("IsNegative");
                break;
            case DaniTech_EntityAnimState.InteractionStart:
                Animator_Entity.SetTrigger("IsInteractionStart");
                break;
            default:
                // 혹시라도 Enum에 없는 이상한 값이 들어왔을 경우를 대비한 안전장치
                // 의도되지 않은 상황이라면 모든 파라미터를 초기화
                ResetAllAnimParameters();
                break;
        }
    }

    // 애니메이션 Bool 파라미터들을 초기 상태(전부 꺼짐)로 되돌려주는 도우미 함수
    // 스위치(Bool) 형태의 애니메이터를 전부 꺼주는 함수
    private void ResetAllAnimParameters()
    {
        // 걷기 상태를 강제로 끕니다. 
        Animator_Entity.SetBool("IsWalk", false);
    } 

}
