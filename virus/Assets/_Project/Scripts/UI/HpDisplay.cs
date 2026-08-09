using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 상단 체력 표시
public class HpDisplay : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("표시")]
    // 체력 텍스트 (현재/최대)
    public TextMeshProUGUI hpText;

    // 줄어드는 체력 바 (Image Type을 Filled로 둘 것)
    public Image fillImage;

    // 매 프레임 체력을 텍스트와 바에 반영
    private void Update()
    {
        if (gameState == null || gameState.hp == null) return;

        if (hpText != null)
            hpText.text = gameState.hp.current + "/" + gameState.hp.max;

        if (fillImage != null)
            fillImage.fillAmount = gameState.hp.max > 0 ? (float)gameState.hp.current / gameState.hp.max : 0f;
    }
}
