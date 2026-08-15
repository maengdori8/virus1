using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 랭킹 목록 표시 (로비 랭킹 패널)
public class RankDisplay : MonoBehaviour
{
    [Header("참조")]
    public RankManager rankManager;

    [Header("목록")]
    // 한 줄짜리 프리팹 (RankData). 헤더인 Tag 와 같은 칸 배치를 쓴다
    public GameObject rowPrefab;

    // 줄이 붙을 곳. Scroll View 의 Content
    public Transform rowParent;

    [Header("기록이 없을 때")]
    public TextMeshProUGUI emptyText;

    // 이번에 붙인 줄. 헤더까지 지우면 안 되니 따로 들고 있는다
    private readonly List<GameObject> rows = new List<GameObject>();

    // 패널이 열릴 때 랭킹 갱신
    private void OnEnable()
    {
        rankManager.Load();
        ClearRows();

        List<RankData> list = rankManager.rankList;

        if (emptyText != null)
        {
            emptyText.text = list.Count == 0 ? "기록 없음" : "";
            emptyText.gameObject.SetActive(list.Count == 0);
        }

        if (rowPrefab == null || rowParent == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            GameObject row = Instantiate(rowPrefab, rowParent);
            rows.Add(row);

            SetCell(row, "Score", (i + 1) + "위");
            SetCell(row, "Namae", list[i].playerName);
            SetCell(row, "Time", list[i].clearDay + "일");
            SetCell(row, "Text (TMP)", "");
        }
    }

    private void OnDisable()
    {
        ClearRows();
    }

    // 프리팹 안의 칸을 이름으로 찾아 채운다
    private void SetCell(GameObject row, string cellName, string value)
    {
        Transform cell = row.transform.Find(cellName);
        if (cell == null) return;

        TextMeshProUGUI text = cell.GetComponent<TextMeshProUGUI>();
        if (text != null) text.text = value;
    }

    private void ClearRows()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] != null) Destroy(rows[i]);
        }
        rows.Clear();
    }
}
