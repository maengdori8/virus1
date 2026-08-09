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
    private int enemyActionIndex;   // 현재 적 행동 순서
    private int enemyDefendBonus;   // 적 방어 행동으로 얻은 임시 방어

    // 약 버프 상태
    private int buffAttack;
    private int buffDefense;
    private int buffTurns;

    private Action onWin;
    private Action onLose;

    // 전투 진행 중 여부
    private bool inBattle;

    private bool useStamina;

    public void StartBattle(EnemySO enemy, Action winCallback, Action loseCallback, bool spendStamina = false)
    {
        currentEnemy = enemy;
        useStamina = spendStamina;
        enemyHp = enemy.hp.max;
        inBattle = true;
        enemyActionIndex = 0;
        enemyDefendBonus = 0;
        buffAttack = 0;
        buffDefense = 0;
        buffTurns = 0;
        onWin = winCallback;
        onLose = loseCallback;
    }

    // 전투 중 여부 (UI 표시용)
    public bool InBattle()
    {
        return inBattle;
    }

    // 현재 적 (UI 표시용)
    public EnemySO GetEnemy()
    {
        return currentEnemy;
    }

    // 현재 적 체력 (UI 표시용)
    public int GetEnemyHp()
    {
        return enemyHp;
    }

    // 적의 다음 행동 (UI 예고용). 패턴 없으면 null
    public EnemyAction GetNextEnemyAction()
    {
        if (currentEnemy == null || currentEnemy.actions == null || currentEnemy.actions.Length == 0)
            return null;
        return currentEnemy.actions[enemyActionIndex];
    }

    // 약효 남은 턴 (UI 표시용)
    public int GetBuffTurns()
    {
        return buffTurns;
    }

    // 약 공격 버프량 (UI 표시용)
    public int GetBuffAttack()
    {
        return buffAttack;
    }

    // 약 방어 버프량 (UI 표시용)
    public int GetBuffDefense()
    {
        return buffDefense;
    }

    public void PlayerAttack()
    {
        int damage = (gameState.battle.attack + buffAttack) - (currentEnemy.defense + enemyDefendBonus);
        if (damage < 1) damage = 1;

        if (IsStrong(gameState.battle.element, currentEnemy.element))
            damage = (int)(damage * elementBonus);
        if (damage < 1) damage = 1;

        enemyHp -= damage;
        enemyDefendBonus = 0;   // 방어는 1회성
        if(useStamina)staminaManager.Spend(1);

        TickBuff();

        if (enemyHp <= 0)
        {
            Win();
            return;
        }

        EnemyTurn();
    }

    // 약을 사용해 버프. 전투 턴을 소모하므로 적이 행동함
    public void UseDrug(ItemSO drug)
    {
        if (drug == null) return;

        buffAttack = drug.buffAttack;
        buffDefense = drug.buffDefense;
        buffTurns = drug.buffDuration;

        gameState.itemInventory.Remove(drug);

        EnemyTurn();
    }

    // 적의 한 턴. 패턴이 있으면 순서대로 반복, 없으면 일반 공격
    private void EnemyTurn()
    {
        if (currentEnemy.actions == null || currentEnemy.actions.Length == 0)
        {
            EnemyAttack(currentEnemy.attack);
            return;
        }

        EnemyAction act = currentEnemy.actions[enemyActionIndex];
        enemyActionIndex = (enemyActionIndex + 1) % currentEnemy.actions.Length;

        switch (act.type)
        {
            case EnemyActionType.Attack:
                EnemyAttack(currentEnemy.attack);
                break;
            case EnemyActionType.StrongAttack:
                EnemyAttack(currentEnemy.attack + act.value);
                break;
            case EnemyActionType.Defend:
                enemyDefendBonus = act.value;
                break;
        }
    }

    // 적 공격 처리
    private void EnemyAttack(int power)
    {
        int damage = power - (gameState.battle.defense + buffDefense);
        if (damage < 1) damage = 1;

        if (IsStrong(currentEnemy.element, gameState.battle.element))
            damage = (int)(damage * elementBonus);
        if (damage < 1) damage = 1;

        gameState.hp.current -= damage;
        gameState.hp.Clamp();

        if (gameState.hp.current <= 0)
            Lose();
    }

    // 약효 턴 감소. 0 되면 버프 해제
    private void TickBuff()
    {
        if (buffTurns <= 0) return;

        buffTurns--;
        if (buffTurns <= 0)
        {
            buffAttack = 0;
            buffDefense = 0;
        }
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
        inBattle = false;
        onWin?.Invoke();
        onWin = null;
        onLose = null;
    }

    private void Lose()
    {
        inBattle = false;
        onLose?.Invoke();
        onWin = null;
        onLose = null;
    }
}
