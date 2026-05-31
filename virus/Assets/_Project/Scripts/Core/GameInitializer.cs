using UnityEngine;

// 게임 초기값 세팅
public class GameInitializer : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public TimeData timeData;

    [Header("체력 초기값")]
    public int startHp = 100;
    public int startMaxHp = 100;
    public int startDailyHeal = 5;

    [Header("스태미나 초기값")]
    public int startMaxStamina = 10;

    [Header("물자 초기값")]
    public int startSupply = 20;
    public int startDailyCost = 3;

    [Header("전투 초기값")]
    public int startAttack = 10;
    public int startDefense = 5;
    public ElementType startElement = ElementType.Wood;

    [Header("시간 초기값")]
    public int startDayTurn = 30;
    public int startMaxTimeTurn = 5;

    // Inspector의 초기값으로 GameState와 TimeData를 전부 세팅
    public void Init()
    {
        gameState.hp.current = startHp;
        gameState.hp.max = startMaxHp;
        gameState.hp.dailyHeal = startDailyHeal;

        gameState.stamina.current = 0;
        gameState.stamina.max = startMaxStamina;

        gameState.supply.current = startSupply;
        gameState.supply.dailyCost = startDailyCost;

        gameState.battle.attack = startAttack;
        gameState.battle.defense = startDefense;
        gameState.battle.element = startElement;

        gameState.vaccineProgress = 0;
        gameState.sampleInventory = new int[3];

        timeData.dayTurn = startDayTurn;
        timeData.timeTurn = startMaxTimeTurn;
        timeData.maxTimeTurn = startMaxTimeTurn;
    }
}
