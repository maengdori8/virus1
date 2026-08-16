using UnityEngine;

// 전투 씬을 다른 씬 위에 얹었을 때 쓰는 연결.
// 이미 돌고 있는 전투가 있으면 이 씬 매니저는 켜지 않고 화면만 그 전투에 물리고,
// 씬을 혼자 열었을 때는 매니저·카메라를 켜서 그대로 굴린다
public class BattleOverlay : MonoBehaviour
{
    [Header("혼자 열었을 때만 켤 것")]
    // 매니저 묶음, 카메라, EventSystem 처럼 얹히면 아래 씬 것과 겹치는 것들.
    // 씬에는 꺼진 채로 저장해 둔다
    public GameObject[] aloneOnly;

    [Header("화면")]
    public BattleUI battleUI;
    public BuffDisplay buffDisplay;
    public AttackSelectUI attackSelect;

    private void Awake()
    {
        BattleManager live = FindLiveBattleManager();

        // 얹힌 게 아니면 이 씬 것으로 혼자 돈다
        if (live == null)
        {
            for (int i = 0; i < aloneOnly.Length; i++)
            {
                if (aloneOnly[i] != null) aloneOnly[i].SetActive(true);
            }
            return;
        }

        Bind(live);
    }

    // 이 씬 밖에서 이미 돌고 있는 BattleManager.
    // 이 씬 매니저는 꺼진 채라 여기 걸리지 않는다
    private BattleManager FindLiveBattleManager()
    {
        BattleManager[] all = FindObjectsOfType<BattleManager>();

        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].gameObject.scene != gameObject.scene) return all[i];
        }

        return null;
    }

    // 화면이 보던 대상을 바깥 전투로 갈아끼운다
    private void Bind(BattleManager live)
    {
        if (battleUI != null)
        {
            battleUI.battleManager = live;
            battleUI.gameState = live.gameState;
        }

        if (buffDisplay != null) buffDisplay.battleManager = live;
        if (attackSelect != null) attackSelect.battleManager = live;
    }
}
