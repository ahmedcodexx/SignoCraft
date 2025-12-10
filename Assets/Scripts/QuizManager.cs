using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("UI")]
    public Image signImage;
    public TMP_Text questionText;
    public TMP_Text[] choiceTexts;
    public Button[] choiceButtons;
    public TMP_Text feedbackText;
    public TMP_Text scoreText;
    public TMP_Text questionNumberText;
    public TMP_Text levelText;

    // ===== TIMER =====
    [Header("Timer")]
    public TMP_Text timerText;        // UI Text للوقت
    public float totalTime = 600f;    // 10 دقايق = 600 ثانية
    private float timer;
    private bool timerRunning = false;

    [Header("Panels")]
    public GameObject Startgame;
    public GameObject MainMenu;
    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteText;

    [Header("Main Menu Buttons")]
    public Button level2Button;

    [Header("Settings")]
    public float feedbackDuration = 0.8f;
    public int minScoreForLevel2 = 20;

    [Header("Levels Questions")]
    public QuestionData[] numberQuestions;
    public QuestionData[] letterQuestions;

    private QuestionData[] questions;
    private int currentIndex = 0;
    private int score = 0;
    private int currentCorrectIndexInUI;
    private int currentLevel = 0;
    private bool level1Unlocked = false;

    private const int letterQuestionsLimit = 10;

    // ================= START =================
    void Start()
    {
        Startgame.SetActive(true);
        MainMenu.SetActive(false);
        levelCompletePanel.SetActive(false);

        if (level2Button != null)
            level2Button.interactable = false;
    }

    // زر Start
    public void OpenMainMenu()
    {
        Startgame.SetActive(false);
        MainMenu.SetActive(true);

        if (level2Button != null)
            level2Button.interactable = level1Unlocked;
    }

    // ================= MAIN MENU =================
    public void PlayLevel1()
    {
        MainMenu.SetActive(false);
        StartLevel(0);
    }

    public void PlayLevel2()
    {
        if (!level1Unlocked)
        {
            feedbackText.text =
                $"يجب الحصول على {minScoreForLevel2} نقطة في Level 1 لفتح Level 2";
            return;
        }

        MainMenu.SetActive(false);
        StartLevel(1);
    }

    // ================= GAME =================
    void StartLevel(int levelIndex)
    {
        currentLevel = levelIndex;

        questions = (levelIndex == 0)
            ? numberQuestions
            : GetRandomSubset(letterQuestions, letterQuestionsLimit);

        currentIndex = 0;
        score = 0;
        UpdateScoreUI();

        // RESET TIMER
        timer = totalTime;
        timerRunning = true;
        StopAllCoroutines();
        StartCoroutine(TimerCountdown());

        if (levelText != null)
            levelText.text = "Level " + (currentLevel + 1);

        ShuffleQuestions();
        LoadQuestion(currentIndex);
    }

    // ===== TIMER SYSTEM =====
    IEnumerator TimerCountdown()
    {
        while (timerRunning && timer > 0)
        {
            timer -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            timerText.text = $"{minutes:00}:{seconds:00}";

            yield return null;
        }

        if (timer <= 0)
        {
            timerRunning = false;
            GameOverTimeUp();
        }
    }

    void GameOverTimeUp()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteText.text = $"Time's Up! ⏳\nScore: {score}";
        timerRunning = false;
    }

    QuestionData[] GetRandomSubset(QuestionData[] source, int count)
    {
        List<QuestionData> list = new List<QuestionData>(source);

        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }

        return list.GetRange(0, Mathf.Min(count, list.Count)).ToArray();
    }

    void LoadQuestion(int index)
    {
        QuestionData q = questions[index];

        signImage.sprite = q.signImage;
        questionNumberText.text = $"{index + 1} / {questions.Length}";
        feedbackText.text = "";

        List<int> order = new List<int>();
        for (int i = 0; i < q.choices.Length; i++)
            order.Add(i);

        for (int i = order.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (order[i], order[r]) = (order[r], order[i]);
        }

        currentCorrectIndexInUI = -1;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < order.Count)
            {
                int src = order[i];
                choiceTexts[i].text = q.choices[src];
                choiceButtons[i].interactable = true;
                choiceButtons[i].gameObject.SetActive(true);

                if (src == q.correctIndex)
                    currentCorrectIndexInUI = i;
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }

            ResetButtonVisual(choiceButtons[i]);
        }
    }

    public void CheckAnswer(int uiIndex)
    {
        if (!timerRunning)
            return;

        foreach (Button b in choiceButtons)
            b.interactable = false;

        bool correct = (uiIndex == currentCorrectIndexInUI);

        if (correct)
        {
            score += 5;
            StartCoroutine(ShowFeedback("Correct ✅", true, uiIndex));
        }
        else
        {
            StartCoroutine(ShowFeedback("Incorrect ❌", false, uiIndex));
        }

        UpdateScoreUI();
    }

    IEnumerator ShowFeedback(string text, bool isCorrect, int uiIndex)
    {
        SetButtonResultColor(choiceButtons[uiIndex], isCorrect);

        if (!isCorrect && currentCorrectIndexInUI >= 0)
            SetButtonResultColor(choiceButtons[currentCorrectIndexInUI], true);

        feedbackText.text = text;
        yield return new WaitForSeconds(feedbackDuration);

        currentIndex++;

        if (currentIndex < questions.Length && timerRunning)
            LoadQuestion(currentIndex);
        else
            ShowLevelComplete();
    }

    void ShowLevelComplete()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteText.text =
            $"Level {currentLevel + 1} Complete 🎉\nScore: {score}";

        timerRunning = false;

        if (currentLevel == 0)
        {
            if (score >= minScoreForLevel2)
            {
                level1Unlocked = true;
                if (level2Button != null)
                    level2Button.interactable = true;
            }
        }
    }

    public void NextLevel()
    {
        levelCompletePanel.SetActive(false);

        if (currentLevel == 0)
        {
            if (score >= minScoreForLevel2)
                OpenMainMenu();
            else
                StartLevel(0);
        }
        else
        {
            feedbackText.text = $"Game Over! Final Score: {score}";
        }
    }

    // ================= UI HELPERS =================
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    void SetButtonResultColor(Button b, bool correct)
    {
        ColorBlock cb = b.colors;
        cb.normalColor = correct ? Color.green : Color.red;
        b.colors = cb;
    }

    void ResetButtonVisual(Button b)
    {
        ColorBlock cb = b.colors;
        cb.normalColor = Color.white;
        b.colors = cb;
    }

    void ShuffleQuestions()
    {
        for (int i = questions.Length - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (questions[i], questions[r]) = (questions[r], questions[i]);
        }
    }
}