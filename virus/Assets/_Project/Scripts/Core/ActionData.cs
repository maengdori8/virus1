// 선택지 실행 결과
[System.Serializable]
public class ActionData
{
    // 체력 변화량
    public int hpChange;

    // 스태미나 변화량
    public int staminaChange;

    // 물자 변화량
    public int suppliesChange;

    // 샘플 변화량 (Wood/Fire/Earth/Metal/Water 순)
    public int[] sampleChange = new int[5];

    // 백신 진행도 변화량
    public int vaccineChange;
}
