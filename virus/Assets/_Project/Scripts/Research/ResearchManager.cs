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

    private int clearedCount; // 클리어한 단계 수 (잠금 해제 기준)
    private int stageIndex;   // 현재 도전 중인 단계
    private int enemyIndex;   // 단계 내 현재 적

    // 새 게임 시작 시 연구 진행 초기화
    public void ResetProgress()
    {
        clearedCount = 0;
    }

    // 해당 단계가 열렸는지 (이전 단계까지 클리어해야 열림)
    public bool IsStageUnlocked(int index)
    {
        return index >= 0 && index <= clearedCount;
    }

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

    // 단계 진입. 잠금·샘플 확인 후 첫 적과 전투
    public void StartStage(int index)
    {
        // 잘못된 인덱스 / 빈 단계 / 잠긴 단계 방어
        if (index < 0 || index >= stages.Length) return;
        if (!IsStageUnlocked(index)) return;

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

        // 단계 클리어: 백신 진행도 획득 + 다음 단계 잠금 해제
        gameState.vaccineProgress += stage.progressGain;
        gameState.vaccineProgress = Mathf.Clamp(gameState.vaccineProgress, 0, 100);
        if (stageIndex + 1 > clearedCount) clearedCount = stageIndex + 1;

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
