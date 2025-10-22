# 🚀 Robotech-TBS Installation & Testing Guide

**Last Updated:** 2025-10-22
**Project:** Robotech: Macross Era - Turn-Based Strategy
**Status:** Early Development / Prototype Phase

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Installation Steps](#installation-steps)
3. [Opening the Project](#opening-the-project)
4. [Running the Game](#running-the-game)
5. [Testing](#testing)
6. [Project Structure](#project-structure)
7. [Troubleshooting](#troubleshooting)
8. [Next Steps](#next-steps)

---

## 📦 Prerequisites

### Required Software

#### 1. Unity Hub & Unity Editor

**Unity Hub:**
- **Download:** https://unity.com/download
- **Purpose:** Manages Unity Editor installations and projects
- **Version:** Latest stable version (2023+)

**Unity Editor:**
- **Required Version:** `2022.3.39f1 LTS` (or compatible 2022.3.x LTS)
- **Download Method:** Install via Unity Hub (see steps below)
- **Why LTS?** Long-Term Support = more stable, better for projects

**Modules to Install with Unity:**
- ✅ **Windows Build Support** (if on Windows - included by default)
- ✅ **Documentation** (recommended)
- ⚠️ **Not Required:** Android, iOS, WebGL (this is a Windows-only project currently)

---

#### 2. Git (Version Control)

**Download:** https://git-scm.com/downloads

**Purpose:** Clone the repository and manage code changes

**Verify Installation:**
```bash
git --version
# Should show: git version 2.x.x
```

---

#### 3. Visual Studio Code or Visual Studio (Recommended)

**Option A: Visual Studio Code (Lighter)**
- Download: https://code.visualstudio.com/
- Install C# extension: `ms-dotnettools.csharp`
- Unity integration works out of the box

**Option B: Visual Studio 2022 Community (Full IDE)**
- Download: https://visualstudio.microsoft.com/
- Select "Game development with Unity" workload during installation
- Better debugging integration with Unity

**Note:** Unity can work with either - choose based on preference!

---

### System Requirements

**Minimum:**
- **OS:** Windows 10 (64-bit)
- **CPU:** Intel/AMD quad-core processor
- **RAM:** 8 GB
- **GPU:** DirectX 11 compatible graphics card
- **Storage:** 5 GB free space

**Recommended:**
- **OS:** Windows 11 (64-bit)
- **CPU:** Intel i5/AMD Ryzen 5 or better
- **RAM:** 16 GB
- **GPU:** NVIDIA GTX 1060 / AMD RX 580 or better
- **Storage:** 10 GB free space (SSD preferred)

---

## 🔧 Installation Steps

### Step 1: Install Unity Hub

1. **Download Unity Hub**
   - Go to: https://unity.com/download
   - Click "Download Unity Hub"
   - Run the installer

2. **Sign In / Create Account**
   - Open Unity Hub
   - Sign in with Unity account (or create one - it's free!)
   - Activate Personal license (free for non-commercial projects)

3. **Verify Installation**
   - Unity Hub should open showing "Projects" and "Installs" tabs

---

### Step 2: Install Unity Editor 2022.3.39f1 LTS

**Via Unity Hub:**

1. Open Unity Hub
2. Click **"Installs"** tab on the left
3. Click **"Install Editor"** button (top right)
4. Select **"Archive"** tab
5. Find **2022.3 (LTS)** section
6. Locate **2022.3.39f1** (or latest 2022.3.x)
7. Click **"Install"**

**Modules to Select:**
- ✅ **Windows Build Support** (should be selected by default)
- ✅ **Documentation**
- ❌ Uncheck: Android, iOS, WebGL, Mac, Linux (not needed)

8. Click **"Continue"** and wait for download/installation (~5-10 minutes)

**Alternative: Using Unity Hub Install Link**
```
unityhub://2022.3.39f1/
```
Copy this into your browser address bar - Unity Hub will open and start installation.

---

### Step 3: Clone the Repository

**Using Command Line (Recommended):**

```bash
# Navigate to where you want the project
cd C:/Users/YourName/Unity Projects/

# Clone the repository
git clone https://github.com/MatthewSnow2/robotech-tbs.git

# Enter the project folder
cd robotech-tbs

# Verify files are present
dir  # (or 'ls' on Mac/Linux)
```

**Using GitHub Desktop (Alternative):**

1. Download GitHub Desktop: https://desktop.github.com/
2. File → Clone Repository
3. Enter: `MatthewSnow2/robotech-tbs`
4. Choose local path
5. Click "Clone"

---

### Step 4: Verify Repository Structure

After cloning, your folder should contain:

```
robotech-tbs/
├── Assets/              ✅ Game content
├── Packages/            ✅ Unity package manager
├── ProjectSettings/     ✅ Unity project settings
├── docs/                ✅ Documentation
├── .gitignore           ✅ Git configuration
├── README.md            ✅ Project overview
├── CLAUDE.md            ✅ Architecture guide
├── SECURITY.md          ✅ Security policy
└── LICENSE              ✅ License file
```

**If you see these folders, you're ready to proceed!**

---

## 🎮 Opening the Project

### First Time Setup

**Important:** Unity will import/compile assets on first open. This takes 2-10 minutes depending on your computer.

1. **Open Unity Hub**

2. **Add Project:**
   - Click **"Projects"** tab
   - Click **"Add"** button (top right)
   - Navigate to your `robotech-tbs` folder
   - Select the folder (not any file inside)
   - Click **"Add Project"**

3. **Open Project:**
   - You should now see "Robotech-tbs" in your projects list
   - **Verify Unity Version** shows `2022.3.39f1` (or your installed 2022.3.x)
   - Click on the project to open

4. **Wait for Import:**
   - Unity Editor will open
   - **Console** will show: "Importing assets..." or "Compiling scripts..."
   - **Progress bar** at bottom right
   - ⏱️ **This is normal!** First import takes 2-10 minutes
   - ☕ Grab a coffee while Unity sets up

5. **Import Complete Signs:**
   - Console shows: "All compiler errors have to be fixed before entering play mode!"
     - ✅ This is actually good - means compilation finished!
   - Progress bar disappears
   - Hierarchy shows scene objects
   - Project window shows Assets folder

---

### Expected Unity Layout

Once open, you should see:

**Top Menu Bar:**
- File, Edit, Assets, GameObject, Component, Window, Help

**Left Panel (Hierarchy):**
- Scene objects when a scene is loaded

**Center (Scene View):**
- Visual representation of your game world
- Gray grid with objects

**Bottom (Project Window):**
- Assets folder structure
- Scripts, Scenes, etc.

**Right (Inspector):**
- Properties of selected objects

**Bottom (Console):**
- Log messages, warnings, errors

---

## ▶️ Running the Game

### Step 1: Load the Bootstrap Scene

**If scene isn't loaded automatically:**

1. In **Project** window (bottom), navigate to:
   ```
   Assets → Scenes → Bootstrap.unity
   ```

2. **Double-click** `Bootstrap.unity`

3. **Hierarchy** should now show:
   - Main Camera
   - Directional Light
   - GameBootstrap (with components)

---

### Step 2: Enter Play Mode

**Method 1: Play Button**
1. Look for **Play button** (▶️ triangle) at top center of Unity
2. Click it
3. Game view should appear and start running

**Method 2: Keyboard Shortcut**
- Press `Ctrl + P` (Windows) or `Cmd + P` (Mac)

---

### Step 3: What You Should See

**Console Messages:**
```
Robotech TBS Bootstrap initialized.
Turn 1 started.
Current phase: Player
```

**Game View:**
- You should see a hex grid (if debug visualization is enabled)
- Blue capsules = RDF units (your faction)
- Red capsules = Zentradi units (AI faction)
- Grid overlay in Scene view

**Scene View:**
- Switch to Scene tab to see debug visualization
- Colored hexagons representing terrain
- Unit capsules with faction colors

---

### Step 4: Basic Interactions

**Mouse Controls:**
- **Left Click:** Select unit
- **Right Click:** (If unit selected) Move to hex or attack

**Keyboard Shortcuts:**
- **B:** Found city (if settler unit is selected)
- **Space:** End turn (if implemented)

**Camera Controls (Scene View):**
- **Middle Mouse:** Pan camera
- **Mouse Wheel:** Zoom in/out
- **Right Mouse + WASD:** Fly camera

---

### Step 5: Stop Play Mode

**When Done Testing:**
1. Click **Play button** again (it should be highlighted blue)
2. Or press `Ctrl + P` / `Cmd + P`

**⚠️ IMPORTANT:** Changes made during Play Mode are **NOT SAVED!**
- Always exit Play Mode before modifying objects
- Unity will revert to pre-Play state when stopped

---

## 🧪 Testing

### Running Unit Tests

**Via Unity Test Runner:**

1. **Open Test Runner:**
   - Menu: `Window → General → Test Runner`
   - Dock the Test Runner window (drag to side panel)

2. **View Tests:**
   - Click **"EditMode"** tab
   - You should see test assemblies/folders
   - Expand to see individual tests

3. **Run All Tests:**
   - Click **"Run All"** button
   - Tests will execute
   - Green ✅ = Passed
   - Red ❌ = Failed

4. **Run Specific Test:**
   - Right-click test name
   - Select "Run"

**Via Command Line (Advanced):**

```bash
# From project root
"C:\Program Files\Unity\Hub\Editor\2022.3.39f1\Editor\Unity.exe" ^
  -runTests ^
  -testPlatform EditMode ^
  -projectPath . ^
  -testResults results.xml ^
  -batchmode ^
  -quit
```

**Expected Results:**
- All tests should pass (green)
- If any fail, check Console for error messages

---

### Manual Testing Checklist

**Basic Functionality:**

- [ ] **Game Starts:** Bootstrap scene loads without errors
- [ ] **Turn System:** Turn counter increments, phase changes
- [ ] **Unit Selection:** Can click and select units
- [ ] **Unit Movement:** Can move units to adjacent hexes
- [ ] **Combat:** Can attack enemy units (if adjacent)
- [ ] **City Founding:** Press B with settler → city appears
- [ ] **Fog of War:** Map reveals around units
- [ ] **Resource System:** Resources displayed/updated

**Performance:**
- [ ] **Frame Rate:** Check FPS in Stats window (Window → Analysis → Stats)
- [ ] **No Errors:** Console shows no red errors during gameplay

**Save/Exit:**
- [ ] **Exit Play Mode:** Can exit without crashes
- [ ] **Console Clear:** No lingering errors after exiting

---

## 📁 Project Structure

Understanding what goes where:

```
Assets/
├── Scenes/
│   └── Bootstrap.unity          # Main game scene - START HERE
│
├── Scripts/
│   ├── Bootstrap/               # Initialization logic
│   │   ├── GameBootstrap.cs     # Main entry point
│   │   ├── DefinitionsFactory.cs
│   │   └── UnitFactory.cs
│   │
│   ├── Core/
│   │   └── TurnManager.cs       # Turn system (Player/AI phases)
│   │
│   ├── Hex/                     # Hex grid math
│   │   ├── HexCoord.cs          # Coordinate system
│   │   ├── HexGrid.cs           # Grid management
│   │   └── HexMath.cs           # Conversions & calculations
│   │
│   ├── Units/
│   │   └── Unit.cs              # Unit entity (HP, armor, movement)
│   │
│   ├── Cities/
│   │   ├── City.cs              # City system
│   │   └── CityManager.cs       # City lifecycle
│   │
│   ├── Systems/                 # Game systems
│   │   ├── ResourceManager.cs   # Protoculture, Materials, etc.
│   │   ├── TechManager.cs       # Technology tree
│   │   └── MapRules.cs
│   │
│   ├── Combat/
│   │   └── CombatResolver.cs    # Attack resolution
│   │
│   ├── Fog/
│   │   └── FogOfWarSystem.cs    # Visibility management
│   │
│   ├── Map/
│   │   └── MapGenerator.cs      # Procedural terrain
│   │
│   ├── Input/
│   │   └── SelectionController.cs # Mouse input handling
│   │
│   ├── Rendering/
│   │   └── HexDebugRenderer.cs  # Visual debugging
│   │
│   ├── Data/                    # ScriptableObject definitions
│   │   ├── UnitDefinition.cs
│   │   ├── WeaponDefinition.cs
│   │   └── TerrainType.cs
│   │
│   └── Debug/
│       └── DevHotkeys.cs        # Development shortcuts
│
└── Tests/
    └── EditMode/                # Unit tests
```

---

## 🔧 Troubleshooting

### Common Issues & Solutions

---

#### ❌ "Unity Version Mismatch" Warning

**Problem:** Project shows yellow triangle, "Open with Unity 2022.3.39f1"

**Solution:**
1. Install correct Unity version (2022.3.39f1 LTS)
2. Or: Right-click project in Unity Hub → "Change Editor Version" → Select installed version
3. Accept upgrade if using newer 2022.3.x version

**Note:** Minor version differences (e.g., 2022.3.39 vs 2022.3.45) usually work fine.

---

#### ❌ "Compiler Errors" on First Open

**Problem:** Console shows red errors about missing scripts

**Solution:**
1. **Wait** - Unity is still importing
2. Check progress bar at bottom right
3. Once import completes, click "Clear" in Console
4. Menu: `Assets → Reimport All`
5. Wait for re-import (2-5 minutes)

**Still Broken?**
- Delete `Library/` folder (safe - Unity regenerates it)
- Reopen project in Unity Hub

---

#### ❌ Scene Appears Empty / Gray

**Problem:** Nothing visible when entering Play Mode

**Solution:**
1. Check **Scene View** instead of **Game View**
2. Click Scene tab at top of center panel
3. Debug visualization uses Gizmos (only visible in Scene view)
4. Or: Switch to Game view and check camera position

**Verify:**
- Main Camera exists in Hierarchy
- GameBootstrap object exists
- Console shows "Bootstrap initialized"

---

#### ❌ "Cannot Find MonoBehaviour Script" Errors

**Problem:** Components show "Script missing" icon

**Causes:**
1. Script file was renamed/moved
2. Class name doesn't match filename
3. Script is in wrong folder

**Solution:**
1. Find the script in Project window
2. Check filename matches class name exactly
   - File: `TurnManager.cs`
   - Class: `public class TurnManager`
3. Ensure script is inside `Assets/Scripts/`
4. Right-click script → Reimport

---

#### ❌ Play Button is Grayed Out

**Problem:** Cannot enter Play Mode

**Solution:**
1. Check Console for **compiler errors** (red messages)
2. Fix all errors before playing
3. Menu: `Edit → Project Settings → Editor`
4. Verify "Enter Play Mode Options" settings

---

#### ❌ Units Don't Respond to Clicks

**Problem:** Clicking units does nothing

**Checks:**
1. **EventSystem exists:** Check Hierarchy for "EventSystem" object
2. **Camera has Physics Raycaster:** Select Main Camera → Inspector → Physics Raycaster component
3. **Units have Colliders:** Select unit → Inspector → should have Box/Capsule Collider
4. **Console errors:** Check for input-related errors

---

#### ❌ Very Low Frame Rate (< 30 FPS)

**Problem:** Game runs slowly/laggy

**Solutions:**
1. **Lower Quality:** Edit → Project Settings → Quality → Set to "Low"
2. **Disable VSync:** Project Settings → Quality → VSync Count → "Don't Sync"
3. **Check GPU:** Stats window (Window → Analysis → Stats) shows GPU usage
4. **Reduce grid size:** Edit map generation parameters

---

#### ❌ Git Issues (Can't Pull/Push)

**Problem:** Git commands fail or show conflicts

**Solution:**
1. **Check current branch:**
   ```bash
   git branch
   # Should show: * main
   ```

2. **Discard local changes to meta files:**
   ```bash
   git checkout -- *.meta
   ```

3. **Pull latest:**
   ```bash
   git pull origin main
   ```

4. **If conflicts:** Resolve in VS Code or Unity's Version Control window

---

#### ❌ Unity Hub Can't Find Project

**Problem:** Project doesn't appear in Unity Hub projects list

**Solution:**
1. Click "Add" → Select `robotech-tbs` folder
2. Verify folder contains `ProjectSettings/` directory
3. If still not showing: Create new project, then replace files

---

## 🎯 Next Steps

### Once Game is Running Successfully

**Familiarize Yourself:**
1. **Read CLAUDE.md** - Understand architecture
2. **Explore Scripts** - Open `GameBootstrap.cs` and read comments
3. **Review Scenes** - Study Bootstrap scene hierarchy
4. **Check Documentation** - Read GDD.md in `docs/` folder

**Start Modifying:**
1. **Adjust Balance:** Edit values in `DefinitionsFactory.cs`
   - Unit HP, movement range, weapon damage
2. **Change Map Size:** Modify `MapGenerator.cs` grid dimensions
3. **Add Debug Logs:** Insert `Debug.Log("message")` to understand flow
4. **Experiment in Play Mode:** Try different unit compositions

**Development Tools:**
1. **Unity Profiler:** Window → Analysis → Profiler (check performance)
2. **Frame Debugger:** Window → Analysis → Frame Debugger (visual debugging)
3. **Console Filters:** Click icons in Console to filter warnings/errors
4. **Scene View Gizmos:** Toggle Gizmos button to show/hide debug visualization

---

### Useful Resources

**Unity Learning:**
- Unity Learn: https://learn.unity.com/
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/

**Project-Specific:**
- **README.md** - Project overview and goals
- **CLAUDE.md** - Architecture and system details
- **GDD.md** (in docs/) - Game design document
- **SECURITY.md** - Security policy and best practices

**Community:**
- Unity Forums: https://forum.unity.com/
- Unity Discord: https://discord.gg/unity
- Stack Overflow: https://stackoverflow.com/questions/tagged/unity3d

---

## 📝 Development Workflow

**Recommended Process:**

1. **Pull Latest Changes:**
   ```bash
   git pull origin main
   ```

2. **Open in Unity Hub**

3. **Make Changes:**
   - Edit scripts in Visual Studio Code / Visual Studio
   - Modify prefabs/scenes in Unity Editor
   - Test frequently in Play Mode

4. **Test:**
   - Run unit tests (Test Runner)
   - Manual testing in Play Mode
   - Check Console for errors

5. **Commit Changes:**
   ```bash
   git add .
   git commit -m "feat: description of changes"
   git push origin main
   ```

6. **Document:**
   - Update CLAUDE.md if architecture changes
   - Add comments to complex code
   - Update GDD.md for design changes

---

## ✅ Installation Checklist

Before reporting issues, verify:

**Software:**
- [ ] Unity Hub installed
- [ ] Unity 2022.3.39f1 LTS installed
- [ ] Git installed and working
- [ ] Visual Studio Code / Visual Studio installed

**Project:**
- [ ] Repository cloned successfully
- [ ] All folders present (Assets, ProjectSettings, Packages)
- [ ] Project opens in Unity Hub
- [ ] No red compiler errors after import

**Functionality:**
- [ ] Bootstrap scene loads
- [ ] Play Mode starts without errors
- [ ] Console shows "Bootstrap initialized"
- [ ] Units visible in Scene view
- [ ] Can exit Play Mode cleanly

---

## 🆘 Getting Help

**If You're Stuck:**

1. **Check Console:** Look for specific error messages (copy full text)
2. **Check This Guide:** Search for your issue in Troubleshooting section
3. **Check CLAUDE.md:** Understand how systems work
4. **Search Unity Forums:** Your issue may have been solved before
5. **Create GitHub Issue:**
   - Go to: https://github.com/MatthewSnow2/robotech-tbs/issues
   - Click "New Issue"
   - Describe problem with screenshots/error messages

**Information to Include:**
- Unity version
- Operating system
- Error messages (from Console)
- Steps to reproduce
- Screenshots if visual issue

---

## 🎉 Success!

If you can:
- ✅ Open the project in Unity
- ✅ See Bootstrap scene
- ✅ Enter Play Mode
- ✅ See units and terrain
- ✅ Console shows "Bootstrap initialized"

**You're ready to start developing!** 🚀

Welcome to Robotech: Macross Era development! Check out the GDD.md in the `docs/` folder to see the vision for this game.

---

**Happy Coding! 🎮**

---

**Document Version:** 1.0
**Last Updated:** 2025-10-22
**Maintainer:** MatthewSnow2
