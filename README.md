# CloudNotesApp ☁️📝

[![ru](https://img.shields.io/badge/lang-ru-green.svg)](README.ru.md)

A cross-platform mobile application built with **.NET MAUI** for creating and storing notes.

The main feature of the project is a **hybrid storage system**. The user decides where to store each specific note: only on the device (locally) or synchronized with the cloud (Firebase).

## 🚀 Features

* **Authentication:** User registration and login via Email/Password (Firebase Authentication).
* **Hybrid Storage:**
    * 📁 **Locally:** Notes are saved to a JSON file on the device. Always available, even without an internet connection.
    * ☁️ **Cloud:** Notes are saved to the Firebase Realtime Database. Available from any device after logging in.
* **Visual Indication:**
    * 🟩 Green background: Cloud note.
    * ⬛ Black/Gray background: Local note.
* **Offline Mode:** Ability to use the app without logging in (local storage only).
* **Network Indicator:** Real-time display of internet connection status.
* **Search:** Filter notes by content.

## 🛠 Tech Stack

* **Platform:** .NET 8 MAUI (Android, iOS, Windows, Mac).
* **Language:** C#, XAML.
* **Architecture:** Code-Behind / Services.
* **Backend:** Firebase (uses **REST API** directly via `HttpClient`).
* **Data:**
    * `System.Text.Json` for serialization.
    * Firebase Realtime Database for the cloud.
    * Local file system (`FileSystem.AppDataDirectory`) for offline notes.

## 📂 Project Structure

* `Views/` — Application pages (Login, Registration, NotesList, NoteEdit).
* `Services/` — Data handling logic:
    * `FirebaseService.cs` — Interaction with Firebase Auth and Database via REST API.
    * `LocalNoteService.cs` — Reading and writing the local `notes.json`.
* `Models/` — Description of `User` and `Note` entities.

## ⚙️ Setup and Run

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/YOUR-USERNAME/CloudNotesApp.git](https://github.com/YOUR-USERNAME/CloudNotesApp.git)
    ```
2.  **Firebase Configuration:**
    * In `Services/FirebaseConfig.cs`, you need to specify your Firebase project keys:
    ```csharp
    public static class FirebaseConfig
    {
        public static string ApiKey = "YOUR_API_KEY";
        public static string DatabaseUrl = "YOUR_DATABASE_URL";
    }
    ```
    > **Note:** For authentication to work, the "Email/Password" sign-in method must be enabled in the Firebase console, and read/write rules must be configured for the Realtime Database.

3.  **Run:**
    * Open the project in Visual Studio 2022.
    * Select the target platform (e.g., Android Emulator or Windows Machine).
    * Press Play ▶️.

## 📸 Screenshots

| Login Screen | Notes List | Edit Note |
|:---:|:---:|:---:|
| <img src="Screenshots/login.jpg" width="250"> | <img src="Screenshots/notesPage.jpg" width="250"> | <img src="Screenshots/NoteEdit.jpg" width="250"> |

## 🔮 Roadmap

* [ ] Implement MVVM architecture (CommunityToolkit.Mvvm).
* [ ] Implement Dependency Injection for services.
* [ ] Encryption for local notes.
* [ ] UI/UX design improvements.

---
Author: [Ilya]
