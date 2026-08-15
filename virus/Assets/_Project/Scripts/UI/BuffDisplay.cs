using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 약 버프 상태 표시 (전투 화면)
public class BuffDisplay : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;

    [Header("표시")]
    // 버프 종류와 남은 턴
    public TextMeshProUGUI buffText;

    // 버프에 따라 색이 변할 이미지
    public Image tintTarget;

    [Header("색")]
    public Color normalColor = Color.white;
    public Color attackColor = new Color(1f, 0.6f, 0.6f);
    public Color defenseColor = new Color(0.6f, 0.8f, 1f);

    // 매 프레임 버프 상태 반영
    private void Update()
    {
        if (battleManager == null) return;

        int turns = battleManager.GetBuffTurns();

        // 약효 없음
        if (turns <= 0)
        {
            if (buffText != null) buffText.text = "";
            if (tintTarget != null) tintTarget.color = normalColor;
            return;
        }

        int attack = battleManager.GetBuffAttack();
        int defense = battleManager.GetBuffDefense();

        if (buffText != null)
            buffText.text = Label(attack, defense) + "  " + turns + "턴";

        if (tintTarget != null)
            tintTarget.color = attack >= defense ? attackColor : defenseColor;
    }

    // 버프 종류 문구. 두 약을 같이 마셨으면 둘 다 보여준다
    private string Label(int attack, int defense)
    {
        if (attack > 0 && defense > 0) return "공격 +" + attack + " 방어 +" + defense;
        if (attack > 0) return "공격 +" + attack;
        return "방어 +" + defense;
    }
}
