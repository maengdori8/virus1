using UnityEngine;

// 연구 관리
public class ResearchManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public BattleManager battleManager;
    public GameManager gameManager;
    public ConsciousnessManager consciousnessManager;
    public StaminaManager staminaManager;

    [Header("연구 단계 (순서대로 1→2→3)")]
    public ResearchStageSO[] stages;

    [Header("입장 비용")]
    // 단계에 들어갈 때 한 번만 나간다. 벙커 안이라 전투 중 행동은 스태미나를 쓰지 않는다
    public int enterStaminaCost = 20;

    private int stageIndex;   // 현재 도전 중인 단계
    private int enemyIndex;   // 단계 내 현재 적

    // 새 게임 시작 시 연구 진행 초기화
    public void ResetProgress()
    {
        gameState.reasearchCleared = 0;
    }

    // 해당 단계가 열렸는지 (이전 단계까지 클리어해야 열림)
    public bool IsStageUnlocked(int index)
    {
        return index == gameState.reasearchCleared;
    }

    // 샘플 충분한지 확인. sampleCost와 보유량 비교
    public bool CanStartStage(ResearchStageSO stage)
    {
        if (stage == null || stage.sampleCost == null) return false;

        int count = Mathf.Min(stage.sampleCost.Length, gameState.sampleInventory.Length);

        for (int i = 0; i < count; i++)
        {
            if (gameState.sampleInventory[i] < stage.sampleCost[i])
                return false;
        }
        return true;
    }

    // 단계 진입. 잠금·샘플 확인 후 첫 적과 전투
    public void StartStage(int index)
    {
        // 잘못된 인덱스 / 빈 단계 / 잠긴 단계 방어
        if (stages == null || index < 0 || index >= stages.Length) return;
        if (!IsStageUnlocked(index)) return;

        // 전투 중에 또 들어오면 샘플만 한 번 더 깎이고 진행 중인 전투가 덮어써진다
        if (battleManager.InBattle()) return;

        ResearchStageSO stage = stages[index];
        if (stage == null || stage.enemies == null || stage.enemies.Length == 0) return;
        if (!CanStartStage(stage)) return;

        int costCount = Mathf.Min(stage.sampleCost.Length, gameState.sampleInventory.Length);
        for (int i = 0; i < costCount; i++)
            gameState.sampleInventory[i] -= stage.sampleCost[i];

        // 입장할 때만 소모. 전투가 길어져도 더 나가지 않는다
        if (staminaManager != null) staminaManager.Spend(enterStaminaCost);

        stageIndex = index;
        enemyIndex = 0;
        battleManager.StartBattle(stage.enemies[enemyIndex], OnStageWin, OnStageLose);
    }

    // 전투 승리. 다음 적이 있으면 이어 싸우고, 없으면 단계 클리어
    private void OnStageWin()
    {
        ResearchStageSO stage = stages[stageIndex];
        enemyIndex++;

        // 단계 안에 적이 더 있으면 다음 적과 전투. 한 판이 이어지는 거라 약효도 이어진다
        if (enemyIndex < stage.enemies.Length)
        {
            battleManager.StartBattle(stage.enemies[enemyIndex], OnStageWin, OnStageLose, false, true);
            return;
        }

        // 단계 클리어: 백신 진행도 획득 + 다음 단계 잠금 해제
        gameState.vaccineProgress += stage.progressGain;
        gameState.vaccineProgress = Mathf.Clamp(gameState.vaccineProgress, 0, 100);
        if (stageIndex + 1 > gameState.reasearchCleared) gameState.reasearchCleared = stageIndex + 1;

        // 추가 보상: 스태미나 최대치 / 체력 / 의식 회복
        gameState.stamina.max += stage.staminaMaxGain;
        gameState.hp.current += stage.healAmount;
        gameState.hp.Clamp();
        if (consciousnessManager != null)
            consciousnessManager.Recover(stage.consciousnessGain);

        // 마지막 단계 = 최종보스 처치 → 백신 완성
        if (stageIndex == stages.Length - 1)
        {
            gameState.vaccineProgress = 100;
            Debug.Log("최종보스 처치 - 백신 완성!");
        }

        gameManager.CheckClear();
    }

    // 전투 패배. 소비한 샘플은 복구 안 됨
    private void OnStageLose()
    {
        Debug.Log("연구 단계 실패");
    }
}
