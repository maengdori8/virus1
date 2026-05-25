using UnityEngine;

// 랭킹 기록
[System.Serializable]
public class RankData
{
    [Header("랭킹")]
    // 플레이어 이름
    public string playerName;

    // 클리어 일수
    public int clearDay;
}
