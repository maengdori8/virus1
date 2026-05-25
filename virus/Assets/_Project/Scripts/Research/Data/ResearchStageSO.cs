using UnityEngine;

// 백신 연구 단계
[CreateAssetMenu(fileName = "ResearchStage", menuName = "Data/ResearchStage")]
public class ResearchStageSO : ScriptableObject
{
    [Header("단계 정보")]
    // 단계 (1~3)
    public int stage;

    // 필요 샘플 (Wood/Fire/Earth/Metal/Water 순)
    public int[] sampleCost = new int[5];

    [Header("전투")]
    // 등장 적
    public EnemySO[] enemies;

    [Header("보상")]
    // 백신 진행도 획득량
    public int progressGain;
}
