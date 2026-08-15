using UnityEngine;

// 사운드 관리
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

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
        if(instance != null) { 
            Destroy(gameObject); 
            return;
        }
        instance = this;

        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
    }

    // bgmClips[index]를 스피커에 넣고 반복 재생.
    // 아직 클립을 안 넣은 자리는 조용히 넘긴다. 없는 번호로 부르면 재생만 안 될 뿐 게임은 굴러가야 한다
    public void PlayBGM(int index)
    {
        if (!Has(bgmClips, index)) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    // 배경음 재생 중지
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // sfxClips[index]를 한 번만 재생. 중첩 재생 가능
    public void PlaySFX(int index)
    {
        if (!Has(sfxClips, index)) return;

        sfxSource.PlayOneShot(sfxClips[index], sfxVolume);
    }

    // 해당 번호에 실제로 클립이 들어 있는지
    private bool Has(AudioClip[] clips, int index)
    {
        return clips != null && index >= 0 && index < clips.Length && clips[index] != null;
    }
}
