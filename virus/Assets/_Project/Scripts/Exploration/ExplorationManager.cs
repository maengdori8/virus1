using UnityEngine;

// 탐사 관리
public class ExplorationManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public RewardManager rewardManager;
    public TimeManager timeManager;

    // 현재 탐사 중인 지역
    private ExplorationSO currentArea;

    // 탐사 시작
    public void StartExploration(ExplorationSO area)
    {
        currentArea = area;
        gameState.stamina.current = area.staminaLimit;
    }

    // 이벤트 랜덤 발생
    public EventSO GetRandomEvent()
    {
        int index = Random.Range(0, currentArea.events.Length);
        return currentArea.events[index];
    }

    // 선택지 선택
    public void SelectChoice(ChoiceData choice)
    {
        gameState.stamina.current -= choice.staminaCost;
        rewardManager.Apply(choice.result);
    }

    // 복귀 (1턴 소모)
    public void Return()
    {
        timeManager.SpendTimeTurn();
        currentArea = null;
    }
}
