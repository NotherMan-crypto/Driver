# MAUI Project Structure Completion Plan

> **Project Type:** MOBILE (MAUI)  
> **Agent:** `mobile-developer`  
> **Phase:** PLANNING

---

## Overview

Complete the MAUI project structure for TracNghiemLaiXe (Vietnamese Driving Theory Quiz). The existing files (Models, ViewModels, Views) need to be integrated into a proper MAUI project with all required system files and dependencies.

---

## Success Criteria

| Criteria | Metric |
|----------|--------|
| Project builds | `dotnet build` succeeds |
| App launches | QuizPage displays as initial route |
| Questions load | JSON data loads from Resources/Raw |
| No warnings | Clean compilation |

---

## Tech Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET MAUI 8.0 |
| MVVM | CommunityToolkit.Mvvm 8.2.2 |
| JSON | System.Text.Json |
| Targets | Android, iOS, Windows, MacCatalyst |

---

## File Structure (Target)

```
D:\TracNghiemLaiXe\
├── TracNghiemLaiXe.csproj    # [CREATE] Project file
├── TracNghiemLaiXe.sln       # [CREATE] Solution file
├── App.xaml                   # [CREATE] Application
├── App.xaml.cs               # [CREATE] Application code-behind
├── AppShell.xaml             # [CREATE] Shell navigation
├── AppShell.xaml.cs          # [CREATE] Shell code-behind
├── MauiProgram.cs            # [EXISTS] Update DI registration
│
├── Models/
│   └── QuestionModel.cs      # [EXISTS] OK
│
├── ViewModels/
│   ├── BaseViewModel.cs      # [EXISTS] OK
│   └── QuizViewModel.cs      # [FIX] Add missing usings
│
├── Views/
│   ├── QuizPage.xaml         # [FIX] Fix namespaces
│   └── QuizPage.xaml.cs      # [EXISTS] OK
│
├── Behaviors/
│   └── TintColorBehavior.cs  # [EXISTS] OK
│
├── Resources/
│   ├── AppIcon/              # [CREATE] App icons
│   ├── Fonts/                # [CREATE] Custom fonts
│   ├── Images/               # [CREATE] Question images
│   ├── Raw/                  # [CREATE] JSON data
│   │   └── question_bank.json # [MOVE from data/]
│   ├── Splash/               # [CREATE] Splash screen
│   └── Styles/               # [CREATE] Global styles
│       ├── Colors.xaml       # [CREATE]
│       └── Styles.xaml       # [CREATE]
│
├── Platforms/
│   ├── Android/              # [CREATE] Android config
│   ├── iOS/                  # [CREATE] iOS config
│   ├── MacCatalyst/          # [CREATE] Mac config
│   └── Windows/              # [CREATE] Windows config
│
└── data/                     # [REMOVE after moving]
```

---

## Task Breakdown

### Task 1: Create Project File
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P0 (Blocker) |
| **INPUT** | NuGet package requirements |
| **OUTPUT** | `TracNghiemLaiXe.csproj` with all dependencies |
| **VERIFY** | File exists, XML valid |

**Dependencies:** CommunityToolkit.Mvvm, System.Text.Json (implicit)

---

### Task 2: Create Solution File
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P0 |
| **INPUT** | .csproj path |
| **OUTPUT** | `TracNghiemLaiXe.sln` |
| **VERIFY** | `dotnet sln list` shows project |

---

### Task 3: Create App.xaml + App.xaml.cs
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P0 |
| **INPUT** | Standard MAUI App template |
| **OUTPUT** | Application entry point with resources |
| **VERIFY** | Compiles without error |

---

### Task 4: Create AppShell.xaml + AppShell.xaml.cs
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P0 |
| **INPUT** | QuizPage as initial route |
| **OUTPUT** | Shell navigation with QuizPage |
| **VERIFY** | Route "//QuizPage" resolves |

---

### Task 5: Create Resources Folder Structure
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P1 |
| **INPUT** | MAUI resource conventions |
| **OUTPUT** | AppIcon, Fonts, Images, Raw, Splash, Styles folders |
| **VERIFY** | Folders exist |

---

### Task 6: Create Colors.xaml + Styles.xaml
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P1 |
| **INPUT** | Color palette from QuizPage |
| **OUTPUT** | Global style resources |
| **VERIFY** | Resources load in App.xaml |

---

### Task 7: Move question_bank.json to Resources/Raw
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P1 |
| **INPUT** | `data/question_bank.json` |
| **OUTPUT** | `Resources/Raw/question_bank.json` |
| **VERIFY** | File accessible via FileSystem.OpenAppPackageFileAsync |

---

### Task 8: Create Platform Folders
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P1 |
| **INPUT** | MAUI platform templates |
| **OUTPUT** | Android, iOS, MacCatalyst, Windows configs |
| **VERIFY** | All platforms configured |

---

### Task 9: Fix QuizViewModel.cs
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P2 |
| **INPUT** | Missing usings analysis |
| **OUTPUT** | Updated file with correct imports |
| **VERIFY** | No CS errors |

**Fixes needed:**
- Add `using System.Text.Json.Serialization;`
- Update JSON file path to `question_bank.json` (Resources/Raw)

---

### Task 10: Fix QuizPage.xaml
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P2 |
| **INPUT** | XAML namespace issues |
| **OUTPUT** | Corrected XAML with proper DataTypes |
| **VERIFY** | XAML compiles |

**Fixes needed:**
- Add Behaviors namespace
- Fix DataTemplate x:DataType references

---

### Task 11: Update MauiProgram.cs
| Attribute | Value |
|-----------|-------|
| **Agent** | `mobile-developer` |
| **Priority** | P2 |
| **INPUT** | Existing file |
| **OUTPUT** | Complete DI registration |
| **VERIFY** | All services registered |

---

## Phase X: Verification Checklist

- [ ] `dotnet build` succeeds
- [ ] `dotnet run` launches app
- [ ] QuizPage displays as initial screen
- [ ] Timer starts counting down
- [ ] Questions load from JSON
- [ ] Answer selection works
- [ ] Navigation buttons work
- [ ] Question grid displays

---

## Execution Order

```
P0: Task 1 → Task 2 → Task 3 → Task 4 (Sequential - core structure)
P1: Task 5, 6, 7, 8 (Parallel - resources)
P2: Task 9, 10, 11 (Parallel - fixes)
```

---

## Next Steps

After plan approval, run `/create` to:
1. Generate all missing files
2. Apply fixes to existing files
3. Build and verify
