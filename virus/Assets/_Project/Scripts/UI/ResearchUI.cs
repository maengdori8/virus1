using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 연구 화면 UI (Research 씬)
public class ResearchUI : MonoBehaviour
{
    [Header("참조")]
    public ResearchManager researchManager;
    public BattleManager battleManager;
    public GameState gameState;

    [Header("연구 카드 (1→3 순)")]
    // 카드 뿌리만 넣으면 안의 버튼과 필요 샘플 칸은 알아서 찾는다.
    // 카드 모양을 바꿔도 이름만 그대로면 다시 물릴 게 없다
    public Transform[] cards;

    [Header("진행도")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    // 카드 안에서 찾아둔 것들
    private class Card
    {
        public Button button;
        public TextMeshProUGUI label;      // 버튼 안 문구
        public TextMeshProUGUI[] need;     // 필요 샘플 바다 / 산 / 도시
    }

    private Card[] found;

    private void Start()
    {
        Build();
    }

    // 카드 구성을 한 번만 훑어두고, 버튼 클릭도 여기서 걸어준다
    private void Build()
    {
        if (cards == null)
        {
            found = new Card[0];
            return;
        }

        found = new Card[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null) continue;

            Card card = new Card();
            card.button = cards[i].GetComponentInChildren<Button>(true);

            if (card.button != null)
                card.label = card.button.GetComponentInChildren<TextMeshProUGUI>(true);

            card.need = new TextMeshProUGUI[3];
            card.need[0] = FindText(cards[i], "NeedSea");
            card.need[1] = FindText(cards[i], "NeedMont");
            card.need[2] = FindText(cards[i], "NeedCity");

            found[i] = card;

            // 인스펙터에서 또 걸면 한 번 눌러도 두 번 들어간다. 여기서만 건다
            if (card.button == null) continue;

            int index = i;
            card.button.onClick.RemoveAllListeners();
            card.button.onClick.AddListener(() => OnClickStage(index));
        }
    }

    // 이름이 같은 자식을 찾아 그 밑의 글자를 집는다
    private TextMeshProUGUI FindText(Transform card, string childName)
    {
        Transform[] all = card.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name == childName) return all[i].GetComponentInChildren<TextMeshProUGUI>(true);
        }

        return null;
    }

    // 매 프레임 잠금/비용/진행도 갱신
    private void Update()
    {
        UpdateCards();

        if (progressBar != null)
            progressBar.value = gameState.vaccineProgress / 100f;

        if (progressText != null)
            progressText.text = gameState.vaccineProgress + "%";
    }

    private void UpdateCards()
    {
        if (found == null) return;

        for (int i = 0; i < found.Length; i++)
        {
            Card card = found[i];
            if (card == null) continue;

            ResearchStageSO stage = researchManager.stages != null && i < researchManager.stages.Length
                ? researchManager.stages[i]
                : null;

            bool cleared = i < gameState.reasearchCleared;
            bool unlocked = stage != null && researchManager.IsStageUnlocked(i);
            bool enough = stage != null && researchManager.CanStartStage(stage);

            if (card.button != null)
                card.button.interactable = unlocked && enough && !battleManager.InBattle();

            // 왜 못 누르는지가 버튼에 그대로 적히게
            if (card.label != null)
                card.label.text = Label(cleared, unlocked, enough);

            if (stage == null) continue;

            for (int s = 0; s < card.need.Length && s < stage.sampleCost.Length; s++)
                SetNeed(card.need[s], stage.sampleCost[s], s);
        }
    }

    private string Label(bool cleared, bool unlocked, bool enough)
    {
        if (cleared) return "완료";
        if (!unlocked) return "잠김";
        if (!enough) return "샘플 부족";

        return "연구시작";
    }

    // 모자라면 보유량을 같이 적는다
    private void SetNeed(TextMeshProUGUI text, int need, int sampleIndex)
    {
        if (text == null) return;

        int have = gameState.sampleInventory[sampleIndex];

        text.text = have >= need ? "X" + need : have + "/" + need;
    }

    // 카드 버튼에 연결 (Build 에서 코드로 걸어둠)
    public void OnClickStage(int index)
    {
        researchManager.StartStage(index);
    }
}
