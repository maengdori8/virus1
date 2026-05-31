using UnityEngine;
using TMPro;

// 상단 물자 표시
public class SupplyDisplay : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("물자 텍스트")]
    public TextMeshProUGUI supplyText;

    // 매 프레임 물자 개수를 텍스트에 반영
    private void Update()
    {
        if (gameState == null || gameState.supply == null) return;

        supplyText.text = "X" + gameState.supply.current;
    }
}
