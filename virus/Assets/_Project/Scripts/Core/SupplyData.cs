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
}
