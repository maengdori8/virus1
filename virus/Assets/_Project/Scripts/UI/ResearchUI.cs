using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 연구 화면 UI (Research 씬)
public class ResearchUI : MonoBehaviour
{
    [Header("참조")]
    public ResearchManager researchManager;
    public BattleManager battleManager;
    public GameState gameState;

    [Header("단계 버튼 (1→3 순)")]
    public Button[] stageButtons;

    // 단계별 필요 샘플 표시
    public TextMeshProUGUI[] stageCostTexts;

    [Header("진행도")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    // 매 프레임 잠금/비용/진행도 갱신
    private void Update()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            ResearchStageSO stage = i < researchManager.stages.Length ? researchManager.stages[i] : null;
            bool open = stage != null && researchManager.IsStageUnlocked(i) && !battleManager.InBattle();

            stageButtons[i].interactable = open && researchManager.CanStartStage(stage);

            if (stage != null && i < stageCostTexts.Length)
                stageCostTexts[i].text = CostLabel(stage);
        }

        if (progressBar != null)
            progressBar.value = gameState.vaccineProgress / 100f;

        if (progressText != null)
            progressText.text = gameState.vaccineProgress + "%";
    }

    // 단계 버튼에 연결
    public void OnClickStage(int index)
    {
        researchManager.StartStage(index);
    }

    // 필요 샘플 문구 (바다/산/도시 순)
    private string CostLabel(ResearchStageSO stage)
    {
        return "바다 " + stage.sampleCost[0] + " / 산 " + stage.sampleCost[1] + " / 도시 " + stage.sampleCost[2];
    }
}
