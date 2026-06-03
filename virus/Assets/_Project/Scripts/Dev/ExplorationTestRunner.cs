using TMPro;
using UnityEngine;

public class ExplorationTestRunner : MonoBehaviour
{
    public GameInitializer initializer;
    public ExplorationManager explorationManager;
    public BattleManager battleManager;
    public ExplorationSO area;
    public EnemySO enemy;
    public TextMeshProUGUI returnText;

    private void Start()
    {
        initializer.Init();
        explorationManager.gameManager.StartNight();
        explorationManager.StartExploration(area);

        EventSO testEvent = explorationManager.GetRandomEvent();
        if (testEvent != null && testEvent.choices != null && testEvent.choices.Length > 0)
            explorationManager.SelectChoice(testEvent.choices[0]);

        explorationManager.StartBattle(enemy);
        HideReturnText();

        Debug.Log("[ExplorationTest] Started. Press Space to attack, R to return.");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            battleManager.PlayerAttack();

        if (Input.GetKeyUp(KeyCode.R))
        {
            explorationManager.Return();
            ShowReturnText();
        }
    }

    private void HideReturnText()
    {
        if (returnText == null) return;

        returnText.gameObject.SetActive(false);
    }

    private void ShowReturnText()
    {
        if (returnText == null) return;

        returnText.text = "복귀";
        returnText.gameObject.SetActive(true);
    }
}
