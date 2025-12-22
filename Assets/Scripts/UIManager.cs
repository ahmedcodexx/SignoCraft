using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject mainMenuPanel;
    public GameObject level1Panel;
    public GameObject level2Panel;
    public GameObject quizPanel;
    public GameObject levelCompletePanel;

    [Header("UI Elements")]
    public TMP_Text levelText;
    public TMP_Text[] choiceTexts;
    public Button[] choiceButtons;
    public TMP_Text scoreText;
    public TMP_Text questionNumberText;
    public TMP_Text levelCompleteText;
    public Image signImage;

    // ========================= PANELS =========================
    public void ShowPanel(GameObject panelToShow)
    {
        startPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        level1Panel.SetActive(false);
        level2Panel.SetActive(false);
        quizPanel.SetActive(false);
        levelCompletePanel.SetActive(false);

        if (panelToShow != null)
            panelToShow.SetActive(true);
    }

    // ========================= QUIZ UI =========================
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"{score}";
    }

    public void UpdateLevelText(int level)
    {
        if (levelText != null)
            levelText.text = $"Level {level}";
    }

    public void UpdateQuestionNumber(int current, int total)
    {
        if (questionNumberText != null)
            questionNumberText.text = $"{current} / {total}";
    }

    public void UpdateSignImage(Sprite sprite)
    {
        if (signImage != null)
            signImage.sprite = sprite;
    }

    public void UpdateChoices(List<string> choices, int correctIndex, out int currentCorrectIndexInUI)
    {
        currentCorrectIndexInUI = -1;
        List<int> order = new List<int>();
        for (int i = 0; i < choices.Count; i++) order.Add(i);
        ShuffleList(order);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < order.Count)
            {
                int src = order[i];
                choiceTexts[i].text = choices[src];
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].interactable = true;
                ResetButtonVisual(choiceButtons[i]);

                if (src == correctIndex)
                    currentCorrectIndexInUI = i;
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowAnswerColors(int selected, int correctIndex)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (!choiceButtons[i].gameObject.activeSelf) continue;

            if (i == selected)
                SetButton(choiceButtons[i], selected == correctIndex);
            else if (i == correctIndex)
                SetButton(choiceButtons[i], true);
            else
                ResetButtonVisual(choiceButtons[i]);
        }
    }

    // ========================= HELPERS =========================
    void SetButton(Button b, bool correct)
    {
        b.image.color = correct ? Color.green : Color.red;
    }

    void ResetButtonVisual(Button b)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#2B4A2B", out color);
        b.image.color = color;
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