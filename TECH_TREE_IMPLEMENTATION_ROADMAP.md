# Tech Tree Implementation Roadmap

**Status:** Planning Phase
**Priority:** High (Core MVP Feature)
**Estimated Effort:** 3-4 weeks (phased approach)

---

## 🎯 Goal

Implement the complete technology tree system (Gen 0-4) based on `TECH_TREE.md` framework, enabling:
- Progressive tech unlocks (60+ technologies)
- Branching research paths (Air/Ground/Tech)
- Era transitions (Gen 0 → Gen 4)
- Unit/district unlock requirements
- Global stat bonuses from techs

---

## 📋 Implementation Phases

### Phase 1: Enhanced Data Layer (Week 1)

**Goal:** Expand TechDefinition to support full tech tree features

#### Tasks:
1. **Expand TechDefinition.cs**
   - [ ] Add `TechGeneration` enum (Gen0-Gen5)
   - [ ] Add `TechCategory` enum (Power, Mecha, Weapons, Defense, Aerospace, Special)
   - [ ] Add `prerequisites` list
   - [ ] Add `unlocksUnits`, `unlocksDistricts`, `unlocksAbilities` lists
   - [ ] Add bonus fields (Protoculture, Science, Production, HP, Armor, Movement, etc.)
   - [ ] Add flags (`isCriticalPath`, `allowsEraTransition`)
   - [ ] Add `Sprite icon` for UI

2. **Create Tech Factory Method**
   - [ ] Add `DefinitionsFactory.CreateTech()` helper method
   - [ ] Support flexible parameter lists for bonuses/unlocks

3. **Define All Gen 0-1 Techs**
   - [ ] Jet Propulsion
   - [ ] Conventional Ballistics
   - [ ] Protoculture Discovery (critical path)
   - [ ] Energy Reactors Mk I
   - [ ] Mecha Chassis I
   - [ ] Metallurgy I
   - [ ] Missile Guidance I
   - [ ] Global Communications Network

**Deliverable:** All Gen 0-1 tech definitions created in `GameBootstrap.cs`

---

### Phase 2: Enhanced Tech Manager (Week 1-2)

**Goal:** Implement prerequisite checking, era transitions, and tech effects

#### Tasks:
1. **Expand TechManager.cs**
   - [ ] Add `allTechs` (full tree)
   - [ ] Add `availableTechs` (currently researchable based on prerequisites)
   - [ ] Add `researchedTechs` (completed)
   - [ ] Add `currentGeneration` tracking
   - [ ] Implement `UpdateAvailableTechs()` (checks prerequisites)
   - [ ] Implement era transition logic in `CompleteCurrentTech()`
   - [ ] Implement `ApplyTechEffects()` (applies bonuses to ResourceManager, units, etc.)

2. **Integrate with ResourceManager**
   - [ ] Add `protocultureTechBonus`, `scienceTechBonus`, `productionTechBonus` fields
   - [ ] Add `AddProtocultureBonus()`, `AddScienceBonus()`, `AddProductionBonus()` methods
   - [ ] Modify `ApplyIncome()` to include tech bonuses

3. **Integrate with Unit System**
   - [ ] Add `requiredTech` field to `UnitDefinition`
   - [ ] Implement `Unit.ApplyTechUpgrade()` (upgrades units in field when tech completes)
   - [ ] Modify `UnitFactory.SpawnUnit()` to check tech requirements

4. **Create Test Cases**
   - [ ] Test prerequisite validation (can't research without prereqs)
   - [ ] Test era transitions (unlocks higher-gen techs)
   - [ ] Test tech effects (bonuses apply correctly)
   - [ ] Test unit unlocking (can produce after researching)

**Deliverable:** Fully functional tech tree system with prerequisite/era logic

---

### Phase 3: Gen 2-3 Tech Definitions (Week 2)

**Goal:** Define all mid-game technologies (VF-1 Valkyrie era)

#### Tasks:
1. **Define Gen 2 Techs (Contact and Conflict)**
   - [ ] Transformation Engineering I (critical path)
   - [ ] Sensor Suite Integration I
   - [ ] Reactor Mk II
   - [ ] Mecha Chassis II
   - [ ] Missile Control II

2. **Define Gen 3 Techs (Escalation and Armament)**
   - [ ] Transformation Engineering II (critical path, unlocks VF-1 line)
   - [ ] Targeting AI
   - [ ] Composite Armor I
   - [ ] Reactor Mk III
   - [ ] Metallurgy II
   - [ ] Armored Veritech Program
   - [ ] Advanced Destroids

3. **Create Unit Tech Dependencies**
   - [ ] VF-0 Prototype requires Transformation Engineering I
   - [ ] VF-1A/J/S require Transformation Engineering II
   - [ ] Spartan Mk I requires Mecha Chassis II
   - [ ] Excalibur/Gladiator require Advanced Destroids
   - [ ] VF-1R (Armored) requires Armored Veritech Program

4. **Balance Science Costs**
   - [ ] Playtest Gen 0-3 progression
   - [ ] Adjust costs for ~30-40 turn timeline to reach Gen 3
   - [ ] Verify branching paths are viable

**Deliverable:** Complete Gen 0-3 tech tree with all unit dependencies

---

### Phase 4: UI Layer (Week 3)

**Goal:** Create Tech Tree UI screen with selection and tooltips

#### Tasks:
1. **Tech Tree Screen**
   - [ ] Create UI panel (Canvas + scroll view)
   - [ ] Display available technologies (grid or list)
   - [ ] Show current research + progress bar
   - [ ] Show generation progress
   - [ ] "Select Research" button for each available tech

2. **Tech Tooltip System**
   - [ ] Display on hover: name, cost, prerequisites, unlocks, effects
   - [ ] Visual indication: researched (green), available (white), locked (gray)
   - [ ] Show prerequisite connections (optional: graph view)

3. **Integration**
   - [ ] Hotkey to open Tech Tree (default: T)
   - [ ] Click tech → `TechManager.SetResearch()`
   - [ ] Update UI when tech completes (event-driven)

4. **Visual Polish**
   - [ ] Add tech icons (placeholders OK for MVP)
   - [ ] Color-code by category (Power=blue, Mecha=green, Weapons=red, etc.)
   - [ ] Generation separators (visual grouping)

**Deliverable:** Functional Tech Tree UI screen

---

### Phase 5: Gen 4 + Polish (Week 4)

**Goal:** Complete late-game techs and balance full tree

#### Tasks:
1. **Define Gen 4 Techs (Post-War Superiority)**
   - [ ] Aerospace Integration I (critical path)
   - [ ] FAST Pack Engineering I
   - [ ] Weapon Amplifiers I
   - [ ] Barrier Field Technology I
   - [ ] Reactor Mk IV
   - [ ] Excalibur Mk II Program

2. **Create Elite Units**
   - [ ] VF-1S Super Valkyrie (requires FAST Pack Engineering I)
   - [ ] VF-1A Strike Valkyrie (requires FAST Pack Engineering I)
   - [ ] Excalibur Mk II (requires Excalibur Mk II Program)

3. **Balance Pass**
   - [ ] Playtest all three paths (Air, Ground, Tech)
   - [ ] Ensure no dominant strategy
   - [ ] Verify science costs scale appropriately
   - [ ] Check tech completion timing (Gen 0 by turn 5, Gen 1 by turn 15, Gen 2 by turn 25, etc.)

4. **Documentation**
   - [ ] Update TECH_TREE.md with any changes
   - [ ] Add tech tree diagram (optional)
   - [ ] Document tech unlock requirements in unit files

**Deliverable:** Complete Gen 0-4 tech tree, balanced and playable

---

### Phase 6: Gen 5 (Optional/Future)

**Stretch Goal:** Southern Cross Initiative techs for expansion

#### Tasks:
- [ ] Define Gen 5 techs (Reactor Mk V, Bio-Integration I, Reflex Cannon Theory, Modular Armor Systems)
- [ ] Create next-gen units (if applicable)
- [ ] Add campaign scenario support (pre-unlocked techs for specific missions)

**Note:** Gen 5 is NOT required for MVP. Consider post-1.0 expansion.

---

## 🧪 Testing Strategy

### Unit Tests (EditMode)
```csharp
[Test]
public void TechPrerequisites_BlocksUnavailableTechs()
{
    // Arrange
    var reactorMk2 = CreateTech(..., prerequisites: reactorMk1);
    var techManager = new TechManager();

    // Act
    bool canResearch = techManager.IsTechAvailable(reactorMk2);

    // Assert
    Assert.IsFalse(canResearch, "Should not be able to research Reactor Mk II without Mk I");
}

[Test]
public void EraTransition_UnlocksNextGeneration()
{
    // Arrange
    var protocultureDiscovery = CreateTech(...);
    protocultureDiscovery.allowsEraTransition = true;
    var techManager = new TechManager();
    techManager.currentGeneration = TechGeneration.Gen0;

    // Act
    techManager.SetResearch(protocultureDiscovery);
    techManager.AddScience(1000); // Force completion

    // Assert
    Assert.AreEqual(TechGeneration.Gen1, techManager.currentGeneration);
}
```

### Integration Tests (Play Mode)
- [ ] Start new game, research Protoculture Discovery, verify Gen 1 unlocks
- [ ] Research Transformation Engineering I, verify VF-0 can be produced
- [ ] Complete Reactor Mk II, verify +15 Protoculture per turn applied
- [ ] Research all Gen 3 techs, verify no crashes or missing dependencies

### Balance Testing
- [ ] Play full match focusing Air path (VF-1 rush)
- [ ] Play full match focusing Ground path (Destroid wall)
- [ ] Play full match focusing Tech path (economy/science)
- [ ] Verify all paths reach victory condition in ~40-60 turns

---

## 📊 Success Criteria

**MVP Tech Tree is complete when:**
- [ ] All Gen 0-3 techs implemented and testable
- [ ] Prerequisite system prevents researching locked techs
- [ ] Era transitions work (Gen 0 → Gen 1 → Gen 2 → Gen 3)
- [ ] Unit production checks tech requirements
- [ ] Tech effects apply to resources and units
- [ ] UI allows selecting and viewing techs
- [ ] All three strategic paths are viable
- [ ] No critical bugs or crashes

**Stretch Goals:**
- [ ] Gen 4 fully implemented
- [ ] Visual tech tree graph (Civ6-style)
- [ ] Tech icons for all techs
- [ ] Gen 5 defined for future expansion

---

## 🚧 Known Challenges

### Challenge 1: Balancing Science Costs
**Problem:** Hard to predict turn timing without extensive playtesting
**Mitigation:**
- Start with formula-based costs (Gen X = 10 * (1.5^X))
- Playtest and adjust
- Make costs easily tweakable (ScriptableObject)

### Challenge 2: Unit Upgrade in Field
**Problem:** Applying tech bonuses to existing units
**Mitigation:**
- Store original stats in UnitDefinition
- Apply multipliers/bonuses when tech completes
- Units keep bonuses even if "downgraded" (no tech loss mechanic)

### Challenge 3: UI Complexity
**Problem:** 60+ techs is a lot to display
**Mitigation:**
- Filter by generation (show only current + next)
- Filter by category (Power/Mecha/Weapons tabs)
- Search/filter functionality

### Challenge 4: Save/Load
**Problem:** Need to persist researched techs
**Mitigation:**
- Store list of `techId` strings in save file
- Reconstruct `researchedTechs` list on load
- Reapply all tech effects to resources/units

---

## 🔗 Dependencies

**Must be implemented first:**
- [x] ResourceManager (Science accumulation)
- [x] TurnManager (Per-turn science from cities)
- [x] City/District system (Science yields from Lab districts)
- [ ] Enhanced TechDefinition (this roadmap)
- [ ] Enhanced TechManager (this roadmap)

**Blocks:**
- [ ] Unit production UI (needs tech unlock checks)
- [ ] Advanced units (VF-1 line blocked by Transformation Engineering II)
- [ ] Late-game balance (needs full tech tree to test)

---

## 📅 Suggested Timeline

**Week 1:** Phase 1-2 (Enhanced data layer, TechManager)
**Week 2:** Phase 3 (Gen 2-3 definitions, balancing)
**Week 3:** Phase 4 (UI implementation)
**Week 4:** Phase 5 (Gen 4, polish, testing)

**Total:** ~4 weeks for complete Gen 0-4 tech tree system

---

## 🎯 Next Steps

1. **Review TECH_TREE.md** - Familiarize with all 60+ technologies
2. **Create TechGeneration and TechCategory enums** - Add to Data namespace
3. **Expand TechDefinition.cs** - Add all new fields from Phase 1
4. **Create CreateTech() factory method** - Enable rapid tech creation
5. **Define Gen 0 techs** - Start small (3-4 techs) to test system
6. **Test prerequisite logic** - Verify TechManager.IsTechAvailable() works

**After that, proceed phase-by-phase through this roadmap.**

---

**Questions? See:**
- `TECH_TREE.md` - Full tech tree design
- `ARCHITECTURE.md` - TechManager system design
- `DEVELOPMENT_GUIDE.md` - Coding standards for implementation
- `best-practices/robotech-tbs.md` - Quick reference patterns

---

*"Technology is the key to survival. Master it, or be left behind."* - Dr. Emil Lang, Robotech Research Group
