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

    // 지역 저장 + 스태미나를 해당 지역 한도로 세팅
    public void StartExploration(ExplorationSO area)
    {
        currentArea = area;
        gameState.stamina.current = area.staminaLimit;
    }

    // 현재 지역의 이벤트 중 하나를 랜덤으로 반환
    public EventSO GetRandomEvent()
    {
        int index = Random.Range(0, currentArea.events.Length);
        return currentArea.events[index];
    }

    // 스태미나 차감 후 결과(ActionData)를 RewardManager에 넘김
    public void SelectChoice(ChoiceData choice)
    {
        gameState.stamina.current -= choice.staminaCost;
        rewardManager.Apply(choice.result);
    }

    // 턴 1 소모 + 현재 지역 초기화. 탐사 종료
    public void Return()
    {
        timeManager.SpendTimeTurn();
        currentArea = null;
    }
}
