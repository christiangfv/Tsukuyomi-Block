<div align="center">

# 🌙 Tsukuyomi-Block

[![Unity](https://img.shields.io/badge/Unity-6000.6.7f1-000000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com)
[![C#](https://img.shields.io/badge/C%23-Game%20Scripts-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

[![LinkedIn](https://img.shields.io/badge/LinkedIn-christiangfv-0A66C2?style=flat&logo=linkedin)](https://linkedin.com/in/christiangfv)
[![GitHub](https://img.shields.io/badge/GitHub-christiangfv-181717?style=flat&logo=github)](https://github.com/christiangfv)

**A classic Breakout / Arkanoid-style block game built in Unity 3D.**  
*Open to the community — contribute new levels!*

</div>

---

## 🎮 Gameplay

Tsukuyomi-Block is a 3D take on the classic Arkanoid/Breakout formula:

- 🏓 **Move the paddle** left/right with keyboard (← →)
- 🎱 **Launch the ball** with the Fire button (Left Ctrl / Z / click)
- 💥 **Destroy blocks** to earn points; automatic victory detection is still pending
- ❤️ **3 lives** — if the ball falls, you lose a life and reset
- 🏆 **Score** increases with each block destroyed

> *"Tsukuyomi" — the moon god in Japanese mythology. The aesthetic inspiration for the game's visual theme.*

---

## 🕹️ Controls

| Action | Key |
|--------|-----|
| Move paddle left | ← Arrow / A |
| Move paddle right | → Arrow / D |
| Launch ball | Left Ctrl / Z / Left Click |

---

## ✨ Features

| Feature | Details |
|---------|---------|
| 🎱 **Ball physics** | Unity Rigidbody with bounce material (`Rebote.physicMaterial`) |
| 💥 **Particle FX** | Block destruction effects via KriptoFX Realistic Effects Pack v4 |
| ❤️ **Lives system** | 3 lives, ball and paddle reset on loss |
| 🏆 **Score tracking** | Points displayed via Unity UI |
| 📦 **Level loading** | Configurable scene loading; automatic completion trigger is pending |
| 🎨 **3D Visual style** | Full 3D environment, not a 2D sprite game |

---

## 🏗️ Project Architecture

```
Tsukuyomi-Block/Assets/Codigo/
├── Blocks.cs           → Block behavior: detect collision, spawn FX, destroy, +1 point
├── movPelota.cs        → Ball: initial force launch, reset on life loss
├── MovBarra.cs         → Paddle: horizontal movement clamped to screen bounds
├── Puntos.cs           → Score: static counter + UI text update
├── Vidas.cs            → Lives: decrement on floor hit, reset ball + paddle
├── SiguienteNivel.cs   → Level transition: load next scene with delay
├── Suelo.cs            → Floor trigger: calls Vidas.PerderVida()
├── ComenzarPartida.cs  → Game start logic
└── BotonSalir.cs       → Exit button
```

### Game Loop

```
[Start Screen]
      │ Press Start
      ▼
[Level Loads] → Ball attached to paddle
      │ Press Fire
      ▼
[Ball launches] → Rigidbody AddForce(500, 500, 0)
      │
      ├── Ball hits block → particle FX + Destroy + score++
      │
      ├── All blocks gone → automatic victory handling is unfinished
      │
      └── Ball hits floor → lives-- → reset paddle + ball
                                │
                                └── Game-over handling is unfinished
```

---

## 🚀 How to Run

### Prerequisites
- [Unity Hub](https://unity.com/download) and **Unity 6.6 (6000.6.7f1)** for Apple Silicon
- An active Unity license in Hub
- Optional: Visual Studio Code with Microsoft’s Unity extension

### Steps

1. **Clone the repository**
```bash
git clone https://github.com/christiangfv/Tsukuyomi-Block.git
cd Tsukuyomi-Block
```

2. **Open in Unity Hub**
   - Open Unity Hub → `Add project from disk` → Select the inner `Tsukuyomi-Block/` folder (the one containing `Assets`, `Packages`, and `ProjectSettings`)
   - Select **6000.6.7f1** as the editor; let the first asset import finish
   - Keep the Built-in Render Pipeline and legacy Input Manager enabled

3. **Open the starting scene**
   - In the Project panel: `Assets/Scenes/Portada`
   - Press ▶️ Play

---

## Minimum remaining gameplay work

See [Mínimos pendientes del juego](docs/minimos-del-juego.md) for the prioritized checklist: defeat, victory, retry/menu actions, visible controls, and ball constraints.

## Unity 6 migration and checks

The project targets **Unity 6.6 / 6000.6.7f1 (ARM64)**. It keeps the original Built-in Render Pipeline, legacy Input Manager, and both game scenes. Unity UI replaces the old standalone TextMesh Pro package. Unused Ads, Analytics, Purchasing, XR helpers, multiplayer-center, and Unity Version Control packages were removed; Git remains the version-control system.

The original scripts and third-party assets are retained. Compatibility changes update entity identifiers, layout APIs, rigidbody velocity/reset ordering, and scene loading. Scene/resource GUIDs are preserved. Some third-party demo scripts still emit deprecation warnings; they compile, but those demos were not play-tested.

On macOS, run from the repository root (close any editor already using the project):

```bash
# Open the editor
./scripts/unity.sh

# Check both scenes, input configuration, shaders, and required references
./scripts/unity.sh -batchmode -nographics -quit -executeMethod MigrationValidation.Validate -logFile /tmp/tsukuyomi-validation.log

# Exercise real physics collisions, particles, score, lives, ball reset, and level-to-menu loading
./scripts/unity.sh -batchmode -executeMethod MigrationValidation.StartPlaySmoke -logFile /tmp/tsukuyomi-smoke.log

# Build a standalone application for Apple Silicon
./scripts/unity.sh -batchmode -quit -executeMethod MigrationValidation.BuildMac -logFile /tmp/tsukuyomi-build.log
```

The app is written to `Tsukuyomi-Block/Builds/macOS/Tsukuyomi-Block.app`; rendered scene captures are in `Tsukuyomi-Block/Builds/screenshots/`. These outputs and Unity's local caches are ignored by Git. Set `UNITY_EDITOR` to the editor executable if it is installed elsewhere. The helper uses a short temporary path to avoid macOS socket path limits in desktop-agent environments.

Validated: scene references, 30 level blocks, legacy input configuration, real block/floor collision callbacks, score/lives UI updates, ball reset, delayed return to the menu, rendered captures of both scenes, and the ARM64 standalone build. Keyboard/mouse input has not been automated; check it with Play in the editor. This migration preserves the existing gameplay rules rather than adding new levels or completing unfinished game-over logic.

---

## 🗺️ Adding New Levels

This project is **open for community contributions** — you can design and submit new levels!

### How to create a level:

1. **Duplicate** an existing level scene
2. **Arrange blocks** in the scene using the existing `Block` prefab
3. **Set the next level** in the `SiguienteNivel` component:
   ```
   nivelACargar = "YourLevelName"
   retraso = 1.0  (seconds before loading)
   ```
4. **Add your scene** to `File → Build Settings → Scenes In Build`
5. **Submit a Pull Request** with your level!

### Level structure
```
Assets/
└── Scenes/
    ├── Portada          ← Main menu / game over screen
    ├── Level1           ← Level 1
    ├── Level2           ← Level 2
    └── YourNewLevel     ← Add yours here!
```

---

## 📦 Third-Party Assets

| Asset | Purpose |
|-------|---------|
| [KriptoFX Realistic Effects Pack v4](https://assetstore.unity.com/publishers/5730) | Particle effects on block destruction |
| Unity UI | UI text rendering |
| Devdog InventoryPro | (Included in template, available for future power-up mechanics) |

---

## 🤝 Contributing

Pull Requests welcome — especially new levels!

```bash
git checkout -b feature/new-level-name
git commit -m "feat: add Level3 with spiral block pattern"
git push origin feature/new-level-name
# Open a Pull Request
```

---

## 👤 Author

**Christian Fuentes** — Developer & AI/ML Engineer

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect-0A66C2?style=flat&logo=linkedin)](https://linkedin.com/in/christiangfv)
[![GitHub](https://img.shields.io/badge/GitHub-@christiangfv-181717?style=flat&logo=github)](https://github.com/christiangfv)

---

## 📄 License

MIT License — see [LICENSE](LICENSE) for details.

---

<div align="center">

*Tags: `unity` `unity3d` `csharp` `breakout` `arkanoid` `game` `open-source` `community`*

⭐ Give it a star if you like it — and submit a level!

</div>
