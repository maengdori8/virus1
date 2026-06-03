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
    private const string SaveKey = "RankData";


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
        Save();
    }

    public void Save()
    {
        RankList wrapper = new RankList();
        wrapper.list = rankList; // 리스트를 상자에 담음
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(wrapper)); // wrapper를 json 텍스트로 바꿈, 그 글자를 SaveKey("RankData)에 넣음
        PlayerPrefs.Save();
    }

    // 저장된 json 읽어서 ranklist 복원
    public void Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;
        RankList wrapper = JsonUtility.FromJson<RankList>(PlayerPrefs.GetString(SaveKey));
        rankList = wrapper.list;
    }

    // rankPanel 활성화/비활성화 
    public void ToggleRankPanel()
    {
        rankPanel.SetActive(!rankPanel.activeSelf);
    }


    [System.Serializable]
    public class RankList
    {
        public List<RankData> list;
    }
}
