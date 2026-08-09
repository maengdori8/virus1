using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 상단 스태미나 표시
public class StaminaDisplay : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("표시")]
    // 스태미나 텍스트 (현재/최대)
    public TextMeshProUGUI staminaText;

    // 줄어드는 스태미나 바 (Image Type을 Filled로 둘 것)
    public Image fillImage;

    // 매 프레임 스태미나를 텍스트와 바에 반영
    private void Update()
    {
        if (gameState == null || gameState.stamina == null) return;

        if (staminaText != null)
            staminaText.text = gameState.stamina.current + "/" + gameState.stamina.max;

        if (fillImage != null)
            fillImage.fillAmount = gameState.stamina.max > 0 ? (float)gameState.stamina.current / gameState.stamina.max : 0f;
    }
}
