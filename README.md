# Animal Matching Game

A cross-platform animal memory game built with **.NET MAUI and C#**. Memorize the animal cards, find all eight matching pairs before the timer runs out, and try to beat your best time.

The original project idea was developed while learning from *Head First C#, 5th Edition*. This repository extends that learning project with randomized rounds, a timed challenge, saved personal bests, a preview phase, and spoken animal calls.

## Features

- **Randomized rounds:** Each game chooses eight animal types from a pool of 20, creates two cards for each, and shuffles their positions. The next round avoids repeating the previous round's animal lineup.
- **Memory preview:** All 16 cards are shown for three seconds before they are hidden.
- **Timed matching:** Find all eight pairs within a 60-second countdown. The countdown starts after the preview.
- **Personal best:** The quickest successful completion is saved locally on the device and shown between rounds. It is not shared or synchronized between devices.
- **Animal calls:** Selecting a card speaks a corresponding call using the platform's built-in text-to-speech voice. These are synthesized spoken cues, not recordings of real animals; voice and availability depend on the device.
- **Cross-platform foundation:** The project is configured for Android, iOS, Mac Catalyst, and Windows, subject to each platform's development requirements.

## How to play

1. Start the app and choose **Start game**.
2. Memorize the animals while the full board is visible for three seconds.
3. After the cards turn face-down, tap two cards to reveal them.
4. Matching cards stay revealed; mismatched cards turn back over after a short pause.
5. Match every pair before the countdown reaches zero.
6. Choose **Play again?** to try another shuffled board and improve your best time.

## Install and run the app

### Is there an installer to download?

This repository currently contains the **source code**, not a published installer or app-store release. The Windows target is configured as an unpackaged app (`WindowsPackageType` is `None`). To use the game now, run it from Visual Studio or build it yourself using the steps below. Do not expect a setup wizard or store download from this repository. A distributable installer/package would need to be created and published separately.

### Run on Windows with Visual Studio

1. Install **Visual Studio 2026** with the **.NET Multi-platform App UI development** workload and the Windows app development components.
2. Install the **.NET 11 SDK**. The project targets `net11.0`.
3. Clone the repository (or your fork; see [Fork and continue development](#fork-and-continue-development)).
4. Open `AnimalMatchingGame.slnx` in Visual Studio.
5. If Visual Studio asks to restore workloads or NuGet packages, allow the restore to finish. NuGet.org must be reachable the first time dependencies are restored.
6. Select the Windows machine target and the `Debug` configuration, then press **F5** to build and run, or **Ctrl+F5** to run without the debugger.

You can also build from PowerShell in the repository root:

```powershell
dotnet restore .\AnimalMatchingGame\AnimalMatchingGame.csproj
dotnet build .\AnimalMatchingGame\AnimalMatchingGame.csproj -f net11.0-windows10.0.19041.0
```

The Windows executable and its supporting files are written under `AnimalMatchingGame\bin\Debug\net11.0-windows10.0.19041.0\`, usually in an architecture-specific subfolder such as `win-x64`. Keep the output files together when running the unpackaged build. To run it from Visual Studio, prefer **F5**; for distribution to other Windows users, create and test a proper publish/package rather than sending only the executable.

### Run on Android

1. Install Visual Studio's **.NET MAUI** and **Android development** components, including the Android SDK and an emulator, or connect an Android device with developer mode enabled.
2. Restore the Android workload/dependencies when prompted.
3. Open the solution in Visual Studio, choose an Android emulator or connected device, and press **F5**.

Android builds can also be started from a terminal with the Android target selected, provided the Android SDK/device or emulator is configured:

```powershell
dotnet build .\AnimalMatchingGame\AnimalMatchingGame.csproj -f net11.0-android
```

Building an APK for local installation or distribution requires choosing and configuring the appropriate Android signing and packaging settings. The repository does not currently provide a signed APK.

### Run on iOS or Mac Catalyst

These targets require Apple's development toolchain. On Windows, use a properly configured Mac build host paired with Visual Studio; alternatively, build directly on a Mac with the required Xcode and .NET MAUI workloads. The project targets iOS 15.0 or later and Mac Catalyst 17.0 or later. No App Store or signed release is currently provided by this repository.

## Fork and continue development

### 1. Fork the repository

On GitHub, open [usman-idris/AnimalMatchingGame](https://github.com/usman-idris/AnimalMatchingGame) and select **Fork**. Create the fork under your GitHub account.

### 2. Clone your fork

Replace `<your-github-username>` with your account name, then run these commands in PowerShell:

```powershell
git clone https://github.com/<your-github-username>/AnimalMatchingGame.git
cd AnimalMatchingGame
```

If you prefer SSH and have configured an SSH key with GitHub:

```powershell
git clone git@github.com:<your-github-username>/AnimalMatchingGame.git
cd AnimalMatchingGame
```

### 3. Set up the development environment

- Install Visual Studio 2026 with the **.NET Multi-platform App UI development** workload, or install the .NET 11 SDK and the MAUI workloads required for your target platform.
- Use Windows for Windows and Android development (Android also needs its SDK and an emulator/device). iOS and Mac Catalyst require a Mac with Xcode, either directly or as a paired build host.
- Open `AnimalMatchingGame.slnx` and allow NuGet restore to complete. A network connection to NuGet.org is normally required to download packages not already cached locally.
- Select a supported target and build/run the app. The Windows build command is shown above.

### 4. Create a feature branch

```powershell
git switch -c feature/describe-your-change
```

Make your changes in the `AnimalMatchingGame` project, run the app on your target platform, and build before submitting changes. Keep platform-specific functionality compatible with the supported targets, and update this README if setup or gameplay behavior changes.

### 5. Commit and push your work

```powershell
git add .
git commit -m "Describe your change"
git push -u origin feature/describe-your-change
```

Then open your fork on GitHub and create a pull request from your feature branch. Include a short description of the change and how you tested it. If you only want to maintain your own version, you can continue committing and pushing to your fork without opening a pull request.

### Keep your fork in sync (optional)

To pull new changes from the original project, add it as the `upstream` remote once, then fetch and merge its default branch when needed:

```powershell
git remote add upstream https://github.com/usman-idris/AnimalMatchingGame.git
git fetch upstream
git switch main
git merge upstream/main
git push origin main
```

If your fork's default branch is not named `main`, replace `main` with its actual branch name. Resolve any merge conflicts before pushing.

## Project structure

```text
AnimalMatchingGame.slnx
AnimalMatchingGame/
  MainPage.xaml                 Game interface and card layout
  MainPage.xaml.cs              Round generation, matching, timer, and saved best time
  MauiProgram.cs                MAUI app setup and dependency configuration
  Platforms/                    Platform-specific entry points and configuration
  Resources/                    App icon, splash screen, fonts, and assets
```

The project uses .NET MAUI controls and platform APIs directly. The best time uses MAUI Preferences for local device storage, and animal-call cues use MAUI Text-to-Speech; no additional audio package is required.

## Configuration and development notes

- **Target frameworks:** `net11.0-android`, `net11.0-ios`, `net11.0-maccatalyst`, and `net11.0-windows10.0.19041.0` (Windows target included on Windows hosts).
- **Round settings:** The preview duration, round duration, and animal-call mappings are defined near the top of `MainPage.xaml.cs`.
- **Best time:** Stored locally using the `BestCompletionTimeTicks` preference key. Clearing the app's local data may remove the record.
- **Randomization:** The board is shuffled each round, and the previous round's animal lineup is excluded from repeating immediately.
- **Dependencies:** Core dependencies are declared in `AnimalMatchingGame/AnimalMatchingGame.csproj`. Restore packages with `dotnet restore` after cloning.

## Troubleshooting

- **Restore cannot reach NuGet.org:** Check your internet connection, proxy, or firewall, then retry `dotnet restore`. The project has no special package requirement beyond its declared dependencies, but missing dependencies cannot be restored while NuGet is unreachable.
- **MAUI workload or target is missing:** Open Visual Studio Installer and modify the installation to add the .NET MAUI workload and the platform components you need. On the command line, `dotnet workload restore .\AnimalMatchingGame\AnimalMatchingGame.csproj` can install workloads for the project when the corresponding SDK packs are available.
- **Android target does not launch:** Confirm the Android SDK is installed and an emulator is running or a developer-enabled device is connected and selected.
- **iOS/Mac Catalyst build fails on Windows:** These targets need Apple's toolchain on macOS; configure a Mac build host and pair Visual Studio to it.
- **No animal voice is heard:** The game requests speech from the device's installed text-to-speech service. Check the device volume and speech/voice settings; available voices may differ by platform.

## Acknowledgements

The learning project was inspired by the animal matching exercise in *Head First C#, 5th Edition*. This repository is a personal extension of that exercise using .NET MAUI. It is not an official companion app to the book.