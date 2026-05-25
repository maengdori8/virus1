using UnityEngine;

// 물자 관리
public class SupplyManager : MonoBehaviour
{
    [Header("참조")]
    public SupplyData supplyData;
    public HpData hpData;

    // 물자에서 dailyCost 차감. 부족하면 부족분만큼 체력 깎음
    public void ConsumeDaily()
    {
        supplyData.current -= supplyData.dailyCost;

        // 물자 부족하면 체력 깎기
        if (supplyData.current < 0)
        {
            hpData.current += supplyData.current;
            supplyData.current = 0;
        }
    }
}
