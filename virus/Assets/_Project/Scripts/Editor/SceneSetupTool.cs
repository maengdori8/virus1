using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// 씬에 매니저를 붙이고 인스펙터 참조를 채워주는 도구.
// 이미 연결된 값은 건드리지 않고 비어 있는 것만 채운다.
public static class SceneSetupTool
{
    private const string StatePath = "Assets/_Project/ScriptableObjects/GameState.asset";
    private const string ResearchDir = "Assets/_Project/ScriptableObjects/Research/";
    private const string AreaDir = "Assets/_Project/ScriptableObjects/Areas/";
    private const string SceneDir = "Assets/Scenes/";

    // 상단 HUD 씬. 파일이 Uppers/State.unity 로 옮겨져서 씬 이름이 State 다
    private const string UpperScenePath = "Assets/Scenes/Uppers/State.unity";
    private const string UpperSceneName = "State";

    private static readonly string[] ManagerScenes =
    {
        "Lobby", "Ingame", "Adventure_Enter", "Adventure", "Research"
    };

    // 이번 씬에서 뭔가 바꿨는지 (안 바꿨으면 저장도 안 함)
    private static bool changed;

    [MenuItem("Tools/백신 서바이벌/씬 상태 검사", false, 1)]
    public static void Check()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        foreach (string name in ManagerScenes)
        {
            if (!OpenIfExists(SceneDir + name + ".unity")) continue;

            string missing = "";

            if (name == "Ingame")
            {
                if (Find<IngameBootstrap>() == null) missing += "IngameBootstrap ";
                if (Find<ItemManager>() == null) missing += "ItemManager ";
                if (Find<ItemUI>() == null) missing += "ItemUI(수동) ";
                if (Find<ResultUI>() == null) missing += "ResultUI(수동) ";
            }
            if (name == "Adventure")
            {
                if (Find<AdventureUI>() == null) missing += "AdventureUI(수동) ";
                if (Find<BattleUI>() == null) missing += "BattleUI(수동) ";
            }
            if (name == "Adventure_Enter" && Find<AreaSelect>() == null) missing += "AreaSelect ";
            if (name == "Research" && Find<ResearchUI>() == null) missing += "ResearchUI(수동) ";
            if (name == "Lobby" && Find<RankDisplay>() == null) missing += "RankDisplay(수동) ";

            ResearchManager research = Find<ResearchManager>();
            if (research != null && (research.stages == null || research.stages.Length == 0)) missing += "연구단계 ";

            BattleManager battle = Find<BattleManager>();
            if (battle != null && battle.staminaManager == null) missing += "BattleManager.staminaManager ";

            foreach (UpperUISceneLoader loader in Object.FindObjectsOfType<UpperUISceneLoader>(true))
            {
                if (ReadUpperSceneName(loader) != UpperSceneName) missing += "상단UI씬이름 ";
            }

            Debug.Log(missing == "" ? "[" + name + "] 이상 없음" : "[" + name + "] 빠짐: " + missing);
        }

        if (OpenIfExists(UpperScenePath))
        {
            string missing = "";
            if (Find<ConsciousnessManager>() == null) missing += "ConsciousnessManager ";
            if (Find<FaceDisplay>() == null) missing += "FaceDisplay(수동) ";
            if (Find<HpDisplay>() == null) missing += "HpDisplay(수동) ";
            if (Find<StaminaDisplay>() == null) missing += "StaminaDisplay(수동) ";
            if (Find<TimeDisplay>() == null) missing += "TimeDisplay(수동) ";
            Debug.Log(missing == "" ? "[상단UI] 이상 없음" : "[상단UI] 빠짐: " + missing);
        }

        Debug.Log("검사 끝. (수동)은 UI 오브젝트를 직접 만들어야 해서 자동 배선 대상이 아님");
    }

    [MenuItem("Tools/백신 서바이벌/매니저 자동 배선", false, 2)]
    public static void Wire()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        GameState state = AssetDatabase.LoadAssetAtPath<GameState>(StatePath);
        if (state == null)
        {
            Debug.LogError("GameState.asset을 못 찾음: " + StatePath);
            return;
        }

        foreach (string name in ManagerScenes) WireScene(name, state);
        WireUpperScene(state);

        AssetDatabase.SaveAssets();
        Debug.Log("자동 배선 완료. UI 오브젝트(아이템/이벤트/전투/결과 패널)는 직접 만들어 연결할 것");
    }

    [MenuItem("Tools/백신 서바이벌/빌드 세팅 씬 등록", false, 3)]
    public static void FixBuildSettings()
    {
        // 기존 등록은 남기고 없는 것만 추가한다
        List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        List<string> want = new List<string>();
        foreach (string name in ManagerScenes) want.Add(SceneDir + name + ".unity");
        want.Add(UpperScenePath);

        foreach (string path in want)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
            {
                Debug.LogWarning("씬 파일이 없어서 건너뜀: " + path);
                continue;
            }

            bool already = false;
            foreach (EditorBuildSettingsScene s in list)
            {
                if (s.path != path) continue;
                s.enabled = true;
                already = true;
            }

            if (!already)
            {
                list.Add(new EditorBuildSettingsScene(path, true));
                Debug.Log("빌드 세팅에 추가: " + path);
            }
        }

        // 로비를 0번으로 올린다
        int lobby = list.FindIndex(s => s.path == SceneDir + "Lobby.unity");
        if (lobby > 0)
        {
            EditorBuildSettingsScene item = list[lobby];
            list.RemoveAt(lobby);
            list.Insert(0, item);
        }

        EditorBuildSettings.scenes = list.ToArray();
        Debug.Log("빌드 세팅 정리 완료 (기존 등록은 지우지 않음)");
    }

    // 상단 HUD 씬: 의식 관리만 따로 둔다 (겹쳐 로드되므로 다른 씬 참조 불가)
    private static void WireUpperScene(GameState state)
    {
        if (!OpenIfExists(UpperScenePath)) return;

        changed = false;
        ConsciousnessManager conscious = Ensure<ConsciousnessManager>(FindOrCreateRoot("Manager"), "ConsciousnessManager");
        if (conscious.gameState == null) { conscious.gameState = state; changed = true; }

        SaveIfChanged();
    }

    private static void WireScene(string sceneName, GameState state)
    {
        if (!OpenIfExists(SceneDir + sceneName + ".unity")) return;

        changed = false;
        Transform root = FindOrCreateRoot("Manager").transform;

        // 어느 씬이든 상단 UI 로더의 씬 이름이 틀어져 있으면 고친다
        RepairUpperLoaders();

        if (sceneName == "Adventure_Enter")
        {
            AreaSelect select = Ensure<AreaSelect>(root, "AreaSelect");
            if (select.areas == null || select.areas.Length == 0)
            {
                select.areas = new[]
                {
                    AssetDatabase.LoadAssetAtPath<ExplorationSO>(AreaDir + "Area_Beach.asset"),
                    AssetDatabase.LoadAssetAtPath<ExplorationSO>(AreaDir + "Area_Mount.asset"),
                    AssetDatabase.LoadAssetAtPath<ExplorationSO>(AreaDir + "Area_City.asset")
                };
                changed = true;
            }
            SaveIfChanged();
            return;
        }

        if (sceneName == "Lobby")
        {
            RankManager lobbyRank = Ensure<RankManager>(root, "RankManager");
            if (lobbyRank.gameState == null) { lobbyRank.gameState = state; changed = true; }
            SaveIfChanged();
            return;
        }

        // Ingame / Adventure / Research 는 매니저 한 벌이 다 필요하다
        GameInitializer init = Ensure<GameInitializer>(root, "GameInitializer");
        TimeManager time = Ensure<TimeManager>(root, "TimeManager");
        SupplyManager supply = Ensure<SupplyManager>(root, "SupplyManager");
        RewardManager reward = Ensure<RewardManager>(root, "RewardManager");
        StaminaManager stamina = Ensure<StaminaManager>(root, "StaminaManager");
        ConsciousnessManager conscious = Ensure<ConsciousnessManager>(root, "ConsciousnessManager");
        RankManager rank = Ensure<RankManager>(root, "RankManager");
        BattleManager battle = Ensure<BattleManager>(root, "BattleManager");
        ExplorationManager explore = Ensure<ExplorationManager>(root, "ExplorationManager");
        ResearchManager research = Ensure<ResearchManager>(root, "ResearchManager");
        GameManager game = Ensure<GameManager>(root, "GameManager");

        Set(ref init.gameState, state);
        Set(ref time.gameState, state);
        Set(ref supply.gameState, state);
        Set(ref reward.gameState, state);
        Set(ref stamina.gameState, state);
        Set(ref conscious.gameState, state);
        Set(ref rank.gameState, state);

        Set(ref battle.gameState, state);
        Set(ref battle.rewardManager, reward);
        Set(ref battle.staminaManager, stamina);

        Set(ref explore.gameState, state);
        Set(ref explore.rewardManager, reward);
        Set(ref explore.timeManager, time);
        Set(ref explore.battleManager, battle);
        Set(ref explore.staminaManager, stamina);
        Set(ref explore.gameManager, game);

        Set(ref research.gameState, state);
        Set(ref research.battleManager, battle);
        Set(ref research.gameManager, game);
        Set(ref research.consciousnessManager, conscious);
        if (research.stages == null || research.stages.Length == 0)
        {
            research.stages = new[]
            {
                AssetDatabase.LoadAssetAtPath<ResearchStageSO>(ResearchDir + "Research_Stage1.asset"),
                AssetDatabase.LoadAssetAtPath<ResearchStageSO>(ResearchDir + "Research_Stage2.asset"),
                AssetDatabase.LoadAssetAtPath<ResearchStageSO>(ResearchDir + "Research_Stage3.asset")
            };
            changed = true;
        }

        Set(ref game.gameState, state);
        Set(ref game.timeManager, time);
        Set(ref game.supplyManager, supply);
        Set(ref game.rewardManager, reward);
        Set(ref game.explorationManager, explore);
        Set(ref game.researchManager, research);
        Set(ref game.rankManager, rank);
        Set(ref game.gameInitializer, init);
        Set(ref game.consciousnessManager, conscious);

        // 게임 시작은 Ingame 씬에서만
        if (sceneName == "Ingame")
        {
            IngameBootstrap boot = Ensure<IngameBootstrap>(root, "IngameBootstrap");
            Set(ref boot.gameManager, game);

            ItemManager item = Ensure<ItemManager>(root, "ItemManager");
            Set(ref item.gameState, state);
            Set(ref item.rewardManager, reward);
        }

        SaveIfChanged();
    }

    // 비어 있을 때만 채운다
    private static void Set<T>(ref T field, T value) where T : Object
    {
        if (field != null) return;

        field = value;
        changed = true;
    }

    // 씬에 저장된 상단 UI 씬 이름을 실제 씬 이름으로 맞춘다
    private static void RepairUpperLoaders()
    {
        foreach (UpperUISceneLoader loader in Object.FindObjectsOfType<UpperUISceneLoader>(true))
        {
            SerializedObject so = new SerializedObject(loader);
            SerializedProperty prop = so.FindProperty("upperUISceneName");
            if (prop == null || prop.stringValue == UpperSceneName) continue;

            Undo.RecordObject(loader, "상단 UI 씬 이름 수정");
            prop.stringValue = UpperSceneName;
            so.ApplyModifiedProperties();
            changed = true;
            Debug.Log("상단 UI 씬 이름 수정: UpperUI → " + UpperSceneName);
        }
    }

    private static string ReadUpperSceneName(UpperUISceneLoader loader)
    {
        SerializedProperty prop = new SerializedObject(loader).FindProperty("upperUISceneName");
        return prop == null ? "" : prop.stringValue;
    }

    // 꺼져 있는 오브젝트까지 포함해서 찾는다
    private static T Find<T>() where T : Component
    {
        T[] all = Object.FindObjectsOfType<T>(true);
        return all.Length > 0 ? all[0] : null;
    }

    private static bool OpenIfExists(string path)
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
        {
            Debug.LogWarning("씬 파일이 없음: " + path);
            return false;
        }

        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        return true;
    }

    private static GameObject FindOrCreateRoot(string name)
    {
        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go.name == name) return go;
        }

        GameObject created = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(created, "매니저 생성");
        changed = true;
        return created;
    }

    private static T Ensure<T>(Transform parent, string name) where T : Component
    {
        T existing = Find<T>();
        if (existing != null) return existing;

        Transform child = parent.Find(name);
        GameObject target;

        if (child != null)
        {
            target = child.gameObject;

            // 꺼져 있어서 못 찾았을 수도 있으니 한 번 더 확인
            T onTarget = target.GetComponent<T>();
            if (onTarget != null) return onTarget;
        }
        else
        {
            target = new GameObject(name);
            target.transform.SetParent(parent, false);
            Undo.RegisterCreatedObjectUndo(target, "매니저 생성");
        }

        T added = Undo.AddComponent<T>(target);
        changed = true;
        Debug.Log("추가: " + name + " (" + typeof(T).Name + ")");
        return added;
    }

    private static void SaveIfChanged()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!changed)
        {
            Debug.Log("[" + scene.name + "] 바꿀 것 없음");
            return;
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[" + scene.name + "] 저장함");
    }
}
