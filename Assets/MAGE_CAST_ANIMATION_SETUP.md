# 🔥 Mage Cast Animation Setup Guide - QUICK FIX

## Problem 1: Left-Click Not Working ❌
**Root Cause**: The Animator Controller is missing the "Cast" trigger parameter!

## Problem 2: Mage_Cast Animation Not Integrated ❌
**Root Cause**: Animation not added to Animator Controller states

---

## ✅ SOLUTION - Follow These Steps Exactly:

### **STEP 1: Add Cast Trigger to Animator Controller**

1. **Open the Animator Window**:
   - Go to `Assets/Animations/Characters/Mage/Mage_AnimatorController`
   - Double-click to open in Animator window

2. **Add the Cast Trigger**:
   - In Animator window, go to **Parameters** tab (left side)
   - Click the **+ button**
   - Select **Trigger**
   - Name it exactly: `Cast`
   
   ✅ **Your parameters should now include**:
   - MoveX (Float)
   - MoveZ (Float)
   - IsGrounded (Bool)
   - Jump (Trigger)
   - Speed (Float)
   - **Cast (Trigger)** ← NEW!

---

### **STEP 2: Import and Configure Mage_Cast Animation**

1. **Select Mage_Cast.fbx**:
   - Find: `Assets/Animations/Characters/Mage/Mage_Cast.fbx`
   - Click on it in Project window

2. **Configure Import Settings**:
   - In Inspector, go to **Rig** tab:
     - Animation Type: **Humanoid**
     - Avatar Definition: **Copy from Other Avatar**
     - Source: Select your Mage avatar (same as other animations)
   
   - Go to **Animation** tab:
     - ✓ Check "Loop Time" if you want it to loop (usually NO for casting)
     - ✓ Check "Loop Pose" (NO)
     - Click **Apply**

3. **Extract the Animation Clip**:
   - In Animation tab, you should see the clip
   - The clip should be named something like "Mage_Cast" or "Take 001"
   - Note the exact name!

---

### **STEP 3: Add Cast State to Animator**

1. **Open Animator Window** (Mage_AnimatorController)

2. **Create New State**:
   - Right-click in empty space
   - Select **Create State → Empty**
   - Name it: `Casting`

3. **Assign Animation**:
   - Select the `Casting` state
   - In Inspector, find **Motion** field
   - Click the circle button
   - Search for "Mage_Cast" or "Cast"
   - Select your cast animation clip

4. **Configure State Settings**:
   - Speed: `1` (normal speed)
   - Loop Time: **OFF** (unchecked) - we want it to play once

---

### **STEP 4: Create Transitions**

#### **A) Any State → Casting** (for instant casting from any state)

1. **Right-click on "Any State"**
2. **Make Transition** → Click on `Casting` state
3. **Configure Transition**:
   - Click the transition arrow
   - In Inspector:
     - **Conditions**: Add condition
       - Parameter: `Cast`
     - **Has Exit Time**: ✓ OFF (unchecked)
     - **Transition Duration**: `0.1` (quick blend)
     - **Interruption Source**: Current State
     - **Can Transition To Self**: ✓ ON

#### **B) Casting → Idle** (return to idle after casting)

1. **Right-click on `Casting` state**
2. **Make Transition** → Click on `Idle` state
3. **Configure Transition**:
   - Click the transition arrow
   - In Inspector:
     - **Conditions**: None (empty)
     - **Has Exit Time**: ✓ ON (checked)
     - **Exit Time**: `0.9` (near end of animation)
     - **Transition Duration**: `0.15`

#### **C) Casting → Locomotion** (if moving while casting ends)

1. **Right-click on `Casting` state**
2. **Make Transition** → Click on `Locomotion` state
3. **Configure Transition**:
   - Conditions:
     - `Speed` Greater Than `0.01`
   - **Has Exit Time**: ✓ ON
   - **Exit Time**: `0.9`
   - **Transition Duration**: `0.15`

---

### **STEP 5: Adjust Cast Timing in Code**

Now we need to match the code timing to your animation length.

1. **Find Animation Length**:
   - Select `Mage_Cast.fbx` in Project
   - Look in Inspector → Animation tab
   - Note the clip length (e.g., 0.5s, 1.0s, etc.)

2. **Update StaffWeapon Cast Time**:
   - In Unity, select your Staff Weapon GameObject
   - Find `StaffWeapon` component in Inspector
   - Set **Primary Attack Cast Time** to match your animation length
   - Example: If animation is 0.8s, set cast time to `0.8`

   **Recommendation**: Set it slightly BEFORE the animation ends
   - Animation: 1.0s → Cast Time: 0.7s (fireball spawns at 70% of animation)
   - Animation: 0.5s → Cast Time: 0.35s (fireball spawns at 70% of animation)

---

### **STEP 6: Debug Left-Click Issue**

If left-click still doesn't work after above steps:

#### **A) Add Debug Logging**

Add this temporary debug code to test:

Open `PlayerController.cs` and modify `HandleCombat()`:

```csharp
private void HandleCombat()
{
    if (currentWeapon == null)
    {
        Debug.LogWarning("No weapon equipped!");
        return;
    }
    
    // Primary Attack (Left Click) - casting attacks like fireball
    if (leftClickPressed)
    {
        Debug.Log("LEFT CLICK DETECTED! Calling PerformPrimaryAttack()");
        currentWeapon.PerformPrimaryAttack();
    }
    
    // Secondary Attack (Right Click held) - continuous attacks like beam
    if (secondaryAttackInput)
    {
        Debug.Log("RIGHT CLICK DETECTED! Calling PerformAttack()");
        currentWeapon.PerformAttack();
    }
}
```

Open `StaffWeapon.cs` and modify `PerformPrimaryAttack()`:

```csharp
public override void PerformPrimaryAttack()
{
    Debug.Log($"PerformPrimaryAttack called! isCasting={isCasting}, cooldown={Time.time - lastPrimaryAttackTime}, mana={currentMana}");
    
    // Check if we can cast (cooldown and not already casting)
    if (isCasting || Time.time - lastPrimaryAttackTime < primaryAttackCooldown)
    {
        Debug.LogWarning($"Cannot cast: isCasting={isCasting}, cooldownRemaining={primaryAttackCooldown - (Time.time - lastPrimaryAttackTime)}");
        return;
    }
    
    // ... rest of code
}
```

#### **B) Check These Common Issues**:

1. **Weapon Not Assigned**:
   - Check if Staff is in `availableWeapons` array in PlayerController
   - Check if it's equipped (should auto-equip on start)

2. **Animator Not Assigned**:
   - In StaffWeapon component, check if `Animator` field is assigned
   - Should point to the character's Animator component

3. **Spell Cast Point Missing**:
   - Create an empty GameObject as child of Staff
   - Position it at the tip of the staff
   - Assign it to `Spell Cast Point` in StaffWeapon

4. **Prefab Not Assigned**:
   - Make sure `Primary Fireball Prefab` is assigned in StaffWeapon
   - If not assigned, it will log a warning but won't spawn anything

---

### **STEP 7: Verify Setup Checklist**

Before testing, verify:

- ✅ Mage_Cast.fbx imported with Humanoid rig
- ✅ Cast trigger added to Animator Parameters
- ✅ Casting state created with Mage_Cast animation
- ✅ Transitions set up:
  - Any State → Casting (on Cast trigger)
  - Casting → Idle (on exit)
  - Casting → Locomotion (on Speed > 0.01)
- ✅ StaffWeapon cast time matches animation length
- ✅ Staff weapon has Animator reference assigned
- ✅ Spell Cast Point is assigned
- ✅ Primary Fireball Prefab is assigned (you'll create this next)

---

## 🎬 Testing the Animation

1. **Enter Play Mode**
2. **Left-Click**
3. **You should see**:
   - Console: "LEFT CLICK DETECTED!"
   - Console: "PerformPrimaryAttack called!"
   - Console: "Launched fireball!"
   - Character plays casting animation
   - After cast time, fireball spawns

---

## 🎯 Expected Timeline

When you left-click:
```
0.0s - Left click detected
0.0s - "Cast" trigger fired
0.0s - Casting animation starts playing
0.0s - Casting VFX spawns at staff tip (optional)
0.5s - (or your cast time) Fireball spawns
0.5s - Casting VFX destroyed
0.8s - Animation can be interrupted by movement
1.0s - Animation fully completes
1.0s - Return to Idle or Locomotion
```

---

## 📊 Animator State Machine Visual

```
┌──────────────┐
│  Any State   │
│              │
└──────┬───────┘
       │ Cast trigger
       │ (instant, no exit time)
       ↓
┌──────────────┐
│   Casting    │──────┐
│ (Mage_Cast)  │      │ Speed > 0.01
└──────┬───────┘      │ + Exit Time
       │              │
       │ Exit Time    ↓
       ↓         ┌──────────────┐
┌──────────────┐ │  Locomotion  │
│     Idle     │ │  (Movement)  │
└──────────────┘ └──────────────┘
```

---

## 🔧 Advanced: Animation Events (Optional)

For even more precise timing, use Animation Events:

1. **Open Animation Window**: Window → Animation
2. **Select your Mage GameObject** in scene (with animator)
3. **Select Mage_Cast clip** in Animation window
4. **Add Event** at the frame where fireball should spawn:
   - Click the timeline at 70% mark
   - Click **Add Event** button
   - Function: Type `OnFireballSpawn`

5. **Add method to StaffWeapon.cs**:
```csharp
// Called by animation event
public void OnFireballSpawn()
{
    if (isCasting)
    {
        SpawnPrimaryFireball();
    }
}
```

6. **Modify CastPrimaryFireball coroutine**:
```csharp
private IEnumerator CastPrimaryFireball()
{
    isCasting = true;
    
    PlayAnimation("Cast");
    PlaySound(attackSound);
    
    GameObject castEffect = null;
    if (castingEffect != null && spellCastPoint != null)
    {
        castEffect = Instantiate(castingEffect, spellCastPoint.position, spellCastPoint.rotation, spellCastPoint);
    }
    
    // Wait for full animation (event will spawn fireball)
    yield return new WaitForSeconds(primaryAttackCastTime);
    
    // If event didn't fire (no animation events), spawn manually
    if (isCasting)
    {
        SpawnPrimaryFireball(); // Fallback
    }
    
    if (castEffect != null)
    {
        Destroy(castEffect);
    }
    
    isCasting = false;
}
```

---

## 🐛 Still Not Working? Debug Steps:

### Test 1: Basic Input Detection
```csharp
// Add to PlayerController Update():
if (Input.GetMouseButtonDown(0))
{
    Debug.Log("RAW LEFT CLICK DETECTED!");
}
```

### Test 2: Weapon Check
```csharp
// Add to PlayerController Update():
Debug.Log($"Current Weapon: {(currentWeapon != null ? currentWeapon.WeaponName : "NONE")}");
```

### Test 3: Animator Check
```csharp
// Add to StaffWeapon Start():
if (animator == null)
{
    Debug.LogError("ANIMATOR IS NULL! Cannot play animations!");
    animator = GetComponentInChildren<Animator>();
    if (animator == null)
    {
        Debug.LogError("Still no animator found!");
    }
}
```

### Test 4: Manual Trigger Test
In Update() of StaffWeapon, add:
```csharp
if (Input.GetKeyDown(KeyCode.T)) // Test with T key
{
    Debug.Log("Testing Cast animation manually");
    PlayAnimation("Cast");
}
```

---

## 📝 Quick Reference: What Changed

### Code Files Modified:
1. **PlayerController.cs**:
   - Changed `Input.GetAxis()` to `Input.GetAxisRaw()` for responsive input
   - Added third parameter to `Move()` call: `moveInput`
   - Separated left-click (primary) and right-click (secondary) attacks

2. **MovementComponent.cs**:
   - Updated `Move()` signature to accept optional `Vector2 input` parameter
   - Stores raw input for animation system

3. **StaffWeapon.cs**:
   - Calls `PlayAnimation("Cast")` trigger
   - Uses coroutine for timing
   - Spawns fireball after cast time

### Unity Setup Needed:
1. Add "Cast" trigger to Animator Controller ← **CRITICAL!**
2. Create "Casting" state with Mage_Cast animation
3. Set up transitions (Any State → Casting, Casting → Idle/Locomotion)
4. Assign all references in StaffWeapon component

---

## ⚡ Quick Setup (5 Minutes)

1. **Animator**: Add "Cast" trigger parameter
2. **Animator**: Create "Casting" state with Mage_Cast animation
3. **Animator**: Create transition: Any State → Casting (on Cast trigger, no exit time)
4. **Animator**: Create transition: Casting → Idle (with exit time)
5. **StaffWeapon**: Set cast time to match animation length (start with 0.5)
6. **Test**: Press Play and left-click

---

That's it! After following these steps, your left-click should play the casting animation and spawn a fireball! 🔥

Let me know if you get stuck on any step!

