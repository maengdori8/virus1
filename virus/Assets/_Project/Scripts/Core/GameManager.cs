using UnityEngine;
using TMPro;

// 게임 루프 관리
public class GameManager : MonoBehaviour
{
    [Header("참조")]
    public TimeManager timeManager;
    public SupplyManager supplyManager;
    public RewardManager rewardManager;
    public ExplorationManager explorationManager;
    public ResearchManager researchManager;
    public RankManager rankManager;

    [Header("클리어 UI")]
    // 이름 입력 패널
    public GameObject namePanel;

    // 이름 입력창
    public TMP_InputField nameInput;

    [Header("상태")]
    public GameState gameState;

    private bool isNight = false;

    // 물자 소비 → 체력 회복 → 게임오버/클리어 체크
    public void StartDay()
    {
        supplyManager.ConsumeDaily();
        HealDaily();

        if (gameState.hp.current <= 0)
        {
            GameOver();
            return;
        }

        if (gameState.vaccineProgress >= 100)
        {
            GameClear();
            return;
        }
    }

    // 밤 진입, 탐사 가능 상태로 전환
    public void StartNight()
    {
        isNight = true;
    }

    // 탐사 끝나고 낮으로 복귀. 턴 소모 후 다음 날로
    public void EndNight()
    {
        isNight = false;
        timeManager.SpendTimeTurn();

        // 남은 날이 다 되면 게임오버 (보스 못 잡음)
        if (timeManager.timeData.dayTurn <= 0)
        {
            GameOver();
            return;
        }

        StartDay();
    }

    // dailyHeal만큼 회복. max 초과 시 max로 고정
    private void HealDaily()
    {
        gameState.hp.current += gameState.hp.dailyHeal;

        if (gameState.hp.current > gameState.hp.max)
        {
            gameState.hp.current = gameState.hp.max;
        }
    }

    // hp 0 이하 시 호출. 게임오버 처리 (임시)
    private void GameOver()
    {
        Debug.Log("게임오버");
    }

    // 백신 100 이상 시 호출. 이름 입력 패널 띄움
    private void GameClear()
    {
        Debug.Log("백신 완성");
        namePanel.SetActive(true);
    }

    // 확인 버튼에 연결. 입력한 이름으로 랭킹 저장
    public void OnSubmitName()
    {
        rankManager.AddRank(nameInput.text);
        namePanel.SetActive(false);
    }
}
