using UnityEngine;

// 탐사 관리
public class ExplorationManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public RewardManager rewardManager;
    public TimeManager timeManager;
    public BattleManager battleManager;
    public StaminaManager staminaManager;
    public GameManager gameManager;

    [Header("휴식")]
    // 턴 낭비 시 회복할 스태미나
    public int restStaminaGain = 3;

    [Header("깊이")]
    // 안쪽으로 한 번 들어갈 때 드는 스태미나
    public int deeperStaminaCost = 1;

    // 더 들어갈 수 있는 최대 깊이
    public int maxDepth = 3;

    // 현재 탐사 중인 지역
    private ExplorationSO currentArea;

    // 안쪽으로 들어간 횟수. 깊을수록 샘플을 더 챙김
    private int depth;

    // 지역 저장 + 스태미나 한도 세팅 + 패널티 리셋
    public void StartExploration(ExplorationSO area)
    {
        currentArea = area;
        depth = 0;
        gameState.stamina.current = area.staminaLimit;
        gameState.stamina.Clamp();
        staminaManager.ResetPenalty();
    }

    // 현재 지역의 이벤트 중 하나를 랜덤으로 반환
    public EventSO GetRandomEvent()
    {
        int index = Random.Range(0, currentArea.events.Length);
        return currentArea.events[index];
    }

    // 스태미나 차감 후 결과 적용. 스태미나 소진되면 자동 복귀
    public void SelectChoice(ChoiceData choice)
    {
        bool depleted = staminaManager.Spend(choice.staminaCost);
        rewardManager.Apply(choice.result);
        ApplyDepthBonus(choice.result);

        // 보상으로 스태미나가 바닥난 경우도 복귀
        if (depleted || gameState.stamina.current <= 0) Return();
    }

    // 안쪽에서 캔 만큼 샘플 추가 획득. 깊이에 비례
    private void ApplyDepthBonus(ActionData result)
    {
        if (depth <= 0) return;
        if (result.sampleChange == null) return;

        for (int i = 0; i < result.sampleChange.Length && i < 3; i++)
        {
            if (result.sampleChange[i] <= 0) continue;

            gameState.sampleInventory[i] += result.sampleChange[i] * depth;
        }
    }
    // 적이랑 전투 시작, 콜백으로 승패 처리
    public void StartBattle(EnemySO ememy)
    {
        battleManager.StartBattle(ememy, OnExeploreWin, OnExeploreLose, true);
    }

    // 탐사 전투 승리. 보상 받고 탐사 이어서 진행
    private void OnExeploreWin()
    {
        Debug.Log("탐사 전투 승리");
        rewardManager.Apply(battleManager.GetEnemy().reward);
    }

    // 탐사 진행 중 여부 (UI 표시용)
    public bool IsExploring()
    {
        return currentArea != null;
    }

    // 탐사 전투 패배. 강제복귀
    private void OnExeploreLose()
    {
        Debug.Log("탐사 전투 패배 - 강제복귀");
        Return();
    }

    // 턴을 낭비해 스태미나 회복 (표지판: 휴식)
    public void Rest()
    {
        timeManager.SpendTimeTurn();
        staminaManager.Gain(restStaminaGain);
    }

    // 표지판: 안쪽으로 더 들어가기. 스태미나를 쓰고 깊이가 오름
    public void GoDeeper()
    {
        // 스태미나가 없으면 더 못 들어가고 그대로 복귀
        if (gameState.stamina.current <= 0)
        {
            Return();
            return;
        }

        if (depth < maxDepth) depth++;

        bool depleted = staminaManager.Spend(deeperStaminaCost);
        if (depleted) Return();
    }

    // 현재 깊이 (UI 표시용)
    public int GetDepth()
    {
        return depth;
    }

    // 턴 1 소모 + 지역 초기화 + 낮으로 복귀
    public void Return()
    {
        timeManager.SpendTimeTurn();
        currentArea = null;
        gameManager.EndNight();
    }
}
