# Tower Defense 2D - PRU213 Project

A 2D tower defense game built with Unity for the PRU213 course project. The game combines classic tower defense gameplay with pixel-art visuals, active skills, wave-based enemies, boss mechanics, UI feedback, and scene flow from menu to victory or game over.

## Overview

In this game, the player moves around the map to build and upgrade Archer Towers, cast active skills, and protect the base from incoming enemy waves. Enemies follow predefined waypoint paths toward the base. If all waves are cleared, the player wins. If the base health reaches zero, the game ends.

The project focuses on creating a complete playable Unity demo instead of isolated features. It includes menu navigation, level selection, guide screen, tower placement, enemy waves, boss phase behavior, audio feedback, victory, game over, and retry flow.

## Core Gameplay Loop

1. Start from `MainMenu`.
2. Open `Guide` or go to `LevelSelect`.
3. Choose one of three playable levels.
4. Build Archer Towers on valid buildable areas.
5. Defeat enemies to earn gold.
6. Upgrade towers and use active skills to survive harder waves.
7. Clear all waves to reach `Victory`, or lose all base health to reach `GameOver`.

## Main Features

- Three playable levels with different maps and wave setups.
- Grid-based tower placement with valid/invalid preview feedback.
- Archer Tower upgrade system using separate prefabs for each tower level.
- Enemy wave system with support for multiple enemy groups and multiple spawn lanes.
- Enemy movement through waypoint paths.
- Base health system with automatic game over handling.
- Active skills mapped to `E`, `R`, and `Space`.
- Cooldown UI and skill targeting preview.
- Boss enemy with a defensive phase below 30% HP.
- Card choice system after selected waves.
- Tower range and radar-style visual feedback.
- UI sound effects, menu background music, boss music, victory, and game over audio.
- Scene flow for menu, guide, level select, gameplay, victory, game over, retry, and exit.

## Controls

| Action | Input |
| --- | --- |
| Move player | `WASD` / keyboard movement |
| Build tower | `Q` |
| Cast skill E | `E` |
| Cast skill R | `R` |
| Cast poison skill | `Space` |
| Confirm skill target | Left mouse click |
| Cancel skill targeting | Right mouse click |

## Scenes

All main scenes are located in `Assets/Scenes`.

| Scene | Purpose |
| --- | --- |
| `MainMenu` | Start screen with Play, Guide, and Exit |
| `Guide` | Basic gameplay instructions |
| `LevelSelect` | Level selection screen |
| `Level0` | Playable level 1 |
| `Level05` | Playable level 2 |
| `Level1` | Playable level 3 |
| `Victory` | Win screen after clearing all waves |
| `GameOver` | Lose screen when base health reaches zero |

Note: `Level05` is used as the second level name because the map was originally designed as an intermediate level between `Level0` and `Level1`.

## Project Structure

```text
Assets/
  Animations/          Animation clips and controllers
  Audio/               Imported audio files
  Monster/             Enemy and boss art assets
  Prefabs/
    Enemies/           Enemy and boss prefabs
    Projectiles/       Projectile prefabs
    Towers/            Tower prefabs
    UI/                UI prefabs
    VFX/               Skill and effect prefabs
  Resources/
    Audio/SFX/         Runtime-loaded sound effects
  Scenes/              Menu, gameplay, victory, and game over scenes
  Scripts/
    Abilities/         Active skill casting
    Base/              Base health and tower placement
    Cards/             Card choice system
    Core/              Audio and camera helpers
    Economy/           Gold/currency management
    Effects/           Explosion and poison effects
    Enemies/           Enemy health, movement, boss phase
    Player/            Player movement and visuals
    Projectiles/       Arrow/projectile behavior
    Scenes/            Scene navigation
    Towers/            Tower attack, upgrade, selection
    UI/                Hover, radar, range, and UI helpers
    Waves/             Wave manager and enemy spawner
```

## Key Scripts

| Script | Responsibility |
| --- | --- |
| `WaveManager.cs` | Controls waves, enemy groups, card timing, and victory transition |
| `EnemySpawner.cs` | Spawns enemies and assigns waypoint paths |
| `EnemyMovement.cs` | Moves enemies along waypoints |
| `EnemyHealth.cs` | Handles HP, damage, death, gold reward, poison, and health bar updates |
| `BossMechaPhaseController.cs` | Controls the boss defensive phase and boss music |
| `TowerPlacementController.cs` | Validates build locations, gold cost, preview, and tower placement |
| `TowerAttack.cs` | Finds enemies in range and fires projectiles |
| `TowerArcherVisual.cs` | Switches tower visuals when upgrading |
| `UltimateSpawner.cs` | Handles skill input, targeting preview, casting, cooldowns, and skill audio |
| `MainMenuManager.cs` | Loads menu, guide, level select, gameplay, victory, and game over scenes |
| `GameAudio.cs` | Central audio helper for UI, gameplay, boss, and result sounds |

## Tower System

The game currently uses one tower family: Archer Tower.

| Tower Level | Prefab |
| --- | --- |
| Level 1 | `Tower_Archer` |
| Level 2 | `Tower_Archer 1` |
| Level 3 | `Tower_Archer 2` |

Tower upgrades improve damage, range, and cooldown. Each tower level uses a separate prefab so the visual upgrade is clear and easier to manage in Unity.

## Enemy And Boss System

Enemy prefabs are located in `Assets/Prefabs/Enemies`. The game includes regular enemies such as bats, mushrooms, slimes, golems, and mages, plus the `Boss_MechaStoneGolem` boss prefab.

The boss has a special phase:

- Above 30% HP: moves normally toward the base.
- At 30% HP or lower: enters a defensive animation, stops moving, and becomes temporarily invulnerable.
- After the defense phase: resumes movement with slightly higher speed.

## Audio

Audio is managed through `GameAudio.cs` and runtime clips under `Assets/Resources/Audio/SFX`.

Implemented audio includes:

- UI hover and click sounds.
- Menu background music.
- Wave start sound.
- Tower build and upgrade sounds.
- Not enough gold feedback.
- Skill casting sounds.
- Poison loop sound.
- Boss music while the boss is alive.
- Boss death, victory, and game over sounds.

## Setup

1. Open the project with Unity 6.
2. Open `Assets/Scenes/MainMenu.unity`.
3. Make sure the main scenes are added to Build Settings:
   - `MainMenu`
   - `Guide`
   - `LevelSelect`
   - `Level0`
   - `Level05`
   - `Level1`
   - `Victory`
   - `GameOver`
4. Press Play from `MainMenu` to test the full flow.

## Development Notes

- The player is used for movement and interaction only. Player direct attack is not part of the current gameplay.
- Tower placement depends on buildable tilemaps and occupied cell tracking.
- Enemy paths depend on waypoint objects in each gameplay scene.
- Victory is triggered by `WaveManager` after the final wave is cleared.
- Game over is triggered by `BaseHealth` when base HP reaches zero.
- Retry uses `PlayerPrefs` to reload the last selected level.

## Team Handoff

For new contributors, the recommended reading order is:

1. `TowerDefenseGuide.md` for a full Vietnamese project explanation.
2. `Assets/Scripts/Waves/WaveManager.cs` to understand the wave loop.
3. `Assets/Scripts/Base/TowerPlacementController.cs` to understand building towers.
4. `Assets/Scripts/Towers/TowerAttack.cs` to understand tower combat.
5. `Assets/Scripts/Abilities/UltimateSpawner.cs` to understand skills.
6. `Assets/Scripts/Scenes/MainMenuManager.cs` to understand scene navigation.

## Course Context

This project was developed as a PRU213 Unity course project. The goal is to demonstrate practical Unity development through a complete 2D game loop with gameplay systems, UI, audio, prefabs, animations, scene management, and team-ready project structure.
