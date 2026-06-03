using UnityEngine;

// 보상 적용
public class RewardManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    // ActionData의 변화량을 GameState에 전부 더함. +면 보상, -면 패널티
    public void Apply(ActionData reward)
    {
        gameState.hp.current += reward.hpChange;
        gameState.stamina.current += reward.staminaChange;
        gameState.supply.current += reward.suppliesChange;
        gameState.vaccineProgress += reward.vaccineChange;

        // 백신 진행도 0~100 범위로 고정
        gameState.vaccineProgress = Mathf.Clamp(gameState.vaccineProgress, 0, 100);

        // 보유 샘플 더하거나 차감
        for (int i = 0; i < 3; i++)
        {
            gameState.sampleInventory[i] += reward.sampleChange[i];
        }
    }
}
// +면 보상 -면 페널티
