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

    // 전투 화면에 뜨는 그림
    public Sprite portrait;

    [Header("스탯")]
    // 체력
    public HpData hp;

    // 공격력
    public int attack;

    // 방어력
    public int defense;

    [Header("행동 패턴 (순서대로 반복, 비우면 일반 공격만)")]
    // 반복되는 행동 순서
    public EnemyAction[] actions;

    [Header("보상 (탐사 전투 승리 시)")]
    // 승리 보상
    public ActionData reward;
}
