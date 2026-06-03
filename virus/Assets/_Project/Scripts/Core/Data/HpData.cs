using UnityEngine;

// 체력 데이터
[System.Serializable]
public class HpData
{
    [Header("체력")]
    // 현재 체력
    public int current;

    // 최대 체력
    public int max;

    // 일일 자연 회복량
    public int dailyHeal;

    // 현재 체력을 0~max 범위로 고정
    public void Clamp()
    {
        current = Mathf.Clamp(current, 0, max);
    }
}
