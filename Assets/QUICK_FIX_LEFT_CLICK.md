# 🔥 QUICK FIX: Left-Click Not Working

## The Problem
When you left-click in Unity, nothing happens - no animation, no fireball.

## The Solution (5 Minutes)

### ✅ STEP 1: Add "Cast" Trigger to Animator (CRITICAL!)

1. **Open Animator Window**:
   - `Window → Animation → Animator`
   - Or double-click: `Assets/Animations/Characters/Mage/Mage_AnimatorController`

2. **Add Cast Parameter**:
   - In Animator window, click **Parameters** tab (left side)
   - Click the **+** button
   - Select **Trigger**
   - Type exactly: `Cast`
   - Press Enter

**✓ Done! This is the #1 reason left-click doesn't work!**

---

### ✅ STEP 2: Add Mage_Cast Animation to Animator

1. **Still in Animator Window**:
   - Right-click in the grid area (empty space)
   - Select **Create State → Empty**
   - Name it: `Casting`

2. **Assign Animation**:
   - Click on the `Casting` state box
   - In Inspector (right side), find **Motion** field
   - Click the small circle button next to it
   - Type "Cast" in search
   - Double-click `Mage_Cast` animation

3. **Uncheck Loop**:
   - In the same Inspector, find **Loop Time**
   - Make sure it's **UNCHECKED** (we want it to play once)

---

### ✅ STEP 3: Create Transition from Any State

1. **In Animator Window**:
   - Right-click on **"Any State"** box (at top)
   - Select **Make Transition**
   - Click on the `Casting` state box you just created

2. **Configure Transition**:
   - Click on the white arrow connecting Any State → Casting
   - In Inspector:
     - **Conditions**: Click **+**
       - Select `Cast` from dropdown
     - **Has Exit Time**: ✓ **UNCHECK THIS** (important!)
     - **Transition Duration**: Set to `0.1`

---

### ✅ STEP 4: Create Return Transition

1. **In Animator Window**:
   - Right-click on **Casting** state box
   - Select **Make Transition**
   - Click on **Idle** state box

2. **Configure Transition**:
   - Click the arrow: Casting → Idle
   - In Inspector:
     - **Conditions**: Leave EMPTY
     - **Has Exit Time**: ✓ **CHECK THIS**
     - **Exit Time**: Set to `0.9`
     - **Transition Duration**: `0.15`

---

### ✅ STEP 5: Configure Staff Weapon

1. **Find your Mage Character in Hierarchy**
2. **Find the Staff Weapon** (child GameObject)
3. **In Inspector, check StaffWeapon component**:

   **Required Settings:**
   - ☐ **Animator**: Drag your character's Animator component here
   - ☐ **Spell Cast Point**: Create empty child GameObject at staff tip, assign it
   - ☐ **Primary Fireball Prefab**: You'll create this next (can be null for now to test animation)
   - ☐ **Primary Attack Cast Time**: `0.5` (adjust to match your animation length later)
   - ☐ **Primary Attack Cooldown**: `0.8`

---

## 🧪 Testing

### Test 1: Check Console Logs

1. **Press Play**
2. **Left-Click**
3. **Check Console** (bottom of Unity)

**You should see:**
```
[PlayerController] Left-click detected! Calling PerformPrimaryAttack() on Mage's Staff
[StaffWeapon] PerformPrimaryAttack called! isCasting=False, cooldownRemaining=0, mana=100/100
[StaffWeapon] Starting fireball cast!
[StaffWeapon] Playing 'Cast' animation trigger
[StaffWeapon] Waiting 0.5 seconds for cast animation...
```

### Test 2: Use Debug Helper (Optional)

1. **Add Debug Script**:
   - Select your Mage character in Hierarchy
   - Click **Add Component**
   - Type: `MageAttackDebugHelper`
   - Add it

2. **Press Play**

3. **Read the console** - it will tell you exactly what's wrong!

4. **Test with keyboard**:
   - Press **T** = Trigger animation manually
   - Press **F** = Call attack function manually
   - Press **I** = Check status

---

## 🐛 Common Issues & Fixes

### Issue: "No weapon equipped!"
**Fix**: 
- Check `PlayerController` component
- Find `Available Weapons` array
- Add your Staff to slot 0

### Issue: Animation doesn't play
**Fix**:
- Check you added "Cast" trigger to Animator Parameters
- Check Animator Controller is assigned to character
- Check StaffWeapon has Animator reference

### Issue: "Primary fireball prefab is NOT ASSIGNED"
**Fix**:
- This is OK for now (just testing animation)
- You'll create the fireball prefab later
- Animation should still play

### Issue: "Animator not found!"
**Fix**:
- In StaffWeapon component, assign the Animator field
- Drag the Animator from your character (should be on same GameObject as PlayerController)

### Issue: Nothing happens, no console messages
**Fix**:
- Check PlayerController is on the character
- Check you're clicking in Game view (not Scene view)
- Check cursor is locked (should be invisible in play mode)

---

## 📋 Quick Checklist

Before testing, verify:

- [ ] "Cast" trigger exists in Animator Parameters
- [ ] "Casting" state exists with Mage_Cast animation
- [ ] Transition: Any State → Casting (on Cast trigger, no exit time)
- [ ] Transition: Casting → Idle (with exit time)
- [ ] StaffWeapon has Animator assigned
- [ ] StaffWeapon Primary Attack Cast Time = 0.5 (or your animation length)
- [ ] Staff is in PlayerController's availableWeapons array

---

## 🎯 What Should Happen

When you left-click:

1. Console logs appear ✓
2. Character plays casting animation ✓
3. After 0.5 seconds, fireball should spawn (if prefab assigned)
4. Animation returns to idle/walk ✓

---

## 🆘 Still Not Working?

Run this checklist:

1. **Is left-click being detected?**
   - Add MageAttackDebugHelper script
   - Watch for yellow "RAW LEFT-CLICK DETECTED" message
   - If NO → Check you're clicking in Game view

2. **Is weapon equipped?**
   - Check console for "No weapon equipped!"
   - If YES → Add Staff to PlayerController's availableWeapons

3. **Is animator found?**
   - Look for "ANIMATOR NOT FOUND" error on start
   - If YES → Assign Animator in StaffWeapon component

4. **Does animator have Cast parameter?**
   - Use MageAttackDebugHelper to check
   - Look for green "✓ Animator has 'Cast' trigger"
   - If NO → Add Cast trigger to Animator Parameters

5. **Test manually**:
   - Press **T** key = Should play animation
   - Press **F** key = Should call attack function
   - If T works but left-click doesn't → Input system issue
   - If F works but T doesn't → Animator issue

---

## 📝 Next Steps After Animation Works

Once the casting animation plays correctly:

1. Create the fireball prefab (follow MAGE_PRIMARY_ATTACK_GUIDE.md)
2. Assign fireball prefab to StaffWeapon
3. Create Spell Cast Point at staff tip
4. Test complete attack with projectile

---

## Expected Console Output (Success):

```
[PlayerController] Left-click detected! Calling PerformPrimaryAttack() on Mage's Staff
[StaffWeapon] PerformPrimaryAttack called! isCasting=False, cooldownRemaining=0, mana=100/100
[StaffWeapon] Starting fireball cast!
[StaffWeapon] Playing 'Cast' animation trigger
[StaffWeapon] Waiting 0.5 seconds for cast animation...
[StaffWeapon] Cast complete! Spawning fireball now...
[StaffWeapon] Primary fireball prefab is NOT ASSIGNED! (or fireball spawns here)
[StaffWeapon] Casting finished, ready for next cast
```

---

That's it! The #1 issue is usually the missing "Cast" trigger parameter. Add that and it should work! 🔥

