using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // 날짜 턴
    public int currentDay;

    // 체력
    public int hp;
    public int maxHp;

    // 스테미나
    public int stamina;
    public int maxStamina;

    // 물자
    public int supplies;
    public int dailySupplyCost;

    // 샘플 보유량 (Wood/Fire/Earth/Metal/Water 순)
    public int[] sampleInventory = new int[5];

    // 밤 턴 (0이면 낮)
    public int nightTurn;

    // 백신 완성도 (0~100)
    public int vaccineProgress;

    // 보유 아이템
    public List<ItemSO> itemInventory = new List<ItemSO>();
}