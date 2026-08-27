using UnityEngine;
using UnityEngine.UI;

// 탐사 배경. 다녀온 지역에 맞는 그림 묶음만 켠다 (Adventure 씬)
public class AreaBackground : MonoBehaviour
{
    // 지역 하나에 딸린 배경 한 벌
    [System.Serializable]
    public class Set
    {
        public ExplorationSO area;

        // 그 지역에만 나오는 것들 (바다의 갈매기·파도 같은)
        public GameObject group;

        // 하늘과 바닥은 한 장씩 갈아끼운다
        public Sprite sky;
        public Sprite ground;
    }

    [Header("공통")]
    public Image skyImage;
    public Image groundImage;

    [Header("지역별")]
    public Set[] sets;

    // 지역은 AreaSelect 가 씬을 넘기기 전에 정해둔다
    private void Start()
    {
        Apply(AreaSelect.selected);
    }

    // 고른 지역 것만 남기고 나머지는 끈다
    public void Apply(ExplorationSO area)
    {
        if (sets == null) return;

        for (int i = 0; i < sets.Length; i++)
        {
            if (sets[i] == null) continue;

            bool here = sets[i].area == area;

            if (sets[i].group != null) sets[i].group.SetActive(here);
            if (!here) continue;

            if (skyImage != null && sets[i].sky != null) skyImage.sprite = sets[i].sky;
            if (groundImage != null && sets[i].ground != null) groundImage.sprite = sets[i].ground;
        }
    }
}
