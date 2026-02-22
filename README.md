<div align="center">

# 🌙 Tsukuyomi-Block

[![Unity](https://img.shields.io/badge/Unity-2018.2.7f1-000000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com)
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
- 🎱 **Launch the ball** with the Fire button (Space / click)
- 💥 **Destroy all blocks** to progress to the next level
- ❤️ **3 lives** — if the ball falls, you lose a life and reset
- 🏆 **Score** increases with each block destroyed

> *"Tsukuyomi" — the moon god in Japanese mythology. The aesthetic inspiration for the game's visual theme.*

---

## 🕹️ Controls

| Action | Key |
|--------|-----|
| Move paddle left | ← Arrow / A |
| Move paddle right | → Arrow / D |
| Launch ball | Space / Left Click |

---

## ✨ Features

| Feature | Details |
|---------|---------|
| 🎱 **Ball physics** | Unity Rigidbody with bounce material (`Rebote.physicMaterial`) |
| 💥 **Particle FX** | Block destruction effects via KriptoFX Realistic Effects Pack v4 |
| ❤️ **Lives system** | 3 lives, ball and paddle reset on loss |
| 🏆 **Score tracking** | Points displayed via TextMesh Pro |
| 📦 **Level loading** | Sequential scenes with configurable delay |
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
      ├── All blocks gone → SiguienteNivel → load next scene
      │
      └── Ball hits floor → lives-- → reset paddle + ball
                                │
                                └── lives == 0 → Game Over → Portada
```

---

## 🚀 How to Run

### Prerequisites
- [Unity 2018.2.7f1](https://unity3d.com/get-unity/download/archive) (recommended for full compatibility)
- Unity 2019+ may work but may require package migration

### Steps

1. **Clone the repository**
```bash
git clone https://github.com/christiangfv/Tsukuyomi-Block.git
cd Tsukuyomi-Block
```

2. **Open in Unity Hub**
   - Open Unity Hub → `Add` → Select the `Tsukuyomi-Block/` folder
   - Unity will import all assets automatically

3. **Open the starting scene**
   - In the Project panel: `Assets/Scenes/Portada`
   - Press ▶️ Play

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
| TextMesh Pro | UI text rendering |
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
