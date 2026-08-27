using UnityEngine;
using UnityEngine.UI;

// 전투 씬을 다른 씬 위에 얹었을 때 쓰는 연결.
// 이미 돌고 있는 전투가 있으면 이 씬 매니저는 켜지 않고 화면만 그 전투에 물리고,
// 씬을 혼자 열었을 때는 매니저·카메라를 켜서 그대로 굴린다
public class BattleOverlay : MonoBehaviour
{
    [Header("혼자 열었을 때만 켤 것")]
    // 매니저 묶음, 카메라, EventSystem 처럼 얹히면 아래 씬 것과 겹치는 것들.
    // 씬에는 꺼진 채로 저장해 둔다
    public GameObject[] aloneOnly;

    [Header("무대")]
    // 아래 씬 배경을 눌러주는 검은 막. 0 이면 안 깐다
    public float dimAlpha = 0.5f;

    // 몹 뒤에 까는 빛
    public float glowAlpha = 0.34f;

    // 몹 발밑 그림자
    public float shadowAlpha = 0.45f;

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

    // 배경이 그대로 비치면 몹이 묻힌다. 배경을 한 단계 눌러주고
    // 몹 뒤에 빛, 발밑에 그림자를 깔아 실루엣을 띄운다
    private void Start()
    {
        if (battleUI == null || battleUI.enemyPortrait == null) return;

        RectTransform enemy = battleUI.enemyPortrait.rectTransform;
        Transform parent = enemy.parent;
        int index = enemy.GetSiblingIndex();

        if (dimAlpha > 0f)
        {
            Image dim = MakePiece("BattleDim", parent, null, new Color(0f, 0f, 0f, dimAlpha));
            RectTransform rt = dim.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            dim.transform.SetSiblingIndex(0);
            index++;
        }

        Sprite circle = MakeCircle();

        Image glow = MakePiece("BattleGlow", parent, circle, new Color(1f, 0.96f, 0.85f, glowAlpha));
        Place(glow.rectTransform, enemy.anchoredPosition, enemy.sizeDelta * 1.9f);
        glow.transform.SetSiblingIndex(index);

        Image shadow = MakePiece("BattleShadow", parent, circle, new Color(0f, 0f, 0f, shadowAlpha));
        Place(shadow.rectTransform,
              enemy.anchoredPosition - new Vector2(0f, enemy.sizeDelta.y * 0.46f),
              new Vector2(enemy.sizeDelta.x * 0.8f, enemy.sizeDelta.y * 0.16f));
        shadow.transform.SetSiblingIndex(index + 1);
    }

    // 가장자리가 부드러운 원. 빛과 그림자 양쪽에 쓴다
    private Sprite MakeCircle()
    {
        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        float half = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - half) / half;
                float dy = (y + 0.5f - half) / half;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                // 가운데는 꽉 차고 바깥으로 갈수록 옅어진다
                float alpha = Mathf.Clamp01(1f - distance);
                alpha = alpha * alpha;

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();

        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
    }

    private Image MakePiece(string name, Transform parent, Sprite sprite, Color color)
    {
        GameObject go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;

        return image;
    }

    private void Place(RectTransform rt, Vector2 position, Vector2 size)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
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
