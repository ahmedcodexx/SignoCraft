using UnityEngine;

public class LevelStars : MonoBehaviour
{
    public GameObject[] stars; // Stars1, Stars2, Stars3
    public int levelIndex;     // 1, 2, 3 ...

    void Start()
    {
        int savedStars = PlayerPrefs.GetInt($"Level{levelIndex}_Stars", 0);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < savedStars);
        }
    }

    public void SetStars(int count)
    {
        PlayerPrefs.SetInt($"Level{levelIndex}_Stars", count);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < count);
        }
    }
}