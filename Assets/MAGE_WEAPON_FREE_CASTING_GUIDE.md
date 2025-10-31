# 🔥 Mage Weapon-Free Casting Setup Guide

## Overview
This guide explains how to set up the Mage character to cast spells directly without using a weapon/staff. The mage will cast spells from their hands/body using the new `MageSpellCaster` component.

---

## ✅ What Changed

### **Before**: Weapon-Based Casting ❌
- Mage needed a Staff weapon GameObject
- Required StaffWeapon component with Animator reference
- Animator was on Staff, not character
- Complex setup with many dependencies

### **After**: Direct Spell Casting ✅
- No weapon required!
- Mage casts spells directly from hands/body
- Simpler setup, cleaner architecture
- Animator on character (where it should be)

---

## 🎯 Quick Setup (5 Minutes)

### **Step 1: Add MageSpellCaster Component**

1. **Select your Mage character** in Hierarchy (root GameObject)
2. **Click "Add Component"**
3. **Type**: `MageSpellCaster`
4. **Add it**

That's it for the basic setup! The component will auto-detect:
- ✅ Animator (on character)
- ✅ Entity component
- ✅ AudioSource (creates one if missing)
- ✅ Cast point (creates at chest height)

---

### **Step 2: Configure Inspector Settings**

Select your Mage character and find the **MageSpellCaster** component:

#### **Auto-Detected (No Action Needed):**
- ✓ Animator - auto-found on character
- ✓ Owner Entity - auto-found
- ✓ Chest Cast Point - auto-created at chest height

#### **Required Settings:**
1. **Primary Fireball Prefab**: Assign your fireball prefab
   - If you don't have one yet, leave empty (animation will still work)
   
2. **Primary Attack Cast Time**: `0.5` (or your Mage_Cast animation length)
   - Check animation length in Assets/Animations/Characters/Mage/Mage_Cast.fbx
   
3. **Primary Attack Cooldown**: `0.8` (time between casts)

#### **Optional Settings:**
- **Left Hand Cast Point**: Empty GameObject at left hand bone (optional)
- **Right Hand Cast Point**: Empty GameObject at right hand bone (optional)
- **Casting Effect**: VFX prefab that plays during cast (optional)
- **Current Mana**: `100` (or uncheck "Use Mana" to disable mana system)

---

### **Step 3: Verify Animator Setup**

Make sure your Animator Controller has the Cast trigger:

1. **Open Animator Window**: Window → Animation → Animator
2. **Double-click**: `Assets/Animations/Characters/Mage/Mage_AnimatorController.controller`
3. **Check Parameters tab**: Must have `Cast` trigger parameter
4. **Check States**: Must have `Casting` state with Mage_Cast animation
5. **Check Transitions**:
   - Any State → Casting (on Cast trigger)
   - Casting → Idle (automatic return)

*If these are missing, see the detailed Animator setup section below.*

---

### **Step 4: Test in Play Mode**

1. **Enter Play Mode**
2. **Left-Click** → Character plays casting animation
3. **After 0.5s** → Fireball spawns (if prefab assigned)
4. **Check Console** for:
   ```
   [PlayerController] Left-click detected! Calling spell caster PerformPrimaryAttack()
   [MageSpellCaster] PerformPrimaryAttack called! isCasting=False, cooldownRemaining=0, mana=100/100
   [MageSpellCaster] Starting fireball cast!
   [MageSpellCaster] Playing 'Cast' animation trigger
   [MageSpellCaster] Triggered animation: Cast
   [MageSpellCaster] Waiting 0.5 seconds for cast animation...
   [MageSpellCaster] Cast complete! Spawning fireball now...
   [MageSpellCaster] ✓ Fireball launched successfully! (15 damage)
   ```

---

## 🎮 Controls

### **Primary Attack (Left-Click)**
- Casts fireball with animation
- Costs 10 mana (if enabled)
- 0.8 second cooldown

### **Abilities**
- **Q** - Explosive Fireball (AOE damage)
- **E** - Ice Nova (freeze enemies around you)
- **R** - Lightning Strike (high single-target damage)

---

## 🔧 Detailed Configuration

### **Cast Point Options**

The spell can cast from different points. Priority order:
1. **Right Hand Cast Point** (if assigned) - most accurate
2. **Left Hand Cast Point** (if assigned)
3. **Chest Cast Point** (auto-created)
4. **Character Center** (fallback)

#### To Set Up Hand Cast Points (Optional):

1. **Find hand bone in Hierarchy**:
   - Expand your Mage character
   - Find skeleton/armature
   - Locate right hand bone (usually "mixamorig:RightHand" or similar)

2. **Create empty GameObject as child**:
   - Right-click hand bone → Create Empty
   - Name it "RightHandCastPoint"
   - Position it slightly forward of the palm

3. **Assign to MageSpellCaster**:
   - Select Mage character
   - Drag RightHandCastPoint into "Right Hand Cast Point" field

4. **Repeat for left hand** (optional)

---

### **Fireball Prefab Setup**

If you don't have a fireball prefab yet:

1. **Create GameObject**: Name it "MageFireballPrimary"
2. **Add Components**:
   - Rigidbody (Use Gravity: OFF, Collision Detection: Continuous Dynamic)
   - Sphere Collider (Radius: 0.5, Is Trigger: OFF)
   - `MageFireballProjectile` script
   - Trail Renderer (optional, for trail effect)

3. **Add Visuals**:
   - Child GameObject with sphere mesh or particle system
   - Point Light (orange/red color)

4. **Configure MageFireballProjectile**:
   - Speed: 25
   - Lifetime: 5
   - Fire Light: Drag the Light component
   - Fireball Scale: 1

5. **Save as Prefab**: Drag to Assets/Prefabs/

6. **Assign to MageSpellCaster**: Drag into "Primary Fireball Prefab" field

*For detailed fireball setup, see MAGE_PRIMARY_ATTACK_GUIDE.md*

---

### **Mana System**

The mana system is enabled by default:

- **Max Mana**: 100
- **Starting Mana**: 100
- **Primary Attack Cost**: 10 mana
- **Regen Rate**: 5 mana per second

**To Disable Mana**:
- Uncheck "Use Mana" in MageSpellCaster component
- Spells will have no mana cost

**To Adjust Mana**:
- Change "Max Mana" for larger mana pool
- Change "Basic Spell Mana Cost" for attack cost
- Change "Mana Regen Rate" for faster/slower regen

---

### **Cooldown System**

Each spell has its own cooldown:

- **Primary Attack**: 0.8s (adjustable)
- **Fireball Ability (Q)**: 8s
- **Ice Nova (E)**: 12s
- **Lightning Strike (R)**: 15s

Adjust these in the Inspector to balance gameplay.

---

## 🎬 Animation Setup (If Not Already Done)

### **Step 1: Import Mage_Cast Animation**

1. **Select**: `Assets/Animations/Characters/Mage/Mage_Cast.fbx`
2. **Inspector → Rig Tab**:
   - Animation Type: **Humanoid**
   - Avatar Definition: **Copy from Other Avatar**
   - Source: Your Mage avatar
   - Click **Apply**

3. **Inspector → Animation Tab**:
   - Uncheck "Loop Time" (we want one-shot animation)
   - Click **Apply**
   - Note the clip length (e.g., 0.5s, 1.0s)

---

### **Step 2: Add Cast Parameter to Animator**

1. **Open Animator**: Assets/Animations/Characters/Mage/Mage_AnimatorController.controller
2. **Parameters Tab** (left side):
   - Click **+** button
   - Select **Trigger**
   - Name: `Cast`
   - Press Enter

---

### **Step 3: Create Casting State**

1. **In Animator window**:
   - Right-click empty space → Create State → Empty
   - Name: `Casting`

2. **Select Casting state**:
   - Inspector → Motion field
   - Click circle button → Search "Mage_Cast"
   - Select Mage_Cast animation

3. **Uncheck Loop Time** in state settings

---

### **Step 4: Create Transitions**

#### **Transition 1: Any State → Casting**

1. Right-click "Any State" → Make Transition → Click "Casting"
2. Click the transition arrow, configure:
   - **Conditions**: Add condition → `Cast` trigger
   - **Has Exit Time**: ❌ UNCHECK
   - **Transition Duration**: `0.1`
   - **Interruption Source**: Current State

#### **Transition 2: Casting → Idle**

1. Right-click "Casting" → Make Transition → Click "Idle"
2. Click the transition arrow, configure:
   - **Conditions**: None (leave empty)
   - **Has Exit Time**: ✅ CHECK
   - **Exit Time**: `0.9`
   - **Transition Duration**: `0.15`

#### **Transition 3: Casting → Locomotion** (optional)

1. Right-click "Casting" → Make Transition → Click "Locomotion"
2. Configure:
   - **Conditions**: `Speed` Greater Than `0.01`
   - **Has Exit Time**: ✅ CHECK
   - **Exit Time**: `0.8`

---

## 🐛 Troubleshooting

### **Issue: Animation doesn't play**

**Solutions**:
1. ✓ Check Animator is assigned to character (not a weapon)
2. ✓ Verify "Cast" trigger exists in Animator Parameters
3. ✓ Verify "Casting" state exists with Mage_Cast animation
4. ✓ Check transitions are set up correctly
5. ✓ Make sure MageSpellCaster found the Animator (check console on start)

### **Issue: No console messages when left-clicking**

**Solutions**:
1. ✓ Make sure MageSpellCaster component is added to character
2. ✓ Check you're clicking in Game view (not Scene view)
3. ✓ Verify PlayerController exists on character
4. ✓ Cursor should be locked (invisible) in play mode

### **Issue: Animation plays but no fireball**

**Solution**: This is expected if Primary Fireball Prefab is not assigned yet
- The casting animation should still work fine
- Create and assign fireball prefab to spawn projectiles

### **Issue: "Already casting!" warning**

**Solution**: Working as intended - can't cast while already casting
- Wait for current cast to finish
- Default cast time is 0.5 seconds

### **Issue: "On cooldown!" warning**

**Solution**: Working as intended - cooldown prevents spam
- Wait 0.8 seconds between casts (default cooldown)
- Adjust "Primary Attack Cooldown" in Inspector if needed

### **Issue: "Not enough mana!" warning**

**Solutions**:
1. Wait for mana to regenerate (5 per second)
2. Reduce "Basic Spell Mana Cost" in Inspector
3. Increase "Max Mana" or "Mana Regen Rate"
4. Disable mana system by unchecking "Use Mana"

---

## 🔄 Migration from StaffWeapon

If you previously had a Staff weapon setup:

### **Step 1: Add MageSpellCaster**
- Add MageSpellCaster component to your Mage character

### **Step 2: Copy Settings**
- Copy these values from StaffWeapon to MageSpellCaster:
  - Primary Fireball Prefab
  - Primary Attack Cast Time
  - Primary Attack Cooldown
  - Casting Effect
  - Mana settings

### **Step 3: Remove Old Weapon**
- Remove the Staff GameObject (or disable it)
- Clear the "Available Weapons" array in PlayerController
- Delete StaffWeapon component

### **Step 4: Test**
- MageSpellCaster will now handle all spell casting
- No weapon needed!

---

## 📊 Architecture

### **Component Flow**:
```
PlayerController
    ↓ (detects MageSpellCaster component)
    ↓ (left-click input)
MageSpellCaster.PerformPrimaryAttack()
    ↓ (checks cooldown & mana)
    ↓ (starts coroutine)
CastPrimaryFireball()
    ↓ (plays Cast animation)
    ↓ (waits for cast time)
SpawnPrimaryFireball()
    ↓ (instantiates fireball prefab)
    ↓ (initializes with damage/direction)
MageFireballProjectile
    ↓ (flies forward)
    ↓ (detects collision)
    ↓ (applies damage via CombatSystem)
```

### **Priority System**:
PlayerController checks in this order:
1. **MageSpellCaster** (if exists) → Use weapon-free casting
2. **CurrentWeapon** (if exists) → Use weapon-based combat
3. **None** → Show warning

This means weapon and weapon-free systems can coexist!

---

## ✅ Checklist Before Testing

- [ ] MageSpellCaster component added to Mage character
- [ ] Animator assigned (auto-detected from character)
- [ ] Entity assigned (auto-detected)
- [ ] "Cast" trigger exists in Animator Parameters
- [ ] "Casting" state exists with Mage_Cast animation
- [ ] Transitions set up (Any State → Casting, Casting → Idle)
- [ ] Primary Attack Cast Time matches animation length
- [ ] Current Mana > 0 (or Use Mana disabled)
- [ ] Primary Fireball Prefab assigned (optional for testing animation)

---

## 🎯 Expected Behavior

When everything is working:

1. **Left-Click** in game
2. **Console shows**: "Left-click detected! Calling spell caster..."
3. **Animation plays**: Mage performs casting animation
4. **VFX spawns**: Casting effect at chest/hand (if assigned)
5. **After cast time**: Fireball spawns and flies forward
6. **Animation ends**: Returns to Idle or Locomotion
7. **Cooldown**: Can cast again after 0.8 seconds

---

## 🎨 Customization

### **Cast Time**:
- Instant: `0.1` - `0.2` seconds
- Quick: `0.3` - `0.5` seconds
- Slow: `0.8` - `1.2` seconds

### **Cooldown**:
- Rapid Fire: `0.3` - `0.5` seconds
- Balanced: `0.8` - `1.2` seconds
- Powerful: `2` - `3` seconds

### **Damage**:
- Low: `10` - `15` damage
- Medium: `20` - `30` damage
- High: `40` - `50` damage

### **Mana Cost**:
- Cheap: `5` - `10` mana
- Medium: `15` - `20` mana
- Expensive: `25` - `30` mana

---

## 📝 Files Created/Modified

### **New Files**:
- `Assets/Scripts/Weapons/MageSpellCaster.cs` - Weapon-free spell casting
- `Assets/MAGE_WEAPON_FREE_CASTING_GUIDE.md` - This guide

### **Modified Files**:
- `Assets/Scripts/Core/PlayerController.cs` - Added support for MageSpellCaster

### **Old Files (No Longer Needed for Mage)**:
- `Assets/Scripts/Weapons/StaffWeapon.cs` - Keep for reference, not used by Mage

---

## 🚀 Advantages of Weapon-Free System

✅ **Simpler Setup**: No need for weapon GameObject
✅ **Cleaner Hierarchy**: Mage is just the character, no child weapons
✅ **Better Animation**: Animator is on character where it belongs
✅ **More Flexible**: Can add multiple cast points (hands, chest, etc.)
✅ **Easier to Debug**: All logic in one component
✅ **More Intuitive**: Mages cast spells, not swing weapons!

---

## 🎓 Next Steps

1. ✅ Add MageSpellCaster component
2. ✅ Verify Animator setup
3. ✅ Test left-click casting
4. ⏭ Create fireball prefab (optional)
5. ⏭ Add hand cast points (optional)
6. ⏭ Create casting VFX (optional)
7. ⏭ Customize damage/cooldown values
8. ⏭ Add ability spell prefabs for Q/E/R

---

Good luck with your weapon-free mage casting! 🔥✨

**Questions?** Check the console logs - they're very detailed and will tell you exactly what's happening!

