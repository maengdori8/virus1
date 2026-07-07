using UnityEngine;

// Ingame 씬 진입 시 게임 시작
public class IngameBootstrap : MonoBehaviour
{
    [Header("참조")]
    public GameManager gameManager;

    private void Start()
    {
        gameManager.StartGame();
    }
}
