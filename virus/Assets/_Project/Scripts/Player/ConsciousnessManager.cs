using UnityEngine;

// 주인공 의식 관리 (오락가락)
public class ConsciousnessManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("설정")]
    // 하루 지날 때 의식 감소량
    public int dailyDrift = 5;

    // 이 값 이하면 의식 혼미 (페널티)
    public int lowThreshold = 30;

    // 혼미 상태에서 하루 추가 체력 손실
    public int lowHpPenalty = 3;

    // 하루마다 호출. 의식 감소 후 혼미면 체력 깎기
    public void Tick()
    {
        gameState.consciousness -= dailyDrift;
        Clamp();

        if (IsLow())
        {
            gameState.hp.current -= lowHpPenalty;
            gameState.hp.Clamp();
        }
    }

    // 의식 회복 (연구 보상 / 아이템)
    public void Recover(int amount)
    {
        gameState.consciousness += amount;
        Clamp();
    }

    // 0~100 범위 고정
    public void Clamp()
    {
        gameState.consciousness = Mathf.Clamp(gameState.consciousness, 0, 100);
    }

    // 의식 혼미 여부
    public bool IsLow()
    {
        return gameState.consciousness <= lowThreshold;
    }

    // 얼굴 표시용 단계 (0 명료 ~ 2 혼미)
    public int FaceLevel()
    {
        return FaceLevelOf(gameState.consciousness);
    }

    // 상단 UI는 씬이 따로라 매니저를 붙들 수 없어서 숫자만 넘겨받는다
    public static int FaceLevelOf(int consciousness)
    {
        if (consciousness > 66) return 0;
        if (consciousness > 33) return 1;
        return 2;
    }
}
