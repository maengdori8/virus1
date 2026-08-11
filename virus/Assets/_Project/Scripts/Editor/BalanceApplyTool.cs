using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// 시뮬레이션으로 검증한 권장 밸런스 값을 씬과 에셋에 넣어준다.
// 값이 이미 같으면 건드리지 않고, 바꾼 것은 전부 콘솔에 남긴다.
public static class BalanceApplyTool
{
    private const string SceneDir = "Assets/Scenes/";
    private const string EnemyDir = "Assets/_Project/ScriptableObjects/Enemies/";
    private const string ResearchDir = "Assets/_Project/ScriptableObjects/Research/";
    private const string EventDir = "Assets/_Project/ScriptableObjects/Events/";

    private static readonly string[] ManagerScenes = { "Ingame", "Adventure", "Research" };

    private static int touched;

    [MenuItem("Tools/백신 서바이벌/권장 밸런스 값 적용", false, 20)]
    public static void Apply()
    {
        bool go = EditorUtility.DisplayDialog(
            "권장 밸런스 값 적용",
            "씬의 GameInitializer/ConsciousnessManager 값과 적·연구단계·이벤트 에셋 값을 권장치로 바꾼다.\n\n" +
            "바뀐 값은 전부 콘솔에 남는다. 되돌리려면 git 으로 복구할 것.\n\n진행할까?",
            "적용", "취소");

        if (!go) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        touched = 0;

        ApplySceneValues();
        ApplyEnemyValues();
        ApplyResearchValues();
        ApplyEventValues();

        AssetDatabase.SaveAssets();
        Debug.Log("밸런스 적용 끝. 바꾼 값 " + touched + "개");
    }

    // 씬 안의 초기값 컴포넌트
    private static void ApplySceneValues()
    {
        foreach (string name in ManagerScenes)
        {
            string path = SceneDir + name + ".unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null) continue;

            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            bool changed = false;

            foreach (GameInitializer init in Object.FindObjectsOfType<GameInitializer>(true))
            {
                Undo.RecordObject(init, "밸런스 적용");

                bool one = false;
                one |= Field(init, "startMaxTimeTurn", ref init.startMaxTimeTurn, 1);
                one |= Field(init, "startDayTurn", ref init.startDayTurn, 14);
                one |= Field(init, "startAttack", ref init.startAttack, 12);
                one |= Field(init, "startDefense", ref init.startDefense, 6);
                one |= Field(init, "startMaxStamina", ref init.startMaxStamina, 12);
                one |= Field(init, "startDailyHeal", ref init.startDailyHeal, 3);
                one |= Field(init, "startDailyCost", ref init.startDailyCost, 5);
                one |= Field(init, "startSupply", ref init.startSupply, 14);

                if (one) EditorUtility.SetDirty(init);
                changed |= one;
            }

            foreach (ConsciousnessManager con in Object.FindObjectsOfType<ConsciousnessManager>(true))
            {
                Undo.RecordObject(con, "밸런스 적용");

                bool one = false;
                one |= Field(con, "dailyDrift", ref con.dailyDrift, 8);
                one |= Field(con, "lowHpPenalty", ref con.lowHpPenalty, 6);

                if (one) EditorUtility.SetDirty(con);
                changed |= one;
            }

            if (!changed)
            {
                Debug.Log("[" + name + "] 밸런스 바꿀 것 없음");
                continue;
            }

            Scene scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[" + name + "] 저장함");
        }
    }

    // 최종보스가 현재 수치로는 이길 수 없어서 낮춘다
    private static void ApplyEnemyValues()
    {
        EnemySO host = AssetDatabase.LoadAssetAtPath<EnemySO>(EnemyDir + "Enemy_Host.asset");
        if (host == null)
        {
            Debug.LogWarning("Enemy_Host.asset 없음");
            return;
        }

        SerializedObject so = new SerializedObject(host);
        bool changed = false;
        changed |= SetInt(so, "hp.current", 47, "숙주 체력");
        changed |= SetInt(so, "hp.max", 47, "숙주 최대체력");
        changed |= SetInt(so, "attack", 11, "숙주 공격력");

        if (!changed) return;

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(host);
    }

    // 샘플 요구량을 올려서 탐사 분량을 확보한다
    private static void ApplyResearchValues()
    {
        ApplyStage("Research_Stage1.asset", 3, 2, 2);
        ApplyStage("Research_Stage2.asset", 4, 3, 3);
        ApplyStage("Research_Stage3.asset", 6, 5, 5);
    }

    private static void ApplyStage(string file, int ocean, int mount, int city)
    {
        ResearchStageSO stage = AssetDatabase.LoadAssetAtPath<ResearchStageSO>(ResearchDir + file);
        if (stage == null)
        {
            Debug.LogWarning(file + " 없음");
            return;
        }

        SerializedObject so = new SerializedObject(stage);
        SerializedProperty cost = so.FindProperty("sampleCost");
        if (cost == null || !cost.isArray || cost.arraySize < 3)
        {
            Debug.LogWarning(file + " 의 sampleCost 모양이 예상과 다름");
            return;
        }

        bool changed = false;
        changed |= SetInt(so, "sampleCost.Array.data[0]", ocean, file + " 바다샘플");
        changed |= SetInt(so, "sampleCost.Array.data[1]", mount, file + " 산샘플");
        changed |= SetInt(so, "sampleCost.Array.data[2]", city, file + " 도시샘플");

        if (!changed) return;

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(stage);
    }

    // 도시 샘플 공급원이 사실상 하나뿐이라 병목이 생긴다
    private static void ApplyEventValues()
    {
        ApplyCitySample("Event_City_Store.asset", 0, 1);
        ApplyCitySample("Event_City_Pharmacy.asset", 1, 2);
        ApplyCitySample("Event_City_Lab.asset", 1, 3);
    }

    private static void ApplyCitySample(string file, int choiceIndex, int amount)
    {
        EventSO evt = AssetDatabase.LoadAssetAtPath<EventSO>(EventDir + file);
        if (evt == null)
        {
            Debug.LogWarning(file + " 없음");
            return;
        }

        SerializedObject so = new SerializedObject(evt);
        SerializedProperty choices = so.FindProperty("choices");
        if (choices == null || !choices.isArray || choices.arraySize <= choiceIndex)
        {
            Debug.LogWarning(file + " 에 선택지 " + choiceIndex + " 가 없음. 건너뜀");
            return;
        }

        // 선택지 이름을 같이 찍어서 엉뚱한 곳을 고치는지 눈으로 확인할 수 있게 한다
        SerializedProperty text = choices.GetArrayElementAtIndex(choiceIndex).FindPropertyRelative("text");
        string label = text != null ? text.stringValue : "(이름없음)";

        string path = "choices.Array.data[" + choiceIndex + "].result.sampleChange.Array.data[2]";
        SerializedProperty target = so.FindProperty(path);
        if (target == null)
        {
            Debug.LogWarning(file + " 의 샘플 배열 모양이 예상과 다름. 건너뜀");
            return;
        }

        if (!SetInt(so, path, amount, file + " [" + label + "] 도시샘플")) return;

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(evt);
    }

    // 값이 다를 때만 바꾸고 기록을 남긴다
    private static bool SetInt(SerializedObject so, string path, int value, string label)
    {
        SerializedProperty prop = so.FindProperty(path);
        if (prop == null)
        {
            Debug.LogWarning("필드를 못 찾음: " + path);
            return false;
        }

        if (prop.intValue == value) return false;

        Debug.Log(label + ": " + prop.intValue + " → " + value);
        prop.intValue = value;
        touched++;
        return true;
    }

    private static bool Field(Object owner, string label, ref int field, int value)
    {
        if (field == value) return false;

        Debug.Log(owner.name + "." + label + ": " + field + " → " + value);
        field = value;
        touched++;
        return true;
    }
}
