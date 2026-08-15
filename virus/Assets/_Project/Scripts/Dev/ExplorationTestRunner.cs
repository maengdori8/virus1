using TMPro;
using UnityEngine;

// 탐사 확인용. 선택지, 깊이, 휴식, 전투를 키로 굴려본다
public class ExplorationTestRunner : MonoBehaviour
{
    [Header("참조")]
    public GameInitializer initializer;
    public ExplorationManager explorationManager;
    public BattleManager battleManager;

    [Header("데이터")]
    public ExplorationSO area;
    public EnemySO enemy;

    [Header("표시 (없어도 됨)")]
    public TextMeshProUGUI returnText;

    // 지금 보고 있는 이벤트
    private EventSO current;

    private void Start()
    {
        if (explorationManager == null || explorationManager.gameManager == null || area == null)
        {
            Debug.LogError("[ExplorationTest] 참조가 비어 있음");
            enabled = false;
            return;
        }

        if (initializer != null) initializer.Init();
        explorationManager.gameManager.StartNight();
        explorationManager.StartExploration(area);

        Draw();
        HideReturnText();

        Debug.Log("[ExplorationTest] 1~3 선택지 / D 더 들어가기 / E 휴식 / F 전투 / Space 공격 / R 복귀");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) Choose(0);
        if (Input.GetKeyUp(KeyCode.Alpha2)) Choose(1);
        if (Input.GetKeyUp(KeyCode.Alpha3)) Choose(2);

        if (Input.GetKeyUp(KeyCode.D)) Deeper();
        if (Input.GetKeyUp(KeyCode.E)) Rest();
        if (Input.GetKeyUp(KeyCode.F)) Fight();

        if (Input.GetKeyUp(KeyCode.Space) && battleManager != null && battleManager.InBattle())
            battleManager.PlayerAttack();

        if (Input.GetKeyUp(KeyCode.R) && Active())
        {
            explorationManager.Return();
            ShowReturnText();
        }
    }

    // 탐사가 끝난 뒤에도 키가 먹으면 턴과 물자가 두 번씩 소모된다.
    // 전투 중에도 막는다. 전투가 탐사보다 오래 남으면 복귀가 두 번 불린다
    private bool Active()
    {
        if (explorationManager == null || !explorationManager.IsExploring())
        {
            Debug.Log("[ExplorationTest] 탐사가 이미 끝났음");
            return false;
        }

        if (battleManager != null && battleManager.InBattle())
        {
            Debug.Log("[ExplorationTest] 전투를 먼저 끝낼 것 (Space)");
            return false;
        }

        return true;
    }

    // 이벤트를 새로 뽑아 선택지를 콘솔에 출력
    private void Draw()
    {
        if (!explorationManager.IsExploring())
        {
            Debug.Log("[ExplorationTest] 탐사가 끝났음");
            return;
        }

        current = explorationManager.GetRandomEvent();
        if (current == null) return;

        int deeperCost = explorationManager.GetDeeperCost();

        string line = "깊이 " + explorationManager.GetDepth() +
                      (deeperCost > 0 ? " (더 들어가기 " + deeperCost + ")" : " (제일 안쪽)") +
                      " / 스태미나 " + explorationManager.gameState.stamina.current +
                      "\n" + current.description;

        if (current.enemy != null)
        {
            line += "\n  F: 싸운다 (" + current.enemy.enemyName + ")";
        }
        else if (current.choices != null)
        {
            for (int i = 0; i < current.choices.Length; i++)
                line += "\n  " + (i + 1) + ": " + current.choices[i].text +
                        " (스태미나 " + current.choices[i].staminaCost + ")";
        }

        Debug.Log(line);
    }

    private void Choose(int index)
    {
        if (!Active()) return;
        if (current == null || current.choices == null) return;
        if (index >= current.choices.Length) return;

        int[] before = (int[])explorationManager.gameState.sampleInventory.Clone();

        explorationManager.SelectChoice(current.choices[index]);

        int[] after = explorationManager.gameState.sampleInventory;
        Debug.Log("샘플 변화: 바다 +" + (after[0] - before[0]) +
                  " 산 +" + (after[1] - before[1]) +
                  " 도시 +" + (after[2] - before[2]));

        Draw();
    }

    private void Deeper()
    {
        if (!Active()) return;

        // 제일 안쪽이면 안 들어가진다. 그때 새로 뽑으면 공짜로 이벤트만 바꾸는 셈
        if (!explorationManager.GoDeeper())
        {
            Debug.Log("[ExplorationTest] 더 못 들어감 (제일 안쪽이거나 스태미나 없음)");
            return;
        }

        Draw();
    }

    private void Rest()
    {
        if (!Active()) return;

        explorationManager.Rest();
        Debug.Log("휴식. 스태미나 " + explorationManager.gameState.stamina.current);
        Draw();
    }

    private void Fight()
    {
        if (!Active()) return;

        EnemySO target = current != null && current.enemy != null ? current.enemy : enemy;
        if (target == null) return;

        explorationManager.StartBattle(target);
        Debug.Log("전투 시작: " + target.enemyName + " (Space 로 공격)");
    }

    private void HideReturnText()
    {
        if (returnText == null) return;

        returnText.gameObject.SetActive(false);
    }

    private void ShowReturnText()
    {
        if (returnText == null) return;

        returnText.text = "복귀";
        returnText.gameObject.SetActive(true);
    }
}
