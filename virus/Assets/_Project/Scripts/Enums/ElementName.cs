// 오행 속성을 화면에 쓸 이름으로
public static class ElementName
{
    public static string Of(ElementType element)
    {
        switch (element)
        {
            case ElementType.Wood: return "목";
            case ElementType.Fire: return "화";
            case ElementType.Earth: return "토";
            case ElementType.Metal: return "금";
            default: return "수";
        }
    }
}
