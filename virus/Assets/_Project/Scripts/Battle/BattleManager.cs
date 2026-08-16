using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public GameState gameState;
    public RewardManager rewardManager;
    public StaminaManager staminaManager;

    public float elementBonus = 1.2f;

    // 공격 데이터 없이 그냥 때렸을 때 쓰는 스태미나
    public int basicStaminaCost = 3;

    [Header("방어 행동")]
    // 회피 성공 확률 (0~1)
    public float dodgeChance = 0.3f;

    // 방어했을 때 깎이는 피해 비율 (0~1). 0.5 면 절반만 맞는다
    public float defendReduce = 0.5f;

    // 방어·회피 한 번에 쓰는 스태미나
    public int guardStaminaCost = 1;

    [Header("효과음 번호 (SoundManager.sfxClips)")]
    // -1 이면 소리 없이 넘어간다
    public int basicAttackSfx = -1;
    public int defendSfx = -1;
    public int dodgeSfx = -1;
    public int fleeSfx = -1;
    public int enemyAttackSfx = -1;

    private EnemySO currentEnemy;
    private int enemyHp;
    private int enemyActionIndex;   // 현재 적 행동 순서
    private int enemyDefendBonus;   // 적 방어 행동으로 얻은 임시 방어

    private bool defending;         // 이번 적 행동을 막는 중
    private bool dodging;           // 이번 적 행동을 피한 상태
    private bool lastDodgeSuccess;  // 마지막 회피 성패 (UI 표시용)

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
        defending = false;
        dodging = false;
        lastDodgeSuccess = false;
        if (!keepBuffs) buffs.Clear();
        onWin = winCallback;
        onLose = loseCallback;
    }

    // 전투 진행 중 여부 (UI 표시용)
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

    // 마지막 회피가 성공했는지 (UI 표시용)
    public bool LastDodgeSuccess()
    {
        return lastDodgeSuccess;
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

    // 공격 데이터 없이 때리는 기본 공격. 예전 공격 버튼과 테스트 씬이 쓴다
    public void PlayerAttack()
    {
        Attack(null);
    }

    // 고른 공격으로 때린다. 하단 공격 목록 버튼이 쓴다
    public void PlayerAttack(AttackSO attack)
    {
        Attack(attack);
    }

    // attack 이 null 이면 내 속성으로 위력 그대로 한 대
    private void Attack(AttackSO attack)
    {
        if (!inBattle) return;

        // 급습처럼 뜸을 들이는 공격은 적이 먼저 움직인다.
        // 그 사이에 쓰러지면 공격은 나가지 않는다
        if (attack != null && attack.enemyMovesFirst)
        {
            EnemyTurn();
            if (!inBattle) return;
        }

        PlaySfx(attack != null ? attack.sfxIndex : basicAttackSfx);

        int power = gameState.battle.attack + GetBuffAttack();
        ElementType element = gameState.battle.element;
        int hits = 1;
        bool ignoreDefense = false;
        int cost = basicStaminaCost;

        if (attack != null)
        {
            power = power * attack.powerPercent / 100;
            element = attack.element;
            hits = Mathf.Max(attack.hitCount, 1);
            ignoreDefense = attack.ignoreDefense;
            cost = attack.staminaCost;
        }

        // 상성은 방어를 빼기 전에 곱해야 단단한 적한테도 효과가 남음
        if (IsStrong(element, currentEnemy.element))
            power = Mathf.RoundToInt(power * elementBonus);

        // 연타는 한 대씩 따로 방어를 뺀다. 그래서 단단한 적한테 약하다
        int guard = ignoreDefense ? 0 : currentEnemy.defense + enemyDefendBonus;
        int total = 0;

        for (int i = 0; i < hits; i++)
        {
            int damage = power - guard;
            if (damage < 1) damage = 1;

            enemyHp -= damage;
            total += damage;
        }

        Debug.Log((attack != null ? attack.attackName : "공격") + ": 적한테 " + total
                  + (hits > 1 ? " (" + hits + "타)" : "") + ". 적 체력 " + Mathf.Max(enemyHp, 0));

        enemyDefendBonus = 0;   // 방어는 1회성
        if (useStamina && staminaManager != null) staminaManager.Spend(cost);

        TickBuff();

        if (enemyHp <= 0)
        {
            Win();
            return;
        }

        // 적이 앞에서 이미 움직였으면 또 때리게 두지 않는다
        if (attack == null || !attack.enemyMovesFirst) EnemyTurn();
    }

    // 방어. 이번 적 공격 피해를 defendReduce 만큼 깎는다
    public void PlayerDefend()
    {
        if (!inBattle) return;

        PlaySfx(defendSfx);
        if (useStamina && staminaManager != null) staminaManager.Spend(guardStaminaCost);

        defending = true;
        TickBuff();
        EnemyTurn();
        defending = false;
    }

    // 회피. dodgeChance 확률로 이번 적 공격을 통째로 흘린다
    public void PlayerDodge()
    {
        if (!inBattle) return;

        PlaySfx(dodgeSfx);
        if (useStamina && staminaManager != null) staminaManager.Spend(guardStaminaCost);

        dodging = UnityEngine.Random.value < dodgeChance;
        lastDodgeSuccess = dodging;
        Debug.Log(dodging ? "회피 성공" : "회피 실패");

        TickBuff();
        EnemyTurn();
        dodging = false;
    }

    // 도주. 그 자리에서 전투만 끝낸다. 이긴 것도 진 것도 아니라 콜백은 부르지 않는다
    public void Flee()
    {
        if (!inBattle) return;

        PlaySfx(fleeSfx);
        Debug.Log("도주. 전투를 빠져나왔다");

        inBattle = false;
        onWin = null;
        onLose = null;
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
            EnemyAttack(currentEnemy.attack, "공격");
            return;
        }

        EnemyAction act = currentEnemy.actions[enemyActionIndex];
        enemyActionIndex = (enemyActionIndex + 1) % currentEnemy.actions.Length;

        switch (act.type)
        {
            case EnemyActionType.Attack:
                EnemyAttack(currentEnemy.attack, "공격");
                break;
            case EnemyActionType.StrongAttack:
                EnemyAttack(currentEnemy.attack + act.value, "강공격");
                break;
            case EnemyActionType.Defend:
                enemyDefendBonus = act.value;
                Debug.Log("적이 방어. 이번 한 대는 방어 +" + act.value);
                break;
        }
    }

    // 적 공격 처리
    private void EnemyAttack(int power, string actionName)
    {
        // 회피에 성공했으면 그대로 흘린다
        if (dodging) return;

        PlaySfx(enemyAttackSfx);

        if (IsStrong(currentEnemy.element, gameState.battle.element))
            power = Mathf.RoundToInt(power * elementBonus);

        int damage = power - (gameState.battle.defense + GetBuffDefense());

        if (defending)
            damage = Mathf.RoundToInt(damage * (1f - defendReduce));

        if (damage < 1) damage = 1;

        gameState.hp.current -= damage;
        gameState.hp.Clamp();

        Debug.Log("적 " + actionName + ": " + (defending ? "막고 " : "") + damage
                  + " 맞음. 내 체력 " + gameState.hp.current);

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

    // 효과음. SoundManager 가 없거나 번호가 비어 있으면 조용히 넘어간다
    private void PlaySfx(int index)
    {
        if (index < 0 || SoundManager.instance == null) return;

        SoundManager.instance.PlaySFX(index);
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
