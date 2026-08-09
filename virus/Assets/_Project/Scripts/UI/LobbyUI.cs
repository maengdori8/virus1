using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [Header("�г�")]
    public GameObject rankPanel;

    public void OnClickStart()
    {
        IngameBootstrap.ResetRun();
        StoryUI.ResetIntro();
        SceneManager.LoadScene("Ingame");
    }

    public void OnClickRank()
    {
        rankPanel.SetActive(!rankPanel.activeSelf);
    }
}
