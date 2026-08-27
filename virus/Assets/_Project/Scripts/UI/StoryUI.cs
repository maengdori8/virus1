using UnityEngine;
using TMPro;

// 스토리 텍스트를 한 장씩 넘겨서 보여줌 (인트로/엔딩 공용)
public class StoryUI : MonoBehaviour
{
    [Header("패널")]
    public GameObject panel;

    // 스토리 텍스트
    public TextMeshProUGUI storyText;

    [Header("페이지")]
    [TextArea(2, 4)]
    public string[] pages = {
        "한약이 맛있어서 한의사가 됐다.",
        "다들 쓰다고 인상을 쓸 때 나는 그 맛이 좋았다.",
        "그래서 나를 실험했다. 내 몸에 약을 붓고 전부 기록했다.",
        "아무개 바이러스가 퍼졌다. 도시는 사흘 만에 무너졌다.",
        "사람들은 정신을 잃었다. 나만 빼고.",
        "평생 삼킨 약들이 무슨 짓을 한 모양이다.",
        "의식이 남아 있는 동안, 내 방식대로 치료제를 만든다."
    };

    [Header("설정")]
    // 씬 시작하자마자 재생할지 (인트로면 체크)
    public bool playOnStart;

    // 한 판에 한 번만 재생할지 (인트로면 체크)
    public bool oncePerRun;

    // 다 넘긴 뒤 켤 패널 (없으면 비워둠)
    public GameObject nextPanel;

    // 다 넘긴 뒤 부를 것 (패널 대신 다른 걸 띄울 때)
    public UnityEngine.Events.UnityEvent onClosed;

    // 이번 판에 인트로를 봤는지
    private static bool introShown;

    // 현재 페이지
    private int index;

    private void Start()
    {
        if (!playOnStart) return;

        // 이미 본 인트로면 건너뛰고 다음 패널로
        if (oncePerRun && introShown)
        {
            Close();
            return;
        }

        Play();
    }

    // 첫 페이지부터 재생
    public void Play()
    {
        if (pages == null || pages.Length == 0)
        {
            Close();
            return;
        }

        if (oncePerRun) introShown = true;

        index = 0;

        if (panel != null) panel.SetActive(true);
        if (storyText != null) storyText.text = pages[0];
    }

    // 다음 버튼에 연결. 마지막 장이면 닫음
    public void OnClickNext()
    {
        index++;

        if (pages == null || index >= pages.Length)
        {
            Close();
            return;
        }

        if (storyText != null) storyText.text = pages[index];
    }

    // 건너뛰기 버튼에 연결
    public void OnClickSkip()
    {
        Close();
    }

    // 패널 닫고 다음 패널 켜기
    private void Close()
    {
        if (panel != null) panel.SetActive(false);
        if (nextPanel != null) nextPanel.SetActive(true);

        if (onClosed != null) onClosed.Invoke();
    }

    // 로비로 나갈 때 호출. 다음 판에 인트로 다시 보여줌
    public static void ResetIntro()
    {
        introShown = false;
    }
}
