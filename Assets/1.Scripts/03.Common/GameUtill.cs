using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;
// using Cysharp.Threading.Tasks;

public static class GameUtill
{
    /*
    // 마지막으로 할당된 ID를 전역적으로 기록 (스레드 안전)
    private static long _lastId = 0;

    // 게임 처음 켜질때 모든 기획 데이터를 불러옴
    public static void LoadFullData()
    {
        // 싱글턴(Instance)을 통해 데이터를 메모리에 올려두고, 이후 게임 중에 빠르게 꺼냄
        // 게임 로딩할 때 불러올 데이터는 여기서! 
        DaniTechGameDataManager.Instance.LoadSkillData("Skill");
        DaniTechGameDataManager.Instance.LoadCharacterData("Character");
        DaniTechGameDataManager.Instance.LoadWeaponData("Weapon");
        DaniTechGameDataManager.Instance.LoadCostumeData("Costume");
        DaniTechGameDataManager.Instance.LoadDNItemData("DNItem");
        DaniTechGameDataManager.Instance.LoadDNDialogueData();
        DaniTechGameDataManager.Instance.LoadAll();
    }

    // 캐릭터의 레벨과 추가 데미지를 더한 뒤, 크리티컬(치명타)이 터지면 데미지를 2배로 뻥튀기해 주는 순수 계산
    public static int CalcCharacterFinalDamage(int curCharacterLevel, int levelPerDamage, bool isCritical)
    {
        int damagePerLevel = (curCharacterLevel + levelPerDamage);
        int finalDamage = isCritical ? (damagePerLevel * 2) : damagePerLevel;
        return finalDamage;
    }

    // 구식 방식 (동기) [Resources.Load]
    // 단점: 리소스를 불러오는 동안 게임 화면이 멈춥니다(프레임 드랍)
    // 이제 잘 안씀
    public static Sprite LoadSpriteCanBeNull(string spriteName)
    {
        // 1. Resources/ 경로에서 이름으로 스프라이트 로드
        // 예: spriteName이 "Sword"라면 Assets/Resources/2D/Sword.png를 찾음
        // 이 2D같은 경로는 나중에 Sprite, Texture 등등 다양하게 바꿔도 무관합니다!
        Sprite loadedSprite = Resources.Load<Sprite>($"{spriteName}");

        if (loadedSprite != null)
        {
            return loadedSprite;
        }

        Debug.LogError($"에셋을 찾을 수 없습니다: {spriteName}");
        return null;
    }


    // 시리즈 : 현대적 방식 (비동기) => LoadAndSetSpriteImage, LoadAndPlayAudioClip, LoadAndSetTexture
    //  async와 await => (메모리나 어드레서블)에서 찾아올 테니, 게임은 멈추지 말고 계속 돌아가.
    // 다 찾으면 그때 화면에 띄워줌, 화면 버벅거림(렉)을 완벽하게 없애주는 최신 유니티 기술
    public static async UniTask<Sprite> LoadAndSetSpriteImage(Image targetImage, string spritePath) 
    {
        // 스프라이트
        Sprite sprite = await DaniTechResourceManager.Inst.LoadSprite(spritePath);
        if (sprite != null)
        {
            targetImage.sprite = sprite;
        }
        return sprite;
    }

    public static async UniTaskVoid LoadAndPlayAudioClip(AudioSource audioSource, string audioPath, bool isLoop = false)
    {
        // 오디오
        AudioClip clip = await DaniTechResourceManager.Inst.LoadAsset<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError($"{audioPath}를 찾을 수 없습니다! 어드레서블 설정이 되어 있는지 확인해주세요.");
            return;
        }

        if (isLoop == true)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public static async UniTaskVoid LoadAndSetTexture(RawImage targetRawImage, string texturePath)
    {
        // RawImage (텍스쳐)
        // 비동기로 로드하기 전까지는 해당 오브젝트를 잠깐 비활성화 해준다
        targetRawImage.gameObject.SetActive(false);
        Texture texture = await DaniTechResourceManager.Inst.LoadAsset<Texture>(texturePath);
        if (texture != null)
        {
            targetRawImage.texture = texture;
        }
        targetRawImage.gameObject.SetActive(true);
    }

    // 데이터 리스트 추출 로직
    // 대화 그룹 ID("dialogue_group_mainstream_1_1" 같은 것)를 던져주면,
    // 그 안에 포함된 세부 대화들의 ID 번호표들을 줄줄이 리스트로 뽑아다 주는 헬퍼 함수
    public static List<string> GetDialogueIdList(string dialogueGroupId)
    {
        var list = new List<string>();

        // "dialogue_group_mainstream_1_1"
        var data = DaniTechGameDataManager.Instance.GetDNDialogueGroupData(dialogueGroupId);
        if (data != null)
        {
            var idArr = data.DialogueIdList;
            // foreach문을 사용해 배열에 있는 데이터를 안전하게 List<string>으로 변환하여 반환
            foreach (var id in idArr)
            {
                list.Add(id);
            }
        }

        return list;
    }

    // 스레드 안전성 보장
    // 그냥 유니크 키가 발급되어야 할 때 사용하려고 만든 것 (의미가 있는 건 아니므로 사용만 하세요)
    public static long GenerateUniqueId()
    {
        long newId = DateTime.UtcNow.Ticks;

        // 원자적 연산으로 안전하게 ID 갱신
        while (true)
        {
            long lastId = Volatile.Read(ref _lastId);

            // 만약 현재 시간이 이전 ID보다 작거나 같다면 (루프가 너무 빠른 경우 포함)
            // 이전 ID + 1로 강제 설정하여 중복 방지
            long idToAssign = (newId <= lastId) ? lastId + 1 : newId;

            // _lastId가 내가 읽은 시점과 같다면 idToAssign으로 교체 (성공 시 루프 탈출)
            if (Interlocked.CompareExchange(ref _lastId, idToAssign, lastId) == lastId)
            {
                return idToAssign;
            }
            // 그 사이 다른 스레드가 값을 바꿨다면 다시 시도
        }
    } */
}
