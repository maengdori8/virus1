using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    [Header("시간")]
    // 날짜 턴
    public int currentDay;

    // 밤 턴 (0이면 낮)
    public int nightTurn;

    [Header("자원")]
    // 체력
    public HpData hp;

    // 스태미나
    public StaminaData stamina;

    // 물자
    public SupplyData supply;

    // 샘플 보유량 (Wood/Fire/Earth/Metal/Water 순)
    public int[] sampleInventory = new int[5];

    [Header("전투")]
    // 전투 스탯
    public PlayerBattleData battle;

    [Header("진행도")]
    // 백신 완성도 (0~100)
    public int vaccineProgress;

    // 보유 아이템
    public List<ItemSO> itemInventory = new List<ItemSO>();
}
