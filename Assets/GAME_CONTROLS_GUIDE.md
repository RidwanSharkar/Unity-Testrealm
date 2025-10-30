# Bloomscythe Game Controls Guide

## 🎮 Complete Control Scheme

### Movement Controls
- **W** - Move Forward
- **A** - Move Left
- **S** - Move Backward
- **D** - Move Right
- **Spacebar** - Jump
- **Left Shift** - Sprint (hold while moving)

### Camera Controls
- **Left Click + Drag** - Rotate camera and character
  - Hold left mouse button and move mouse to rotate
  - Character faces the camera direction

### Combat Controls
- **Right Click** - Primary Attack
  - Knight: Sword combo (3-hit chain)
  - Archer: Charge and release arrow
  - Mage: Cast basic spell projectile
  
- **Q** - Ability 1
  - Knight: Shield Bash (stun enemies)
  - Archer: Multi-Shot (fire multiple arrows)
  - Mage: Fireball (explosive projectile)
  
- **E** - Ability 2
  - Knight: Block (toggle defensive stance)
  - Archer: Arrow Rain (AOE attack)
  - Mage: Ice Nova (freeze nearby enemies)
  
- **R** - Ability 3
  - Knight: Shield Charge (dash forward)
  - Archer: Rapid Fire (increased fire rate)
  - Mage: Lightning Strike (high damage single target)

### Weapon Switching
- **Mouse Wheel Up/Down** - Cycle through weapons
- **1** - Equip weapon slot 1
- **2** - Equip weapon slot 2
- **3** - Equip weapon slot 3

### UI/Menu Controls
- **Escape** - Pause menu (to be implemented)
- **Tab** - Show scoreboard/stats (to be implemented)

---

## 📊 Character Classes

### 🛡️ Knight (Tank/Melee)
**Weapon:** Shield and Sword

**Stats:**
- Health: ⭐⭐⭐⭐⭐ (150% of base)
- Armor: ⭐⭐⭐⭐⭐ (150% of base)
- Speed: ⭐⭐⭐ (90% of base)
- Damage: ⭐⭐⭐⭐ (120% of base)

**Playstyle:** Close-range tank that can absorb damage and protect allies. High survivability with shield blocking.

**Abilities:**
1. **Shield Bash (Q)** - Stun enemies in a cone in front
   - Cooldown: 8 seconds
   - Damage: 25
   - Stun Duration: 2 seconds
   
2. **Block (E)** - Toggle defensive stance
   - Reduces incoming damage by 50%
   - Cannot attack while blocking
   
3. **Shield Charge (R)** - Dash forward with shield
   - Cooldown: 12 seconds
   - Knockback enemies in path

**Combat Tips:**
- Use Block to mitigate large attacks
- Shield Bash interrupts enemy attacks
- 3-hit sword combo deals increasing damage

---

### 🏹 Archer (Ranged DPS)
**Weapon:** Bow and Arrow

**Stats:**
- Health: ⭐⭐⭐ (80% of base)
- Armor: ⭐⭐⭐⭐ (100% of base)
- Speed: ⭐⭐⭐⭐⭐ (120% of base)
- Damage: ⭐⭐⭐⭐ (100% of base)

**Playstyle:** Long-range precision damage dealer. Stay at distance and kite enemies. Charge arrows for maximum damage.

**Abilities:**
1. **Multi-Shot (Q)** - Fire 3 arrows at once
   - Cooldown: 10 seconds
   - Each arrow deals 70% damage
   - 15-degree spread
   
2. **Arrow Rain (E)** - AOE attack from above
   - Cooldown: 12 seconds
   - Area damage over time
   
3. **Rapid Fire (R)** - Increased fire rate for 5 seconds
   - Cooldown: 15 seconds
   - 3x fire rate multiplier

**Combat Tips:**
- Hold left click to charge arrows (up to 200% damage)
- Keep distance from enemies
- Use Rapid Fire for burst damage
- Multi-Shot is great for groups

---

### 🔮 Mage (Magic DPS/Support)
**Weapon:** Staff (Spellcasting)

**Stats:**
- Health: ⭐⭐ (70% of base)
- Armor: ⭐⭐⭐ (50% of base)
- Speed: ⭐⭐⭐⭐ (100% of base)
- Damage: ⭐⭐⭐⭐⭐ (150% of base)

**Playstyle:** High damage magic user with area control. Manage mana to maximize spell output.

**Mana System:**
- Max Mana: 100
- Basic Spell Cost: 10 mana
- Mana Regen: 5 per second

**Abilities:**
1. **Fireball (Q)** - Explosive projectile
   - Cooldown: 8 seconds
   - Damage: 50
   - Explosion Radius: 3 meters
   
2. **Ice Nova (E)** - Freeze enemies around you
   - Cooldown: 12 seconds
   - Damage: 30
   - Radius: 5 meters
   - Slows enemies for 3 seconds
   
3. **Lightning Strike (R)** - Massive single-target damage
   - Cooldown: 15 seconds
   - Damage: 80
   - Range: 20 meters

**Combat Tips:**
- Watch your mana bar
- Use basic spells to conserve mana
- Ice Nova is great for crowd control
- Lightning Strike for high-priority targets

---

## 🎯 Combat System

### Attack Types
- **Melee** (Knight) - Close range, high defense
- **Ranged** (Archer) - Long range, requires aiming
- **Magic** (Mage) - Medium range, area effects

### Damage Types
- **Physical** - Reduced by armor
- **Magic** - Bypasses some armor
- **True** - Ignores all defenses

### Critical Hits
- 15% base critical chance
- Critical hits deal 150% damage
- Displayed with special visual effect

### Combo System (Knight Only)
- Hit 1: 15 damage
- Hit 2: 20 damage (if within 0.5s)
- Hit 3: 30 damage (if within 0.5s)
- Combo resets if you wait too long

---

## ⚙️ Input System Configuration

The game uses Unity's **New Input System**.

### Input Actions Asset Location:
`Assets/Settings/PlayerInputActions.inputactions`

### Action Map: "Player"
```
Movement
├── Move (Vector2) → WASD
├── Look (Vector2) → Mouse Delta
├── Jump (Button) → Spacebar
├── Sprint (Button) → Left Shift
└── LeftClick (Button) → Left Mouse Button

Combat
├── Attack (Button) → Right Mouse Button
├── Ability1 (Button) → Q
├── Ability2 (Button) → E
├── Ability3 (Button) → R
└── Ability4 (Button) → F

Equipment
├── WeaponSwitch (Float) → Mouse Scroll Wheel
├── Weapon1 (Button) → 1
├── Weapon2 (Button) → 2
└── Weapon3 (Button) → 3
```

---

## 🔧 Customizing Controls

### Changing Control Sensitivity
In the `PlayerController` component:
- **Camera Sensitivity** - Mouse look speed (default: 2.0)

### Changing Movement Speed
In the `MovementComponent` component:
- **Base Movement Speed** - Default walk speed (default: 5.0)
- **Sprint Multiplier** - Speed increase when sprinting (default: 1.5x)
- **Jump Height** - How high the character jumps (default: 2.0)

### Modifying Abilities
Each weapon script has its own ability settings:
- `ShieldSwordWeapon.cs` - Knight abilities
- `BowWeapon.cs` - Archer abilities
- `StaffWeapon.cs` - Mage abilities

You can adjust:
- Damage values
- Cooldown times
- Range/radius
- Effect durations

---

## 🎮 Testing Your Setup

### Quick Test Checklist

1. **Movement:**
   - [ ] WASD moves character in correct directions
   - [ ] Spacebar makes character jump
   - [ ] Shift + WASD makes character sprint

2. **Camera:**
   - [ ] Left click + drag rotates camera
   - [ ] Camera rotation is smooth
   - [ ] Character faces camera direction

3. **Combat:**
   - [ ] Right click performs attack
   - [ ] Attack animation plays
   - [ ] Enemies take damage

4. **Abilities:**
   - [ ] Q, E, R trigger abilities
   - [ ] Abilities have cooldowns
   - [ ] Ability animations play

5. **Class-Specific:**
   - [ ] Knight: 3-hit combo works
   - [ ] Archer: Arrow charging works
   - [ ] Mage: Mana regenerates

---

## 🐛 Troubleshooting

### Problem: Camera doesn't rotate when dragging
**Solution:** Check that left mouse button input is properly mapped in Input Actions

### Problem: Abilities don't work
**Solution:** 
1. Check Input Actions asset is assigned
2. Verify ability sounds/prefabs are assigned
3. Check cooldowns in Inspector

### Problem: Character doesn't move
**Solution:**
1. Check CharacterController component is present
2. Verify Ground Layer is set in MovementComponent
3. Check if character is grounded

### Problem: Animations don't play
**Solution:**
1. Verify Animator Controller is assigned
2. Check animation clips are imported correctly
3. Verify Avatar is assigned to Animator

---

## 📝 Next Steps

After importing your character models and animations:

1. ✅ Import character FBX files (see CHARACTER_IMPORT_GUIDE.md)
2. ✅ Create Animator Controllers
3. ✅ Setup character prefabs
4. ✅ Assign CharacterClass component
5. ✅ Test each class in gameplay
6. ✅ Tune ability values for balance
7. ✅ Create enemy AI
8. ✅ Build first level/dungeon

---

## 🎯 Pro Tips

1. **Camera Control:** Hold left-click and drag to look around. Use right-click to attack. This keeps controls clean and separate.

2. **Class Selection:** 
   - New players: Start with Knight (most forgiving)
   - Experienced: Archer or Mage for high skill ceiling

3. **Ability Usage:**
   - Don't waste abilities on single enemies
   - Save ultimates (R abilities) for tough fights
   - Learn ability cooldowns for optimal rotation

4. **Movement:**
   - Sprint costs no stamina, use it liberally
   - Jump to dodge AOE attacks
   - Backpedal while attacking as Archer

5. **Co-op Tips:**
   - Knight should lead and tank
   - Archer stays at medium range
   - Mage controls crowds and burst damage
   - Communication is key!

---

## 📚 Additional Resources

- **Mixamo Import Guide:** `Assets/Mixamo_Import_Guide.md`
- **Character Import Guide:** `Assets/CHARACTER_IMPORT_GUIDE.md`
- **Project Structure:** `Assets/PROJECT_STRUCTURE.md`
- **Setup Complete:** `Assets/SETUP_COMPLETE.md`

---

**Happy Gaming! 🎮⚔️🏹🔮**

