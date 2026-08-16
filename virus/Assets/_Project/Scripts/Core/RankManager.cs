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

    // 남겨둘 기록 수 (0이면 제한 없음)
    public int maxRecords = 10;
    private const string SaveKey = "RankData";


    // 랭킹 패널 UI
    public GameObject rankPanel;

    // 클리어 시 이름과 일수를 rankList에 추가. 일수 기준 오름차순 정렬
    public void AddRank(string playerName)
    {
        // 이름을 넣는 건 연구 씬의 RankManager 인데 저장된 기록을 읽은 적이 없는 놈이다.
        // 그대로 저장하면 이번 판 하나만 남고 예전 기록이 전부 날아간다
        Load();

        string trimmed = playerName == null ? "" : playerName.Trim();

        RankData newRank = new RankData();
        newRank.playerName = trimmed.Length == 0 ? "이름없음" : trimmed;
        newRank.clearDay = gameState.currentDay;

        rankList.Add(newRank);
        rankList.Sort((a, b) => a.clearDay.CompareTo(b.clearDay));

        // 계속 쌓이기만 하면 저장값이 끝없이 길어진다. 빠른 순으로 남긴다
        if (maxRecords > 0 && rankList.Count > maxRecords)
            rankList.RemoveRange(maxRecords, rankList.Count - maxRecords);

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

        // 데이터가 깨졌으면 빈 리스트 유지
        if (wrapper != null && wrapper.list != null)
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
