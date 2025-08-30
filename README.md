# 🏰 Board Defense Prototype

This is a lightweight tower defense prototype built in Unity (portrait mode). Enemies spawn from the top and travel through a predefined path. Defenders (towers) are placed along the board to intercept and eliminate them.

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