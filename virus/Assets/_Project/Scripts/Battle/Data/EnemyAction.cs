using UnityEngine;

// 적 행동 종류
public enum EnemyActionType
{
    Attack,       // 일반 공격
    StrongAttack, // 강공격
    Defend        // 방어
}

// 적의 한 턴 행동
[System.Serializable]
public class EnemyAction
{
    [Header("행동")]
    // 행동 종류
    public EnemyActionType type;

    // 수치 (강공격 추가 데미지 / 방어량)
    public int value;
}
