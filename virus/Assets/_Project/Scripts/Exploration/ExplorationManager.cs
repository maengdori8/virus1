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
    // 안쪽으로 한 번 들어갈 때 드는 스태미나. 깊이에 비례해서 늘어난다 (1, 2, 3…)
    public int deeperStaminaCost = 1;

    // 더 들어갈 수 있는 최대 깊이
    public int maxDepth = 3;

    // 현재 탐사 중인 지역
    private ExplorationSO currentArea;

    // 안쪽으로 들어간 횟수. 깊을수록 샘플을 더 챙김
    private int depth;

    // 직전에 뽑은 이벤트
    private EventSO lastEvent;

    // 지역 저장 + 스태미나 한도 세팅 + 패널티 리셋
    public void StartExploration(ExplorationSO area)
    {
        currentArea = area;
        depth = 0;
        lastEvent = null;

        // 다녀온 지역의 속성이 내 속성이 된다.
        // 다음 탐사 전까지 유지되므로 낮의 연구 전투도 이 속성으로 치른다. 버그 아님
        gameState.battle.element = area.element;

        gameState.stamina.current = area.staminaLimit;
        gameState.stamina.Clamp();
        staminaManager.ResetPenalty();
    }

    // 현재 지역의 이벤트 중 하나를 랜덤으로 반환.
    // 직전에 나온 건 빼고 뽑는다. 같은 게 연달아 뜨면 탐사가 고장난 것처럼 보인다
    public EventSO GetRandomEvent()
    {
        EventSO[] pool = currentArea.events;

        if (pool.Length <= 1)
        {
            lastEvent = pool[0];
            return lastEvent;
        }

        int index = Random.Range(0, pool.Length);
        if (pool[index] == lastEvent)
            index = (index + Random.Range(1, pool.Length)) % pool.Length;

        lastEvent = pool[index];
        return lastEvent;
    }

    // 스태미나 차감 후 결과 적용. 스태미나 소진되면 자동 복귀
    public void SelectChoice(ChoiceData choice)
    {
        // 복귀가 이미 걸린 뒤에 들어온 입력. 스태미나는 안 깎이는데 보상만 또 들어간다
        if (!IsExploring()) return;

        bool depleted = staminaManager.Spend(choice.staminaCost);
        rewardManager.Apply(choice.result);
        ApplyDepthBonus(choice.result);

        // 보상으로 스태미나가 바닥난 경우도 복귀
        if (depleted || gameState.stamina.current <= 0) Return();
    }

    // 안쪽에서 캔 만큼 샘플 추가 획득. 깊이만큼 종류별로 몇 개 더 붙는다.
    // 원래 획득량에 곱하면 큰 선택지만 계속 불어나서 깊이가 무조건 정답이 된다
    private void ApplyDepthBonus(ActionData result)
    {
        if (depth <= 0) return;
        if (result.sampleChange == null) return;

        for (int i = 0; i < result.sampleChange.Length && i < 3; i++)
        {
            if (result.sampleChange[i] <= 0) continue;

            gameState.sampleInventory[i] += depth;
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
        if (!IsExploring()) return;

        int beforeDay = gameState.time.dayTurn;

        timeManager.SpendTimeTurn();
        staminaManager.Gain(restStaminaGain);

        // 날짜가 넘어갔으면 하루치 소비도 같이 치러야 한다.
        // 안 그러면 밤에 눌러앉아 쉬기만 해도 물자와 의식이 안 깎여서 샘플을 무한정 모을 수 있다
        if (gameState.time.dayTurn < beforeDay)
            gameManager.StartDay();
    }

    // 표지판: 안쪽으로 더 들어가기. 들어갈수록 값이 비싸진다.
    // 더 못 들어가면 false. 이때 화면을 새로 그리면 공짜로 이벤트를 다시 뽑게 된다
    public bool GoDeeper()
    {
        if (!IsExploring()) return false;
        if (depth >= maxDepth) return false;

        // 스태미나가 없으면 더 못 들어가고 그대로 복귀
        if (gameState.stamina.current <= 0)
        {
            Return();
            return false;
        }

        depth++;

        bool depleted = staminaManager.Spend(deeperStaminaCost * depth);
        if (depleted) Return();

        return true;
    }

    // 다음에 더 들어갈 때 드는 스태미나 (UI 표시용). 못 들어가면 0
    public int GetDeeperCost()
    {
        if (depth >= maxDepth) return 0;

        return deeperStaminaCost * (depth + 1);
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
