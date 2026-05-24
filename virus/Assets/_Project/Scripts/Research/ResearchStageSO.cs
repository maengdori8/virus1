using UnityEngine;

// 백신 연구 단계
[CreateAssetMenu(fileName = "ResearchStage", menuName = "Data/ResearchStage")]
public class ResearchStageSO : ScriptableObject
{
    // 단계 (1~3)
    public int stage;

    // 필요 샘플 (Wood/Fire/Earth/Metal/Water 순)
    public int[] sampleCost = new int[5];

    // 등장 적
    public EnemySO[] enemies;

    // 백신 진행도 획득량
    public int progressGain;
}
