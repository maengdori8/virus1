using UnityEngine;

// 탐사 중 발생 이벤트
[CreateAssetMenu(fileName = "Event", menuName = "Data/Event")]
public class EventSO : ScriptableObject
{
    [Header("이벤트")]
    // 이벤트 설명
    public string description;

    // 이벤트 성격 그림 (평범/위험/행운/수상)
    public Sprite tagIcon;

    // 선택지 목록
    public ChoiceData[] choices;

    [Header("전투 이벤트 (비우면 일반 이벤트)")]
    // 등장 적
    public EnemySO enemy;
}
