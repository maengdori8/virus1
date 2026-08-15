using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameInitializer gameInitializer;
    public ConsciousnessManager consciousnessManager;

    [Header("클리어 UI")]
    // 이름 입력 패널
    public GameObject namePanel;

    // 이름 입력창
    public TMP_InputField nameInput;

    [Header("상태")]
    public GameState gameState;

    [Header("결과")]
    // 마지막 게임 결과 (결과 패널이 읽음)
    public bool isCleared;
    public string resultReason;

    private bool isNight = false;

    private const string SaveKey = "GameSave";

    // 게임 시작 버튼에 연결. 초기화 → 연구진행 리셋 → 랭킹 불러오기 → 첫 하루
    public void StartGame()
    {
        gameInitializer.Init();
        researchManager.ResetProgress();
        rankManager.Load();
        StartDay();
    }

    // 현재 진행 상태를 JSON으로 저장
    public void SaveGame()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(gameState));
        PlayerPrefs.Save();
    }

    // 저장된 진행 상태를 GameState 에셋에 덮어쓰기
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(SaveKey), gameState);
    }

    // 물자 소비 → 체력 회복 → 의식 감소 → 게임오버/클리어 체크
    public void StartDay()
    {
        supplyManager.ConsumeDaily();
        
        if(gameState.hp.current <= 0)
        {
            GameOver("체력 소진");
            return;
        }

        // 혼미하면 몸이 스스로 회복하지 못한다.
        // 회복량과 혼미 페널티가 맞물려 상쇄되면 혼미에 빠져도 아무 일이 없다
        bool dazed = consciousnessManager != null && consciousnessManager.IsLow();
        if (!dazed) HealDaily();

        if (consciousnessManager != null) consciousnessManager.Tick();

        if(gameState.hp.current <= 0)
        {
            GameOver("체력 소진");
            return;
        }
        if(gameState.vaccineProgress >= 100)
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

    // 탐사 끝나고 낮으로 복귀. 다음 날로 (턴 소모는 ExplorationManager.Return이 담당)
    public void EndNight()
    {
        isNight = false;

        // 남은 날이 다 되면 게임오버 (보스 못 잡음)
        if (gameState.time.dayTurn <= 0)
        {
            GameOver("기간 초과");
            return;
        }

        StartDay();
    }

    // dailyHeal만큼 회복 후 0~max 범위로 고정
    private void HealDaily()
    {
        gameState.hp.current += gameState.hp.dailyHeal;
        gameState.hp.Clamp();
    }

    // 게임오버 처리. 사유(체력 소진/기간 초과)를 결과에 기록
    private void GameOver(string reason)
    {
        isCleared = false;
        resultReason = reason;
        Debug.Log($"게임오버 - {reason}");
    }

    // 백신 100 도달 시 즉시 클리어 (연구 승리 직후 호출)
    public void CheckClear()
    {
        if (gameState.vaccineProgress >= 100)
            GameClear();
    }

    // 백신 100 이상 시 호출. 결과 기록 후 이름 입력 패널 띄움
    private void GameClear()
    {
        isCleared = true;
        resultReason = "백신 완성";
        Debug.Log("백신 완성");

        if (namePanel != null) namePanel.SetActive(true);
    }

    // 확인 버튼에 연결. 입력한 이름으로 랭킹 저장
    public void OnSubmitName()
    {
        rankManager.AddRank(nameInput != null ? nameInput.text : "");

        if (namePanel != null) namePanel.SetActive(false);

        // 판이 끝났으니 로비로. 다음 입장 때 새 게임으로 시작된다
        IngameBootstrap.ResetRun();
        SceneManager.LoadScene("Lobby");
    }
}
