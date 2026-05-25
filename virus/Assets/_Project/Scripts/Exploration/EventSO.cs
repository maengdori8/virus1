using UnityEngine;

// 탐사 중 발생 이벤트
[CreateAssetMenu(fileName = "Event", menuName = "Data/Event")]
public class EventSO : ScriptableObject
{
    [Header("이벤트")]
    // 이벤트 설명
    public string description;

    // 선택지 목록
    public ChoiceData[] choices;
}
