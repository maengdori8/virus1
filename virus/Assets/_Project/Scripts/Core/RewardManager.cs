using UnityEngine;

// 보상 적용
public class RewardManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    // ActionData를 받아서 GameState에 적용
    public void Apply(ActionData reward)
    {
        gameState.hp.current += reward.hpChange;
        gameState.stamina.current += reward.staminaChange;
        gameState.supply.current += reward.suppliesChange;
        gameState.vaccineProgress += reward.vaccineChange;

        // 보유 샘플 더하거나 차감
        for (int i = 0; i < 5; i++)
        {
            gameState.sampleInventory[i] += reward.sampleChange[i];
        }
    }
}
// +면 보상 -면 페널티
