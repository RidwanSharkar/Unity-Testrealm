# Character & Animation Import Guide

## 📁 Folder Structure

Your character files should be organized as follows:

```
Assets/
├── Models/
│   ├── Characters/
│   │   ├── Knight/
│   │   │   ├── Knight_TPose.fbx          (T-pose reference)
│   │   │   └── Knight.fbx                (Rigged character model)
│   │   ├── Archer/
│   │   │   ├── Archer_TPose.fbx
│   │   │   └── Archer.fbx
│   │   └── Mage/
│   │       ├── Mage_TPose.fbx
│   │       └── Mage.fbx
│   └── Enemies/
│       └── Mutant/
│           ├── Mutant_TPose.fbx
│           └── Mutant.fbx
│
└── Animations/
    ├── Characters/
    │   ├── Knight/
    │   │   ├── Knight_Idle.fbx
    │   │   ├── Knight_Walk.fbx
    │   │   ├── Knight_Run.fbx
    │   │   ├── Knight_Jump.fbx
    │   │   ├── Knight_Attack01.fbx
    │   │   ├── Knight_Attack02.fbx
    │   │   ├── Knight_TurnLeft.fbx
    │   │   └── Knight_TurnRight.fbx
    │   ├── Archer/
    │   │   └── (same structure as Knight)
    │   └── Mage/
    │       └── (same structure as Knight)
    └── Enemies/
        └── Mutant/
            ├── Mutant_Idle.fbx
            ├── Mutant_Walk.fbx
            ├── Mutant_Attack.fbx
            └── (other animations)
```

## 📥 Step-by-Step Import Process

### 1. Import Character Models (T-Pose)

1. **Download from Mixamo:**
   - Select character (Knight, Archer, or Mage)
   - Choose **"FBX for Unity"** format
   - Select **T-pose**
   - Download

2. **Import to Unity:**
   - Drag the FBX file into the appropriate character folder:
     - Knight T-pose → `Assets/Models/Characters/Knight/`
     - Archer T-pose → `Assets/Models/Characters/Archer/`
     - Mage T-pose → `Assets/Models/Characters/Mage/`
     - Mutant T-pose → `Assets/Models/Enemies/Mutant/`

3. **Configure Import Settings:**
   - Select the imported FBX in Unity
   - In Inspector, go to **Rig** tab:
     - Animation Type: **Humanoid**
     - Avatar Definition: **Create From This Model**
     - Click **Apply**

### 2. Import Animations

1. **Download from Mixamo:**
   - Select your character (must match the character model)
   - Choose an animation (Idle, Walk, Jump, Attack, etc.)
   - **IMPORTANT:** Check "In Place" for locomotion animations (Idle, Walk)
   - **IMPORTANT:** Uncheck "In Place" for Jump and Attack animations
   - Format: **FBX for Unity**
   - Download

2. **Import to Unity:**
   - Drag each animation FBX into the appropriate animation folder:
     - Knight animations → `Assets/Animations/Characters/Knight/`
     - Archer animations → `Assets/Animations/Characters/Archer/`
     - Mage animations → `Assets/Animations/Characters/Mage/`
     - Mutant animations → `Assets/Animations/Enemies/Mutant/`

3. **Configure Animation Import Settings:**
   - Select the animation FBX
   - In Inspector, go to **Rig** tab:
     - Animation Type: **Humanoid**
     - Avatar Definition: **Copy From Other Avatar**
     - Source: Select the T-pose Avatar you created earlier
     - Click **Apply**
   
   - Go to **Animation** tab:
     - ✅ Import Animation
     - Check the animation clip settings:
       - Loop Time: ✅ (for Idle, Walk, Run)
       - Loop Pose: ✅
       - Root Transform Rotation: Based On: **Body Orientation**
       - Root Transform Position (Y): Based On: **Original**
       - Root Transform Position (XZ): Based On: **Center of Mass**
     - Click **Apply**

### 3. Extract Animation Clips

For each animation FBX file:

1. Select the animation FBX in Project window
2. Expand it (click the arrow) to see the animation clip inside
3. The animation clip can be used in Animator Controllers

**Naming Convention:**
- Keep the default naming or rename to match your setup:
  - `Knight_Idle`, `Knight_Walk`, `Knight_Jump`, etc.

## 🎨 Creating Materials for Characters

### Knight (Shield & Sword)
```
Assets/Materials/Characters/
├── Knight_Armor.mat      (Metallic: 0.5, Smoothness: 0.6)
├── Knight_Cloth.mat      (Metallic: 0, Smoothness: 0.3)
└── Knight_Sword.mat      (Metallic: 0.8, Smoothness: 0.7)
```

**Suggested Colors:**
- Armor: Silver/Steel gray (#C0C0C0)
- Cloth: Deep blue (#1E3A8A)
- Sword: Metallic steel (#D0D0D0)

### Archer (Bow & Arrow)
```
Assets/Materials/Characters/
├── Archer_Leather.mat    (Metallic: 0, Smoothness: 0.4)
├── Archer_Cloth.mat      (Metallic: 0, Smoothness: 0.3)
└── Archer_Bow.mat        (Metallic: 0, Smoothness: 0.2)
```

**Suggested Colors:**
- Leather: Dark brown (#5C3317)
- Cloth: Forest green (#228B22)
- Bow: Wood brown (#8B4513)

### Mage (Spellcaster)
```
Assets/Materials/Characters/
├── Mage_Robe.mat         (Metallic: 0, Smoothness: 0.5)
├── Mage_Trim.mat         (Metallic: 0.3, Smoothness: 0.6)
└── Mage_Staff.mat        (Metallic: 0.2, Smoothness: 0.5, Emission)
```

**Suggested Colors:**
- Robe: Deep purple (#6B2C91)
- Trim: Gold (#FFD700)
- Staff: Dark wood with glowing blue gem (Emission: #00BFFF)

### Mutant Enemy
```
Assets/Materials/Enemies/
├── Mutant_Skin.mat       (Metallic: 0, Smoothness: 0.3)
└── Mutant_Details.mat    (Metallic: 0, Smoothness: 0.4)
```

**Suggested Colors:**
- Skin: Sickly green (#7FFF00) or corrupted purple (#8B008B)
- Details: Dark gray (#3C3C3C)

## 🎮 Setting Up Animator Controllers

### Create Animator Controller for Each Character

1. **Create Controller:**
   - Right-click in `Assets/Animations/Characters/Knight/`
   - Create → Animator Controller
   - Name: `Knight_AnimatorController`
   - Repeat for Archer, Mage, and Mutant

2. **Setup States:**
   - Open the Animator Controller (double-click)
   - Create states:
     - **Idle** (default state)
     - **Walk**
     - **Run**
     - **Jump**
     - **Attack01**
     - **Attack02**
     - **Turn Left**
     - **Turn Right**

3. **Add Parameters:**
   - Float: `Speed` (0-1 for walk/run blend)
   - Bool: `IsGrounded`
   - Trigger: `Jump`
   - Trigger: `Attack`
   - Trigger: `TurnLeft`
   - Trigger: `TurnRight`

4. **Create Transitions:**
   - Idle → Walk: Condition `Speed > 0.1`
   - Walk → Idle: Condition `Speed < 0.1`
   - Walk → Run: Condition `Speed > 0.5`
   - Any State → Jump: Condition `Jump` trigger
   - Jump → Idle: Condition `IsGrounded = true`
   - Any State → Attack: Condition `Attack` trigger

## 🎯 Character Prefab Setup

### For Each Character (Knight, Archer, Mage):

1. **Create Character Prefab:**
   - Drag the T-pose model into the Scene
   - Rename: `Player_Knight`, `Player_Archer`, or `Player_Mage`

2. **Add Components:**
   ```
   Components:
   ├── CharacterController
   ├── Animator (assign AnimatorController)
   ├── PlayerController (script)
   ├── MovementComponent (script)
   ├── HealthComponent (script)
   ├── CharacterClass (new script - see below)
   └── Audio Source
   ```

3. **Configure CharacterController:**
   - Center: (0, 1, 0)
   - Radius: 0.3
   - Height: 2
   - Slope Limit: 45
   - Step Offset: 0.3

4. **Configure Animator:**
   - Controller: Select the appropriate AnimatorController
   - Avatar: Should auto-assign from the model
   - Apply Root Motion: ✅ (for animations that move the character)

5. **Add Camera:**
   - Add a child object: `PlayerCamera`
   - Position: (0, 1.6, 0) - at head height
   - Add Camera component
   - Tag: "MainCamera"

6. **Save as Prefab:**
   - Drag from Hierarchy to `Assets/Prefabs/Characters/`
   - Name: `Player_Knight.prefab`

### For Enemy (Mutant):

1. **Create Enemy Prefab:**
   - Drag Mutant model into Scene
   - Rename: `Enemy_Mutant`

2. **Add Components:**
   ```
   Components:
   ├── NavMeshAgent (for AI movement)
   ├── Animator
   ├── BaseEnemy (script)
   ├── HealthComponent (script)
   └── Audio Source
   ```

3. **Save as Prefab:**
   - Drag to `Assets/Prefabs/Enemies/Enemy_Mutant.prefab`

## ⚡ Quick Setup Checklist

### Knight Setup:
- [ ] Import Knight T-pose model → `Models/Characters/Knight/`
- [ ] Configure as Humanoid rig
- [ ] Import all Knight animations → `Animations/Characters/Knight/`
- [ ] Link animations to Knight Avatar
- [ ] Create Knight_AnimatorController
- [ ] Create Knight materials (Armor, Cloth, Sword)
- [ ] Create Player_Knight prefab with all components
- [ ] Assign Knight_AnimatorController to Animator
- [ ] Create Shield and Sword weapon prefabs

### Archer Setup:
- [ ] Import Archer T-pose model → `Models/Characters/Archer/`
- [ ] Configure as Humanoid rig
- [ ] Import all Archer animations → `Animations/Characters/Archer/`
- [ ] Link animations to Archer Avatar
- [ ] Create Archer_AnimatorController
- [ ] Create Archer materials (Leather, Cloth, Bow)
- [ ] Create Player_Archer prefab with all components
- [ ] Assign Archer_AnimatorController to Animator
- [ ] Create Bow weapon prefab

### Mage Setup:
- [ ] Import Mage T-pose model → `Models/Characters/Mage/`
- [ ] Configure as Humanoid rig
- [ ] Import all Mage animations → `Animations/Characters/Mage/`
- [ ] Link animations to Mage Avatar
- [ ] Create Mage_AnimatorController
- [ ] Create Mage materials (Robe, Trim, Staff)
- [ ] Create Player_Mage prefab with all components
- [ ] Assign Mage_AnimatorController to Animator
- [ ] Create Staff/Spell effect prefabs

### Mutant Setup:
- [ ] Import Mutant T-pose model → `Models/Enemies/Mutant/`
- [ ] Configure as Humanoid rig
- [ ] Import all Mutant animations → `Animations/Enemies/Mutant/`
- [ ] Link animations to Mutant Avatar
- [ ] Create Mutant_AnimatorController
- [ ] Create Mutant materials
- [ ] Create Enemy_Mutant prefab with all components

## 🔧 Common Animations You'll Need

### Player Characters (Knight, Archer, Mage):
- **Locomotion:** Idle, Walk, Run, Turn Left, Turn Right
- **Combat:** Attack01, Attack02, Attack03 (combo attacks)
- **Movement:** Jump, Fall, Land
- **Abilities:** Cast Spell, Shield Block, Dodge Roll
- **Reactions:** Hit Reaction, Death

### Enemy (Mutant):
- **Locomotion:** Idle, Walk, Run
- **Combat:** Attack01, Attack02, Claw Swipe
- **Reactions:** Hit Reaction, Stagger, Death
- **Special:** Roar, Howl

## 💡 Pro Tips

1. **Animation Consistency:** Use animations from the same character for consistency in proportions
2. **In Place vs Root Motion:** 
   - Use "In Place" for idle, walk, run (controlled by script)
   - Don't use "In Place" for jump, attack (uses root motion)
3. **Avatar Reuse:** Once you create an Avatar from the T-pose, reuse it for all animations of that character
4. **Testing:** Test each animation individually before setting up the full Animator Controller
5. **Naming:** Keep consistent naming conventions for easy management

## 📝 Next Steps After Import

1. ✅ Import all models and animations
2. ✅ Create materials and apply to characters
3. ✅ Setup Animator Controllers
4. ✅ Create character prefabs
5. ✅ Assign CharacterClass scripts (see Scripts/Characters/)
6. ✅ Test in game scene with controls:
   - WASD: Movement
   - Left Click + Drag: Camera rotation
   - Right Click: Attack
   - Spacebar: Jump
   - Q, E, R: Abilities

