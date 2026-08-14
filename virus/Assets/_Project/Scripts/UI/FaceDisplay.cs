using UnityEngine;
using UnityEngine.UI;

// 의식 상태에 따라 얼굴 이미지 교체
public class FaceDisplay : MonoBehaviour
{
    [Header("참조")]
    public GameState gameState;

    // 얼굴 이미지
    public Image faceImage;

    [Header("얼굴 스프라이트 (명료→혼미 순)")]
    public Sprite[] faceSprites;

    [Header("그림 없을 때 쓸 색 (명료→혼미 순)")]
    public Color[] faceColors;

    // 매 프레임 의식 단계에 맞는 얼굴로 갱신
    private void Update()
    {
        if (gameState == null || faceImage == null) return;

        int level = ConsciousnessManager.FaceLevelOf(gameState.consciousness);

        if (faceSprites != null && faceSprites.Length > 0)
        {
            faceImage.sprite = faceSprites[Mathf.Min(level, faceSprites.Length - 1)];
            return;
        }

        // 얼굴 그림이 아직 없으면 색만이라도 바뀌게
        if (faceColors != null && faceColors.Length > 0)
            faceImage.color = faceColors[Mathf.Min(level, faceColors.Length - 1)];
    }
}
