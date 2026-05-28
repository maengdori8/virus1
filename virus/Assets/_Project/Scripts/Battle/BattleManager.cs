using System;
using UnityEngine;

// 전투 관리
public class BattleManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;
    public RewardManager rewardManager;

    [Header("설정")]
    // 상성 배수 (기본 1.2, 버프로 추가 가능)
    public float elementBonus = 1.2f;

    // 현재 상대 적
    private EnemySO currentEnemy;
    private int enemyHp;

    // 승리/패배 시 호출될 콜백
    private Action onWin;
    private Action onLose;

    // 적 SO + 콜백을 받아 전투 시작. enemyHp에 복사
    public void StartBattle(EnemySO enemy, Action winCallback, Action loseCallback)
    {
        currentEnemy = enemy;
        enemyHp = enemy.hp.max;
        onWin = winCallback;
        onLose = loseCallback;
    }

    // 공격력-방어력 계산 후 상성 배수 적용. 스태미나 1 소모
    public void PlayerAttack()
    {
        int damage = gameState.battle.attack - currentEnemy.defense;
        if (damage < 1) damage = 1;

        // 오행 상성 추가 댐지
        if (IsStrong(gameState.battle.element, currentEnemy.element))
        {
            damage = (int)(damage * elementBonus);
        }

        enemyHp -= damage;
        gameState.stamina.current--;

        if (enemyHp <= 0)
        {
            Win();
            return;
        }

        EnemyAttack();
    }

    // PlayerAttack과 동일 구조. 적→플레이어 방향으로 데미지
    private void EnemyAttack()
    {
        int damage = currentEnemy.attack - gameState.battle.defense;
        if (damage < 1) damage = 1;

        if (IsStrong(currentEnemy.element, gameState.battle.element))
        {
            damage = (int)(damage * elementBonus);
        }

        gameState.hp.current -= damage;

        if (gameState.hp.current <= 0)
        {
            Lose();
        }
    }

    // 오행 상극 판별. 목→토→수→화→금→목 순환
    private bool IsStrong(ElementType a, ElementType b)
    {
        if (a == ElementType.Wood && b == ElementType.Earth) return true;
        if (a == ElementType.Earth && b == ElementType.Water) return true;
        if (a == ElementType.Water && b == ElementType.Fire) return true;
        if (a == ElementType.Fire && b == ElementType.Metal) return true;
        if (a == ElementType.Metal && b == ElementType.Wood) return true;
        return false;
    }

    // 콜백 실행 후 초기화
    private void Win()
    {
        onWin?.Invoke();
        onWin = null;
        onLose = null;
    }

    // 콜백 실행 후 초기화
    private void Lose()
    {
        onLose?.Invoke();
        onWin = null;
        onLose = null;
    }
}
