using UnityEngine;
using TMPro;

// 상단 샘플 표시
public class SampleDisplay : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("샘플 텍스트 (바다/산/도시 순)")]
    public TextMeshProUGUI[] sampleTexts;

    // 매 프레임 sampleInventory 값을 TMP 텍스트에 반영
    private void Update()
    {
        if (gameState == null || sampleTexts == null) return;

        for (int i = 0; i < sampleTexts.Length && i < gameState.sampleInventory.Length; i++)
        {
            if (sampleTexts[i] == null) continue;

            sampleTexts[i].text = "X" + gameState.sampleInventory[i];
        }
    }
}
