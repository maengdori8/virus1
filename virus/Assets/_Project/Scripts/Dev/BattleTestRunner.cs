using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTestRunner : MonoBehaviour
{
    public BattleManager bm;
    public EnemySO enemy;
    private void Start()
    {
        bm.StartBattle(enemy, () => Debug.Log("½Â¸®"), () => Debug.Log("ÆÐ¹è"));
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            bm.PlayerAttack();
    }
}
