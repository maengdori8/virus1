using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [Header("패널")]
    public GameObject rankPanel;

    // 로비로 돌아왔다는 건 판이 끝났다는 뜻.
    // 시작 버튼이 아니라 여기서 초기화해야 어느 버튼으로 들어가든 새 판이 된다
    private void Awake()
    {
        IngameBootstrap.ResetRun();
        StoryUI.ResetIntro();
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Ingame");
    }

    public void OnClickRank()
    {
        rankPanel.SetActive(!rankPanel.activeSelf);
    }
}
