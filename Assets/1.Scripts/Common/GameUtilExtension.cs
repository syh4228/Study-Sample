using UnityEngine;

public static class GameUtilExtension // 오브젝트 삭제 컴포넌트
{
    public static void RemoveComponent<T>(this GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component != null)
        {
            Debug.LogWarning($"{obj.name} 오브젝트에서 {typeof(T).Name} 컴포넌트가 제거되었습니다.");
            Object.Destroy(component);
        }
    }
}
