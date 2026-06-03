using UnityEngine;

public class IngameLoopTestRunner : MonoBehaviour
{
    public GameManager gameManager;
    public GameState gameState;

    private void Start()
    {
        gameManager.StartGame();
        gameManager.StartNight();
        Debug.Log("[IngameLoopTest] Started. Press N night, D day, E end night, C clear.");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
            gameManager.StartNight();

        if (Input.GetKeyUp(KeyCode.D))
            gameManager.StartDay();

        if (Input.GetKeyUp(KeyCode.E))
            gameManager.EndNight();

        if (Input.GetKeyUp(KeyCode.C))
        {
            gameState.vaccineProgress = 100;
            gameManager.CheckClear();
        }
    }
}
