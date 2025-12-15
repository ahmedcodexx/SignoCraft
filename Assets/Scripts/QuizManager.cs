using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    // ========================= VARIABLES =========================

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject mainMenuPanel;
    public GameObject level1Panel;
    public GameObject level2Panel;
    public GameObject quizPanel;
    public GameObject levelCompletePanel;

    [Header("Level Buttons")]
    public Button level1Button;
    public Button level2Button;

    [Header("Level Data")]
    public QuestionData[] level1Questions;
    public QuestionData[] level2Questions;

    private QuestionData[] questions;

    [Header("UI")]
    public Image signImage;
    public TMP_Text levelText;
    public TMP_Text[] choiceTexts;
    public Button[] choiceButtons;
    public TMP_Text feedbackText;
    public TMP_Text scoreText;
    public TMP_Text questionNumberText;
    public TMP_Text levelCompleteText;

    [Header("Timer")]
    public TMP_Text timerText;
    public float totalTime = 60f;
    private float timeLeft;
    bool timerRunning = false;

    [Header("Quiz Settings")]
    public float feedbackDuration = 1f;

    [Header("Background Music")]
    public AudioSource backgroundMusic;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    int currentLevel = 0;
    int currentIndex = 0;
    int currentCorrectIndexInUI = -1;
    int score = 0;

    // ========================= START =========================
    // بتشتغل أول ما اللعبة تبدأ
    // بتعرض شاشة البداية وتقفل Level 2
    void Start()
    {
        PlayerPrefs.DeleteAll();    /* متنساش تشيله لما تيجي تسلم  */

        ShowStartPanel();
        if (level2Button != null)
        {
            level2Button.enabled = false;
        }
    }

    // ========================= PANELS =========================

    // بتعرض شاشة البداية وتقفل باقي الشاشات
    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        level1Panel.SetActive(false);
        level2Panel.SetActive(false);
        quizPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
    }

    // بتفتح المينيو الرئيسية
    // وبتشيك هل Level 2 مفتوح ولا لأ
    public void OpenMainMenu()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
        ShowStartPanel();
        startPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // بتفتح شاشة اختيار Level 1
    public void OpenLevel1Panel()
    {
        mainMenuPanel.SetActive(false);
        level1Panel.SetActive(true);
    }

    // بتفتح شاشة اختيار Level 2
    public void OpenLevel2Panel()
    {
        mainMenuPanel.SetActive(false);
        level2Panel.SetActive(true);
    }

    // ========================= START LEVEL =========================

    // بتبدأ Level 1
    // بتصفر السكور وتحط أسئلة الليفل الأول
    public void StartLevel1()
    {
        score = 0;
        currentLevel = 0;
        questions = level1Questions;
        StartQuiz();
    }

    // بتبدأ Level 2
    // بتاخد 10 أسئلة عشوائي من أسئلة الليفل التاني
    public void StartLevel2()
    {
        score = 0;
        currentLevel = 1;
        levelText.text = "Level: 2";

        List<QuestionData> temp = new List<QuestionData>(level2Questions);

        for (int i = temp.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (temp[i], temp[r]) = (temp[r], temp[i]);
        }

        int count = Mathf.Min(10, temp.Count);
        questions = temp.GetRange(0, count).ToArray();

        StartQuiz();
    }

    // بتجهز الكويز قبل ما يبدأ
    // (تايمر – ترتيب الأسئلة – أول سؤال)
    void StartQuiz()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying) { 
            backgroundMusic.Stop();
        }
        quizPanel.SetActive(true);
        level1Panel.SetActive(false);
        level2Panel.SetActive(false);
        levelCompletePanel.SetActive(false);

        currentIndex = 0;
        ShuffleQuestions();

        timeLeft = totalTime;
        timerRunning = true;

        UpdateScoreUI();
        levelText.text = $"Level {currentLevel + 1}";
        LoadQuestion(0);
    }

    // ========================= TIMER =========================

    // بتشتغل كل فريم
    // بتقلل الوقت وبتنهي الكويز لو الوقت خلص
    void Update()
    {
        if (!timerRunning) return;

        timeLeft -= Time.deltaTime;
        timerText.text = $"{Mathf.Ceil(timeLeft)}";

        if (timeLeft <= 0)
        {
            timerRunning = false;
            EndQuizByTimer();
        }
    }

    // بتنهي الكويز لما الوقت يخلص
    void EndQuizByTimer()
    {
        levelCompletePanel.SetActive(true);
        levelCompleteText.text = $"Time's Up!\nScore: {score}";

        UnlockLevel2();
    }

    // ========================= QUIZ =========================

    // بتحميل السؤال الحالي وعرضه في الـ UI
    void LoadQuestion(int index)
    {
        QuestionData q = questions[index];

        signImage.sprite = q.signImage;
        questionNumberText.text = $"{index + 1} / {questions.Length}";
        feedbackText.text = "";

        List<int> order = new List<int>();
        for (int i = 0; i < q.choices.Length; i++) order.Add(i);
        ShuffleList(order);

        currentCorrectIndexInUI = -1;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < order.Count)
            {
                int src = order[i];
                choiceTexts[i].text = q.choices[src];
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].interactable = true;
                ResetButtonVisual(choiceButtons[i]);

                if (src == q.correctIndex)
                    currentCorrectIndexInUI = i;
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // بتتنده لما اللاعب يختار إجابة
    public void CheckAnswer(int uiIndex)
    {
        if (!timerRunning) return;

        bool correct = uiIndex == currentCorrectIndexInUI;

        if (correct)
        {
            score += 10;
            PlaySound(correctSound);
        }
        else
        {
            PlaySound(wrongSound);
        }

        ShowAnswerColors(uiIndex, correct);
        UpdateScoreUI();
        StartCoroutine(NextQuestion());
    }

    // بتستنى شوية وبعدها تنقل للسؤال اللي بعده
    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(feedbackDuration);

        currentIndex++;
        if (currentIndex < questions.Length && timerRunning)
            LoadQuestion(currentIndex);
        else
            ShowLevelComplete();
    }

    // ========================= END =========================

    // بتظهر شاشة نهاية الليفل
    void ShowLevelComplete()
    {
        timerRunning = false;
        levelCompletePanel.SetActive(true);
        levelCompleteText.text = $"Level {currentLevel + 1} Complete\nScore: {score}";

        SaveStars();
        UnlockLevel2();
    }
    void SaveStars()
    {
        float percent = (float)score / (questions.Length * 10);

        int stars = 0;

        if (percent >= 0.9f)
            stars = 3;
        else if (percent >= 0.6f)
            stars = 2;
        else if (percent >= 0.4f)
            stars = 1;

        GameObject levelButton =
            currentLevel == 0 ? level1Button.gameObject : level2Button.gameObject;

        LevelStars ls = levelButton.GetComponent<LevelStars>();
        if (ls != null)
            ls.SetStars(stars);
    }
    // بتفتح Level 2 لو اللاعب جاب 75% في Level 1
    void UnlockLevel2()
    {
        if (currentLevel != 0) return;

        float requiredScore = questions.Length * 10 * 0.75f;
        if (score >= requiredScore)
        {
            PlayerPrefs.SetInt("Level2Unlocked", 1);
            level2Button.enabled = true;
            level2Button.GetComponent<Image>().color = Color.white;
        }
    }

    // زرار الرجوع للمينيو
    public void NextLevel()
    {
        levelCompletePanel.SetActive(false);
        OpenMainMenu();
    }

    // ========================= HELPERS =========================

    // تحديث السكور على الشاشة
    void UpdateScoreUI()
    {
        scoreText.text = $"{score}";
    }

    // تلوين الأزرار حسب الإجابة
    void ShowAnswerColors(int selected, bool correct)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (!choiceButtons[i].gameObject.activeSelf) continue;

            if (i == selected)
                SetButton(choiceButtons[i], correct);
            else if (i == currentCorrectIndexInUI)
                SetButton(choiceButtons[i], true);
            else
                ResetButtonVisual(choiceButtons[i]);
        }
    }

    // تلوين الزر (أزرق صح – أحمر غلط)
    void SetButton(Button b, bool correct)
    {
        b.image.color = correct ? Color.green : Color.red;
    }

    // إعادة لون الزر الطبيعي
    void ResetButtonVisual(Button b)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#35B4C9", out color);
        b.image.color = color;
    }

    // خلط ترتيب الأسئلة
    void ShuffleQuestions()
    {
        ShuffleList(questions);
    }

    // Generic function لخلط أي List
    void ShuffleList<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    // تشغيل صوت صح أو غلط
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}