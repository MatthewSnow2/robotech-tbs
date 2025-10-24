# Robotech TBS - Technical Architecture

**Last Updated:** 2025-10-23
**Unity Version:** 2022.3.39f1 LTS
**Target Platform:** Windows (Standalone)

---

## 📐 Architecture Overview

### Design Principles

1. **Data-Driven Design**
   - Game balance and content in ScriptableObjects
   - Code defines behavior, data defines content
   - Easy iteration without recompilation

2. **Event-Driven Communication**
   - Loosely coupled systems via static events
   - No direct dependencies between high-level systems
   - Easy to add/remove features

3. **Separation of Concerns**
   - **Data Layer:** ScriptableObject definitions
   - **System Layer:** MonoBehaviour game logic
   - **View Layer:** Rendering and UI

4. **Pragmatic Unity Patterns**
   - Not over-engineered (no DI containers, no ECS for now)
   - Standard MonoBehaviours with clear responsibilities
   - Factories for complex initialization
   - Static utilities for stateless operations

---

## 🏗️ System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      GAME BOOTSTRAP                         │
│  (Initialization, Scene Setup, Factory Creation)            │
└──────────────────────┬──────────────────────────────────────┘
                       │
        ┌──────────────┴───────────────┐
        │                              │
┌───────▼────────┐            ┌────────▼────────┐
│  CORE SYSTEMS  │            │  DATA LAYER     │
│                │            │                 │
│ • TurnManager  │◄──────────►│ • UnitDef       │
│ • ResourceMgr  │            │ • WeaponDef     │
│ • TechManager  │            │ • TerrainType   │
│ • CityManager  │            │ • TechDef       │
│ • MapRules     │            │ • DistrictDef   │
└───────┬────────┘            └─────────────────┘
        │
        │
┌───────▼────────────────────────────────────────────┐
│              GAMEPLAY SYSTEMS                       │
│                                                     │
│  ┌───────────┐  ┌────────────┐  ┌──────────────┐  │
│  │  HexGrid  │  │   Units    │  │   Cities     │  │
│  │  • Coords │  │  • Movement│  │  • Territory │  │
│  │  • Path   │  │  • Combat  │  │  • Districts │  │
│  │  • Range  │  │  • Vision  │  │  • Production│  │
│  └───────────┘  └────────────┘  └──────────────┘  │
│                                                     │
│  ┌───────────┐  ┌────────────┐  ┌──────────────┐  │
│  │  Fog of   │  │  Combat    │  │  Map Gen     │  │
│  │  War      │  │  Resolver  │  │  • Terrain   │  │
│  │  • Reveal │  │  • Damage  │  │  • Resources │  │
│  │  • Update │  │  • Accuracy│  │  • Features  │  │
│  └───────────┘  └────────────┘  └──────────────┘  │
└───────┬────────────────────────────────────────────┘
        │
        │
┌───────▼────────────────────────────────────────────┐
│           INPUT & RENDERING                         │
│                                                     │
│  ┌──────────────┐  ┌───────────────┐  ┌─────────┐ │
│  │  Selection   │  │  Hex Debug    │  │  UI     │ │
│  │  Controller  │  │  Renderer     │  │  Shell  │ │
│  └──────────────┘  └───────────────┘  └─────────┘ │
└─────────────────────────────────────────────────────┘
```

---

## 📦 Layer Definitions

### 1. Bootstrap Layer
**Purpose:** Initialize all systems and load initial game state

**Components:**
- `GameBootstrap.cs` - Main initialization coordinator
- `DefinitionsFactory.cs` - Creates ScriptableObject definitions at runtime
- `UnitFactory.cs` - Spawns units with proper configuration

**Initialization Sequence:**
1. Create core system objects (if missing)
2. Generate terrain definitions
3. Generate map
4. Create weapon/unit definitions
5. Spawn starting units
6. Initialize fog of war
7. Setup tech tree
8. Fire initial turn events

**Pattern:** Initialization happens in `Awake()` to guarantee order before any `Start()` calls.

### 2. Data Layer (ScriptableObjects)
**Purpose:** Define game content and balance without code changes

**Key Types:**

```csharp
// Units and Combat
public class UnitDefinition : ScriptableObject
{
    public string unitName;
    public int maxHP, armor, baseDamage;
    public int movementPoints, visionRadius;
    public UnitType type; // Ground, Air, Naval
    public List<WeaponDefinition> weapons;
    public List<UnitAbility> abilities;
}

public class WeaponDefinition : ScriptableObject
{
    public string weaponName;
    public DamageType damageType; // Kinetic, Energy, Missile
    public int salvoCount, baseDamage;
    public int minRange, maxRange;
    public float baseAccuracy;
}

// Map and Terrain
public class TerrainType : ScriptableObject
{
    public string terrainName;
    public int movementCost, defenseBonus;
    public bool isWater, isImpassable, isElevated;
    public ResourceYield baseYields;
}

// Economy and Progress
public class TechDefinition : ScriptableObject
{
    public string techId, displayName;
    public int costScience;
    public TechGeneration generation;          // Gen0-Gen5
    public TechCategory category;              // Power, Mecha, Weapons, Defense, Aerospace
    public List<TechDefinition> prerequisites;

    // Unlocks
    public List<UnitDefinition> unlocksUnits;
    public List<DistrictDefinition> unlocksDistricts;
    public List<string> unlocksAbilities;      // e.g., "transformation_mode"

    // Effects (global bonuses when researched)
    public int bonusProtoculturePerTurn, bonusScience, bonusProduction;
    public int bonusUnitHP, bonusUnitArmor, bonusUnitMovement;
    public float bonusAttackDamagePercent, bonusAccuracyPercent;

    // Flags
    public bool isCriticalPath;                // Required for progression
    public bool allowsEraTransition;           // Unlocks next generation
}

public class DistrictDefinition : ScriptableObject
{
    public string districtName;
    public DistrictType type; // Factory, Lab, Outpost
    public ResourceYield yields;
    public int constructionCost;
}
```

**Why ScriptableObjects?**
- Unity Inspector for rapid iteration
- Serialized data (saveable, editable)
- Asset-based (can be shared across scenes)
- Type-safe references (no string lookups)

### 3. System Layer (MonoBehaviours)
**Purpose:** Core game logic and state management

#### Core Systems

**TurnManager** - Turn and phase progression
```csharp
public class TurnManager : MonoBehaviour
{
    public int TurnNumber { get; private set; }
    public TurnPhase CurrentPhase { get; private set; }

    // Events
    public static event Action<int> OnTurnStarted;
    public static event Action<TurnPhase> OnPhaseChanged;

    // Phase control
    public void EndPhase() // Player → AI → Next Turn
    public void ResetTurnManager() // For match restart
}
```

**ResourceManager** - Economy tracking
```csharp
public class ResourceManager : MonoBehaviour
{
    public int Protoculture, Materials, Credits, Science;

    // Events
    public static event Action<ResourceType, int> OnResourceChanged;

    public void ApplyIncome(ResourceYield yields)
    public void DeductCost(ResourceCost cost)
    public bool CanAfford(ResourceCost cost)
}
```

**TechManager** - Technology progression and tech tree
```csharp
public class TechManager : MonoBehaviour
{
    public List<TechDefinition> allTechs;           // Full tech tree
    public List<TechDefinition> availableTechs;     // Currently researchable
    public List<TechDefinition> researchedTechs;    // Already completed
    public TechDefinition currentResearch;
    public int scienceProgress;
    public TechGeneration currentGeneration;        // Gen0-Gen5

    public void SetResearch(TechDefinition tech)    // Begin researching
    public void AddScience(int amount)              // Accumulate science toward current tech
    public bool IsTechAvailable(TechDefinition tech) // Check prerequisites

    // Tech tree follows Gen 0 → Gen 5 progression (see TECH_TREE.md)
    // Prerequisites create branching paths (Air vs Ground vs Tech Rush)
    // Era transitions gated by critical path techs
}
```
**Note:** See `TECH_TREE.md` for complete tech tree design (60+ technologies across 6 generations).

**CityManager** - City lifecycle and territory
```csharp
public class CityManager : MonoBehaviour
{
    private List<City> allCities = new List<City>();

    public City FoundCity(HexCoord position, Faction faction)
    public List<HexCoord> GetCityTerritory(City city)
    public City GetCityAt(HexCoord hex)
}
```

#### Gameplay Systems

**HexGrid** - Grid management and pathfinding
```csharp
public class HexGrid : MonoBehaviour
{
    public int Width, Height;
    private Dictionary<HexCoord, TerrainType> terrain;

    public bool IsInBounds(HexCoord coord)
    public TerrainType GetTerrainAt(HexCoord coord)
    public List<HexCoord> GetNeighbors(HexCoord coord)
    public int GetDistance(HexCoord a, HexCoord b)
    public List<HexCoord> GetHexesInRange(HexCoord center, int range)
    public List<HexCoord> FindPath(HexCoord start, HexCoord goal)
}
```

**Unit** - Unit entity and state
```csharp
public class Unit : MonoBehaviour
{
    public UnitDefinition Definition { get; private set; }
    public HexCoord Position { get; private set; }
    public Faction Faction { get; private set; }
    public int CurrentHP, CurrentMP;

    // Events
    public static event Action<Unit> OnUnitSpawned;
    public static event Action<Unit, HexCoord> OnUnitMoved;
    public static event Action<Unit> OnUnitDied;

    public void MoveTo(HexCoord destination)
    public void TakeDamage(int damage)
    public void RestoreMP()
}
```

**City** - City entity and production
```csharp
public class City : MonoBehaviour
{
    public HexCoord Position { get; private set; }
    public Faction Faction { get; private set; }
    public List<District> districts;
    public ProductionQueue productionQueue;

    public ResourceYield CalculateTotalYields()
    public void AddToProductionQueue(UnitDefinition unit)
    public void ProcessProduction()
    public void FoundDistrict(DistrictDefinition districtDef, HexCoord hex)
}
```

**FogOfWarSystem** - Visibility management
```csharp
public class FogOfWarSystem : MonoBehaviour
{
    private HashSet<HexCoord> seenHexes;
    private HashSet<HexCoord> visibleHexes;

    public void RevealFrom(HexCoord center, int radius)
    public void UpdateVisibility() // Called each turn
    public bool IsVisible(HexCoord hex)
    public bool HasBeenSeen(HexCoord hex)
}
```

**CombatResolver** - Combat calculations
```csharp
public static class CombatResolver
{
    public static CombatResult ResolveAttack(Unit attacker, Unit target)
    {
        // Calculate damage based on:
        // - Weapon stats (damage, accuracy, salvo count)
        // - Target armor
        // - Terrain defense bonus
        // - Range modifiers

        return new CombatResult
        {
            damageDealt,
            targetKilled,
            accuracyRoll
        };
    }
}
```

### 4. Input/View Layer
**Purpose:** User interaction and visualization

**SelectionController** - Mouse input and unit selection
```csharp
public class SelectionController : MonoBehaviour
{
    public Unit SelectedUnit { get; private set; }

    // Events
    public static event Action<Unit> OnUnitSelected;

    void Update()
    {
        // Raycast for hex selection
        // Calculate reachable/attackable hexes
        // Handle click actions
    }
}
```

**HexDebugRenderer** - Gizmo-based visualization
```csharp
public class HexDebugRenderer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Draw hex grid outlines
        // Draw reachable hexes (green)
        // Draw attackable hexes (red)
        // Draw city territories
    }
}
```

**UIShell** - Main UI coordinator (stub)
```csharp
public class UIShell : MonoBehaviour
{
    // Future: Resource display
    // Future: Unit action panel
    // Future: Production queue
    // Future: Tech tree panel
}
```

---

## 🔄 Event-Driven Architecture

### Why Events?

**Decoupling:** Systems don't need to know about each other
```csharp
// Bad: Direct coupling
cityManager.OnTurnStart(turnNumber); // TurnManager knows about CityManager
resourceManager.OnTurnStart(turnNumber); // TurnManager knows about ResourceManager

// Good: Event-based
TurnManager.OnTurnStarted?.Invoke(turnNumber); // TurnManager doesn't know who listens
```

**Flexibility:** Easy to add/remove features
```csharp
// New system can subscribe without modifying existing code
void OnEnable()
{
    TurnManager.OnTurnStarted += HandleNewTurn;
}
```

### Key Events

```csharp
// Turn Management
TurnManager.OnTurnStarted(int turnNumber)
TurnManager.OnPhaseChanged(TurnPhase phase)

// Unit Lifecycle
Unit.OnUnitSpawned(Unit unit)
Unit.OnUnitMoved(Unit unit, HexCoord newPosition)
Unit.OnUnitDied(Unit unit)

// Selection
SelectionController.OnUnitSelected(Unit unit)

// Resources
ResourceManager.OnResourceChanged(ResourceType type, int newValue)

// Technology
TechManager.OnTechCompleted(TechDefinition tech)
```

### Event Subscription Pattern

```csharp
public class ExampleSystem : MonoBehaviour
{
    void OnEnable()
    {
        // Subscribe when enabled
        TurnManager.OnTurnStarted += HandleTurnStarted;
        Unit.OnUnitDied += HandleUnitDeath;
    }

    void OnDisable()
    {
        // ALWAYS unsubscribe to prevent memory leaks
        TurnManager.OnTurnStarted -= HandleTurnStarted;
        Unit.OnUnitDied -= HandleUnitDeath;
    }

    private void HandleTurnStarted(int turnNumber)
    {
        // Respond to turn start
    }

    private void HandleUnitDeath(Unit deadUnit)
    {
        // Respond to unit death
    }
}
```

---

## 🧮 Hex Coordinate System

### Axial Coordinates

We use **axial coordinates** (q, r) with **pointy-top** orientation.

```
Axial (q, r):
    q = column (horizontal axis, shifts right)
    r = row (diagonal axis, shifts down-right)
    s = -q - r (derived for cube calculations)

Visual Reference:
         r
        ╱
       ╱
      ╱____  q
```

### HexCoord Struct

```csharp
public struct HexCoord : IEquatable<HexCoord>
{
    public int q, r;

    // Cube coordinate (for distance calculations)
    public int S => -q - r;

    // Distance (Manhattan distance in cube space)
    public int Distance(HexCoord other)
    {
        return (Math.Abs(q - other.q) +
                Math.Abs(r - other.r) +
                Math.Abs(S - other.S)) / 2;
    }

    // Six neighbors (clockwise from top)
    public static readonly HexCoord[] Directions = new[]
    {
        new HexCoord(0, -1),  // N
        new HexCoord(1, -1),  // NE
        new HexCoord(1, 0),   // SE
        new HexCoord(0, 1),   // S
        new HexCoord(-1, 1),  // SW
        new HexCoord(-1, 0),  // NW
    };

    public List<HexCoord> GetNeighbors()
    {
        return Directions.Select(d => this + d).ToList();
    }
}
```

### World Position Conversion

```csharp
public static class HexMath
{
    public const float HexSize = 1f;
    public const float HexWidth = HexSize * 2f;
    public const float HexHeight = Mathf.Sqrt(3f) * HexSize;

    public static Vector3 HexToWorld(HexCoord hex)
    {
        float x = HexSize * (3f/2f * hex.q);
        float z = HexSize * (Mathf.Sqrt(3f)/2f * hex.q + Mathf.Sqrt(3f) * hex.r);
        return new Vector3(x, 0, z);
    }

    public static HexCoord WorldToHex(Vector3 worldPos)
    {
        float q = (2f/3f * worldPos.x) / HexSize;
        float r = (-1f/3f * worldPos.x + Mathf.Sqrt(3f)/3f * worldPos.z) / HexSize;
        return RoundToHex(q, r);
    }

    private static HexCoord RoundToHex(float q, float r)
    {
        // Cube rounding algorithm for accurate snapping
        float s = -q - r;
        int rq = Mathf.RoundToInt(q);
        int rr = Mathf.RoundToInt(r);
        int rs = Mathf.RoundToInt(s);

        float dq = Mathf.Abs(rq - q);
        float dr = Mathf.Abs(rr - r);
        float ds = Mathf.Abs(rs - s);

        if (dq > dr && dq > ds)
            rq = -rr - rs;
        else if (dr > ds)
            rr = -rq - rs;

        return new HexCoord(rq, rr);
    }
}
```

---

## 🏭 Factory Pattern

### Why Factories?

Complex objects (especially with ScriptableObject dependencies) need careful initialization. Factories centralize this logic.

### DefinitionsFactory

Creates ScriptableObjects at runtime during prototyping.

```csharp
public class DefinitionsFactory : MonoBehaviour
{
    public static TerrainType CreateTerrain(
        string name,
        int movementCost,
        int defenseBonus,
        bool isWater = false)
    {
        var terrain = ScriptableObject.CreateInstance<TerrainType>();
        terrain.terrainName = name;
        terrain.movementCost = movementCost;
        terrain.defenseBonus = defenseBonus;
        terrain.isWater = isWater;
        // ... more initialization
        return terrain;
    }

    // Similar methods for WeaponDefinition, UnitDefinition, etc.
}
```

**Usage:**
```csharp
var plains = DefinitionsFactory.CreateTerrain("Plains", 1, 0);
var forest = DefinitionsFactory.CreateTerrain("Forest", 2, 2);
```

### UnitFactory

Spawns unit GameObjects with proper component setup.

```csharp
public class UnitFactory : MonoBehaviour
{
    public static Unit SpawnUnit(
        UnitDefinition definition,
        HexCoord position,
        Faction faction,
        Transform parent = null)
    {
        // Create GameObject
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = $"{definition.unitName} ({faction})";

        // Add Unit component
        var unit = go.AddComponent<Unit>();
        unit.Initialize(definition, position, faction);

        // Position in world
        go.transform.position = HexMath.HexToWorld(position);

        // Faction color
        var renderer = go.GetComponent<Renderer>();
        renderer.material.color = faction == Faction.RDF ? Color.blue : Color.red;

        // Fire event
        Unit.OnUnitSpawned?.Invoke(unit);

        return unit;
    }
}
```

---

## 🎮 Game Loop Flow

### Turn Cycle

```
┌─────────────────────┐
│   PLAYER PHASE      │
│                     │
│  • UI Active        │
│  • Units selectable │
│  • Can move/attack  │
│  • Can build cities │
│  • Can queue prod   │
│                     │
│  [End Turn Button]  │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│   PHASE TRANSITION  │
│                     │
│  • Fire OnPhaseChanged
│  • Update visibility│
│  • Deduct upkeep    │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│   AI PHASE          │
│                     │
│  • Coroutine delay  │
│  • AI makes moves   │
│  • AI builds/attacks│
│  • Auto-ends phase  │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│   END TURN          │
│                     │
│  • Increment turn #  │
│  • Restore unit MP  │
│  • Apply income     │
│  • Process production
│  • Fire OnTurnStarted
└─────────┬───────────┘
          │
          ▼
     Back to PLAYER PHASE
```

### Bootstrap Sequence

```
1. GameBootstrap.Awake()
   ├─ Ensure core systems exist
   ├─ Create terrain definitions
   ├─ Generate map (MapGenerator)
   ├─ Create weapon/unit definitions
   ├─ Spawn starting units
   ├─ Initialize fog of war
   ├─ Setup tech tree
   └─ Ready to play

2. TurnManager.Start()
   ├─ Set initial turn and phase
   ├─ Fire OnTurnStarted(1)
   ├─ Fire OnPhaseChanged(Player)
   └─ Game loop begins
```

---

## 📂 Directory Structure & Namespace Organization

```
Assets/Scripts/
├── Bootstrap/                  (Robotech.TBS.Bootstrap)
│   ├── GameBootstrap.cs       // Main initialization
│   ├── DefinitionsFactory.cs  // ScriptableObject creation
│   └── UnitFactory.cs         // Unit spawning
│
├── Core/                       (Robotech.TBS.Core)
│   └── TurnManager.cs         // Turn progression
│
├── Data/                       (Robotech.TBS.Data)
│   ├── UnitDefinition.cs      // Unit data schema
│   ├── WeaponDefinition.cs    // Weapon data schema
│   ├── TerrainType.cs         // Terrain data schema
│   ├── TechDefinition.cs      // Tech tree node schema
│   └── DistrictDefinition.cs  // District data schema
│
├── Hex/                        (Robotech.TBS.Hex)
│   ├── HexCoord.cs            // Coordinate struct
│   ├── HexGrid.cs             // Grid management
│   └── HexMath.cs             // Coordinate conversion
│
├── Map/                        (Robotech.TBS.Map)
│   └── MapGenerator.cs        // Procedural map generation
│
├── Units/                      (Robotech.TBS.Units)
│   └── Unit.cs                // Unit entity
│
├── Cities/                     (Robotech.TBS.Cities)
│   └── City.cs                // City entity
│
├── Systems/                    (Robotech.TBS.Systems)
│   ├── ResourceManager.cs     // Economy tracking
│   ├── TechManager.cs         // Technology tree
│   ├── CityManager.cs         // City lifecycle
│   └── MapRules.cs            // Tile validation rules
│
├── Combat/                     (Robotech.TBS.Combat)
│   └── CombatResolver.cs      // Combat calculations
│
├── Fog/                        (Robotech.TBS.Fog)
│   └── FogOfWarSystem.cs      // Visibility system
│
├── Input/                      (Robotech.TBS.Input)
│   └── SelectionController.cs // Mouse input
│
├── Rendering/                  (Robotech.TBS.Rendering)
│   └── HexDebugRenderer.cs    // Gizmo visualization
│
├── UI/                         (Robotech.TBS.UI)
│   └── UIShell.cs             // Main UI (stub)
│
└── Debug/                      (Robotech.TBS.Debug)
    └── DevHotkeys.cs          // Development shortcuts
```

---

## 🚀 Performance Considerations

### Current Optimizations

1. **Hex Distance Caching**
   - Precalculate common ranges at startup
   - Cache neighbor lists

2. **Visibility Updates**
   - Only recalculate visible hexes on turn start
   - Track "seen" vs "visible" separately

3. **Event Allocation**
   - Use static events (no allocations)
   - Unsubscribe in OnDisable (prevent leaks)

### Future Optimizations

- [ ] Object pooling for units
- [ ] Spatial hashing for unit lookups
- [ ] Incremental pathfinding
- [ ] Texture atlasing for sprites
- [ ] Batched rendering for hex grid

---

## 🧪 Testing Strategy

### Current: Unity Test Framework (EditMode)

```csharp
[Test]
public void HexDistance_ShouldCalculateCorrectly()
{
    var a = new HexCoord(0, 0);
    var b = new HexCoord(3, 2);
    Assert.AreEqual(5, a.Distance(b));
}
```

### Future Testing Layers

**Unit Tests:** Core logic (hex math, combat calculations)
**Integration Tests:** System interactions (turn flow, resource updates)
**PlayMode Tests:** Full gameplay loops
**Manual Testing:** AI behavior, balance, UX

---

## 🔄 Migration Path: Prototype → Production

### Current (Prototype):
- ScriptableObjects created at runtime
- Capsule primitives for units
- Gizmo-based rendering
- Minimal UI

### Future (Production):
1. **Persistent Assets**
   - Create .asset files for all definitions
   - Import sprites/models
   - Asset bundles for modding

2. **Proper Rendering**
   - Sprite renderer for hex tiles
   - Animated unit sprites
   - Particle effects for combat

3. **Polished UI**
   - Unity UI panels
   - Tooltips and help
   - Animations and transitions

4. **Save System**
   - JSON serialization
   - Compressed save files
   - Auto-save and load

---

## 📚 Key Design Patterns Used

| Pattern | Location | Purpose |
|---------|----------|---------|
| **Factory** | UnitFactory, DefinitionsFactory | Complex object creation |
| **Observer** | Static events throughout | Decoupled communication |
| **Data-Driven** | ScriptableObjects | Content separate from code |
| **Component** | MonoBehaviour systems | Modular responsibilities |
| **Singleton** | Manager classes via FindObjectOfType | Global access to systems |
| **Strategy** | CombatResolver | Swappable combat algorithms |

---

## 🤝 Contributing to Architecture

**Before adding new systems:**
1. Does it fit into an existing namespace?
2. Does it need a ScriptableObject definition?
3. Should it fire events for other systems?
4. Can it be tested in isolation?

**When refactoring:**
1. Keep events backward-compatible
2. Add XML documentation
3. Update this ARCHITECTURE.md
4. Write tests for core logic

---

**This architecture is designed to scale from prototype to polished game while remaining approachable for contributors.**

🏗️ **Build with intention, refactor with confidence.**
