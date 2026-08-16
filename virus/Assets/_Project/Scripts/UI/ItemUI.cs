using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 아이템 인벤토리 표시/사용 (Ingame 씬)
public class ItemUI : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public ItemManager itemManager;

    [Header("패널")]
    public GameObject panel;

    [Header("칸")]
    // 한 칸짜리 프리팹 (Item). 루트에 Button 이 있어야 클릭이 먹는다
    public GameObject slotPrefab;

    // 칸이 붙을 곳. Grid Layout Group 을 붙인 오브젝트
    public Transform slotParent;

    // 이번에 붙인 칸. 패널 안의 다른 오브젝트까지 지우면 안 되니 따로 들고 있는다
    private readonly List<GameObject> slots = new List<GameObject>();

    // 마지막으로 그린 개수. 탐사 패널티로 아이템이 사라져도 여기서 걸린다
    private int shownCount = -1;

    // 열기/닫기 버튼에 연결
    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf) Rebuild();
    }

    // 인벤토리가 바뀐 프레임에만 다시 그린다
    private void Update()
    {
        if (!panel.activeSelf) return;
        if (gameState.itemInventory.Count == shownCount) return;

        Rebuild();
    }

    // 칸 버튼에 연결됨. 해당 아이템 사용
    public void OnClickSlot(int index)
    {
        if (index >= gameState.itemInventory.Count) return;

        itemManager.UseItem(gameState.itemInventory[index]);
    }

    // 인벤토리 개수만큼 프리팹을 새로 깐다
    private void Rebuild()
    {
        ClearSlots();

        if (slotPrefab == null || slotParent == null) return;

        List<ItemSO> inventory = gameState.itemInventory;

        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slots.Add(slot);

            SetIcon(slot, inventory[i].icon);
            SetName(slot, inventory[i].itemName);

            // i 를 그대로 넘기면 버튼이 눌릴 때는 이미 마지막 값이라 따로 받아둔다
            int index = i;

            Button button = slot.GetComponentInChildren<Button>();
            if (button != null) button.onClick.AddListener(() => OnClickSlot(index));
        }

        shownCount = inventory.Count;
    }

    // 프리팹 안의 Frame/Img 에 아이템 그림을 넣는다
    private void SetIcon(GameObject slot, Sprite icon)
    {
        Transform img = slot.transform.Find("Frame/Img");
        if (img == null) return;

        Image image = img.GetComponent<Image>();
        if (image == null) return;

        image.sprite = icon;

        // 그림이 없는 아이템은 흰 네모가 되니 아예 감춘다
        image.enabled = icon != null;
    }

    // 이름 칸은 프리팹에 TMP 를 넣었을 때만 채운다
    private void SetName(GameObject slot, string itemName)
    {
        TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = itemName;
    }

    private void ClearSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null) Destroy(slots[i]);
        }
        slots.Clear();

        shownCount = -1;
    }
}
