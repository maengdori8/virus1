using UnityEngine;
using TMPro;

// 아이템 인벤토리 표시/사용 (Ingame 씬)
public class ItemUI : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public ItemManager itemManager;

    [Header("패널")]
    public GameObject panel;

    [Header("슬롯 (버튼 + 이름 텍스트)")]
    public GameObject[] slots;
    public TextMeshProUGUI[] slotTexts;

    // 열기/닫기 버튼에 연결
    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }

    // 매 프레임 인벤토리 내용을 슬롯에 반영
    private void Update()
    {
        if (!panel.activeSelf) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            bool has = i < gameState.itemInventory.Count;
            slots[i].SetActive(has);

            if (has && slotTexts != null && i < slotTexts.Length && slotTexts[i] != null)
                slotTexts[i].text = gameState.itemInventory[i].itemName;
        }
    }

    // 슬롯 버튼에 연결. 해당 아이템 사용
    public void OnClickSlot(int index)
    {
        if (index >= gameState.itemInventory.Count) return;

        itemManager.UseItem(gameState.itemInventory[index]);
    }
}
