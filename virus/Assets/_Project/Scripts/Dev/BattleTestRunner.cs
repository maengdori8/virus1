using UnityEngine;

public class BattleTestRunner : MonoBehaviour
{
    public BattleManager bm;
    public EnemySO enemy;

    void Start()
    {
        bm.StartBattle(enemy, () => Debug.Log("승리"), () => Debug.Log("패배"));
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            bm.PlayerAttack();
    }
}
