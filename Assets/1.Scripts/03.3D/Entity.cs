using UnityEngine;
using Cysharp.Threading.Tasks;

// 무거운 3D 모델(메쉬)이나 애니메이션을 비동기 로딩을 통해 불러오는 스크립트
public class Entity : MonoBehaviour
{
    [Header("주소 설정")]
    // 이 캐릭터가 입을 3D 모델(메쉬) 파일이 있는 경로(주소)를 적어두는 곳
    [SerializeField] private string ResourceMeshObjectPath;
    // 이 캐릭터가 사용할 애니메이션 컨트롤러 파일의 경로 (따로 지정해야 할 때만 씀)
    [SerializeField] private string ResourceAnimControllerPath;


    [Header("캐릭터 3D 메쉬 & 애니메이션 전용")]
    // 불러온 3D 모델을 자식으로 붙여넣을 뼈대(부모 위치)
    [SerializeField] private Transform Tranform_EntityMeshRoot;
    // 불러온 3D 모델을 움직이게 할 애니메이터를 담아둘 공간
    [SerializeField] private Animator Animator_Entity;
    // 모델을 처음 불러왔을 때 설정해 줄 기본 크기(비율)
    [SerializeField] private Vector3 InitialMeshScale;

    private void Start()
    {
        // 어드레서블 주소가 미리 설정되어 있다면, 해당 리소스를 비동기 로딩한다.

        // 인스펙터 창에서 3D 모델 주소(ResourceMeshObjectPath)가 비어있지 않고 잘 적혀있다면?
        if (string.IsNullOrEmpty(ResourceMeshObjectPath) == false)
        {
            // 아래에 있는 비동기 로딩 함수를 실행
            // .Forget() : "이 작업은 백그라운드에서 알아서 돌아가게 두고, 나는 내 할 일(다음 줄) 하러 갈게!"라는 뜻 
            // UniTask를 사용할 때 경고창이 뜨지 않게 해주는 아주 중요한 꼬리표
            InitEntity3DMeshAsync(ResourceMeshObjectPath, ResourceAnimControllerPath).Forget();
        }
    }

    // UniTaskVoid = 이 함수는 시간이 걸리는 작업(비동기)을 할 것이며, 결과를 따로 보고하지 않아도 된다(Void)는 뜻

    // 비동기 로딩 함수
    public async UniTaskVoid InitEntity3DMeshAsync(string meshObjectPath, string specificAnimContollerPath = "")
    {
        // 모델이 로딩되는 동안 뼈대(Root)가 화면에 보이지 않도록 일단 꺼둠 (로딩 중에 팝콘처럼 튀어나오는 현상 방지)
        Tranform_EntityMeshRoot.gameObject.SetActive(false);
        // await: "여기서부터 시간이 좀 걸리니까, 다 불러올 때까지 여기서 잠깐 기다려!"라는 뜻
        // GameUtill이라는 외부 유틸리티 함수를 시켜서 모델을 불러오고, 뼈대에 붙인 뒤, 애니메이터까지 세팅해서 가져오라고 시킴
        var animator = await GameUtill.LoadAndMeshObjectAndBindAnimator(Tranform_EntityMeshRoot, meshObjectPath, specificAnimContollerPath);
        
        if (animator == null) // 만약 가져온 애니메이터가 비어있다면 반환
        {
            Debug.LogError($"{meshObjectPath} 오브젝트 비동기 로딩에 실패했습니다!!");
        }

        // 성공적으로 불러왔다면, 가져온 애니메이터를 내 변수에 연결(저장)
        Animator_Entity = animator;
        // 불러온 3D 모델의 크기를 처음에 설정했던 크기(InitialMeshScale)로 맞춤
        Tranform_EntityMeshRoot.localScale = InitialMeshScale;
        // 모든 준비가 끝났으니, 아까 꺼두었던 뼈대를 다시 켜서 화면에 짠! 하고 나타나게 합
        Tranform_EntityMeshRoot.gameObject.SetActive(true);
    }
}
