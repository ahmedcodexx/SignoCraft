using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("UI")]
    public Image signImage;                // صورة السؤال
    public TMP_Text questionText;          // نص السؤال
    public TMP_Text[] choiceTexts;         // نصوص الأزرار
    public Button[] choiceButtons;         // الأزرار نفسها
    public TMP_Text feedbackText;          // صح/غلط
    public TMP_Text scoreText;             // النقاط
    public TMP_Text questionNumberText;    // رقم السؤال
    public TMP_Text levelText;             // رقم الـLevel
    public GameObject levelCompletePanel;  // Panel بعد انتهاء الـLevel
    public TMP_Text levelCompleteText;     // نص Panel التشجيع

    [Header("Settings")]
    public float feedbackDuration = 0.8f;

    [Header("Levels Questions")]
    public QuestionData[] numberQuestions; // Level 0
    public QuestionData[] letterQuestions; // Level 1

    private QuestionData[] questions;      // الأسئلة الحالية
    private int currentIndex = 0;
    private int score = 0;
    private int currentCorrectIndexInUI;
    private string[] currentChoices;

    private int currentLevel = 0; // 0 = numbers, 1 = letters

    void Start()
    {
        StartLevel(0); // ابدأ بالـNumbers
    }

    void StartLevel(int levelIndex)
    {
        currentLevel = levelIndex;

        if (levelIndex == 0)
            questions = numberQuestions;
        else if (levelIndex == 1)
            questions = letterQuestions;

        if (questions == null || questions.Length == 0)
        {
            Debug.LogError("No questions assigned for this Level!");
            return;
        }

        currentIndex = 0;
        score = 0;
        UpdateScoreUI();

        // تحديث Level Text
        if (levelText != null)
            levelText.text = "Level " + (currentLevel + 1);

        // اخفاء Panel التشجيع
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        ShuffleQuestions();
        LoadQuestion(currentIndex);
    }

    void LoadQuestion(int qIndex)
    {
        var q = questions[qIndex];

        if (signImage != null)
            signImage.sprite = q.signImage;

        questionNumberText.text = $"{qIndex + 1} / {questions.Length}";

        currentChoices = new string[q.choices.Length];
        q.choices.CopyTo(currentChoices, 0);

        // خلط الاختيارات
        List<int> idx = new List<int>();
        for (int i = 0; i < currentChoices.Length; i++) idx.Add(i);
        for (int i = idx.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            int tmp = idx[i];
            idx[i] = idx[r];
            idx[r] = tmp;
        }

        currentCorrectIndexInUI = -1;

        for (int ui = 0; ui < choiceTexts.Length; ui++)
        {
            if (ui < idx.Count)
            {
                int sourceChoiceIndex = idx[ui];
                choiceTexts[ui].text = currentChoices[sourceChoiceIndex];
                choiceButtons[ui].gameObject.SetActive(true);
                choiceButtons[ui].interactable = true;

                if (sourceChoiceIndex == q.correctIndex)
                    currentCorrectIndexInUI = ui;
            }
            else
            {
                choiceTexts[ui].text = "";
                choiceButtons[ui].gameObject.SetActive(false);
            }

            ResetButtonVisual(choiceButtons[ui]);
        }

        feedbackText.text = "";
    }

    public void CheckAnswer(int uiIndex)
    {
        foreach (var b in choiceButtons)
            if (b != null) b.interactable = false;

        bool correct = (uiIndex == currentCorrectIndexInUI);
        if (correct)
        {
            score++;
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

        if (!isCorrect && currentCorrectIndexInUI >= 0 && currentCorrectIndexInUI < choiceButtons.Length)
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
            // انتهى Level الحالي → أظهر Panel التشجيع
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
                if (levelCompleteText != null)
                    levelCompleteText.text = $"Well done! You have completed Level {currentLevel + 1}";
            }
        }
    }

    public void NextLevel()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        // ابدأ Level الجديد
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