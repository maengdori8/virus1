using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public GameState gameState;
    public RewardManager rewardManager;
    public StaminaManager staminaManager;

    public float elementBonus = 1.2f;

    private EnemySO currentEnemy;
    private int enemyHp;
    private Action onWin;
    private Action onLose;

    public void StartBattle(EnemySO enemy, Action winCallback, Action loseCallback)
    {
        currentEnemy = enemy;
        enemyHp = enemy.hp.max;
        onWin = winCallback;
        onLose = loseCallback;
    }

    public void PlayerAttack()
    {
        int damage = gameState.battle.attack - currentEnemy.defense;
        if (damage < 1) damage = 1;

        if (IsStrong(gameState.battle.element, currentEnemy.element))
            damage = (int)(damage * elementBonus);
        if (damage < 1) damage = 1;

        enemyHp -= damage;
        staminaManager.Spend(1);

        if (enemyHp <= 0)
        {
            Win();
            return;
        }

        EnemyAttack();
    }

    private void EnemyAttack()
    {
        int damage = currentEnemy.attack - gameState.battle.defense;
        if (damage < 1) damage = 1;

        if (IsStrong(currentEnemy.element, gameState.battle.element))
            damage = (int)(damage * elementBonus);
        if (damage < 1) damage = 1;

        gameState.hp.current -= damage;
        gameState.hp.Clamp();

        if (gameState.hp.current <= 0)
            Lose();
    }

    // 오행 상극 (목토수화금)
    private bool IsStrong(ElementType a, ElementType b)
    {
        if (a == ElementType.Wood && b == ElementType.Earth) return true;
        if (a == ElementType.Earth && b == ElementType.Water) return true;
        if (a == ElementType.Water && b == ElementType.Fire) return true;
        if (a == ElementType.Fire && b == ElementType.Metal) return true;
        if (a == ElementType.Metal && b == ElementType.Wood) return true;
        return false;
    }

    private void Win()
    {
        onWin?.Invoke();
        onWin = null;
        onLose = null;
    }

    private void Lose()
    {
        onLose?.Invoke();
        onWin = null;
        onLose = null;
    }
}
