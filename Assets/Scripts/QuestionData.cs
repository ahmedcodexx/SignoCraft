using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Quiz/Question")]
public class QuestionData : ScriptableObject
{
    public Sprite signImage;      // صورة الإشارة
    public string[] choices;      // مثلاً ["أ","ب","ت","ث"]
    public int correctIndex;      // index للجواب الصح داخل choices
}