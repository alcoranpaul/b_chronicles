# Bible Chronicles

Bible Chronicles is an ADHD-friendly Bible reader with gamification elements. The application allows users to read Bible passages, complete typing sessions, and unlock characters, traits, and events as you progress through the Bible.

Video Demo: https://www.youtube.com/watch?v=bdNx5X-w5aY

## Status
- [![Release All Platforms Build](https://github.com/alcoranpaul/b_chronicles/actions/workflows/publish-and-release.yml/badge.svg?branch=main&event=push)](https://github.com/alcoranpaul/b_chronicles/actions/workflows/publish-and-release.yml)
- [![Test Linux](https://github.com/alcoranpaul/b_chronicles/actions/workflows/build-test-linux.yml/badge.svg?branch=main&event=push)](https://github.com/alcoranpaul/b_chronicles/actions/workflows/build-test-linux.yml)
- [![Test MacOS](https://github.com/alcoranpaul/b_chronicles/actions/workflows/build-test-macos.yml/badge.svg?branch=main&event=push)](https://github.com/alcoranpaul/b_chronicles/actions/workflows/build-test-macos.yml)
- [![Test Windows(https://github.com/alcoranpaul/b_chronicles/actions/workflows/build-test-windows.yml/badge.svg?branch=main&event=push)](https://github.com/alcoranpaul/b_chronicles/actions/workflows/build-test-windows.yml)


## Supported Platforms
- **Windows**: The application is primarily developed for Windows using .NET 8.0.
- **Linux/macOS**: The application does run on Linux and macOS.
- **Web**: The project is not designed for web deployment, but the code can be adapted for web applications if desired.
- **Mobile**: The project is not currently designed for mobile platforms, but could be adapted for mobile development.

## Features

- **Bible Reading & Typing Sessions:** Read and type Bible verses to track your progress.
- **Progress Tracking:** Maintains your current reading progress and stores it in JSON.
- **Logging:** Uses Serilog to log application events, with both file and console sinks.
- **Auto Updates:** Checks for updates via GitHub releases.

## Upcoming Features

- [ ] **Unlockable Characters & Traits:** Unlock new characters and traits as you complete verses.
- [ ] **Profile System:** Create and manage user profiles to track individual progress and preferences.
- [ ] **Events System:** Participate in story or seasonal events to unlock unique rewards and content.
- [ ] **Daily Reading Goals & Streaks**: Set daily verse goal (e.g., "Read 5 verses")
- [ ] **Achievements & Badges**: “First Chapter Completed”
- [ ] **Freemode**: Explore the bible
- [ ] **Progress Dashboard**: Stats screen
- [ ] **Mini-Games or Typing Challenges**: Short quiz after a passage
- [ ] **Bookmarks & Notes System**: Tag notes by topic (e.g., “Grace”, “Faith”)
- [ ] **Seasonal Events & Themes**: Easter event: unlock special content
- [ ] **Accuracy + Speed Tracking**


## Project Structure

```
├── .editorconfig
├── .gitignore
├── b_chronicles.csproj
├── b_chronicles.sln
├── README.md
├── .github/
│   └── workflows/
│       └── publish-and-release.yml
├── .vscode/
│   ├── launch.json
│   ├── settings.json
│   └── tasks.json
├── bin/
│   └── Debug/
├── json/
│   ├── books/
│   ├── player/
│   └── unlocks/
├── logs/
├── obj/
└── src/
    ├── AutoUpdate/
    ├── Bible/
    ├── BMain.cs
    ├── Components/
    ├── DataFetcher/
    ├── Interfaces/
    ├── MainProgram/
    │   └── Menus/
    ├── Player/
    │   ├── Book/
    │   ├── Character/
    │   └── User.cs
    ├── Storage/
    ├── TypingSession/
    │   ├── TypingEngine.cs
    │   ├── TypingSession.cs
    │   └── TypingSessionManager.cs
    └── Utils/
        ├── ConsoleHelper.cs
        └── LoggingConfig.cs
```

## Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- External Terminal support if you want colored logs when debugging (configured in `.vscode/launch.json`).

## Setup & Configuration

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/yourusername/b_chronicles.git
   cd b_chronicles
   ```

2. **Set Environment Variables:**

   The application checks for the `DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT` variables.  
   For development, you can set them in your terminal:
   
   **Windows (Command Prompt):**

   ```cmd
   set DOTNET_ENVIRONMENT=Development
   set ASPNETCORE_ENVIRONMENT=Development
   ```

   **Linux/macOS:**

   ```bash
   export DOTNET_ENVIRONMENT=Development
   export ASPNETCORE_ENVIRONMENT=Development
   ```

   You can also configure these in your `.vscode/launch.json`.

3. **Install Dependencies:**

   The project uses NuGet packages managed in the `.csproj` file. Use the following command to restore packages:

   ```bash
   dotnet restore
   ```

## Building the Project

You can build the project using the .NET CLI. For a Debug build run:

```bash
dotnet build
```

To publish a self-contained single file for Release (see also the instructions below):

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true
```

## Running the Application

You can run the application with:

```bash
dotnet run
```

Or using Visual Studio Code debugging by pressing `F5` (ensure your launch configuration in `.vscode/launch.json` is correct).

## Logging

The application uses Serilog for logging. Logs are stored in the `logs` directory and use a custom console formatter for timestamped, colored output.

## Auto Updates

The application can check for new releases on GitHub. When a new version is available, it will prompt you to update automatically.

## Contribution

Contributions are welcome. Please fork the repository and submit pull requests for any improvements.

## License

This project is licensed under the MIT License.

---

Enjoy reading and typing the Bible with Bible Chronicles!
