using UnityEngine;

// 플레이어 전투 데이터
[System.Serializable]
public class PlayerBattleData
{
    [Header("전투 스탯")]
    // 공격력
    public int attack;

    // 방어력
    public int defense;

    // 오행 속성
    public ElementType element;
}
