using UnityEngine;

// 적 데이터
[CreateAssetMenu(fileName = "Enemy", menuName = "Data/Enemy")]
public class EnemySO : ScriptableObject
{
    // 적 이름
    public string enemyName;

    // 체력
    public int hp;

    // 공격력
    public int attack;

    // 방어력
    public int defense;

    // 오행 속성
    public ElementType element;
}
