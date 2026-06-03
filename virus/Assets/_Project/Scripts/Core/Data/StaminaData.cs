using UnityEngine;

// 스태미나 데이터
[System.Serializable]
public class StaminaData
{
    [Header("스태미나")]
    // 현재 스태미나
    public int current;

    // 최대 스태미나
    public int max;

    // 현재 스태미나를 0~max 범위로 고정
    public void Clamp()
    {
        current = Mathf.Clamp(current, 0, max);
    }
}
