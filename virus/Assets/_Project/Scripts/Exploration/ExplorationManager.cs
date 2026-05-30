using UnityEngine;

// 탐사 관리
public class ExplorationManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public RewardManager rewardManager;
    public TimeManager timeManager;
    public BattleManager battleManager;

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
    // 적이랑 전투 시작, 콜백으로 승패 처리
    public void StartBattle(EnemySO ememy)
    {
        battleManager.StartBattle(ememy, OnExeploreWin, OnExeploreLose);
    }

    // 탐사 전투 승리 / 탐사 이어서 진행
    private void OnExeploreWin()
    {
        Debug.Log("탐사 전투 승리");
    }

    // 탐사 전투 패배. 강제복귀
    private void OnExeploreLose()
    {
        Debug.Log("탐사 전투 패배 - 강제복귀");
        Return();
    }


    // 턴 1 소모 + 현재 지역 초기화. 탐사 종료
    public void Return()
    {
        timeManager.SpendTimeTurn();
        currentArea = null;
    }
}
