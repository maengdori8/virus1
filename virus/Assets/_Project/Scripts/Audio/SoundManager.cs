using UnityEngine;

// 사운드 관리
public class SoundManager : MonoBehaviour
{
    // 배경음 목록
    public AudioClip[] bgmClips;

    // 효과음 목록
    public AudioClip[] sfxClips;

    // 배경음 볼륨
    public float bgmVolume;

    // 효과음 볼륨
    public float sfxVolume;
}
