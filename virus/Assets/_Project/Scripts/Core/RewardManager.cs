using UnityEngine;

// 보상 적용
public class RewardManager : MonoBehaviour
{
    public GameState gameState;

    // ActionData를 받아서 GameState에 적용? <- 단어 안떠올라요
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
// 패널티 또한 이 메서드로 처리합니다
// +면 보상 -면 페널티
// 복잡하면 페넕티/보상으로 나눠도 되는데 하나의 코드를 응용하는 게 좋다고 판단해서 이렇게 썼습니다