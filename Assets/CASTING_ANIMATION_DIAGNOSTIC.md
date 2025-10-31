# 🔥 CASTING ANIMATION DIAGNOSTIC CHECKLIST

## Issue: Left-Click Attack Not Showing Cast Animation or Spell Effect

Based on your setup, here's a systematic diagnostic to identify the exact problem:

---

## ✅ STEP-BY-STEP DIAGNOSTIC

### 1️⃣ Test Left-Click Detection

**Goal**: Confirm left-click input is being received

1. **Enter Play Mode**
2. **Left-Click** in the Game view
3. **Check Console** for this message:
   ```
   [PlayerController] Left-click detected! Calling PerformPrimaryAttack() on Mage's Staff
   ```

**If you see this** ✅ → Input works, go to Step 2
**If you DON'T see this** ❌ → Problem is with input or weapon assignment:
   - Check PlayerController's `availableWeapons` array has Staff in slot 0
   - Make sure you're clicking in **Game view**, not Scene view
   - Check cursor is locked (should be invisible in play mode)

---

### 2️⃣ Verify Staff Weapon Component References

**Goal**: Ensure all Inspector fields are assigned correctly

Select your **Staff** GameObject (child of Mage character), check the **StaffWeapon** component:

#### Required References:
- ☐ **Animator**: Should reference the Mage character's Animator component
  - **How to fix**: Drag the Animator from your Mage character (usually on root of character)
  
- ☐ **Owner Entity**: Should reference the Mage's Entity component
  - **How to fix**: Drag the Entity from your Mage character
  
- ☐ **Spell Cast Point**: Empty GameObject positioned at staff tip
  - **How to fix**: 
    1. Create empty GameObject as child of Staff
    2. Name it "SpellCastPoint"
    3. Position it at the tip/end of the staff model
    4. Drag into this field

- ☐ **Primary Fireball Prefab**: Your fireball prefab (can be null for testing animation only)
  - **Note**: If null, you'll see the animation but no projectile

#### Settings to Check:
- **Primary Attack Cast Time**: Should match your animation length (try `0.5` first)
- **Primary Attack Cooldown**: Prevents spam (try `0.8`)
- **Use Mana**: If enabled, check you have enough mana
- **Basic Spell Mana Cost**: Default `10`
- **Current Mana**: Should be > 0 (default `100`)

---

### 3️⃣ Verify Animator Controller Setup

**Goal**: Ensure the Mage's Animator Controller has the Cast trigger and state

1. **Open Animator Window**: 
   - Window → Animation → Animator
   - Or double-click: `Assets/Animations/Characters/Mage/Mage_AnimatorController.controller`

2. **Check Parameters Tab** (left panel):
   - ☐ Must have a **Trigger** parameter named exactly `Cast`
   - **How to add**: Click + → Trigger → Name: "Cast"

3. **Check States** (in the grid):
   - ☐ Must have a state named `Casting` with the Mage_Cast animation assigned
   - **How to add**:
     1. Right-click → Create State → Empty
     2. Name it "Casting"
     3. Click on it, in Inspector find "Motion" field
     4. Click circle button, search for "Mage_Cast"
     5. Uncheck "Loop Time"

4. **Check Transitions**:
   - ☐ **Any State → Casting**:
     - Condition: `Cast` trigger
     - Has Exit Time: **OFF**
     - Transition Duration: `0.1`
   
   - ☐ **Casting → Idle** (or Locomotion):
     - Condition: None
     - Has Exit Time: **ON**
     - Exit Time: `0.9`
     - Transition Duration: `0.15`

---

### 4️⃣ Test Animation Manually

**Goal**: Test if animator responds to Cast trigger at all

Add this temporary test to `StaffWeapon.cs` in the `Update()` method:

```csharp
void Update()
{
    // TEMPORARY TEST - Remove after fixing
    if (Input.GetKeyDown(KeyCode.T))
    {
        Debug.Log("[TEST] Manually triggering Cast animation");
        if (animator != null)
        {
            animator.SetTrigger("Cast");
            Debug.Log($"[TEST] Animator exists: {animator.name}");
        }
        else
        {
            Debug.LogError("[TEST] Animator is NULL!");
        }
    }
}
```

**Testing**:
1. Enter Play Mode
2. Press **T** key
3. Watch for:
   - Animation plays → Animator setup is correct, issue is with PerformPrimaryAttack call
   - Animation doesn't play → Animator issue (missing trigger, wrong state, etc.)
   - "Animator is NULL!" → Animator reference not assigned

---

### 5️⃣ Check Console for Detailed Logs

When you left-click, you should see this sequence in Console:

```
[PlayerController] Left-click detected! Calling PerformPrimaryAttack() on Mage's Staff
[StaffWeapon] PerformPrimaryAttack called! isCasting=False, cooldownRemaining=0, mana=100/100
[StaffWeapon] Starting fireball cast!
[StaffWeapon] Playing 'Cast' animation trigger
[StaffWeapon] Waiting 0.5 seconds for cast animation...
[StaffWeapon] Cast complete! Spawning fireball now...
[StaffWeapon] Primary fireball prefab is NOT ASSIGNED!
[StaffWeapon] Casting finished, ready for next cast
```

**If logs stop early**, note where they stop - that tells you the exact problem.

---

## 🐛 COMMON ISSUES & SOLUTIONS

### Issue: "No weapon equipped!"
**Solution**: 
- Find PlayerController component on Mage
- Expand `Available Weapons` array
- Add Staff to slot 0

### Issue: No console messages at all
**Solution**:
- Make sure you're in Play Mode
- Click in **Game view** (not Scene view)
- Check PlayerController component exists on character

### Issue: "Animator is NULL!"
**Solution**:
- Select Staff weapon GameObject
- In StaffWeapon component, find "Animator" field
- Drag the Animator component from your Mage character
- The Animator is usually on the root GameObject of the character

### Issue: Animation plays but no fireball
**Solution**: This is expected if you haven't created the fireball prefab yet
- The casting animation should still play
- Follow `MAGE_PRIMARY_ATTACK_GUIDE.md` to create the fireball prefab

### Issue: "Already casting!" or "On cooldown!"
**Solution**: Working as intended - wait for cooldown to finish
- Default cooldown is 0.8 seconds
- Can't cast while already casting

### Issue: "Not enough mana!"
**Solution**:
- Either disable "Use Mana" in StaffWeapon component
- Or increase "Current Mana" value
- Mana regenerates automatically over time

---

## 🎯 QUICK FIX CHECKLIST

Before testing again, verify ALL of these:

1. ✅ Staff is in PlayerController's `availableWeapons[0]`
2. ✅ StaffWeapon has `Animator` field assigned (drag from Mage character)
3. ✅ StaffWeapon has `Owner Entity` assigned
4. ✅ StaffWeapon has `Spell Cast Point` assigned (empty GameObject at staff tip)
5. ✅ Animator Controller has `Cast` **trigger** parameter (not bool!)
6. ✅ Animator Controller has `Casting` state with Mage_Cast animation
7. ✅ Animator has transition: Any State → Casting (on Cast trigger, no exit time)
8. ✅ Animator has transition: Casting → Idle (with exit time)
9. ✅ `Primary Attack Cast Time` = 0.5 (or your animation length)
10. ✅ `Current Mana` > 10 (if using mana)

---

## 🔧 DEBUGGING SCRIPT

Add the `MageAttackDebugHelper` component to your Mage character for automated diagnostics:

1. **Select Mage character** in Hierarchy
2. **Add Component** → `MageAttackDebugHelper`
3. **Enter Play Mode**
4. **Read console** for detailed diagnostic messages

The debug helper will check:
- Is input detected?
- Is weapon assigned?
- Is animator found?
- Does Cast parameter exist?
- Is everything configured correctly?

**Keyboard shortcuts in debug mode**:
- **T** = Manually trigger Cast animation
- **F** = Call PerformPrimaryAttack() directly
- **I** = Print current status info

---

## 📊 EXPECTED BEHAVIOR

When working correctly:

1. **Left-Click** → Character immediately starts casting animation
2. **During animation** → Casting VFX at staff tip (if assigned)
3. **After cast time** → Fireball spawns and flies forward (if prefab assigned)
4. **Animation ends** → Return to Idle or Locomotion state
5. **Cooldown** → Can cast again after 0.8 seconds

---

## 🆘 STILL NOT WORKING?

If you've checked everything and it still doesn't work:

### Last Resort Test:
1. Create a **brand new scene**
2. Add **only** the Mage character
3. Test left-click
4. If it works → something in your main scene is interfering
5. If it doesn't → there's a prefab configuration issue

### Check These Edge Cases:
- Is another script disabling the animator?
- Is the Mage character active in hierarchy?
- Is the Staff weapon active?
- Are there any errors in Console on Play Mode start?
- Is the Animator Controller actually assigned to the Animator component?
- Is the Mage_Cast.fbx imported with Humanoid rig?

---

## 📝 WHAT SHOULD BE HAPPENING

The code flow is:

1. **PlayerController.cs** detects left-click (`Input.GetMouseButtonDown(0)`)
2. Calls `currentWeapon.PerformPrimaryAttack()`
3. **StaffWeapon.cs** `PerformPrimaryAttack()` checks cooldown and mana
4. Starts coroutine `CastPrimaryFireball()`
5. Calls `PlayAnimation("Cast")` → triggers animator
6. Waits `primaryAttackCastTime` seconds
7. Calls `SpawnPrimaryFireball()` to create projectile
8. Animation automatically returns to Idle via transition

The animation trigger is fired on line 153 of StaffWeapon.cs:
```csharp
PlayAnimation("Cast");  // This calls animator.SetTrigger("Cast")
```

If the animation doesn't play, one of these is wrong:
- Animator reference is null
- Cast parameter doesn't exist
- Cast parameter is wrong type (must be Trigger, not Bool)
- No transition FROM Any State TO Casting state with Cast trigger condition
- Casting state has no animation assigned

---

## 🎬 TESTING PROCEDURE

1. **Fix all items** in Quick Fix Checklist above
2. **Save** all changes
3. **Enter Play Mode**
4. **Watch Console** for messages
5. **Left-Click** in Game view
6. **Should see**: Character plays casting animation
7. **Should see**: Console logs the full sequence
8. **Should see**: Fireball spawns after cast time (if prefab assigned)

If you still have issues after checking everything, note which console message is the LAST one you see - that tells us exactly where it's failing.

---

## 📚 RELATED GUIDES

- `MAGE_PRIMARY_ATTACK_GUIDE.md` - Complete setup guide for fireball system
- `QUICK_FIX_LEFT_CLICK.md` - 5-minute quick fix for common issues
- `MAGE_CAST_ANIMATION_SETUP.md` - Detailed animator setup instructions

---

Good luck! The most common issue is the Animator reference not being assigned in the StaffWeapon component. Check that first! 🔥✨

