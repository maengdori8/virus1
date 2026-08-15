using UnityEngine;
using TMPro;

// 하루 루프 확인용. 물자 소비, 체력 회복, 의식 감소가 어떻게 물리는지 본다
public class IngameLoopTestRunner : MonoBehaviour
{
    [Header("참조")]
    public GameManager gameManager;
    public GameState gameState;

    // 있으면 T 키로 턴을 하나씩 소모해볼 수 있다
    public TimeManager timeManager;

    [Header("표시 (없어도 됨)")]
    public TextMeshProUGUI infoText;

    // 직전 값 (하루 넘길 때 변화량을 보려고)
    private int lastHp;
    private int lastSupply;
    private int lastConscious;
    private int lastDay;

    private void Start()
    {
        Begin();
    }

    private void Begin()
    {
        gameManager.StartGame();
        gameManager.StartNight();
        Snapshot();

        Debug.Log("[IngameLoopTest] D 하루시작 / N 밤 / E 밤종료 / T 턴소모 / C 백신100 / R 다시하기");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R)) { Begin(); return; }

        if (Input.GetKeyUp(KeyCode.N)) gameManager.StartNight();

        if (Input.GetKeyUp(KeyCode.D)) Step(() => gameManager.StartDay(), "하루 시작");
        if (Input.GetKeyUp(KeyCode.E)) Step(() => gameManager.EndNight(), "밤 종료");

        if (Input.GetKeyUp(KeyCode.T) && timeManager != null)
            Step(() => timeManager.SpendTimeTurn(), "턴 소모");

        if (Input.GetKeyUp(KeyCode.C))
        {
            gameState.vaccineProgress = 100;
            gameManager.CheckClear();
        }

        Show();
    }

    // 실행 전후 값을 비교해서 뭐가 얼마나 변했는지 찍는다
    private void Step(System.Action act, string label)
    {
        Snapshot();
        act();

        string diff = "";
        diff += Delta("체력", lastHp, gameState.hp.current);
        diff += Delta("물자", lastSupply, gameState.supply.current);
        diff += Delta("의식", lastConscious, gameState.consciousness);
        diff += Delta("일수", lastDay, gameState.currentDay);

        Debug.Log("[" + label + "] 남은날 " + gameState.time.dayTurn +
                  " 턴 " + gameState.time.timeTurn + "/" + gameState.time.maxTimeTurn +
                  (Dazed() ? " [혼미]" : "") +
                  (diff == "" ? "  (변화 없음)" : "  " + diff));

        Snapshot();
    }

    private string Delta(string name, int before, int now)
    {
        if (before == now) return "";

        return name + " " + before + "→" + now + " (" + (now - before >= 0 ? "+" : "") + (now - before) + ")  ";
    }

    // 혼미면 자연 회복이 멈추므로 하루 계산이 달라진다. 그게 안 보이면 수치를 못 읽는다
    private bool Dazed()
    {
        return gameManager != null
            && gameManager.consciousnessManager != null
            && gameManager.consciousnessManager.IsLow();
    }

    private void Snapshot()
    {
        lastHp = gameState.hp.current;
        lastSupply = gameState.supply.current;
        lastConscious = gameState.consciousness;
        lastDay = gameState.currentDay;
    }

    private void Show()
    {
        if (infoText == null) return;

        infoText.text =
            "남은날 " + gameState.time.dayTurn + "  턴 " + gameState.time.timeTurn + "/" + gameState.time.maxTimeTurn +
            "  경과 " + gameState.currentDay + "일\n" +
            "체력 " + gameState.hp.current + "/" + gameState.hp.max +
            "  물자 " + gameState.supply.current +
            "  의식 " + gameState.consciousness + (Dazed() ? " (혼미)" : "") + "\n" +
            "백신 " + gameState.vaccineProgress + "%";
    }
}
