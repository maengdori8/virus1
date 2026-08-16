using UnityEngine;

// 전투에서 고를 수 있는 공격 한 종류
[CreateAssetMenu(fileName = "Attack", menuName = "Data/Attack")]
public class AttackSO : ScriptableObject
{
    [Header("기본 정보")]
    // 버튼에 뜨는 이름
    public string attackName;

    // 이 공격의 오행 속성. 상성은 내 속성이 아니라 이 값으로 따진다
    public ElementType element;

    // 설명
    public string description;

    [Header("위력")]
    // 내 공격력 대비 위력 (100 이면 그대로)
    public int powerPercent = 100;

    // 때리는 횟수. 한 대마다 따로 적 방어를 뺀다
    public int hitCount = 1;

    // 적 방어를 무시하고 위력을 그대로 넣는다
    public bool ignoreDefense;

    // 적이 먼저 움직인 뒤에 이쪽 공격이 들어간다.
    // 뜸을 들이는 대신 때린 뒤에는 적이 반격할 턴이 없다
    public bool enemyMovesFirst;

    [Header("소모")]
    // 이 공격 한 번에 쓰는 스태미나 (탐사 전투에서만 실제로 빠짐)
    public int staminaCost = 3;

    [Header("효과음")]
    // SoundManager.sfxClips 의 번호. -1 이면 소리 없음
    public int sfxIndex = -1;
}
