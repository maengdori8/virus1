using UnityEngine;

// 연구 관리
public class ResearchManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public BattleManager battleManager;
    public GameManager gameManager;

    [Header("연구 단계 (순서대로 1→2→3)")]
    public ResearchStageSO[] stages;

    private int stageIndex;  // 현재 단계
    private int enemyIndex;  // 단계 내 현재 적

    // 샘플 충분한지 확인. sampleCost와 보유량 비교
    public bool CanStartStage(ResearchStageSO stage)
    {
        for (int i = 0; i < 3; i++)
        {
            if (gameState.sampleInventory[i] < stage.sampleCost[i])
                return false;
        }
        return true;
    }

    // 단계 진입. 샘플 소비 후 첫 적과 전투
    public void StartStage(int index)
    {
        // 잘못된 단계 인덱스 / 적 없는 단계 방어
        if (index < 0 || index >= stages.Length) return;
        ResearchStageSO stage = stages[index];
        if (stage == null || stage.enemies == null || stage.enemies.Length == 0) return;

        if (!CanStartStage(stage)) return;

        for (int i = 0; i < 3; i++)
            gameState.sampleInventory[i] -= stage.sampleCost[i];

        stageIndex = index;
        enemyIndex = 0;
        battleManager.StartBattle(stage.enemies[enemyIndex], OnStageWin, OnStageLose);
    }

    // 전투 승리. 다음 적이 있으면 이어 싸우고, 없으면 단계 클리어
    private void OnStageWin()
    {
        ResearchStageSO stage = stages[stageIndex];
        enemyIndex++;

        // 단계 안에 적이 더 있으면 다음 적과 전투
        if (enemyIndex < stage.enemies.Length)
        {
            battleManager.StartBattle(stage.enemies[enemyIndex], OnStageWin, OnStageLose);
            return;
        }

        // 단계 클리어: 백신 진행도 획득
        gameState.vaccineProgress += stage.progressGain;
        gameState.vaccineProgress = Mathf.Clamp(gameState.vaccineProgress, 0, 100);
        gameManager.CheckClear();
    }

    // 전투 패배. 소비한 샘플은 복구 안 됨
    private void OnStageLose()
    {
        Debug.Log("연구 단계 실패");
    }
}
