using UnityEngine;

// 탐사 지역
[CreateAssetMenu(fileName = "Exploration", menuName = "Data/Exploration")]
public class ExplorationSO : ScriptableObject
{
    // 지역 이름
    public string areaName;

    // 탐사 스태미나 한도
    public int staminaLimit;

    // 오행 속성
    public ElementType element;

    // 발생 가능 이벤트
    public EventSO[] events;
}
