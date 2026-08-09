using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 상단 시간 표시 (남은 날짜 / 하루 턴)
public class TimeDisplay : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("표시")]
    // 남은 날짜
    public TextMeshProUGUI dayText;

    // 하루 안에서 남은 턴
    public TextMeshProUGUI turnText;

    // 하루 턴을 시계처럼 채우는 이미지 (Image Type을 Filled로 둘 것)
    public Image turnFill;

    // 매 프레임 남은 날짜와 턴을 반영
    private void Update()
    {
        if (gameState == null || gameState.time == null) return;

        if (dayText != null)
            dayText.text = "D-" + gameState.time.dayTurn;

        if (turnText != null)
            turnText.text = gameState.time.timeTurn + "/" + gameState.time.maxTimeTurn;

        if (turnFill != null)
            turnFill.fillAmount = gameState.time.maxTimeTurn > 0 ? (float)gameState.time.timeTurn / gameState.time.maxTimeTurn : 0f;
    }
}
