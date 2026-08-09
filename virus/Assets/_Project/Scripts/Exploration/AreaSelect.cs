using UnityEngine;
using UnityEngine.SceneManagement;

// 탐사 지역 선택 (Adventure_Enter 씬)
public class AreaSelect : MonoBehaviour
{
    // 선택된 지역 (Adventure 씬이 읽음)
    public static ExplorationSO selected;

    [Header("지역 목록 (바다/산/도시 순)")]
    public ExplorationSO[] areas;

    // 지역 버튼에 연결. 선택 저장 후 탐사 씬으로
    public void SelectArea(int index)
    {
        if (index < 0 || index >= areas.Length) return;

        selected = areas[index];
        SceneManager.LoadScene("Adventure");
    }
}
