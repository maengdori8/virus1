using System;
using UnityEngine;

// 게임 루프 관리
public class GameManager : MonoBehaviour
{
    [Header("참조")]
    public TimeManager timeManager;
    public SupplyManager supplyManager;
    public RewardManager rewardManager;
    public ExplorationManager explorationManager;
    public ResearchManager researchManager;

    [Header("상태")]
    public GameState gameState;

    private bool isNight = false;
    // 물자 소비 → 체력 회복 → 게임오버/클리어 체크 순서로 실행
    public void StartDay()
    {
        supplyManager.ConsumeDaily();
        HealDaily();

        if (gameState.hp.current <= 0)
        {
            GameOver();
            return;
        }

        if (gameState.vaccineProgress >= 100)
        {
            GameClear();
            return;
        }
    }
    

    // 밤 진입, 탐가 가능 상태로 전환
    public void StartNight()
    {
        isNight = true;
    }


    // 탐사 끝나고 낮으로 복귀, 연구 가능 상태
    public void EndNight()
    {
        isNight = false;
        timeManager.SpendTimeTurn();
    }

    // dailyHeal만큼 회복. max 초과 시 max로 고정
    private void HealDaily()
    {
        gameState.hp.current += gameState.hp.dailyHeal;

        if (gameState.hp.current > gameState.hp.max)
        {
            gameState.hp.current = gameState.hp.max;
        }
    }

    // hp 0 이하 시 호출. 게임오버 처리 (임시)
    private void GameOver()
    {
        Debug.Log("게임오버");
        // 임시로 출력
    }

    // 백신 100 이상 시 호출. 엔딩 처리 (임시)
    private void GameClear()
    {
        Debug.Log("백신 완성");
        // 임시로 출력
    }
}
