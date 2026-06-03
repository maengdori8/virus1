using UnityEngine;

// 시간 관리
public class TimeManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    // 타임 턴 1 감소. 0 되면 EndDay() 호출
    public void SpendTimeTurn()
    {
        gameState.time.timeTurn--;

        if (gameState.time.timeTurn <= 0)
        {
            EndDay();
        }
    }

    // timeTurn 초기화 + dayTurn 감소 + 누적 일수 증가
    private void EndDay()
    {
        gameState.time.timeTurn = gameState.time.maxTimeTurn;
        gameState.time.dayTurn--;
        gameState.currentDay++;
    }
}
