using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("UI Manager")]
    public UIManager uiManager;

    [Header("Sound Manager")]
    public SoundManager soundManager;

    [Header("Score Manager")]
    public ScoreManager scoreManager;

    [Header("Timer")]
    public TimerController timer;

    [Header("Level Buttons")]
    public Button level1Button;
    public Button level2Button;

    [Header("Quiz Settings")]
    public float feedbackDuration = 1f;

    // ========================= PRIVATE =========================
    private QuestionData[] questions;
    private int currentLevel = 0;
    private int currentIndex = 0;
    private int currentCorrectIndexInUI = -1;

    // ========================= START =========================
    void Start()
    {
        PlayerPrefs.DeleteAll();
        uiManager.ShowPanel(uiManager.startPanel);

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

    // ========================= START QUIZ (CALLED FROM LevelManager) =========================
    public void StartQuiz(int levelIndex, QuestionData[] questionsData)
    {
        currentLevel = levelIndex;
        questions = questionsData;

        currentIndex = 0;
        ShuffleQuestions();

        uiManager.ShowPanel(uiManager.quizPanel);

        if (scoreManager != null)
            scoreManager.ResetScore();

        if (soundManager != null)
            soundManager.StopBackground();

        timer.OnTimerEnd = EndQuizByTimer;
        timer.StartTimer();

        uiManager.UpdateLevelText(currentLevel + 1);
        LoadQuestion(currentIndex);
    }

    // ========================= QUIZ =========================
    void LoadQuestion(int index)
    {
        QuestionData q = questions[index];

        uiManager.UpdateSignImage(q.signImage);
        uiManager.UpdateQuestionNumber(index + 1, questions.Length);
        uiManager.UpdateChoices(
            new List<string>(q.choices),
            q.correctIndex,
            out currentCorrectIndexInUI
        );
    }

    public void CheckAnswer(int uiIndex)
    {
        if (!timer.IsRunning()) return;

        bool correct = uiIndex == currentCorrectIndexInUI;

        if (correct)
        {
            if (scoreManager != null)
                scoreManager.AddScore(10);

            if (soundManager != null)
                soundManager.PlayCorrect();
        }
        else
        {
            if (soundManager != null)
                soundManager.PlayWrong();
        }

        uiManager.ShowAnswerColors(uiIndex, currentCorrectIndexInUI);
        StartCoroutine(NextQuestion());
    }

    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(feedbackDuration);

        currentIndex++;

        if (currentIndex < questions.Length && timer.IsRunning())
            LoadQuestion(currentIndex);
        else
            ShowLevelComplete();
    }

    // ========================= END QUIZ =========================
    void ShowLevelComplete()
    {
        timer.StopTimer();

        uiManager.ShowPanel(uiManager.levelCompletePanel);
        uiManager.levelCompleteText.text =
            $"Level {currentLevel + 1} Complete\nScore: {scoreManager.GetScore()}";

        if (soundManager != null)
            soundManager.PlayBackground();

        SaveStars();
        UnlockLevel2();
    }

    void EndQuizByTimer()
    {
        timer.StopTimer();

        uiManager.ShowPanel(uiManager.levelCompletePanel);
        uiManager.levelCompleteText.text =
            $"Time's Up!\nScore: {scoreManager.GetScore()}";

        if (soundManager != null)
            soundManager.PlayBackground();

        UnlockLevel2();
    }

    // ========================= STARS & UNLOCK =========================
    void SaveStars()
    {
        float percent = (float)scoreManager.GetScore() / (questions.Length * 10);
        int stars = 0;

        if (percent >= 0.9f) stars = 3;
        else if (percent >= 0.6f) stars = 2;
        else if (percent >= 0.4f) stars = 1;

        GameObject levelButton =
            currentLevel == 0 ? level1Button.gameObject : level2Button.gameObject;

        LevelStars ls = levelButton.GetComponent<LevelStars>();
        if (ls != null)
            ls.SetStars(stars);
    }

    void UnlockLevel2()
    {
        if (currentLevel != 0) return;

        float requiredScore = questions.Length * 10 * 0.75f;

        if (scoreManager.GetScore() >= requiredScore)
        {
            PlayerPrefs.SetInt("Level2Unlocked", 1);
            level2Button.enabled = true;
            level2Button.GetComponent<Image>().color = Color.white;
        }
    }

    // ========================= HELPERS =========================
    void ShuffleQuestions()
    {
        ShuffleList(questions);
    }

    void ShuffleList<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}