using UnityEngine;

// 아이템 정의
[CreateAssetMenu(fileName = "Item", menuName = "Data/Item")]
public class ItemSO : ScriptableObject
{
    // 아이템 이름
    public string itemName;

    // 아이템 분류
    public ItemType itemType;

    // 설명
    public string description;

    // 사용 효과
    public ActionData effect;
}
