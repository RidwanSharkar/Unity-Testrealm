# Bloomscythe - Unity 3D Co-op Combat Roguelike

![Unity](https://img.shields.io/badge/Unity-2022.3_LTS-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-In_Development-yellow)

## 🎮 Game Overview

**Bloomscythe** is a 2-player cooperative combat roguelike featuring two distinct classes:
- **Sword (Tank)**: Frontline melee combatant with defensive abilities
- **Scythe (Healer)**: Support healer with life-drain mechanics

**Graphics Style**: World of Warcraft / V Rising inspired visuals
**Gameplay**: Wave-based combat with progression through runes and unlockable abilities

---

## 📁 Project Setup Complete

✅ **Folder structure created**  
✅ **Core game systems implemented**  
✅ **ECS-inspired architecture**  
✅ **Combat system with damage calculation**  
✅ **Player controller with Input System**  
✅ **Weapon system (Sword & Scythe)**  
✅ **Enemy AI with NavMesh**  
✅ **UI systems (damage numbers, HUD)**  
✅ **Network multiplayer foundation (Mirror-ready)**  
✅ **Utility classes and helpers**

---

## 🎯 Core Features Implemented

### 1. Combat System
- **Damage Calculator** with critical hits (11% base + rune bonuses)
- **Critical damage multiplier** (2x base + rune bonuses)
- **Weapon-specific modifiers** and passive abilities
- **Damage queue system** for efficient processing
- **Multiple damage types**: Physical, Magical, True, Healing

### 2. Player Classes

#### Sword (Tank)
- **3-hit combo** with increasing damage multipliers (1x → 1.2x → 1.5x)
- **Q - Charge**: Dash forward and deal damage to enemies in path
- **E - Deflect**: Brief invulnerability with damage reflection
- **R - Colossus Strike**: Massive 360° AOE attack
- **F - Wind Shear**: Ranged slash projectile
- **Passive**: +15% damage when unlocked

#### Scythe (Healer)
- **Life Drain**: 30% of damage dealt heals the wielder
- **Q - Healing Wave**: AOE heal for all nearby allies
- **E - Soul Harvest**: Damage enemies + heal based on hits
- **R - Resurrection**: Revive a dead ally at 50% health
- **F - Spirit Link**: Link with ally to share 50% of healing
- **Passive**: +25% healing effectiveness when unlocked

### 3. Enemy AI System
- **NavMesh pathfinding** for intelligent movement
- **AI States**: Idle, Patrolling, Chasing, Attacking, Stunned, Frozen, Retreating
- **Enemy types**: Grunt, Elite, Miniboss, Boss (with scaling health/damage)
- **Level scaling** for progressive difficulty
- **Status effects**: Freeze, Stun

### 4. Rune System
- **Critical Rune**: +3% crit chance per rune
- **Crit Damage Rune**: +15% crit damage per rune
- **Health Rune**: Increases max health
- **Speed Rune**: +5% movement speed per rune

### 5. Multiplayer Ready (Mirror)
- Network manager for 2-player co-op
- Player spawn points
- Network synchronization foundation
- Ready/lobby system

---

## 🎮 Controls

### Movement
| Input | Action |
|-------|--------|
| **W/A/S/D** | Move character |
| **Spacebar** | Jump |
| **Shift (Hold)** | Sprint |
| **Right Click (Hold)** | Camera control |

### Combat
| Input | Action |
|-------|--------|
| **Left Click** | Primary attack |
| **Q** | Ability 1 |
| **E** | Ability 2 |
| **R** | Ability 3 (Ultimate) |
| **F** | Ability 4 |

### Weapons
| Input | Action |
|-------|--------|
| **1-5 Keys** | Switch weapons directly |
| **Mouse Wheel** | Cycle through weapons |

---

## 🛠️ Technical Architecture

### ECS-Inspired Design
```
Entity (Base Class)
  ↓
Components (HealthComponent, MovementComponent, TransformComponent)
  ↓
Systems (CombatSystem processes all combat events)
```

### Key Design Patterns
- **Singleton**: GameManager, CombatSystem, DamageNumberManager
- **Component-Based**: Modular entity components
- **Event-Driven**: UnityEvents for health changes, death, abilities
- **Object Pooling**: Damage numbers, projectiles (for performance)
- **State Machine**: Enemy AI states
- **Queue-Based Processing**: Combat damage queue

### Performance Optimizations
- Component caching to reduce `GetComponent()` calls
- Object pooling for frequently spawned objects
- Damage batching through queue system
- NavMesh for efficient pathfinding
- Layered rendering for optimal draw calls

---

## 📦 Required Unity Packages

### Essential (Install via Package Manager)
1. **Cinemachine** - Camera system
2. **Input System** - Modern input handling
3. **Post Processing** - Visual effects
4. **TextMeshPro** - UI text rendering
5. **Addressables** - Asset management
6. **Universal RP** - Rendering pipeline

### Networking (Optional for Multiplayer)
7. **Mirror Networking** - Add via Git URL:
   ```
   https://github.com/MirrorNetworking/Mirror.git
   ```

---

## 🚀 Quick Start Guide

### Step 1: Install Packages
1. Open Unity Package Manager (`Window → Package Manager`)
2. Install all packages listed above
3. For Mirror, use `Add package from git URL`

### Step 2: Setup Input System
1. Switch to new Input System:
   - `Edit → Project Settings → Player`
   - Set "Active Input Handling" to "Input System Package (New)"
   - Restart Unity

2. Create Input Actions:
   - Right-click in Project → `Create → Input Actions`
   - Name: `PlayerInputActions`
   - Configure actions (see Controls section)

### Step 3: Create Game Scene
1. Create new scene: `GameScene`
2. Add the following GameObjects:
   - **GameManager** (empty GameObject with `GameManager.cs`)
   - **CombatSystem** (empty GameObject with `CombatSystem.cs`)
   - **DamageNumberManager** (Canvas with `DamageNumberManager.cs`)
   - **Main Camera** (add Cinemachine Brain component)
   - **Directional Light**

### Step 4: Setup NavMesh
1. Select ground/floor objects
2. Mark as "Navigation Static"
3. `Window → AI → Navigation`
4. Click "Bake"

### Step 5: Create Player Prefab
1. Create empty GameObject: `SwordPlayer`
2. Add components:
   - `CharacterController`
   - `PlayerController`
   - `MovementComponent`
   - `HealthComponent`
   - `PlayerInputComponent` (Input System)
3. Add child object with `SwordWeapon` script
4. Configure in Inspector
5. Save as prefab

### Step 6: Create Enemy Prefab
1. Create GameObject with 3D model
2. Add components:
   - `NavMeshAgent`
   - `BaseEnemy`
   - `HealthComponent`
3. Configure stats (health, damage, speeds)
4. Save as prefab

### Step 7: Setup HUD
1. Create Canvas (Screen Space - Overlay)
2. Add child objects:
   - Health bar (Slider)
   - Ability icons (Images with Fill)
   - Weapon name (TextMeshPro)
3. Add `PlayerHUD` script to Canvas
4. Link references in Inspector

---

## 📊 File Structure

```
Assets/
├── Scripts/
│   ├── Core/                    # Core systems
│   │   ├── GameManager.cs
│   │   ├── Entity.cs
│   │   ├── GameSystem.cs
│   │   ├── DamageCalculator.cs
│   │   └── PlayerController.cs
│   ├── Systems/                 # Game systems
│   │   └── CombatSystem.cs
│   ├── Components/              # Entity components
│   │   ├── HealthComponent.cs
│   │   ├── MovementComponent.cs
│   │   └── TransformComponent.cs
│   ├── Weapons/                 # Weapon classes
│   │   ├── BaseWeapon.cs
│   │   ├── SwordWeapon.cs
│   │   └── ScytheWeapon.cs
│   ├── Enemies/                 # Enemy AI
│   │   └── BaseEnemy.cs
│   ├── UI/                      # User interface
│   │   ├── DamageNumberManager.cs
│   │   └── PlayerHUD.cs
│   ├── Network/                 # Multiplayer
│   │   ├── GameNetworkManager.cs
│   │   └── NetworkPlayer.cs
│   └── Utilities/               # Helpers
│       ├── ObjectPool.cs
│       ├── Timer.cs
│       ├── MathUtils.cs
│       ├── ExtensionMethods.cs
│       └── Singleton.cs
├── Prefabs/                     # Game prefabs
├── Scenes/                      # Unity scenes
├── Materials/                   # Materials & shaders
├── Textures/                    # Textures
├── Models/                      # 3D models
├── Animations/                  # Animations
├── Audio/                       # Sound effects
└── Resources/                   # Runtime assets
```

---

## 🎨 Graphics Setup (Stylized Look)

### Post-Processing Stack
1. Create Volume Profile: `Assets → Create → Volume Profile`
2. Name: `StylizedPostProcessing`
3. Add effects:
   - **Color Grading**: High contrast, saturated colors
   - **Bloom**: Subtle glow (intensity: 0.2)
   - **Vignette**: Focus attention (intensity: 0.3)
   - **Film Grain**: Texture (intensity: 0.1)

4. Create Global Volume:
   - `GameObject → Volume → Global Volume`
   - Assign profile

### Cel Shader (Optional)
A basic cel shader structure is documented in the guide.
Create: `Assets/Shaders/CelShader.shader`

---

## 🌐 Multiplayer Setup (Mirror)

### Enable Mirror Networking
1. Install Mirror package
2. Uncomment Mirror code in:
   - `GameNetworkManager.cs`
   - `NetworkPlayer.cs`

### Setup Network Manager
1. Add `NetworkManager` component to scene
2. Configure:
   - **Player Prefab**: Your networked player prefab
   - **Network Address**: localhost (for testing)
   - **Max Connections**: 2

### Test Multiplayer
1. Build the game
2. Run one instance as Host
3. Run second instance as Client
4. Connect to localhost

---

## 🐛 Debugging Tips

### Common Issues

**Problem**: Player not moving
- ✅ Check `MovementComponent` is attached
- ✅ Verify `CharacterController` is present
- ✅ Check Input System is configured

**Problem**: Damage not showing
- ✅ Verify `CombatSystem` is in scene
- ✅ Check `DamageNumberManager` is active
- ✅ Ensure camera reference is set

**Problem**: Enemies not chasing
- ✅ NavMesh is baked
- ✅ `NavMeshAgent` component present
- ✅ Ground has "Navigation Static" enabled
- ✅ Check aggro range in Inspector

**Problem**: Abilities not working
- ✅ Weapon is equipped
- ✅ Cooldowns are ready
- ✅ Input System actions configured
- ✅ Check Console for errors

---

## 🔮 Future Enhancements

### Planned Features
- [ ] Additional weapon types (Bow, Runeblade, Sabres)
- [ ] Boss AI with unique attack patterns
- [ ] Procedural dungeon generation
- [ ] Loot system with rarity tiers
- [ ] Character progression/leveling
- [ ] Skill tree system
- [ ] More enemy types and variants
- [ ] Environmental hazards
- [ ] Audio system with spatial sound
- [ ] Minimap and objective markers
- [ ] Save/load system
- [ ] Steam integration

### Visual Enhancements
- [ ] Particle effects for abilities
- [ ] Trail effects for weapons
- [ ] Screen shake on impacts
- [ ] Dynamic lighting
- [ ] Fog of war
- [ ] Weather system

---

## 📝 Code Documentation

All scripts include:
- ✅ XML documentation for public methods
- ✅ Clear variable naming
- ✅ Region organization
- ✅ Performance considerations
- ✅ Usage examples in comments

### Extending the System

**Add New Weapon**:
```csharp
public class BowWeapon : BaseWeapon
{
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Bow";
        baseDamage = 20;
        attackRange = 15f;
    }
    
    public override void PerformAttack()
    {
        // Implement ranged attack
    }
    
    public override void PerformAbility(string abilityKey)
    {
        // Implement bow abilities
    }
}
```

**Add New Enemy**:
```csharp
public class RangedEnemy : BaseEnemy
{
    protected override void PerformAttack()
    {
        // Implement ranged attack logic
    }
}
```

---

## 📄 License

This project is part of the Bloomscythe game development.

---

## 🤝 Contributing

This is a private project, but suggestions are welcome!

---

## 💬 Support

For questions or issues, refer to:
- `PROJECT_STRUCTURE.md` for detailed architecture
- Unity documentation
- Mirror Networking documentation

---

**Happy Gaming! ⚔️🌿**

