using UnityEngine;

// 스태미나 관리
public class StaminaManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("설정")]
    // 소진 시 아이템 드랍 비율 (0~100)
    public int dropPercent = 50;

    // 패널티 발동 여부
    private bool penaltyApplied = false;

    // 0이면 차단. 차감 후 0 이하 되면 패널티 1회 발동
    public bool Spend(int amount)
    {
        if (gameState.stamina.current <= 0)
            return false;

        gameState.stamina.current -= amount;

        if (gameState.stamina.current <= 0 && !penaltyApplied)
        {
            gameState.stamina.current = 0;
            ApplyPenalty();
            penaltyApplied = true;
            return true;
        }
        return false;
    }

    // 탐사 시작 시 호출. 패널티 플래그 초기화
    public void ResetPenalty()
    {
        penaltyApplied = false;
    }

    // 보유 아이템의 dropPercent%만큼 뒤에서부터 제거
    private void ApplyPenalty()
    {
        int total = gameState.itemInventory.Count;
        int drop = total * dropPercent / 100;

        for (int i = 0; i < drop; i++)
        {
            gameState.itemInventory.RemoveAt(gameState.itemInventory.Count - 1);
        }
    }
}
