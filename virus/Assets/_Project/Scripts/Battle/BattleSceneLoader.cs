using UnityEngine;
using UnityEngine.SceneManagement;

// 전투가 시작되면 전투 씬을 위에 얹고, 끝나면 내린다.
// 씬을 갈아타지 않으므로 탐사 깊이나 뽑아둔 이벤트가 그대로 남는다
public class BattleSceneLoader : MonoBehaviour
{
    [Header("참조")]
    public BattleManager battleManager;

    [Header("씬")]
    // 빌드 세팅에 있는 전투 씬 이름
    public string battleSceneName = "Battle";

    [Header("같이 켤 것")]
    // 전투 중에만 켤 이 씬 오브젝트. 연구 씬의 전투 배경처럼 화면을 덮을 것들
    public GameObject[] battleOnly;

    // 지금 얹혀 있는지
    private bool loaded;

    // 도주는 콜백을 안 부르기 때문에 전투 여부를 매 프레임 본다
    private void Update()
    {
        if (battleManager == null) return;

        bool inBattle = battleManager.InBattle();
        if (inBattle == loaded) return;

        for (int i = 0; i < battleOnly.Length; i++)
        {
            if (battleOnly[i] != null) battleOnly[i].SetActive(inBattle);
        }

        if (inBattle) Load();
        else Unload();
    }

    private void Load()
    {
        SceneManager.LoadScene(battleSceneName, LoadSceneMode.Additive);
        loaded = true;
    }

    private void Unload()
    {
        loaded = false;

        Scene scene = SceneManager.GetSceneByName(battleSceneName);
        if (!scene.isLoaded) return;

        SceneManager.UnloadSceneAsync(scene);
    }
}
