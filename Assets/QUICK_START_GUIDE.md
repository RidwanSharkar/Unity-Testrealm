# 🚀 Bloomscythe Quick Start Guide

## What's Been Set Up For You

Your Unity project now has:

✅ **3 Character Classes:**
- Knight (Shield & Sword)
- Archer (Bow & Arrow)
- Mage (Staff & Spells)

✅ **Complete Control System:**
- WASD movement
- Left-click + drag for camera rotation
- Right-click for attacks
- Spacebar for jumping
- Q, E, R for abilities

✅ **Weapon Systems:**
- Shield & Sword weapon with 3-hit combo
- Bow weapon with charge mechanics
- Staff weapon with mana system

✅ **Organized Folder Structure:**
```
Assets/
├── Models/Characters/
│   ├── Knight/
│   ├── Archer/
│   └── Mage/
├── Models/Enemies/
│   └── Mutant/
├── Animations/Characters/
│   ├── Knight/
│   ├── Archer/
│   └── Mage/
└── Animations/Enemies/
    └── Mutant/
```

---

## 📥 Step 1: Import Your Character Models

You mentioned you have FBX files for the **Mage**, **Knight**, and **Mutant** enemy.

### Where to Put Your Files:

1. **Character T-Pose Models:**
   - `Knight_TPose.fbx` → `/Assets/Models/Characters/Knight/`
   - `Mage_TPose.fbx` → `/Assets/Models/Characters/Mage/`
   - `Mutant_TPose.fbx` → `/Assets/Models/Enemies/Mutant/`

2. **Character Animations:**
   - Knight animations → `/Assets/Animations/Characters/Knight/`
     - `Knight_Idle.fbx`
     - `Knight_Jump.fbx`
     - `Knight_TurnLeft.fbx`
     - `Knight_TurnRight.fbx`
     - (any other animations)
   
   - Mage animations → `/Assets/Animations/Characters/Mage/`
     - `Mage_Idle.fbx`
     - `Mage_Jump.fbx`
     - (etc.)
   
   - Mutant animations → `/Assets/Animations/Enemies/Mutant/`
     - `Mutant_Idle.fbx`
     - `Mutant_Walk.fbx`
     - (etc.)

### Import Settings (IMPORTANT):

**For T-Pose Models:**
1. Select the FBX in Unity
2. Inspector → **Rig** tab:
   - Animation Type: `Humanoid`
   - Avatar Definition: `Create From This Model`
3. Click **Apply**

**For Animation Files:**
1. Select the animation FBX in Unity
2. Inspector → **Rig** tab:
   - Animation Type: `Humanoid`
   - Avatar Definition: `Copy From Other Avatar`
   - Source: *Select the T-pose Avatar you created*
3. Click **Apply**
4. Go to **Animation** tab:
   - ✅ Import Animation
   - Loop Time: ✅ (for Idle, Walk, Run)
   - Root Transform Rotation: `Body Orientation`
5. Click **Apply**

📖 **Detailed Instructions:** See `CHARACTER_IMPORT_GUIDE.md`

---

## 🎨 Step 2: Create Materials

Your Mixamo models don't come with textures, so you need to create materials.

### Quick Material Setup:

1. **Right-click** in `Assets/Materials/` → Create → Material
2. Name it based on character:
   - `Knight_Material`
   - `Mage_Material`
   - `Mutant_Material`
3. In Inspector:
   - **Base Color:** Pick a color for your character
   - **Metallic:** 0 (for non-metal characters)
   - **Smoothness:** 0.3-0.5
4. Drag the material onto your character model in the Scene

**Suggested Colors:**
- Knight: Steel gray (#C0C0C0) with blue accents (#1E3A8A)
- Mage: Deep purple (#6B2C91) with gold trim (#FFD700)
- Mutant: Sickly green (#7FFF00) or corrupted purple (#8B008B)

---

## 🎮 Step 3: Create Animator Controllers

Each character needs an Animator Controller to manage animation states.

### Creating an Animator Controller:

1. **Right-click** in `Assets/Animations/Characters/Knight/`
2. Create → **Animator Controller**
3. Name: `Knight_AnimatorController`
4. **Double-click** to open Animator window
5. Create states:
   - **Idle** (set as default)
   - **Walk**
   - **Run**
   - **Jump**
   - **Attack**
6. Drag animation clips into states
7. Create transitions between states:
   - Idle ↔ Walk
   - Walk ↔ Run
   - Any State → Jump

**Repeat for Mage and Mutant.**

---

## 🤖 Step 4: Create Character Prefabs

Now let's turn your models into playable characters!

### For Player Characters (Knight, Mage):

1. **Drag** the Knight T-pose model from `Assets/Models/Characters/Knight/` into the Scene
2. **Rename** it to `Player_Knight`
3. **Add Components** (Inspector → Add Component):
   ```
   - CharacterController
   - Animator
   - PlayerController
   - MovementComponent
   - HealthComponent
   - CharacterClass
   - Audio Source
   ```
4. **Configure CharacterController:**
   - Center: (0, 1, 0)
   - Radius: 0.3
   - Height: 2
5. **Configure Animator:**
   - Controller: Select `Knight_AnimatorController`
   - Avatar: Should auto-assign
6. **Configure CharacterClass:**
   - Class Type: `Knight`
   - Class Name: "Knight"
   - Health Modifier: 1.5
   - Armor Modifier: 1.5
   - Speed Modifier: 0.9
   - Damage Modifier: 1.2
7. **Add Camera** (Create child object):
   - Right-click `Player_Knight` → Create Empty
   - Name: `PlayerCamera`
   - Add Component → Camera
   - Position: (0, 1.6, 0)
8. **Assign Camera** to PlayerController:
   - In PlayerController component
   - Player Camera: Drag the `PlayerCamera` child object
9. **Save as Prefab:**
   - Drag `Player_Knight` from Hierarchy to `Assets/Prefabs/Characters/`

**Repeat for Player_Mage** with different class settings:
- Class Type: `Mage`
- Health Modifier: 0.7
- Armor Modifier: 0.5
- Speed Modifier: 1.0
- Damage Modifier: 1.5

### For Enemy (Mutant):

1. **Drag** Mutant T-pose model into Scene
2. **Rename** to `Enemy_Mutant`
3. **Add Components:**
   ```
   - NavMeshAgent (for AI movement)
   - Animator
   - BaseEnemy
   - HealthComponent
   - Capsule Collider
   - Rigidbody
   ```
4. **Configure Animator:**
   - Controller: Select `Mutant_AnimatorController`
5. **Save as Prefab:**
   - Drag to `Assets/Prefabs/Enemies/Enemy_Mutant.prefab`

---

## 🗡️ Step 5: Create Weapon Prefabs

Your character classes need their signature weapons!

### Knight - Shield & Sword:

1. Create empty GameObject: `Knight_ShieldSword`
2. Add Component: `ShieldSwordWeapon`
3. Configure in Inspector:
   - Base Damage: 15
   - Attack Range: 2.5
   - Fire Rate: 1.2
4. Add visual weapon models (if you have them)
5. Save as prefab: `Assets/Prefabs/Weapons/ShieldSword.prefab`

### Archer - Bow (for future):

1. Create empty GameObject: `Archer_Bow`
2. Add Component: `BowWeapon`
3. Configure settings
4. Save as prefab: `Assets/Prefabs/Weapons/Bow.prefab`

### Mage - Staff:

1. Create empty GameObject: `Mage_Staff`
2. Add Component: `StaffWeapon`
3. Configure settings
4. Save as prefab: `Assets/Prefabs/Weapons/Staff.prefab`

### Assign Weapons to Characters:

1. Select `Player_Knight` prefab
2. In PlayerController component:
   - Available Weapons: Set size to 1
   - Element 0: Drag `ShieldSword` prefab

---

## 🎯 Step 6: Test Your Game!

### Quick Test Setup:

1. **Create a test scene:**
   - Open `SampleScene` (or create new scene)
2. **Add ground:**
   - 3D Object → Plane
   - Scale: (10, 1, 10)
   - Position: (0, 0, 0)
3. **Add player:**
   - Drag `Player_Knight` prefab into scene
   - Position: (0, 1, 0)
4. **Add lighting:**
   - GameObject → Light → Directional Light (if not present)
5. **Press Play!**

### Test Controls:

- **WASD** - Move around
- **Spacebar** - Jump
- **Left Click + Drag** - Rotate camera
- **Right Click** - Attack
- **Q** - Shield Bash
- **E** - Block

---

## ⚠️ Common Issues & Solutions

### Issue: Character falls through ground
**Solution:** Make sure the Plane has a Collider (should be automatic)

### Issue: Camera doesn't move
**Solution:** 
1. Check PlayerCamera is assigned in PlayerController
2. Verify cursor is locked (mouseLocked = true)

### Issue: Character doesn't move
**Solution:**
1. Check CharacterController component exists
2. Verify MovementComponent is present
3. Set Ground Layer in MovementComponent to "Default"

### Issue: Animations don't play
**Solution:**
1. Check Animator Controller is assigned
2. Verify Avatar is set in Animator
3. Make sure animation clips are imported correctly

### Issue: "Can't attack" or no damage
**Solution:**
1. Weapon must be assigned in PlayerController
2. Check CombatSystem exists in scene (create empty GameObject, add CombatSystem component)

---

## 📋 Checklist

### Character Setup:
- [ ] Imported Knight T-pose and animations
- [ ] Imported Mage T-pose and animations
- [ ] Imported Mutant T-pose and animations
- [ ] Created materials for all characters
- [ ] Created Animator Controllers
- [ ] Created Player_Knight prefab
- [ ] Created Player_Mage prefab
- [ ] Created Enemy_Mutant prefab

### Weapon Setup:
- [ ] Created ShieldSword weapon prefab
- [ ] Created Staff weapon prefab
- [ ] Assigned weapons to character prefabs

### Scene Setup:
- [ ] Created test scene with ground
- [ ] Added player character
- [ ] Added lighting
- [ ] Tested movement (WASD)
- [ ] Tested camera (left-click + drag)
- [ ] Tested combat (right-click attack)
- [ ] Tested abilities (Q, E, R)

---

## 🎓 Learning Resources

### Documentation Files:
- **CHARACTER_IMPORT_GUIDE.md** - Detailed character import instructions
- **GAME_CONTROLS_GUIDE.md** - Complete control scheme and character classes
- **Mixamo_Import_Guide.md** - Mixamo-specific import guide
- **PROJECT_STRUCTURE.md** - Overall project organization

### Unity Learning:
- [Unity Manual - Character Controller](https://docs.unity3d.com/Manual/class-CharacterController.html)
- [Unity Manual - Animation](https://docs.unity3d.com/Manual/AnimationOverview.html)
- [Unity Learn - Character Control](https://learn.unity.com/)

---

## 🚀 Next Steps

Once you have the basics working:

1. **Create the Archer Class:**
   - Import Archer model and animations
   - Create Player_Archer prefab
   - Assign Bow weapon

2. **Implement Enemy AI:**
   - Create AI behavior scripts
   - Add NavMesh to scene
   - Make enemies chase and attack player

3. **Build First Level:**
   - Design dungeon/arena layout
   - Add spawn points
   - Create objective system

4. **Add Co-op Multiplayer:**
   - Use the existing Network scripts
   - Test with 2+ players
   - Implement team mechanics

5. **Polish & Effects:**
   - Add particle effects
   - Implement sound effects
   - Create UI/HUD
   - Add damage numbers

---

## 💡 Pro Tips

1. **Start Small:** Get ONE character working perfectly before moving to the next
2. **Test Often:** Press Play frequently to catch issues early
3. **Save Prefabs:** Always work with prefabs, not scene objects
4. **Organize:** Keep your folders clean and files named consistently
5. **Backup:** Commit to Git often (if using version control)

---

## 🆘 Need Help?

If you get stuck:

1. Check the detailed guides in `Assets/` folder
2. Review the Console for error messages
3. Verify all components are assigned in Inspector
4. Make sure you followed import settings correctly
5. Test each system individually

---

**You're ready to bring your characters to life! 🎮**

Import your models, follow these steps, and you'll have a playable character in no time!

Good luck, and have fun building Bloomscythe! ⚔️🏹🔮

