# Robotech TBS - Development Guide

**Last Updated:** 2025-10-23
**For:** Contributors, AI Assistants, Future You

---

## 🎯 Quick Start

### First Time Setup

1. **Clone Repository**
```bash
git clone https://github.com/MatthewSnow2/Robotech-tbs.git
cd Robotech-tbs
```

2. **Open in Unity**
   - Unity Hub → Add → Select project folder
   - Unity Version: **2022.3.39f1** (LTS)
   - Wait for import to complete

3. **Load Bootstrap Scene**
   - Open: `Assets/Scenes/Bootstrap.unity`
   - Press Play
   - Console should show: "Robotech TBS Bootstrap initialized."

4. **Verify Setup**
   - Hex grid visible in Scene view
   - Units spawned (blue RDF, red Zentradi)
   - No errors in Console

---

## 💻 Development Workflow

### Daily Workflow

```
1. Pull latest changes
   git pull origin main

2. Create feature branch (optional)
   git checkout -b feature/my-feature

3. Open Unity, make changes

4. Test in Play mode

5. Commit frequently
   git add .
   git commit -m "feat: description"

6. Push to remote
   git push origin main
```

### Testing Changes

**Quick Test:**
- Press Play in Unity
- Check Console for errors
- Verify core functionality

**Full Test:**
- Run EditMode tests: Window → General → Test Runner
- Play a full turn cycle
- Test new features manually

---

## 📝 Coding Standards

### C# Style Guide

**Naming Conventions:**
```csharp
// Classes, Methods, Properties: PascalCase
public class UnitManager
{
    public int MaxHealth { get; private set; }

    public void MoveUnit() { }
}

// Fields (private): camelCase
private int currentHealth;
private List<Unit> units;

// Constants: PascalCase
private const int MaxUnits = 100;

// Events: PascalCase with "On" prefix
public static event Action<Unit> OnUnitSpawned;
```

**File Organization:**
```csharp
// 1. Using statements
using UnityEngine;
using System.Collections.Generic;

// 2. Namespace
namespace Robotech.TBS.Systems
{
    // 3. XML documentation
    /// <summary>
    /// Manages unit lifecycle and state.
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        // 4. Serialized fields
        [SerializeField] private int maxUnits = 50;

        // 5. Public properties
        public int UnitCount => units.Count;

        // 6. Private fields
        private List<Unit> units;

        // 7. Events
        public static event Action<Unit> OnUnitDestroyed;

        // 8. Unity methods
        void Awake() { }
        void Start() { }
        void Update() { }

        // 9. Public methods
        public void AddUnit(Unit unit) { }

        // 10. Private methods
        private void RemoveDeadUnits() { }
    }
}
```

### XML Documentation

**Always document public APIs:**
```csharp
/// <summary>
/// Moves a unit to the specified hex coordinate.
/// </summary>
/// <param name="unit">The unit to move.</param>
/// <param name="destination">Target hex coordinate.</param>
/// <returns>True if movement succeeded, false if blocked.</returns>
public bool MoveUnit(Unit unit, HexCoord destination)
{
    // Implementation
}
```

### Event Pattern

**Subscribing:**
```csharp
void OnEnable()
{
    TurnManager.OnTurnStarted += HandleTurnStart;
}

void OnDisable()
{
    TurnManager.OnTurnStarted -= HandleTurnStart;  // ALWAYS unsubscribe!
}

private void HandleTurnStart(int turnNumber)
{
    // Handle event
}
```

---

## 🏗️ Adding New Features

### Adding a New Unit Type

**1. Create Definition (via Factory)**
```csharp
// In GameBootstrap or DefinitionsFactory
var newUnit = DefinitionsFactory.CreateUnit(
    name: "Super Veritech",
    maxHP: 150,
    armor: 50,
    movementPoints: 6,
    visionRadius: 4,
    type: UnitType.Air
);
```

**2. Add Weapons**
```csharp
newUnit.weapons.Add(heavyMissileSalvo);
newUnit.weapons.Add(headLasers);
```

**3. Spawn in Game**
```csharp
var unit = UnitFactory.SpawnUnit(
    newUnit,
    new HexCoord(5, 5),
    Faction.RDF
);
```

### Adding a New System

**1. Create Script**
```csharp
// Assets/Scripts/Systems/NewSystem.cs
namespace Robotech.TBS.Systems
{
    public class NewSystem : MonoBehaviour
    {
        // System logic here
    }
}
```

**2. Add to Bootstrap**
```csharp
// In GameBootstrap.Awake()
if (FindObjectOfType<NewSystem>() == null)
{
    var go = new GameObject("NewSystem");
    go.AddComponent<NewSystem>();
}
```

**3. Subscribe to Events (if needed)**
```csharp
void OnEnable()
{
    TurnManager.OnTurnStarted += HandleTurn;
}
```

### Adding a New ScriptableObject Type

**1. Create Definition Class**
```csharp
// Assets/Scripts/Data/AbilityDefinition.cs
[CreateAssetMenu(fileName = "NewAbility", menuName = "Robotech/Ability")]
public class AbilityDefinition : ScriptableObject
{
    public string abilityName;
    public int cooldown;
    public int range;
    // ... more fields
}
```

**2. Create Factory Method**
```csharp
// In DefinitionsFactory
public static AbilityDefinition CreateAbility(...)
{
    var ability = ScriptableObject.CreateInstance<AbilityDefinition>();
    // Set fields
    return ability;
}
```

---

## 🐛 Debugging Tips

### Common Issues

**Units not moving:**
- Check MovementPoints > 0
- Verify hex is reachable (pathfinding)
- Check fog of war (can see destination?)

**Events not firing:**
- Did you subscribe in OnEnable?
- Did you unsubscribe in OnDisable?
- Check for null before invoke: `OnEvent?.Invoke()`

**Grid alignment wrong:**
- Verify HexMath.HexToWorld() calculation
- Check HexSize constant
- Ensure pointy-top orientation

### Debug Shortcuts (DevHotkeys.cs)

```
B - Found city (when settler selected)
N - Next turn (skip to next turn)
F3 - Toggle fog of war visualization
```

### Unity Console

**Useful Debug Logs:**
```csharp
Debug.Log($"Unit at {hex.q},{hex.r} has {unit.CurrentHP} HP");
Debug.LogWarning("Low Protoculture!");
Debug.LogError("Invalid hex coordinate!");
```

**Conditional Compilation:**
```csharp
#if UNITY_EDITOR
    Debug.Log("Editor-only debug info");
#endif
```

---

## 🧪 Testing Guidelines

### Writing Tests

**EditMode Test Example:**
```csharp
using NUnit.Framework;

[TestFixture]
public class HexCoordTests
{
    [Test]
    public void Distance_ShouldCalculateCorrectly()
    {
        var a = new HexCoord(0, 0);
        var b = new HexCoord(3, 2);
        Assert.AreEqual(5, a.Distance(b));
    }

    [Test]
    public void Neighbors_ShouldReturnSixHexes()
    {
        var hex = new HexCoord(0, 0);
        var neighbors = hex.GetNeighbors();
        Assert.AreEqual(6, neighbors.Count);
    }
}
```

**Run Tests:**
- Window → General → Test Runner
- Select EditMode
- Click "Run All"

---

## 📊 Performance Best Practices

### Do's ✅

```csharp
// Cache component references
private Renderer unitRenderer;
void Awake()
{
    unitRenderer = GetComponent<Renderer>();
}

// Use static events (no allocations)
public static event Action<Unit> OnUnitSpawned;

// Precalculate expensive operations
private Dictionary<HexCoord, List<HexCoord>> neighborCache;

// Use object pooling for frequent spawns (future)
```

### Don'ts ❌

```csharp
// Don't use GetComponent in Update()
void Update()
{
    GetComponent<Renderer>().color = Color.red;  // BAD!
}

// Don't allocate in Update()
void Update()
{
    var list = new List<int>();  // BAD!
}

// Don't use string comparisons
if (unit.name == "Veritech") { }  // Use enums or references instead
```

---

## 🔄 Git Workflow

### Commit Message Format

```
<type>: <short summary>

[optional body]

[optional footer]
```

**Types:**
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation only
- `refactor:` Code restructuring
- `test:` Adding tests
- `chore:` Maintenance

**Examples:**
```
feat: add missile salvo ability for Veritechs

fix: correct hex distance calculation for range 6+

docs: update ARCHITECTURE.md with new combat system

refactor: extract combat logic into CombatResolver
```

### Branch Strategy (Optional)

```
main - Stable, tested code
dev - Active development
feature/* - New features
bugfix/* - Bug fixes
```

---

## 📚 Useful References

### Unity Documentation
- [Hex Grids (Red Blob Games)](https://www.redblobgames.com/grids/hexagons/)
- [Unity Scripting API](https://docs.unity3d.com/ScriptReference/)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

### Project Documents
- `PROJECT_VISION.md` - Overall goals and philosophy
- `ARCHITECTURE.md` - Technical structure and patterns
- `GDD.md` - Game design specifics
- `CLAUDE.md` - AI assistant guidance
- `README.md` - Project overview

### External Tools
- Unity Hub for version management
- Visual Studio or VS Code for C# editing
- Git for version control
- GitHub for collaboration

---

## 🚀 Common Tasks

### Spawn a Test Unit

```csharp
var unit = UnitFactory.SpawnUnit(
    vf1aDefinition,
    new HexCoord(10, 10),
    Faction.RDF
);
```

### Check if Hex is Valid

```csharp
if (hexGrid.IsInBounds(coord) && mapRules.IsValidForUnit(coord, unit))
{
    // Safe to move
}
```

### Calculate Hex Range

```csharp
var hexesInRange = hexGrid.GetHexesInRange(centerHex, radius: 3);
foreach (var hex in hexesInRange)
{
    // Process each hex
}
```

### Subscribe to Turn Events

```csharp
void OnEnable()
{
    TurnManager.OnTurnStarted += turn =>
    {
        Debug.Log($"Turn {turn} started!");
    };
}
```

---

## ⚠️ Common Pitfalls

**1. Forgetting to Unsubscribe from Events**
```csharp
// BAD - causes memory leak
void OnEnable()
{
    TurnManager.OnTurnStarted += HandleTurn;
}
// Missing OnDisable!

// GOOD
void OnDisable()
{
    TurnManager.OnTurnStarted -= HandleTurn;
}
```

**2. Modifying Collections During Iteration**
```csharp
// BAD
foreach (var unit in units)
{
    if (unit.IsDead)
        units.Remove(unit);  // Throws exception!
}

// GOOD
units.RemoveAll(u => u.IsDead);
```

**3. Not Checking Bounds**
```csharp
// BAD
var terrain = hexGrid.GetTerrainAt(coord);  // May be null!

// GOOD
if (hexGrid.IsInBounds(coord))
{
    var terrain = hexGrid.GetTerrainAt(coord);
}
```

---

## 🎓 Learning Path

**Week 1: Understanding the Codebase**
1. Read PROJECT_VISION.md
2. Read ARCHITECTURE.md
3. Open Bootstrap scene and explore
4. Trace code flow from GameBootstrap to TurnManager

**Week 2: Making Small Changes**
1. Adjust unit stats in DefinitionsFactory
2. Add debug logs to track turn flow
3. Modify hex rendering colors
4. Write a simple test

**Week 3: Adding Features**
1. Add a new unit type
2. Implement a basic ability
3. Add a new terrain type
4. Create a UI element

**Week 4+: Major Contributions**
1. Implement AI decision-making
2. Add complex combat mechanics
3. Build production queue system
4. Design new game systems

---

## 🤝 Getting Help

**Stuck on something?**

1. Check this guide
2. Read ARCHITECTURE.md for system details
3. Check CLAUDE.md for AI-specific guidance
4. Review existing code for patterns
5. Add debug logs to trace execution
6. Open an issue on GitHub

**Found a bug?**
1. Document steps to reproduce
2. Check Console for errors
3. Create minimal test case
4. Submit bug report

---

**Happy coding! Build something amazing! 🚀**
