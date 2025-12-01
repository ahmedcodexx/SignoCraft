using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour
{
    public int index; // 0,1,2,3 حسب ترتيب الزر
    public QuizManager manager;

    void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(() => manager.CheckAnswer(index));
    }
}