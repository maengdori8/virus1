using UnityEngine;
using TMPro;

// 전투 패널 표시/조작
public class BattleUI : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;
    public GameState gameState;

    [Header("패널")]
    public GameObject panel;

    [Header("표시")]
    // 적 이름
    public TextMeshProUGUI enemyNameText;

    // 적 체력
    public TextMeshProUGUI enemyHpText;

    // 적 다음 행동 예고
    public TextMeshProUGUI nextActionText;

    // 매 프레임 전투 상태를 패널에 반영
    private void Update()
    {
        bool active = battleManager.InBattle();
        if (panel.activeSelf != active)
            panel.SetActive(active);

        if (!active) return;

        EnemySO enemy = battleManager.GetEnemy();

        // 속성을 안 보여주면 상성이 그냥 랜덤으로 느껴진다
        enemyNameText.text = enemy.enemyName + " (" + ElementName.Of(enemy.element) + ")";
        enemyHpText.text = battleManager.GetEnemyHp() + " / " + enemy.hp.max;
        nextActionText.text = "다음 행동: " + ActionLabel(battleManager.GetNextEnemyAction())
                              + "    내 속성 " + ElementName.Of(gameState.battle.element);
    }

    // 공격 버튼에 연결
    public void OnClickAttack()
    {
        battleManager.PlayerAttack();
    }

    // 약 사용 버튼에 연결. 인벤토리의 첫 번째 약 사용
    public void OnClickDrug()
    {
        for (int i = 0; i < gameState.itemInventory.Count; i++)
        {
            ItemSO item = gameState.itemInventory[i];
            if (item.itemType != ItemType.Drug) continue;

            battleManager.UseDrug(item);
            return;
        }
    }

    // 행동 예고 문구
    private string ActionLabel(EnemyAction act)
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
