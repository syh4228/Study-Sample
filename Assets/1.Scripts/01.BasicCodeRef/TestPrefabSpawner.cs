using UnityEngine;

// 프리팹 동적 생성 기본 스포너 
public class TestPrefabSpawner : MonoBehaviour
{
    public GameObject Prefab_Sample; // 프리팹
    public Transform Transform_SpawnPos; // 스폰지점

    // 하이라키 창에서 소환된 프리팹을 자식으로 둘 부모 빈 오브젝트
    public GameObject Root_InstanceRoot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1) 프리팹으로 게임 오브젝트 객제 동적 생성
            // 가장 기본적인 소환
            Instantiate(Prefab_Sample, Transform_SpawnPos.position, Quaternion.identity);

            // 2) 프리팹으로 게임 오브젝트 객체 동적 생성하고 특정 오브젝트에 하이어라키 설정하기 (Parenting)
            // 소환 후 변수에 담아서 부모 지정하기
            var testGameObject = Instantiate(Prefab_Sample, Transform_SpawnPos.position, Quaternion.identity);
            testGameObject.transform.SetParent(Root_InstanceRoot.transform);

            // 프리팹으로 게임 오브젝트 객체 동적 생성하고 바로 부모 설정
            // 소환과 동시에 부모 지정하기 (실무에서 가장 많이 씀)
            Instantiate(Prefab_Sample, Transform_SpawnPos.position, Quaternion.identity, Root_InstanceRoot.transform);
        }
    }
}
