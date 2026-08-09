using UnityEngine;
using TMPro;

// 랭킹 목록 표시 (로비 랭킹 패널)
public class RankDisplay : MonoBehaviour
{
    [Header("참조")]
    public RankManager rankManager;

    [Header("표시")]
    public TextMeshProUGUI rankText;

    // 패널이 열릴 때 랭킹 갱신
    private void OnEnable()
    {
        rankManager.Load();

        string result = "";
        for (int i = 0; i < rankManager.rankList.Count; i++)
        {
            RankData rank = rankManager.rankList[i];
            result += (i + 1) + "위  " + rank.playerName + "  " + rank.clearDay + "일차\n";
        }

        if (result == "") result = "기록 없음";
        rankText.text = result;
    }
}
