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
    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteText;

    [Header("Settings")]
    public float feedbackDuration = 0.8f;

    [Header("Levels Questions")]
    public QuestionData[] numberQuestions; // Level 0
    public QuestionData[] letterQuestions; // Level 1

    private QuestionData[] questions; // الأسئلة الحالية
    private int currentIndex = 0;
    private int score = 0;
    private int currentCorrectIndexInUI;
    private string[] currentChoices;

    private int currentLevel = 0; // 0 = numbers, 1 = letters
    private const int letterQuestionsLimit = 10; // عدد أسئلة Level 1

    void Start()
    {
        StartLevel(0); // ابدأ بالـNumbers
    }

    void StartLevel(int levelIndex)
    {
        currentLevel = levelIndex;

        if (levelIndex == 0)
        {
            questions = numberQuestions;
        }
        else if (levelIndex == 1)
        {
            questions = GetRandomSubset(letterQuestions, letterQuestionsLimit);
        }

        currentIndex = 0;
        score = 0;
        UpdateScoreUI();

        if (levelText != null)
            levelText.text = "Level " + (currentLevel + 1);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        ShuffleQuestions();
        LoadQuestion(currentIndex);
    }

    QuestionData[] GetRandomSubset(QuestionData[] source, int count)
    {
        List<QuestionData> list = new List<QuestionData>(source);
        List<QuestionData> selected = new List<QuestionData>();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }

        for (int i = 0; i < Mathf.Min(count, list.Count); i++)
            selected.Add(list[i]);

        return selected.ToArray();
    }

    void LoadQuestion(int qIndex)
    {
        var q = questions[qIndex];

        if (signImage != null)
            signImage.sprite = q.signImage;

        questionNumberText.text = $"{qIndex + 1} / {questions.Length}";
        feedbackText.text = "";

        currentChoices = new string[q.choices.Length];
        q.choices.CopyTo(currentChoices, 0);

        List<int> order = new List<int>();
        for (int i = 0; i < currentChoices.Length; i++) order.Add(i);

        for (int i = order.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            int tmp = order[i];
            order[i] = order[r];
            order[r] = tmp;
        }

        currentCorrectIndexInUI = -1;

        for (int ui = 0; ui < choiceTexts.Length; ui++)
        {
            if (ui < order.Count)
            {
                int sourceIndex = order[ui];
                choiceTexts[ui].text = currentChoices[sourceIndex];
                choiceButtons[ui].gameObject.SetActive(true);
                choiceButtons[ui].interactable = true;

                if (sourceIndex == q.correctIndex)
                    currentCorrectIndexInUI = ui;
            }
            else
            {
                choiceTexts[ui].text = "";
                choiceButtons[ui].gameObject.SetActive(false);
            }

            ResetButtonVisual(choiceButtons[ui]);
        }
    }

    public void CheckAnswer(int uiIndex)
    {
        foreach (var b in choiceButtons)
            if (b != null) b.interactable = false;

        bool correct = (uiIndex == currentCorrectIndexInUI);

        if (correct)
        {
            score += 5;
            StartCoroutine(ShowFeedbackCoroutine("Correct", true, uiIndex));
        }
        else
        {
            StartCoroutine(ShowFeedbackCoroutine("Incorrect", false, uiIndex));
        }

        UpdateScoreUI();
    }

    IEnumerator ShowFeedbackCoroutine(string message, bool isCorrect, int uiIndex)
    {
        if (choiceButtons != null && uiIndex >= 0 && uiIndex < choiceButtons.Length)
            SetButtonResultColor(choiceButtons[uiIndex], isCorrect);

        if (!isCorrect && currentCorrectIndexInUI >= 0)
            SetButtonResultColor(choiceButtons[currentCorrectIndexInUI], true);

        feedbackText.text = message;
        yield return new WaitForSeconds(feedbackDuration);

        currentIndex++;

        if (currentIndex < questions.Length)
        {
            LoadQuestion(currentIndex);
        }
        else
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
                if (levelCompleteText != null)
                    levelCompleteText.text = $"Well done! You finished Level {currentLevel + 1}";
            }
        }
    }

    public void NextLevel()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        if (currentLevel >= 1)
        {
            QuizFinished();
            return;
        }

        StartLevel(currentLevel + 1);
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }

    void QuizFinished()
    {
        feedbackText.text = $"Game Over! Score: {score} / {questions.Length}";
    }

    void SetButtonResultColor(Button b, bool correct)
    {
        var colors = b.colors;
        colors.normalColor = correct ? Color.green : Color.red;
        b.colors = colors;
    }

    void ResetButtonVisual(Button b)
    {
        var colors = b.colors;
        colors.normalColor = Color.white;
        b.colors = colors;
    }

    void ShuffleQuestions()
    {
        for (int i = questions.Length - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            var tmp = questions[i];
            questions[i] = questions[r];
            questions[r] = tmp;
        }
    }
}