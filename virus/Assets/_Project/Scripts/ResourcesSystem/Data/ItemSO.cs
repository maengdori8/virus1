using UnityEngine;

// 아이템 정의
[CreateAssetMenu(fileName = "Item", menuName = "Data/Item")]
public class ItemSO : ScriptableObject
{
    [Header("기본 정보")]
    // 아이템 이름
    public string itemName;

    // 아이템 분류
    public ItemType itemType;

    // 설명
    public string description;

    [Header("효과")]
    // 사용 효과
    public ActionData effect;

    [Header("전투 약 효과")]
    // 공격력 증가량
    public int buffAttack;

    // 방어력 증가량
    public int buffDefense;

    // 약효 지속 턴
    public int buffDuration;
}
