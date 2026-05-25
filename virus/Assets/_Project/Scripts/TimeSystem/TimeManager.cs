using UnityEngine;

// 시간 관리
public class TimeManager : MonoBehaviour
{
    [Header("참조")]
    public TimeData timeData;

    // 타임 턴 1 감소. 0 되면 EndDay() 호출
    public void SpendTimeTurn()
    {
        timeData.timeTurn--;

        if (timeData.timeTurn <= 0)
        {
            EndDay();
        }
    }

    // timeTurn 초기화 + dayTurn 1 감소
    private void EndDay()
    {
        timeData.timeTurn = timeData.maxTimeTurn;
        timeData.dayTurn--;
    }
}
