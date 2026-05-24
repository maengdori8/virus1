// 플레이어 선택지 하나
[System.Serializable]
public class ChoiceData
{
    // 선택지 텍스트
    public string text;

    // 스태미나 소모량
    public int staminaCost;

    // 실행 결과
    public ActionData result;
}
