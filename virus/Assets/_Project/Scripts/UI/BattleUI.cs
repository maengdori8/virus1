using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 전투 패널 표시/조작
public class BattleUI : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;
    public GameState gameState;

    [Header("패널")]
    public GameObject panel;

    // 전투 중에만 같이 켤 것. 연구 씬의 전투 배경처럼 패널 밖에 있는 것들
    public GameObject[] battleOnly = new GameObject[0];

    [Header("표시")]
    // 적 이름
    public TextMeshProUGUI enemyNameText;

    // 적 체력
    public TextMeshProUGUI enemyHpText;

    // 적 체력 막대. 남은 비율만큼 채운다
    public Image enemyHpFill;

    // 적 그림. 그림이 없는 적이면 감춘다
    public Image enemyPortrait;

    // 적 다음 행동 예고
    public TextMeshProUGUI nextActionText;

    // 매 프레임 전투 상태를 패널에 반영
    private void Update()
    {
        bool active = battleManager.InBattle();
        if (panel.activeSelf != active)
            panel.SetActive(active);

        for (int i = 0; i < battleOnly.Length; i++)
        {
            if (battleOnly[i] == null) continue;
            if (battleOnly[i].activeSelf != active) battleOnly[i].SetActive(active);
        }

        if (!active) return;

        EnemySO enemy = battleManager.GetEnemy();

        // 속성을 안 보여주면 상성이 그냥 랜덤으로 느껴진다
        enemyNameText.text = enemy.enemyName + " (" + ElementName.Of(enemy.element) + ")";

        if (enemyPortrait != null)
        {
            enemyPortrait.sprite = enemy.portrait;
            enemyPortrait.enabled = enemy.portrait != null;
        }

        int hp = battleManager.GetEnemyHp();
        if (hp < 0) hp = 0;

        enemyHpText.text = hp + " / " + enemy.hp.max;

        if (enemyHpFill != null && enemy.hp.max > 0)
            enemyHpFill.fillAmount = (float)hp / enemy.hp.max;

        nextActionText.text = "다음 행동: " + ActionLabel(battleManager.GetNextEnemyAction())
                              + "    내 속성 " + ElementName.Of(gameState.battle.element);
    }

    // 공격 버튼에 연결
    public void OnClickAttack()
    {
        battleManager.PlayerAttack();
    }

    // 방어 버튼에 연결. 이번 적 공격 피해를 깎는다
    public void OnClickDefend()
    {
        battleManager.PlayerDefend();
    }

    // 회피 버튼에 연결. 확률로 이번 적 공격을 통째로 흘린다
    public void OnClickDodge()
    {
        battleManager.PlayerDodge();
    }

    // 도주 버튼에 연결. 그 자리에서 전투를 끝낸다
    public void OnClickFlee()
    {
        battleManager.Flee();
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
