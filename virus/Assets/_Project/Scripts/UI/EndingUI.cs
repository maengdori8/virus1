using UnityEngine;

// 백신 완성을 감지해서 엔딩 재생
public class EndingUI : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    // 엔딩 텍스트를 재생할 StoryUI
    public StoryUI endingStory;

    // 이름 입력 패널 (엔딩 끝나고 뜸)
    public GameObject namePanel;

    // 중복 재생 방지
    private bool shown;

    // 엔딩이 끝나면 이름 입력 패널이 뜨도록 걸어둠
    private void Start()
    {
        if (endingStory != null && namePanel != null)
            endingStory.nextPanel = namePanel;
    }

    // 매 프레임 백신 완성 확인
    private void Update()
    {
        if (shown) return;
        if (gameState == null || endingStory == null) return;
        if (gameState.vaccineProgress < 100) return;

        shown = true;

        // GameManager가 먼저 띄웠으면 엔딩 뒤로 미룸
        if (namePanel != null) namePanel.SetActive(false);

        endingStory.Play();
    }
}
