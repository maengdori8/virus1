using UnityEngine;

// 적 데이터
[CreateAssetMenu(fileName = "Enemy", menuName = "Data/Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("기본 정보")]
    // 적 이름
    public string enemyName;

    // 오행 속성
    public ElementType element;

    [Header("스탯")]
    // 체력
    public HpData hp;

    // 공격력
    public int attack;

    // 방어력
    public int defense;
}
