using UnityEngine;
using TMPro;

// 전투 확인용. 씬 UI 없이 전투를 굴려보고 수치를 눈으로 볼 수 있다
public class BattleTestRunner : MonoBehaviour
{
    [Header("참조")]
    public BattleManager bm;

    // 있으면 시작할 때 초기값을 다시 넣는다
    public GameInitializer initializer;

    [Header("적")]
    public EnemySO enemy;

    [Header("약 (1, 2 키로 사용)")]
    public ItemSO drugA;
    public ItemSO drugB;

    [Header("표시 (없어도 됨)")]
    public TextMeshProUGUI infoText;

    private void Start()
    {
        Begin();
    }

    // 전투를 처음부터 다시
    private void Begin()
    {
        if (enemy == null || bm == null || bm.gameState == null)
        {
            Debug.LogError("[BattleTest] bm / enemy / bm.gameState 가 비어 있음");
            enabled = false;
            return;
        }

        if (initializer != null) initializer.Init();

        GameState state = bm.gameState;
        state.stamina.current = state.stamina.max;

        // 스태미나 관리가 제대로 연결된 씬에서만 스태미나를 쓴다.
        // gameState 까지 봐야 한다. Spend 안에서 그걸 건드리기 때문
        bool spend = bm.staminaManager != null && bm.staminaManager.gameState != null;
        if (spend) bm.staminaManager.ResetPenalty();

        bm.StartBattle(enemy, () => Debug.Log("[BattleTest] 승리"), () => Debug.Log("[BattleTest] 패배"), spend);

        Debug.Log("[BattleTest] Space 공격 / 1,2 약 / R 다시하기");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R)) Begin();
        if (Input.GetKeyUp(KeyCode.Space)) Attack();
        if (Input.GetKeyUp(KeyCode.Alpha1)) Drink(drugA);
        if (Input.GetKeyUp(KeyCode.Alpha2)) Drink(drugB);

        Show();
    }

    private void Attack()
    {
        if (!Ready()) return;

        int before = bm.gameState.hp.current;
        int enemyBefore = bm.GetEnemyHp();

        bm.PlayerAttack();

        Debug.Log("공격: 적한테 " + (enemyBefore - bm.GetEnemyHp()) +
                  " / 내가 " + (before - bm.gameState.hp.current) + " 맞음");
    }

    // 인벤토리에 없어도 넣어서 쓴다 (테스트라 편하게)
    private void Drink(ItemSO drug)
    {
        if (drug == null || !Ready()) return;

        GameState state = bm.gameState;
        if (!state.itemInventory.Contains(drug)) state.itemInventory.Add(drug);

        int before = state.hp.current;
        bm.UseDrug(drug);

        Debug.Log("약 " + drug.itemName + " 사용. 그 사이 " + (before - state.hp.current) + " 맞음");
    }

    private bool Ready()
    {
        if (bm != null && bm.gameState != null && bm.InBattle()) return true;

        Debug.Log("[BattleTest] 전투 중이 아니거나 참조가 비어 있음. R 로 다시");
        return false;
    }

    // 현재 상태를 화면과 콘솔용 문자열로
    private void Show()
    {
        if (infoText == null || bm == null || !bm.InBattle()) return;

        EnemySO target = bm.GetEnemy();
        GameState state = bm.gameState;
        if (target == null || state == null) return;

        string buff = bm.GetBuffTurns() > 0
            ? "  버프 공+" + bm.GetBuffAttack() + " 방+" + bm.GetBuffDefense() + " (" + bm.GetBuffTurns() + "턴)"
            : "";

        infoText.text =
            target.enemyName + " (" + Element(target.element) + ")  " + bm.GetEnemyHp() + "/" + target.hp.max + "\n" +
            "다음 행동: " + Action(bm.GetNextEnemyAction()) + "\n" +
            "나 (" + Element(state.battle.element) + ")  체력 " + state.hp.current + "/" + state.hp.max +
            "  스태미나 " + state.stamina.current + buff;
    }

    private string Element(ElementType e)
    {
        switch (e)
        {
            case ElementType.Wood: return "목";
            case ElementType.Fire: return "화";
            case ElementType.Earth: return "토";
            case ElementType.Metal: return "금";
            default: return "수";
        }
    }

    private string Action(EnemyAction act)
    {
        if (act == null) return "공격";

        switch (act.type)
        {
            case EnemyActionType.StrongAttack: return "강공격";
            case EnemyActionType.Defend: return "방어";
            default: return "공격";
        }
    }
}
