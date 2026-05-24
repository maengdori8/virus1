// 시간 데이터
[System.Serializable]
public class TimeData
{
    // 남은 날 (0이면 게임오버)
    public int dayTurn;

    // 하루 내 남은 턴
    public int timeTurn;

    // 하루 최대 턴
    public int maxTimeTurn;
}
