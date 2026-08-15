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
        if (stageButtons == null || researchManager.stages == null) return;

        for (int i = 0; i < stageButtons.Length; i++)
        {
            // 카드를 갈아끼우면 여기 연결이 끊긴다. 끊긴 칸 하나 때문에 매 프레임 예외가 쏟아지면 안 된다
            if (stageButtons[i] == null) continue;

            ResearchStageSO stage = i < researchManager.stages.Length ? researchManager.stages[i] : null;
            bool open = stage != null && researchManager.IsStageUnlocked(i) && !battleManager.InBattle();

            stageButtons[i].interactable = open && researchManager.CanStartStage(stage);

            if (stage != null && stageCostTexts != null && i < stageCostTexts.Length && stageCostTexts[i] != null)
                stageCostTexts[i].text = CostLabel(i, stage);
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

    // 필요 샘플 문구 (바다/산/도시 순).
    // 버튼이 왜 안 눌리는지가 화면에 안 보이면 샘플이 모자란 건지 잠긴 건지 알 수가 없다
    private string CostLabel(int index, ResearchStageSO stage)
    {
        if (index < gameState.reasearchCleared) return "완료";
        if (!researchManager.IsStageUnlocked(index)) return "이전 단계를 먼저";

        return Need(stage, 0, "바다") + " / " + Need(stage, 1, "산") + " / " + Need(stage, 2, "도시");
    }

    // 모자란 종류만 보유량을 같이 적는다
    private string Need(ResearchStageSO stage, int index, string label)
    {
        int need = stage.sampleCost[index];
        int have = gameState.sampleInventory[index];

        if (have >= need) return label + " " + need;

        return label + " " + have + "/" + need;
    }
}
