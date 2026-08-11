using UnityEngine;
using TMPro;

// 연구 확인용. 단계별 도전과 여러 마리 연전이 제대로 끝나는지 본다
public class ResearchTestRunner : MonoBehaviour
{
    [Header("참조")]
    public GameInitializer initializer;
    public ResearchManager researchManager;
    public BattleManager battleManager;

    [Header("연구 단계")]
    // 예전부터 쓰던 단일 단계 칸. 씬에 이미 연결돼 있으면 그대로 쓴다
    public ResearchStageSO stage;

    // 여러 단계를 이어서 보고 싶으면 여기에 넣는다 (비우면 위의 stage, 그것도 없으면 ResearchManager 것)
    public ResearchStageSO[] stages;

    [Header("설정")]
    public int startSampleCount = 20;

    [Header("표시 (없어도 됨)")]
    public TextMeshProUGUI infoText;

    // 직전 백신 진행도 (보상 들어온 걸 잡아내려고)
    private int lastProgress;

    private void Start()
    {
        Begin();
    }

    private void Begin()
    {
        if (researchManager == null || battleManager == null || researchManager.gameState == null)
        {
            Debug.LogError("[ResearchTest] 참조가 비어 있음");
            enabled = false;
            return;
        }

        // 전투 중에 다시 시작하면 옛 콜백이 살아남아 보상이 꼬인다
        if (battleManager.InBattle())
        {
            Debug.Log("[ResearchTest] 전투 중에는 다시 시작할 수 없음");
            return;
        }

        if (initializer != null) initializer.Init();

        if (stages != null && stages.Length > 0)
            researchManager.stages = stages;
        else if (stage != null)
            researchManager.stages = new[] { stage };

        GameState state = researchManager.gameState;
        state.sampleInventory = new[] { startSampleCount, startSampleCount, startSampleCount };
        state.stamina.current = state.stamina.max;
        lastProgress = state.vaccineProgress;

        int count = researchManager.stages != null ? researchManager.stages.Length : 0;
        if (count == 0)
            Debug.LogError("[ResearchTest] 연구 단계가 하나도 없음. stages 를 채울 것");

        Debug.Log("[ResearchTest] 1~3 단계 도전 / Space 공격 / R 다시하기  (등록된 단계 " + count + "개)");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R)) Begin();

        if (Input.GetKeyUp(KeyCode.Alpha1)) Try(0);
        if (Input.GetKeyUp(KeyCode.Alpha2)) Try(1);
        if (Input.GetKeyUp(KeyCode.Alpha3)) Try(2);

        if (Input.GetKeyUp(KeyCode.Space)) Attack();

        WatchProgress();
        Show();
    }

    // 단계 진입. 왜 막혔는지도 알려준다
    private void Try(int index)
    {
        // 전투 중에 다른 단계로 들어가면 진행 중인 전투가 덮어써진다
        if (battleManager.InBattle())
        {
            Debug.Log("[ResearchTest] 전투 중에는 단계에 들어갈 수 없음");
            return;
        }

        ResearchStageSO[] list = researchManager.stages;
        if (list == null || index >= list.Length || list[index] == null)
        {
            Debug.Log("[ResearchTest] " + (index + 1) + "단계가 없음");
            return;
        }

        if (list[index].sampleCost == null || list[index].sampleCost.Length < 3)
        {
            Debug.Log("[ResearchTest] " + (index + 1) + "단계의 필요 샘플이 3칸이 아님");
            return;
        }

        if (!researchManager.IsStageUnlocked(index))
        {
            Debug.Log("[ResearchTest] " + (index + 1) + "단계 잠김 (지금 도전할 단계는 " +
                      (researchManager.gameState.reasearchCleared + 1) + "단계)");
            return;
        }

        if (!researchManager.CanStartStage(list[index]))
        {
            int[] c = list[index].sampleCost;
            Debug.Log("[ResearchTest] 샘플 부족. 필요 바다 " + c[0] + " 산 " + c[1] + " 도시 " + c[2]);
            return;
        }

        int enemyCount = list[index].enemies != null ? list[index].enemies.Length : 0;
        Debug.Log("[ResearchTest] " + (index + 1) + "단계 시작. 적 " + enemyCount + "마리");

        researchManager.StartStage(index);
    }

    private void Attack()
    {
        if (!battleManager.InBattle())
        {
            Debug.Log("[ResearchTest] 전투 중이 아님");
            return;
        }

        EnemySO before = battleManager.GetEnemy();
        battleManager.PlayerAttack();

        // 연전이면 여기서 적이 바뀌어 있어야 한다
        if (battleManager.InBattle() && battleManager.GetEnemy() != before)
            Debug.Log("[ResearchTest] 다음 적 등장: " + battleManager.GetEnemy().enemyName);
    }

    // 진행도가 오르면 보상이 들어온 것
    private void WatchProgress()
    {
        int now = researchManager.gameState.vaccineProgress;
        if (now == lastProgress) return;

        Debug.Log("[ResearchTest] 백신 진행도 " + lastProgress + " → " + now +
                  " (클리어한 단계 " + researchManager.gameState.reasearchCleared + ")");
        lastProgress = now;

        if (now >= 100) Debug.Log("[ResearchTest] 백신 완성");
    }

    private void Show()
    {
        if (infoText == null) return;

        GameState state = researchManager.gameState;
        string line =
            "백신 " + state.vaccineProgress + "%  클리어 " + state.reasearchCleared + "단계\n" +
            "샘플 바다 " + state.sampleInventory[0] + " 산 " + state.sampleInventory[1] +
            " 도시 " + state.sampleInventory[2] + "\n" +
            "체력 " + state.hp.current + "/" + state.hp.max;

        if (battleManager.InBattle())
        {
            EnemySO e = battleManager.GetEnemy();
            line += "\n전투: " + e.enemyName + "  " + battleManager.GetEnemyHp() + "/" + e.hp.max;
        }

        infoText.text = line;
    }
}
