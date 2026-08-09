using UnityEngine;

// 아이템 사용
public class ItemManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public RewardManager rewardManager;

    // 아이템 사용. 효과 적용 후 인벤토리에서 제거
    public void UseItem(ItemSO item)
    {
        if (item == null) return;
        if (!gameState.itemInventory.Contains(item)) return;

        // 전투용 약은 전투 화면에서만 사용
        if (item.itemType == ItemType.Drug) return;

        rewardManager.Apply(item.effect);
        gameState.itemInventory.Remove(item);
    }
}
