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
        gameState.consciousness += reward.consciousnessChange;

        // 모든 값을 정상 범위로 고정
        gameState.hp.Clamp();
        gameState.stamina.Clamp();
        gameState.supply.Clamp();
        gameState.vaccineProgress = Mathf.Clamp(gameState.vaccineProgress, 0, 100);
        gameState.consciousness = Mathf.Clamp(gameState.consciousness, 0, 100);

        // 보유 샘플 더하거나 차감 (음수 방지).
        // 인스펙터에서 새로 만든 선택지는 sampleChange 가 빈 배열이라 길이를 믿으면 안 된다
        if (reward.sampleChange != null)
        {
            int count = Mathf.Min(reward.sampleChange.Length, gameState.sampleInventory.Length);

            for (int i = 0; i < count; i++)
            {
                gameState.sampleInventory[i] += reward.sampleChange[i];
                gameState.sampleInventory[i] = Mathf.Max(gameState.sampleInventory[i], 0);
            }
        }

        // 획득 아이템 인벤토리에 추가
        if (reward.itemGain != null)
        {
            for (int i = 0; i < reward.itemGain.Length; i++)
            {
                if (reward.itemGain[i] != null)
                    gameState.itemInventory.Add(reward.itemGain[i]);
            }
        }
    }
}
// +면 보상 -면 페널티
