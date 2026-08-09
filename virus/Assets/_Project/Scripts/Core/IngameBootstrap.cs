using UnityEngine;

// Ingame 씬 진입 시 게임 시작
public class IngameBootstrap : MonoBehaviour
{
    [Header("참조")]
    public GameManager gameManager;

    private static bool started;

    private void Start()
    {
        if (started) return;
        started = true;

        gameManager.StartGame();
    }

    public static void ResetRun()
    {
        started = false;
    }
}
