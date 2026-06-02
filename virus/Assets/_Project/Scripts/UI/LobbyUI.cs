using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [Header("ÆÐ³Î")]
    public GameObject rankPanel;

    public void OnClickStart()
    {
        SceneManager.LoadScene("Ingame");
    }

    public void OnClickRank()
    {
        rankPanel.SetActive(!rankPanel.activeSelf);
    }
}
