using UnityEngine;

// 게임 루프 관리
public class GameManager : MonoBehaviour
{
    [Header("참조")]
    public TimeManager timeManager;
    public SupplyManager supplyManager;
    public RewardManager rewardManager;

    [Header("상태")]
    public GameState gameState;

    // 하루 시작
    public void StartDay()
    {
        supplyManager.ConsumeDaily();
        HealDaily();

        if (gameState.hp.current <= 0)
        {
            GameOver();
            return;
        }

        if (gameState.vaccineProgress >= 100)
        {
            GameClear();
            return;
        }
    }

    // 체력 자연 회복
    private void HealDaily()
    {
        gameState.hp.current += gameState.hp.dailyHeal;

        if (gameState.hp.current > gameState.hp.max)
        {
            gameState.hp.current = gameState.hp.max;
        }
    }

    // 게임오버
    private void GameOver()
    {
        Debug.Log("게임오버");
        // 임시로 출력
    }

    // 클리어
    private void GameClear()
    {
        Debug.Log("백신 완성");
        // 임시로 출력
    }
}
