# SignoCraft 🧩✋

SignoCraft is an interactive educational project designed to help users learn Sign Language through a quiz-based learning system. The project is developed using Unity and follows *Clean Code principles* and *Separation of Concerns* for better scalability and maintainability.

---

## 🎯 Project Idea

The goal of this project is to:

- Teach sign language in a simple and interactive way.
- Test user knowledge through multiple quiz levels.
- Provide instant feedback using scores and star ratings.

The project is suitable for educational purposes, especially for students and beginners learning sign language.

---

## 🕹 Gameplay Flow

1. The game starts at the *Start Screen*.
2. The user navigates to the *Main Menu* to choose a level.
3. Each level contains a set of questions.
4. For each question:
   - A sign image is displayed.
   - Multiple-choice answers are provided.
   - A countdown timer runs.
5. When the level ends:
   - The final score is displayed.
   - Stars are awarded based on performance.
   - The next level is unlocked if requirements are met.

---

## 🧱 Project Architecture

The project is structured using multiple manager scripts to keep responsibilities clear and the codebase clean:

- *QuizManager*  
  Handles the core quiz logic and coordinates between all other managers.

- *LevelManager*  
  Responsible for starting levels and preparing/selecting questions.

- *UIManager*  
  Manages all UI panels and elements, updates texts, images, buttons, and visual feedback.

- *ScoreManager*  
  Manages score calculation, updates the score UI, and provides final score values.

- *SoundManager*  
  Controls background music and plays correct/wrong answer sound effects.

- *TimerController*  
  Manages quiz time and ends the quiz when time runs out.

---

## ⭐ Star Rating System

| Stars | Score Percentage |
|-------|----------------|
| ⭐⭐⭐   | 90% or higher   |
| ⭐⭐    | 60% to less than 90% |
| ⭐     | 40% to less than 60% |
| None  | Below 40%      |

---

## 🛠 Technologies Used

- Unity Engine
- C#
- TextMeshPro
- PlayerPrefs (to save progress)
- Git & GitHub for version control

---

## 📂 Project Structure
Assets/ └── Scripts/ ├── QuizManager.cs ├── LevelManager.cs ├── UIManager.cs ├── ScoreManager.cs ├── SoundManager.cs ├── TimerController.cs ├── QuestionData.cs └── LevelStars.cs

---

## 🚀 Project Features

- Clean and modular architecture.
- Easy to extend with new levels or features.
- Clear separation between logic, UI, audio, and data.
- Suitable as a portfolio or academic project.

---

## 👤 Author

*Ahmed Reda (Codex)*  
Frontend Developer & IT Student
