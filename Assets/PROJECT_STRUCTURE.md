# Bloomscythe - Unity 3D Co-op Combat Roguelike

## Project Overview
A 2-player co-op combat roguelike featuring Sword (Tank) and Scythe (Healer) classes with WoW/V Rising inspired graphics.

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/                  # Core game systems
│   │   ├── GameManager.cs            # Singleton managing global game state
│   │   ├── Entity.cs                 # Base entity class (ECS architecture)
│   │   ├── GameSystem.cs             # Base system class (ECS architecture)
│   │   ├── DamageCalculator.cs       # Damage calculation with crit system
│   │   └── PlayerController.cs       # Player input and control
│   │
│   ├── Systems/               # Game systems (ECS)
│   │   └── CombatSystem.cs           # Centralized combat and damage processing
│   │
│   ├── Components/            # Entity components
│   │   ├── HealthComponent.cs        # Health, damage, healing, shields
│   │   ├── MovementComponent.cs      # Character movement and jumping
│   │   └── TransformComponent.cs     # Enhanced transform utilities
│   │
│   ├── Weapons/               # Weapon classes
│   │   ├── BaseWeapon.cs             # Abstract weapon base class
│   │   ├── SwordWeapon.cs            # Tank class weapon (3-hit combo)
│   │   └── ScytheWeapon.cs           # Healer class weapon (life drain)
│   │
│   ├── Enemies/               # Enemy AI
│   │   └── BaseEnemy.cs              # Base enemy with AI states & NavMesh
│   │
│   ├── UI/                    # User interface
│   │   ├── DamageNumberManager.cs    # Floating damage numbers
│   │   └── PlayerHUD.cs              # Health bar, cooldowns, weapon display
│   │
│   ├── Network/               # Multiplayer (Mirror integration)
│   │   ├── GameNetworkManager.cs     # Network manager for 2-player co-op
│   │   └── NetworkPlayer.cs          # Network synchronization
│   │
│   ├── Utilities/             # Helper classes
│   │   ├── ObjectPool.cs             # Generic object pooling
│   │   ├── Timer.cs                  # Timer utility
│   │   ├── MathUtils.cs              # Math helper functions
│   │   ├── ExtensionMethods.cs       # C# extension methods
│   │   └── Singleton.cs              # Singleton pattern
│   │
│   └── Editor/                # Unity Editor scripts
│
├── Prefabs/                   # Game prefabs
├── Scenes/                    # Unity scenes
├── Materials/                 # Materials and shaders
├── Textures/                  # Texture assets
├── Models/                    # 3D models
├── Animations/                # Animation clips
├── Audio/                     # Sound effects and music
└── Resources/                 # Runtime-loaded assets
```

## Key Systems

### 1. **Core Game Manager**
- `GameManager.cs` - Singleton managing rune counts, passive unlocks, and game state
- Persistent across scenes
- Handles wave progression and game pausing

### 2. **ECS Architecture**
- `Entity.cs` - Base class for all game entities (players, enemies, projectiles)
- `GameSystem.cs` - Base class for game systems that process entities
- Component-based design for flexibility

### 3. **Combat System**
- `CombatSystem.cs` - Centralized damage processing with queue system
- `DamageCalculator.cs` - Critical hit calculations with rune modifiers
- Support for multiple damage types (Physical, Magical, True, Healing)

### 4. **Player System**
- `PlayerController.cs` - WASD movement, Spacebar jump, mouse camera control
- Input System integration (Q, E, R, F for abilities)
- Weapon switching (1-5 keys or mouse wheel)

### 5. **Weapon System**
- **Sword (Tank)**: 3-hit combo, Charge, Deflect, Colossus Strike, Wind Shear
- **Scythe (Healer)**: Life drain, Healing Wave, Soul Harvest, Resurrection, Spirit Link
- Ability cooldown tracking
- Weapon-specific passive bonuses

### 6. **Enemy AI**
- NavMesh-based pathfinding
- AI states: Idle, Patrolling, Chasing, Attacking, Stunned, Frozen
- Enemy types: Grunt, Elite, Miniboss, Boss
- Level scaling for health and damage

### 7. **UI System**
- Damage numbers with critical hit highlighting
- Player HUD with health bar and ability cooldowns
- Color-coded health (green > yellow > red)

### 8. **Network Multiplayer** (Mirror Ready)
- Prepared for Mirror Networking integration
- 2-player co-op support
- Spawn point management
- Network synchronization for positions, health, abilities

## Controls

### Movement
- **WASD** - Move character
- **Spacebar** - Jump
- **Shift** - Sprint (hold)
- **Right Click (Hold)** - Camera control

### Combat
- **Left Click** - Primary attack
- **Q** - Ability 1
- **E** - Ability 2
- **R** - Ability 3 (Ultimate)
- **F** - Ability 4

### Weapons
- **1-5** - Switch weapons directly
- **Mouse Wheel** - Cycle through weapons

## Classes

### Sword (Tank Class)
**Role**: Frontline melee combatant
**Primary**: 3-hit combo with increasing damage
**Abilities**:
- **Q - Charge**: Dash forward dealing damage
- **E - Deflect**: Block and reflect damage (brief invulnerability)
- **R - Colossus Strike**: Massive AOE damage
- **F - Wind Shear**: Ranged slash projectile
**Passive**: +15% damage when unlocked

### Scythe (Healer Class)
**Role**: Support healer with life drain
**Primary**: Melee attack with life steal (30% of damage heals self)
**Abilities**:
- **Q - Healing Wave**: AOE heal for all allies
- **E - Soul Harvest**: AOE damage + heal based on enemies hit
- **R - Resurrection**: Revive a dead ally at 50% HP
- **F - Spirit Link**: Link with ally to share 50% of healing
**Passive**: +25% healing when unlocked

## Rune System
- **Critical Rune**: +3% crit chance per rune
- **Crit Damage Rune**: +15% crit damage per rune
- **Health Rune**: Increases max health
- **Speed Rune**: +5% movement speed per rune

## Next Steps (Unity Editor Setup Required)

### 1. Install Unity Packages
- Cinemachine (camera system)
- Input System (modern input)
- Post Processing (visual effects)
- TextMeshPro (UI text)
- Addressables (asset management)
- Mirror Networking (multiplayer)

### 2. Create Input Actions Asset
- File → Create → Input Actions
- Name: `PlayerInputActions`
- Define actions: Move, Look, Jump, Attack, Sprint, Ability1-4, WeaponSwitch

### 3. Setup Scene
- Create GameScene
- Add GameManager GameObject
- Add CombatSystem GameObject
- Add DamageNumberCanvas
- Setup NavMesh for enemies
- Create spawn points

### 4. Create Player Prefabs
- Sword Player prefab with SwordWeapon
- Scythe Player prefab with ScytheWeapon
- Add CharacterController, MovementComponent, HealthComponent
- Setup PlayerController with Input System

### 5. Create Enemy Prefabs
- Setup NavMeshAgent
- Add BaseEnemy component
- Configure health, damage, aggro ranges
- Add animations

### 6. Setup UI
- Create HUD Canvas
- Setup health bars
- Create ability cooldown UI elements
- Configure damage number prefab

### 7. Visual Polish
- Create cel shader for stylized graphics
- Setup post-processing volume
- Add particle effects for abilities
- Import and configure 3D models

## Performance Considerations
- Object pooling implemented for damage numbers and projectiles
- NavMesh baking for efficient pathfinding
- Component caching to reduce GetComponent calls
- Damage queue system to batch processing

## Multiplayer Architecture (Mirror)
All network scripts are prepared for Mirror integration. To enable:
1. Install Mirror via Package Manager
2. Uncomment Mirror-specific code in:
   - `GameNetworkManager.cs`
   - `NetworkPlayer.cs`
3. Add NetworkIdentity to player prefabs
4. Configure NetworkManager component

## Code Style
- XML documentation for all public methods
- Clear naming conventions (PascalCase for public, camelCase for private)
- Component-based architecture for reusability
- Event-driven design with UnityEvents

## Credits
Created for Bloomscythe - A 3D co-op combat roguelike
Architecture: ECS-inspired with Unity MonoBehaviour
Target Graphics: World of Warcraft / V Rising style

