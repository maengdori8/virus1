using UnityEngine;

// 시간 관리
public class TimeManager : MonoBehaviour
{
    [Header("참조")]
    public TimeData timeData;

    // 타임 턴 1 소모
    public void SpendTimeTurn()
    {
        timeData.timeTurn--;

        if (timeData.timeTurn <= 0)
        {
            EndDay();
        }
    }

    // 하루 종료
    private void EndDay()
    {
        timeData.timeTurn = timeData.maxTimeTurn;
        timeData.dayTurn--;
    }
}
