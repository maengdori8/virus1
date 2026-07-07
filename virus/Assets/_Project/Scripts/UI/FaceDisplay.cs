using UnityEngine;
using UnityEngine.UI;

// 의식 상태에 따라 얼굴 이미지 교체
public class FaceDisplay : MonoBehaviour
{
    [Header("참조")]
    public ConsciousnessManager consciousnessManager;

    // 얼굴 이미지
    public Image faceImage;

    [Header("얼굴 스프라이트 (명료→혼미 순)")]
    public Sprite[] faceSprites;

    // 매 프레임 의식 단계에 맞는 얼굴로 갱신
    private void Update()
    {
        if (consciousnessManager == null || faceImage == null) return;
        if (faceSprites == null || faceSprites.Length == 0) return;

        int level = Mathf.Min(consciousnessManager.FaceLevel(), faceSprites.Length - 1);
        faceImage.sprite = faceSprites[level];
    }
}
