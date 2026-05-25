using UnityEngine;

// 사운드 관리
public class SoundManager : MonoBehaviour
{
    [Header("배경음")]
    // 배경음 목록
    public AudioClip[] bgmClips;

    // 배경음 볼륨
    public float bgmVolume;

    [Header("효과음")]
    // 효과음 목록
    public AudioClip[] sfxClips;

    // 효과음 볼륨
    public float sfxVolume;

    // 배경음 재생기
    private AudioSource bgmSource;

    // 효과음 재생기
    private AudioSource sfxSource;

    private void Awake()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
    }

    // 배경음 재생 (인덱스로 선택)
    public void PlayBGM(int index)
    {
        bgmSource.clip = bgmClips[index];
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    // 배경음 정지
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // 효과음 재생 (인덱스로 선택)
    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(sfxClips[index], sfxVolume);
    }
}
