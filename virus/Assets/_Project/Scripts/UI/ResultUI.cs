using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// 게임오버 결과 표시
public class ResultUI : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("패널")]
    public GameObject gameOverPanel;

    // 게임오버 사유
    public TextMeshProUGUI reasonText;

    // 중복 표시 방지
    private bool shown;

    // 매 프레임 게임오버 조건 확인
    private void Update()
    {
        if (shown) return;

        if (gameState.hp.current <= 0)
            Show("체력 소진");
        else if (gameState.time.dayTurn <= 0 && gameState.vaccineProgress < 100)
            Show("기간 초과");
    }

    // 게임오버 패널 표시
    private void Show(string reason)
    {
        shown = true;
        reasonText.text = reason;
        gameOverPanel.SetActive(true);
    }

    // 로비 버튼에 연결
    public void OnClickLobby()
    {
        IngameBootstrap.ResetRun();
        SceneManager.LoadScene("Lobby");
    }
}
