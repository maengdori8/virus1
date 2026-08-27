using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 전투 하단 필드에 고를 수 있는 행동을 깐다.
// 공격 버튼을 누르면 공격 목록, 방어 버튼을 누르면 회피/방어가 뜬다.
// 목록이 떠 있는 동안은 공격·방어·도주 버튼을 꺼두고, 칸을 하나 고르면 목록이 사라지면서 다시 켠다
public class AttackSelectUI : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;

    [Header("공격")]
    // 고를 수 있는 공격. 넣은 순서대로 칸이 생긴다
    public AttackSO[] attacks;

    // 칸 왼쪽에 붙는 속성 그림. ElementType 순서대로 목 화 토 금 수
    public Sprite[] elementIcons = new Sprite[0];

    [Header("방어")]
    // 방어 버튼을 눌렀을 때 뜨는 두 칸의 이름
    public string dodgeName = "회피";
    public string defendName = "방어";

    [Header("칸")]
    // 한 칸짜리 프리팹 (Selectable). 루트에 Button 이 있어야 눌린다
    public GameObject slotPrefab;

    // 칸이 붙을 곳. Battle 씬 하단의 Battles/Panel
    public Transform slotParent;

    // 공격·방어·도주 버튼 묶음. 칸을 고르는 동안은 꺼둔다
    public GameObject commandButtons;

    // 이번에 깐 칸. 필드 안의 다른 오브젝트까지 지우면 안 되니 따로 들고 있는다
    private readonly List<GameObject> slots = new List<GameObject>();

    // 전투가 끝나면 목록을 치운다
    private void Update()
    {
        if (slots.Count == 0) return;
        if (battleManager != null && battleManager.InBattle()) return;

        Close();
    }

    // 공격 버튼에 연결. 공격 목록을 깐다
    public void ShowAttacks()
    {
        if (!Ready()) return;

        ClearSlots();

        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i] == null) continue;

            // i 를 그대로 넘기면 칸이 눌릴 때는 이미 마지막 값이라 따로 받아둔다
            int index = i;

            AddSlot(attacks[i].attackName, IconOf(attacks[i].element), () => OnClickAttack(index));
        }

        SetCommandsActive(false);
    }

    // 방어 버튼에 연결. 회피와 방어 두 칸을 깐다
    public void ShowGuards()
    {
        if (!Ready()) return;

        ClearSlots();

        // 회피·방어는 속성이 없으니 그림 자리는 비운다
        AddSlot(dodgeName, null, OnClickDodge);
        AddSlot(defendName, null, OnClickDefend);

        SetCommandsActive(false);
    }

    // 목록을 치우고 아래 버튼을 되돌린다
    public void Close()
    {
        ClearSlots();
        SetCommandsActive(true);
    }

    // 고른 공격으로 때린다
    public void OnClickAttack(int index)
    {
        if (attacks == null || index < 0 || index >= attacks.Length) return;

        battleManager.PlayerAttack(attacks[index]);
        Close();
    }

    public void OnClickDodge()
    {
        battleManager.PlayerDodge();
        Close();
    }

    public void OnClickDefend()
    {
        battleManager.PlayerDefend();
        Close();
    }

    private bool Ready()
    {
        return battleManager != null && battleManager.InBattle()
               && attacks != null && slotPrefab != null && slotParent != null;
    }

    private void ClearSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null) Destroy(slots[i]);
        }
        slots.Clear();
    }

    private void SetCommandsActive(bool active)
    {
        if (commandButtons == null) return;
        if (commandButtons.activeSelf == active) return;

        commandButtons.SetActive(active);
    }

    // 속성 그림. 넣어둔 게 없으면 null 이라 그림 자리가 감춰진다
    private Sprite IconOf(ElementType element)
    {
        int index = (int)element;

        if (elementIcons == null || index < 0 || index >= elementIcons.Length) return null;

        return elementIcons[index];
    }

    private void AddSlot(string label, Sprite icon, UnityEngine.Events.UnityAction onClick)
    {
        GameObject slot = Instantiate(slotPrefab, slotParent);
        slots.Add(slot);

        TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = label;

        SetIcon(slot, icon);

        Button button = slot.GetComponentInChildren<Button>();
        if (button != null) button.onClick.AddListener(onClick);
    }

    // 프리팹 안의 그림 자리에 속성 그림을 넣는다
    private void SetIcon(GameObject slot, Sprite icon)
    {
        Transform place = slot.transform.Find("Image");
        if (place == null) return;

        Image image = place.GetComponent<Image>();
        if (image == null) return;

        image.sprite = icon;

        // 그림이 없으면 흰 네모만 남으니 아예 감춘다
        image.enabled = icon != null;
    }
}
