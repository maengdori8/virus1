using UnityEngine;

// 선택지 실행 결과
[System.Serializable]
public class ActionData
{
    [Header("스탯 변화량")]
    // 체력 변화량
    public int hpChange;

    // 스태미나 변화량
    public int staminaChange;

    [Header("자원 변화량")]
    // 물자 변화량
    public int suppliesChange;

    // 샘플 변화량 (바다/산/도시 순)
    public int[] sampleChange = new int[3];

    // 백신 진행도 변화량
    public int vaccineChange;
}
