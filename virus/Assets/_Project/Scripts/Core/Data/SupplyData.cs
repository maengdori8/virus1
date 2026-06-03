using UnityEngine;

// 물자 데이터
[System.Serializable]
public class SupplyData
{
    [Header("물자")]
    // 보유 물자
    public int current;

    // 일일 소비량
    public int dailyCost;

    // 물자가 음수가 되지 않도록 고정
    public void Clamp()
    {
        current = Mathf.Max(current, 0);
    }
}
