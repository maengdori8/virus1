using System.Collections.Generic;
using UnityEngine;

// 랭킹 관리
public class RankManager : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    [Header("랭킹")]
    // 랭킹 리스트
    public List<RankData> rankList = new List<RankData>();

    // 랭킹 패널 UI
    public GameObject rankPanel;

    // 클리어 시 이름과 일수를 rankList에 추가. 일수 기준 오름차순 정렬
    public void AddRank(string playerName)
    {
        RankData newRank = new RankData();
        newRank.playerName = playerName;
        newRank.clearDay = gameState.currentDay;

        rankList.Add(newRank);
        rankList.Sort((a, b) => a.clearDay.CompareTo(b.clearDay));
    }

    // rankPanel 활성화/비활성화 토글
    public void ToggleRankPanel()
    {
        rankPanel.SetActive(!rankPanel.activeSelf);
    }
}
