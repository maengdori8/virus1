using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// 탐사 진행 UI (Adventure 씬)
public class AdventureUI : MonoBehaviour
{
    [Header("참조")]
    public GameManager gameManager;
    public ExplorationManager explorationManager;
    public BattleManager battleManager;
    public GameState gameState;

    [Header("이벤트 패널")]
    public GameObject eventPanel;

    // 이벤트 설명
    public TextMeshProUGUI descriptionText;

    [Header("선택지 버튼 (최대 3개)")]
    public GameObject[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;

    [Header("전투 이벤트")]
    // 싸운다 버튼
    public GameObject fightButton;

    [Header("깊이")]
    // 현재 깊이 표시 (없으면 비워둠)
    public TextMeshProUGUI depthText;

    [Header("지역 (선택 없이 바로 실행될 때 사용)")]
    public ExplorationSO fallbackArea;

    // 현재 표시 중인 이벤트
    private EventSO currentEvent;

    // 직전 프레임 전투 여부 (전투 종료 감지용)
    private bool wasInBattle;

    // 밤 진입 + 탐사 시작 + 첫 이벤트
    private void Start()
    {
        ExplorationSO area = AreaSelect.selected != null ? AreaSelect.selected : fallbackArea;

        gameManager.StartNight();
        explorationManager.StartExploration(area);
        DrawEvent();
    }

    // 전투 종료 감지. 끝나면 탐사 재개하거나 낮으로 복귀
    private void Update()
    {
        bool inBattle = battleManager.InBattle();

        // 전투 중에는 이벤트 패널 숨김
        if (eventPanel.activeSelf == inBattle)
            eventPanel.SetActive(!inBattle);

        if (wasInBattle && !inBattle)
        {
            if (!explorationManager.IsExploring())
            {
                // 패배 강제복귀
                SceneManager.LoadScene("Ingame");
            }
            else if (gameState.stamina.current <= 0)
            {
                // 스태미나 소진
                explorationManager.Return();
                SceneManager.LoadScene("Ingame");
            }
            else
            {
                DrawEvent();
            }
        }

        wasInBattle = inBattle;
    }

    // 선택지 버튼에 연결
    public void OnClickChoice(int index)
    {
        if (currentEvent == null) return;
        if (currentEvent.choices == null || index >= currentEvent.choices.Length) return;

        explorationManager.SelectChoice(currentEvent.choices[index]);

        // 스태미나 소진으로 자동 복귀됐으면 낮으로
        if (!explorationManager.IsExploring())
        {
            SceneManager.LoadScene("Ingame");
            return;
        }

        DrawEvent();
    }

    // 싸운다 버튼에 연결
    public void OnClickFight()
    {
        if (currentEvent == null || currentEvent.enemy == null) return;

        explorationManager.StartBattle(currentEvent.enemy);
    }

    // 휴식 버튼에 연결. 턴 소모하고 스태미나 회복
    public void OnClickRest()
    {
        explorationManager.Rest();
        DrawEvent();
    }

    // 안쪽으로 더 들어가기 버튼에 연결
    public void OnClickDeeper()
    {
        explorationManager.GoDeeper();

        // 스태미나 소진으로 강제복귀됐으면 낮으로
        if (!explorationManager.IsExploring())
        {
            SceneManager.LoadScene("Ingame");
            return;
        }

        DrawEvent();
    }

    // 복귀 버튼에 연결
    public void OnClickReturn()
    {
        explorationManager.Return();
        SceneManager.LoadScene("Ingame");
    }

    // 새 이벤트를 뽑아 표시
    private void DrawEvent()
    {
        currentEvent = explorationManager.GetRandomEvent();
        descriptionText.text = currentEvent.description;

        if (depthText != null)
            depthText.text = "깊이 " + explorationManager.GetDepth();

        bool isBattle = currentEvent.enemy != null;
        fightButton.SetActive(isBattle);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool show = !isBattle && currentEvent.choices != null && i < currentEvent.choices.Length;
            choiceButtons[i].SetActive(show);

            if (show)
                choiceTexts[i].text = currentEvent.choices[i].text + " (스태미나 " + currentEvent.choices[i].staminaCost + ")";
        }
    }
}
