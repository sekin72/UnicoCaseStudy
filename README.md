# 🏰 Board Defense Prototype

This is a lightweight tower defense prototype built in Unity (portrait mode). Enemies spawn from the top and travel through a predefined path. Defenders (towers) are placed along the board to intercept and eliminate them.

---

## 🧠 Project Overview

This prototype is built with **performance** and **modularity** in mind.

- **Async Architecture (UniTask)**  
  Async operations (such as pooling, delayed spawns, and transitions) are managed using `UniTask`, ensuring minimal GC overhead and optimized execution.

- **AppManager (Centralized Manager Hub)**  
  The `AppManager` acts as a global singleton manager that holds and provides access to key components and services.  
  - It replaces traditional Dependency Injection (DI) frameworks with a simpler, direct-access approach.  
  - Managers are initialized and stored here for global reach and consistency.

- **Game Systems**  
  Systems are localized gameplay modules, activated/deactivated contextually via `GameSession`.  
  Examples:  
  - `EnemySpawnerSystem`  
  - `DefencePlacementSystem`  
  - `InputSystem`  
  These systems manage self-contained features depending on the current phase of gameplay, contributing to both scalability and clarity in structure.

- **Code Design**  
  The entire codebase adheres to **SOLID principles** to promote extensibility and clean architecture.  
  Systems and features are modular, replaceable, and easy to extend for future development.

---

## 🎮 Gameplay Overview

- **Placing Towers**  
  Drag a tower from the bottom bar and drop it on a valid tile to place it.

- **Swapping or Moving Towers**  
  After a tower is placed, you can tap/select it again to either:
  - **Swap** with another tower from the bottom bar
  - **Move** it to another valid location

---

## ⚙️ Additional Features (Custom Enhancements)

These additions were made beyond the base design. Each can be reverted via its associated config file:

### 🛡️ Defender Config
- **Defender Placement Cooldown**  
  You must wait for a cooldown period before placing the **same type** of tower again.

### 🧟 Level Config
- **Delay Between Enemy Spawns**  
  Controls the time interval between consecutive enemy spawns in a wave.

---

## 🗂️ Configuration

All config files can be found under:

"Assets/_Sources/ScriptableObjects/LevelData"

- `DefenderConfig.asset` → Controls cooldown settings and other defender parameters.
- `LevelConfig.asset` → Manages wave structure and enemy spawn delay.

---

## 💡 What I Could Have Done

- **Resume Feature**  
  There is already a save system in place (used for sound and vibration settings). I would have loved to extend it to save the random seed, defender placements, and enemy states — allowing the player to continue exactly where they left off after closing the game.

- **Content & Balancing**  
  I would have liked to explore more level designs, defender/enemy types, and balancing ideas using the configuration system. Due to time constraints, only the requested defenders, enemies, and level setups are implemented.

---