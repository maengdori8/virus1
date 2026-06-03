using UnityEngine;

// 물자 관리
public class SupplyManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    // 물자에서 dailyCost 차감. 부족하면 부족분만큼 체력 깎음
    public void ConsumeDaily()
    {
        gameState.supply.current -= gameState.supply.dailyCost;

        // 물자 부족하면 체력 깎기
        if (gameState.supply.current < 0)
        {
            gameState.hp.current += gameState.supply.current;
            gameState.supply.current = 0;
        }
    }
}
