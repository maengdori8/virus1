using UnityEngine;

public class UpperUITestRunner : MonoBehaviour
{
    public GameInitializer initializer;
    public GameState gameState;

    private void Start()
    {
        initializer.Init();
        gameState.supply.current = 77;
        gameState.sampleInventory = new[] { 1, 2, 3 };

        Debug.Log("[UpperUITest] Started. Press S to add supply, 1 to 3 to add samples.");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
            gameState.supply.current++;

        if (Input.GetKeyUp(KeyCode.Alpha1))
            gameState.sampleInventory[0]++;

        if (Input.GetKeyUp(KeyCode.Alpha2))
            gameState.sampleInventory[1]++;

        if (Input.GetKeyUp(KeyCode.Alpha3))
            gameState.sampleInventory[2]++;
    }
}
