using UnityEngine;

// 전투 관리
public class BattleManager : MonoBehaviour
{
    public GameState gameState;
    public RewardManager rewardManager;

    // 상성 배수 (기본 1.2, 버프로 추가 가능)
    public float elementBonus = 1.2f;

    // 현재 상대 적
    private EnemySO currentEnemy;
    private int enemyHp;

    // 전투 시작
    public void StartBattle(EnemySO enemy)
    {
        currentEnemy = enemy;
        enemyHp = enemy.hp.max;
    }

    // 플레이어 공격
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

    // 적 공격
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

    // 상성 (a가 b에게 강한가)
    private bool IsStrong(ElementType a, ElementType b)
    {
        // 목→토, 토→수, 수→화, 화→금, 금→목
        if (a == ElementType.Wood && b == ElementType.Earth) return true;
        if (a == ElementType.Earth && b == ElementType.Water) return true;
        if (a == ElementType.Water && b == ElementType.Fire) return true;
        if (a == ElementType.Fire && b == ElementType.Metal) return true;
        if (a == ElementType.Metal && b == ElementType.Wood) return true;
        return false;
    }

    // 승리
    private void Win()
    {
        Debug.Log("전투 승리");
    }

    // 패배
    private void Lose()
    {
        Debug.Log("전투 패배");
    }
}
