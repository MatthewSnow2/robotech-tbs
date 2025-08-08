# Robotech: Macross Era – Turn-Based Strategy (Civ6-inspired)

Personal fan project (non-commercial). Windows, mouse/keyboard. Built with Unity (target: 2022 LTS). Hex-based, turn-based skirmish focus, Civ6-like systems adapted to Robotech.

## Status
- Phase: Design + scaffolding
- Scope: Single-player skirmish, conquest-only win condition

## Highlights
- Factions: RDF vs Zentradi
- Map: Hex grid, terrain (urban, ocean, mountains, plains, forest, desert, tundra, hills), fog of war like Civ6
- Economy: Cities with districts; resources include Protoculture (refined), raw materials, credits, population. Protoculture powers upkeep/maintenance
- Cities/Construction: Civ-like cities; districts: Factories, Labs, Outposts
- Progression: Tech tree unlocks units and upgrades (Armored/Super Veritech, etc.)
- Combat: Based on Robotech Tactics (Palladium) normalized to TBS pacing; no friendly fire; air & ground layers; abilities except folding
- Diplomacy: Alliances and ceasefires only (skirmish context)
- AI: Personalities with difficulty scaling (non-cheating on lower, limited boosts on higher)

## Directory Structure
- `docs/` – Design docs (GDD, units, economy, tech)
- `Assets/` – Unity project assets (after initialization)
- `data/` – Data tables and templates (pre-Unity ScriptableObject schemas)

## Next Steps
1. Finalize GDD details (map sizes, starting conditions)
2. Initialize Unity project and base systems (hex grid, camera, turn system)
3. Implement ScriptableObject schemas for Units, Terrain, Tech, Districts
4. Prototype one map and a small unit subset for combat loop

## Legal Note
Robotech and Palladium content belong to their respective rights holders. This project is a personal, non-commercial fan work.

## Non-Commercial Notice / License Intent
- This repository is public so others can see the code, learn, and contribute.
- Forking and cloning are permitted for personal, non-commercial purposes.
- No commercial use, redistribution of copyrighted IP assets, or monetization is permitted.
- If you contribute, you agree your contributions are also non-commercial and may be removed if requested by rights holders.

If you are a rights holder and wish content to be removed or altered, please open an issue.
