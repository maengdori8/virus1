using UnityEngine;

public class ResearchTestRunner : MonoBehaviour
{
    public GameInitializer initializer;
    public ResearchManager researchManager;
    public BattleManager battleManager;
    public ResearchStageSO stage;

    public int startSampleCount = 10;

    private void Start()
    {
        initializer.Init();

        GameState state = researchManager.gameState;
        state.sampleInventory = new[] { startSampleCount, startSampleCount, startSampleCount };
        state.stamina.current = state.stamina.max;

        researchManager.stages = new[] { stage };
        researchManager.StartStage(0);

        Debug.Log("[ResearchTest] Started. Press Space to attack.");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            battleManager.PlayerAttack();
    }
}
