using System;
using System.Collections.Generic;
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

    // 약 한 개가 거는 버프. 약마다 따로 시간을 셈
    private class Buff
    {
        public int attack;
        public int defense;
        public int turns;
    }

    // 지금 걸려 있는 약효 목록
    private readonly List<Buff> buffs = new List<Buff>();

    private Action onWin;
    private Action onLose;

    // 전투 진행 중 여부
    private bool inBattle;

    private bool useStamina;

    // keepBuffs: 같은 판에서 적만 갈리는 연전이면 마시던 약효를 그대로 들고 간다.
    // 안 그러면 앞 적한테 약을 쓴 순간 보스 앞에서 맨몸이 되는데, 그걸 알 방법이 화면에 없다
    public void StartBattle(EnemySO enemy, Action winCallback, Action loseCallback, bool spendStamina = false, bool keepBuffs = false)
    {
        currentEnemy = enemy;
        useStamina = spendStamina;
        enemyHp = enemy.hp.max;
        inBattle = true;
        enemyActionIndex = 0;
        enemyDefendBonus = 0;
        if (!keepBuffs) buffs.Clear();
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

    // 약효 남은 턴 중 가장 긴 것 (UI 표시용)
    public int GetBuffTurns()
    {
        int max = 0;
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].turns > max) max = buffs[i].turns;
        }
        return max;
    }

    // 걸려 있는 공격 버프 합 (UI 표시용)
    public int GetBuffAttack()
    {
        int sum = 0;
        for (int i = 0; i < buffs.Count; i++) sum += buffs[i].attack;
        return sum;
    }

    // 걸려 있는 방어 버프 합 (UI 표시용)
    public int GetBuffDefense()
    {
        int sum = 0;
        for (int i = 0; i < buffs.Count; i++) sum += buffs[i].defense;
        return sum;
    }

    public void PlayerAttack()
    {
        if (!inBattle) return;

        int power = gameState.battle.attack + GetBuffAttack();

        // 상성은 방어를 빼기 전에 곱해야 단단한 적한테도 효과가 남음
        if (IsStrong(gameState.battle.element, currentEnemy.element))
            power = Mathf.RoundToInt(power * elementBonus);

        int damage = power - (currentEnemy.defense + enemyDefendBonus);
        if (damage < 1) damage = 1;

        enemyHp -= damage;
        enemyDefendBonus = 0;   // 방어는 1회성
        if (useStamina && staminaManager != null) staminaManager.Spend(1);

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
        if (!inBattle) return;
        if (drug == null || drug.buffDuration <= 0) return;

        // 약마다 따로 걸려서 각성제 + 진통제를 같이 쓸 수 있음
        Buff buff = new Buff();
        buff.attack = drug.buffAttack;
        buff.defense = drug.buffDefense;
        buff.turns = drug.buffDuration;
        buffs.Add(buff);

        gameState.itemInventory.Remove(drug);

        // 약값은 적한테 한 턴 내주는 것. 약효 시간은 공격할 때만 줄어듦
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
        if (IsStrong(currentEnemy.element, gameState.battle.element))
            power = Mathf.RoundToInt(power * elementBonus);

        int damage = power - (gameState.battle.defense + GetBuffDefense());
        if (damage < 1) damage = 1;

        gameState.hp.current -= damage;
        gameState.hp.Clamp();

        if (gameState.hp.current <= 0)
            Lose();
    }

    // 약효 턴을 하나씩 줄이고 떨어진 약은 뺌
    private void TickBuff()
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            buffs[i].turns--;

            if (buffs[i].turns <= 0) buffs.RemoveAt(i);
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

        // 콜백 안에서 다음 전투가 시작될 수 있으니 먼저 비우고 부른다
        Action callback = onWin;
        onWin = null;
        onLose = null;
        callback?.Invoke();
    }

    private void Lose()
    {
        inBattle = false;

        Action callback = onLose;
        onWin = null;
        onLose = null;
        callback?.Invoke();
    }
}
