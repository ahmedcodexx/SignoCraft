using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Quiz Manager")]
    public QuizManager quizManager;

    [Header("Level Buttons")]
    public Button level1Button;
    public Button level2Button;

    [Header("Level Data")]
    public QuestionData[] level1Questions;
    public QuestionData[] level2Questions;

    // ========================= START =========================
    void Start()
    {
        if (PlayerPrefs.GetInt("Level2Unlocked", 0) == 1)
        {
            level2Button.enabled = true;
            level2Button.GetComponent<Image>().color = Color.white;
        }
        else
        {
            level2Button.enabled = false;
        }
    }

    // ========================= LEVEL 1 =========================
    public void StartLevel1()
    {
        quizManager.StartQuiz(
            levelIndex: 0,
            questionsData: level1Questions
        );
    }

    // ========================= LEVEL 2 =========================
    public void StartLevel2()
    {
        List<QuestionData> temp = new List<QuestionData>(level2Questions);
        ShuffleList(temp);

        int count = Mathf.Min(10, temp.Count);
        QuestionData[] selectedQuestions = temp.GetRange(0, count).ToArray();

        quizManager.StartQuiz(
            levelIndex: 1,
            questionsData: selectedQuestions
        );
    }

    // ========================= HELPERS =========================
    void ShuffleList<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}