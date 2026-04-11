[English](README.md) | [Українська](README.ua.md) | [Русский](README.ru.md)

# Text Quest Reader

![Unity](https://img.shields.io/badge/engine-Unity-000000?logo=unity&logoColor=white)
![Language](https://img.shields.io/badge/language-C%23-blue)
![License](https://img.shields.io/badge/license-MIT-green)

A text quest reader inspired by the mechanics of the game "Space Rangers".

Together with the Quest Editor tool, it forms a system for creating and running custom text quests.

Supports location logic, transitions, parameters, as well as images and sounds.

---

## Demo

<!-- Add screenshots or GIFs here -->

---

## About the Project

The project is written in C# using Unity.

It is open source and can be used:
- to play the included quest "Asteroid Station"
- to study the project structure and architecture
- as a base for creating your own text quest reader with a custom UI

---

## Quest Structure

Each quest is stored as a separate folder with the following structure:

Assets/Resources/Quests/YourQuest/
    Images/
    Sounds/
    Musics/
    quest.json

- `quest.json` — main quest file (JSON)
- `Images/`, `Sounds/`, `Musics/` — resource folders (optional)

These folders can be empty — in this case, the quest will contain only text and logic.

---

## Important

The quest name inside `quest.json`:

```json
"questName": "YourQuest"
```

must match the name of the quest folder.

---

## How to Run

- Open the project in Unity
- Run the main scene

---

## Builds

Ready-to-use builds are located in the `_Builds` folder.

---

## Creating Quests

Quests are created using a separate tool — Quest Editor.

👉 Editor repository: [link]

The editor allows you to visually create parameters, locations, transitions, and quest structure, and then export it to a format compatible with this reader.

---

## Localization

The reader supports multiple languages.

You can add files like:
- `quest_en.json`
- `quest_uk.json`

If a localized file is missing — `quest.json` will be used.

See also `Localization.cs` and `LocKeys.cs`.

---

## Requirements

Unity 6.2

---

## License

MIT
