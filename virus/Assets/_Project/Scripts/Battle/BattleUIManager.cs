using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [Header("매니저 참조")]
    public BattleManager battleManager;
    public GameState gameState;

    [Header("테스트용 적")]
    public EnemySO testEnemy;

    [Header("적 정보")]
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyHpText;

    [Header("플레이어 정보")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerStaminaText;
    public TextMeshProUGUI playerElementText;
}
