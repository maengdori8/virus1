using UnityEngine;

// 연구 관리
public class ResearchManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public BattleManager battleManager;
    public RewardManager rewardManager;

    // 현재 진행 중인 연구 단계
    private ResearchStageSO currentStage;

    // 샘플 충분한지 확인. sampleCost와 보유량 비교
    public bool CanStartStage(ResearchStageSO stage)
    {
        for (int i = 0; i < 5; i++)
        {
            if (gameState.sampleInventory[i] < stage.sampleCost[i])
                return false;
        }
        return true;
    }

    // 샘플 소비 후 해당 단계 전투 시작. 패배 시 샘플 날아감
    public void StartStage(ResearchStageSO stage)
    {
        if (!CanStartStage(stage)) return;

        currentStage = stage;

        for (int i = 0; i < 5; i++)
        {
            gameState.sampleInventory[i] -= stage.sampleCost[i];
        }

        battleManager.StartBattle(stage.enemies[0]);
    }

    // 전투 승리 시 호출. 백신 진행도 획득
    public void OnStageWin()
    {
        gameState.vaccineProgress += currentStage.progressGain;
        currentStage = null;
    }

    // 전투 패배 시 호출. 소비한 샘플은 복구 안 됨
    public void OnStageLose()
    {
        currentStage = null;
    }
}
