# 🔮 Mage Character Setup & Testing Guide

## ✅ You Have All The Animations!

Your Mage animations are now in: `Assets/Animations/Characters/Mage/`

- ✅ Mage_Idle
- ✅ Mage_Run (Forward)
- ✅ Mage_Backwards
- ✅ Mage_StrafeLeft
- ✅ Mage_StrafeRight
- ✅ Mage_Jump
- ✅ Mage_TurnLeft
- ✅ Mage_TurnRight
- ✅ Mage_TPose (in Models/Characters/Mage/)

---

## 🎯 Animation Mapping

### Movement (WASD):
- **W** → `Mage_Run` (Forward)
- **A** → `Mage_StrafeLeft` (Strafe Left)
- **S** → `Mage_Backwards` (Backward)
- **D** → `Mage_StrafeRight` (Strafe Right)

### Actions:
- **Spacebar** → `Mage_Jump`
- **No Input** → `Mage_Idle`
- **Left Click + Drag Left** → `Mage_TurnLeft`
- **Left Click + Drag Right** → `Mage_TurnRight`

---

## 📋 Step-by-Step Setup

## PART 1: Configure Animation Import Settings

### Step 1.1: Setup Mage T-Pose Avatar

1. **Select** `Mage_TPose` in Project window (`Assets/Models/Characters/Mage/`)
2. In **Inspector** → **Rig** tab:
   - Animation Type: `Humanoid`
   - Avatar Definition: `Create From This Model`
   - Click **Apply**
3. You should see a checkmark and "✓ Configure..." button
4. The Avatar is now created!

### Step 1.2: Configure Each Animation File

For **EACH** animation (Mage_Idle, Mage_Run, Mage_Jump, etc.):

1. **Select** the animation file in Project window
2. In **Inspector** → **Rig** tab:
   - Animation Type: `Humanoid`
   - Avatar Definition: `Copy From Other Avatar`
   - Source: Click the circle → Select `Mage_TPoseAvatar`
   - Click **Apply**
3. Go to **Animation** tab:
   - ✅ Import Animation (should be checked)
   - **For Locomotion** (Idle, Run, Backwards, Strafe):
     - Loop Time: ✅ (CHECK)
     - Loop Pose: ✅ (CHECK)
     - Root Transform Rotation: Bake Into Pose ✅, Based On: `Body Orientation`
     - Root Transform Position (Y): Bake Into Pose ✅, Based On: `Original`
     - Root Transform Position (XZ): Bake Into Pose ✅, Based On: `Center of Mass`
   - **For Jump**:
     - Loop Time: ❌ (UNCHECK)
     - Loop Pose: ❌ (UNCHECK)
   - **For Turn animations**:
     - Loop Time: ❌ (UNCHECK)
     - Loop Pose: ❌ (UNCHECK)
   - Click **Apply**

**Repeat for all 8 animation files!**

---

## PART 2: Create Animator Controller

### Step 2.1: Create the Animator Controller

1. In Project window, navigate to `Assets/Animations/Characters/Mage/`
2. **Right-click** → Create → **Animator Controller**
3. **Name it:** `Mage_AnimatorController`
4. **Double-click** to open the Animator window

### Step 2.2: Create Animation Parameters

In the **Animator** window, click the **Parameters** tab (left side):

Add these parameters:
1. Click **+** → **Float** → Name: `MoveX` (Default: 0)
2. Click **+** → **Float** → Name: `MoveZ` (Default: 0)
3. Click **+** → **Bool** → Name: `IsGrounded` (Default: true)
4. Click **+** → **Trigger** → Name: `Jump`
5. Click **+** → **Float** → Name: `TurnDirection` (Default: 0)

### Step 2.3: Create Animation States

In the **Animator** window, in the main grid area:

1. **Entry** and **Any State** should already exist

2. **Create Idle State:**
   - Right-click in grid → Create State → Empty
   - Name: `Idle`
   - Click on Idle state
   - In Inspector → Motion: Drag `Mage_Idle` animation clip
   - Right-click Idle → **Set as Layer Default State** (it turns orange)

3. **Create Locomotion Blend Tree:**
   - Right-click in grid → Create State → From New Blend Tree
   - Name: `Locomotion`
   - **Double-click** the Locomotion state to enter it
   - Click on "Blend Tree" in Inspector
   - Blend Type: `2D Freeform Directional`
   - Parameters:
     - Parameter X: `MoveX`
     - Parameter Z: `MoveZ`
   - Click **+** under Motion field → Add Motion Field (do this 5 times)
   - Assign motions:
     - Motion 0: `Mage_Idle` → Pos X: 0, Pos Y: 0
     - Motion 1: `Mage_Run` → Pos X: 0, Pos Y: 1 (Forward)
     - Motion 2: `Mage_Backwards` → Pos X: 0, Pos Y: -1 (Back)
     - Motion 3: `Mage_StrafeLeft` → Pos X: -1, Pos Y: 0 (Left)
     - Motion 4: `Mage_StrafeRight` → Pos X: 1, Pos Y: 0 (Right)
   - Click **Base Layer** at top to go back

4. **Create Jump State:**
   - Right-click in grid → Create State → Empty
   - Name: `Jump`
   - In Inspector → Motion: Drag `Mage_Jump` animation clip

5. **Create Turn Left State:**
   - Right-click in grid → Create State → Empty
   - Name: `TurnLeft`
   - In Inspector → Motion: Drag `Mage_TurnLeft` animation clip

6. **Create Turn Right State:**
   - Right-click in grid → Create State → Empty
   - Name: `TurnRight`
   - In Inspector → Motion: Drag `Mage_TurnRight` animation clip

### Step 2.4: Create Transitions

**From Idle to Locomotion:**
1. Right-click `Idle` → Make Transition → Click `Locomotion`
2. Click the arrow (transition)
3. In Inspector:
   - Has Exit Time: ❌ (UNCHECK)
   - Transition Duration: 0.1
   - Conditions: Click **+**
     - `MoveX` Greater 0.1 OR `MoveZ` Greater 0.1
   - Actually, use this simpler condition:
     - Click **+** → `MoveZ` → `Greater` → 0.01

**From Locomotion to Idle:**
1. Right-click `Locomotion` → Make Transition → Click `Idle`
2. In Inspector:
   - Has Exit Time: ❌ (UNCHECK)
   - Transition Duration: 0.1
   - Conditions: Click **+**
     - `MoveX` → `Less` → 0.01
     - AND `MoveZ` → `Less` → 0.01

**From Any State to Jump:**
1. Right-click `Any State` → Make Transition → Click `Jump`
2. In Inspector:
   - Has Exit Time: ❌ (UNCHECK)
   - Transition Duration: 0.1
   - Can Transition To Self: ❌ (UNCHECK)
   - Conditions: Click **+**
     - `Jump` (trigger)

**From Jump back to Locomotion:**
1. Right-click `Jump` → Make Transition → Click `Locomotion`
2. In Inspector:
   - Has Exit Time: ✅ (CHECK)
   - Exit Time: 0.9
   - Transition Duration: 0.2
   - Conditions: (none needed)

**Optional: Turn animations** (can add later if needed)

### Step 2.5: Save the Animator Controller

- **File** → **Save Project** (or Ctrl+S / Cmd+S)
- Your Animator Controller is ready!

---

## PART 3: Create Mage Prefab

### Step 3.1: Import Mage Model to Scene

1. Open your test scene (or create new: File → New Scene)
2. Drag `Mage_TPose` from `Assets/Models/Characters/Mage/` into **Hierarchy**
3. **Rename** it to `Player_Mage`
4. **Position:** (0, 0, 0) in Inspector

### Step 3.2: Add Required Components

Select `Player_Mage` in Hierarchy, then in Inspector click **Add Component** for each:

1. **CharacterController**
   - Center: (0, 1, 0)
   - Radius: 0.3
   - Height: 2
   - Slope Limit: 45
   - Step Offset: 0.3

2. **PlayerController** (our script)

3. **MovementComponent** (our script)
   - Base Movement Speed: 5
   - Sprint Multiplier: 1.5
   - Jump Height: 2
   - Gravity: -20
   - Ground Layer: `Default` (or create Ground layer)

4. **HealthComponent** (our script)
   - Max Health: 70 (Mage is squishy!)

5. **CharacterClass** (our script)
   - Class Type: `Mage`
   - Class Name: "Mage"
   - Base Health: 70
   - Health Modifier: 0.7
   - Armor Modifier: 0.5
   - Speed Modifier: 1.0
   - Damage Modifier: 1.5

6. **Audio Source**

### Step 3.3: Setup Animator

1. Find the **Animator** component (should already exist on model)
2. If not, Add Component → **Animator**
3. In Animator component:
   - Controller: Drag `Mage_AnimatorController` here
   - Avatar: Should auto-assign to `Mage_TPoseAvatar`
   - Apply Root Motion: ❌ (UNCHECK - we handle movement with scripts)

### Step 3.4: Create Player Camera

1. Right-click `Player_Mage` in Hierarchy → Create Empty
2. **Rename** to `PlayerCamera`
3. **Position:** (0, 1.6, 0) - at head height
4. **Rotation:** (0, 0, 0)
5. Add Component → **Camera**
6. Tag: `MainCamera`

### Step 3.5: Assign Camera to PlayerController

1. Select `Player_Mage`
2. Find **PlayerController** component
3. Drag `PlayerCamera` from Hierarchy into **Player Camera** field

### Step 3.6: Create Weapon (Staff) - Optional for now

We'll add the staff later. For now, we just want to test movement!

### Step 3.7: Save as Prefab

1. Create folder: `Assets/Prefabs/Characters/` (if doesn't exist)
2. Drag `Player_Mage` from Hierarchy to `Assets/Prefabs/Characters/`
3. You now have `Player_Mage.prefab`!

---

## PART 4: Update Movement Component for Animation

We need to make the `MovementComponent` send animation parameters to the Animator.

### Step 4.1: Add Animation Support to MovementComponent

I'll create an updated version that integrates with the Animator.

---

## PART 5: Setup Test Scene

### Step 5.1: Create Ground

1. In Hierarchy: Right-click → 3D Object → **Plane**
2. **Name:** `Ground`
3. **Position:** (0, 0, 0)
4. **Scale:** (10, 1, 10) - makes it bigger
5. **Layer:** `Default` (or create `Ground` layer)

### Step 5.2: Add Lighting

If no light exists:
1. Hierarchy → Right-click → Light → **Directional Light**
2. **Rotation:** (50, -30, 0) for nice lighting

### Step 5.3: Position Camera (Scene Camera - for viewing)

1. In Scene view, position the Scene camera to see the Mage
2. GameObject → Align View to Selected (with Mage selected)

### Step 5.4: Setup Ground Layer Detection

1. Edit → Project Settings → Tags and Layers
2. Find "Layers" section
3. Layer 6: Name it `Ground`
4. Select the Ground plane in Hierarchy
5. Set Layer to `Ground`
6. Select `Player_Mage`
7. In **MovementComponent**: Set Ground Layer to `Ground`

---

## PART 6: Testing Your Mage!

### Step 6.1: Press Play!

Click the **Play** button in Unity!

### Step 6.2: Test Movement

**Basic Movement:**
- [ ] Press **W** - Mage should run forward with `Mage_Run` animation
- [ ] Press **S** - Mage should walk backward with `Mage_Backwards` animation
- [ ] Press **A** - Mage should strafe left with `Mage_StrafeLeft` animation
- [ ] Press **D** - Mage should strafe right with `Mage_StrafeRight` animation
- [ ] Release all keys - Mage should play `Mage_Idle` animation

**Diagonal Movement:**
- [ ] Press **W+A** - Should blend Run and StrafeLeft
- [ ] Press **W+D** - Should blend Run and StrafeRight
- [ ] Press **S+A** - Should blend Backwards and StrafeLeft
- [ ] Press **S+D** - Should blend Backwards and StrafeRight

**Sprint:**
- [ ] Hold **Shift** + **W** - Should run faster (same animation, faster speed)

**Jump:**
- [ ] Press **Spacebar** - Should play `Mage_Jump` animation and jump

**Camera Control:**
- [ ] Hold **Left Click** + **Drag Mouse** - Camera should rotate
- [ ] Character should rotate with camera (face camera direction)

### Step 6.3: Open Animator Window While Playing

1. Window → Animation → **Animator**
2. Select `Player_Mage` in Hierarchy
3. Watch the Animator window while moving:
   - See parameters change (MoveX, MoveZ)
   - See state transitions (Idle → Locomotion → Jump)
   - See the blend tree work!

### Step 6.4: Troubleshooting

**Problem: Character doesn't move**
- Check CharacterController is present
- Check MovementComponent is present
- Check Ground Layer is set
- Make sure cursor is locked (mouseLocked = true in PlayerController)

**Problem: Animations don't play**
- Check Animator Controller is assigned
- Check Avatar is assigned
- Check all animations are imported correctly
- Check parameters exist in Animator

**Problem: Character slides without animation**
- Check Animator Controller is assigned
- Verify animations have "Bake Into Pose" settings correct

**Problem: Character is in T-pose**
- Check Avatar is assigned to Animator
- Verify all animations copied from same Avatar
- Check animation clips are valid

**Problem: Jump doesn't work**
- Check IsGrounded parameter
- Verify Ground Layer is correct
- Check CharacterController is grounded

**Problem: Camera doesn't follow mouse**
- Verify PlayerCamera is assigned in PlayerController
- Check cursor lock (mouseLocked should be true)
- Test with game view focused (click in Game window)

---

## PART 7: Fine-Tuning

### Adjust Movement Speed:
- Select `Player_Mage`
- In `MovementComponent`:
  - Base Movement Speed: Try 4-6
  - Sprint Multiplier: Try 1.3-1.8

### Adjust Jump:
- Jump Height: Try 1.5-2.5
- Gravity: Try -15 to -25

### Adjust Camera:
- Camera Sensitivity: Try 1.5-3.0
- Camera Min Y: -60 to -80
- Camera Max Y: 60 to 80

---

## 🎮 Expected Behavior

When everything is working:

1. **Idle**: Mage stands still with gentle breathing animation
2. **W**: Mage runs forward
3. **A**: Mage strafes left (side-step left while facing forward)
4. **S**: Mage walks backward
5. **D**: Mage strafes right
6. **W+A**: Mage runs forward-left (blend)
7. **Spacebar**: Mage jumps with jump animation
8. **Left Click + Drag**: Camera rotates smoothly, Mage faces camera direction
9. **Shift**: Movement is faster

---

## 📸 Visual Debugging

### Check Animator in Play Mode:

1. Press Play
2. Select `Player_Mage`
3. Open Animator window
4. Move with WASD and watch:
   - Parameters update in real-time
   - Blend tree shows which animations are active
   - Transitions occur smoothly

### Enable Gizmos:

1. In Scene view, toggle **Gizmos** button
2. You should see:
   - CharacterController capsule (green)
   - Camera frustum
   - Ground check sphere

---

## ✅ Success Checklist

- [ ] All animations imported and configured with Mage Avatar
- [ ] Animator Controller created with Blend Tree
- [ ] Mage prefab created with all components
- [ ] Test scene setup with ground and lighting
- [ ] Character moves with WASD (correct animations play)
- [ ] Character jumps with Spacebar
- [ ] Camera rotates with Left Click + Drag
- [ ] Character faces camera direction
- [ ] Animations blend smoothly
- [ ] No T-pose or sliding issues

---

## 🚀 Next Steps After Testing

Once your Mage is working perfectly:

1. **Add Staff Weapon:**
   - Create Staff prefab
   - Assign to PlayerController
   - Test spellcasting animations

2. **Add Ability Animations:**
   - Import spell casting animations
   - Add to Animator Controller
   - Hook up to ability system

3. **Create Enemies:**
   - Import Mutant character
   - Setup AI behavior
   - Test combat

4. **Add Other Classes:**
   - Import Knight character
   - Import Archer character
   - Setup their Animator Controllers

5. **Build First Level:**
   - Design environment
   - Add obstacles
   - Create objectives

---

## 💡 Pro Tips

1. **Test in Play Mode:** Always test with the Game window focused
2. **Watch Animator:** Keep Animator window open to debug issues
3. **Iterate:** Adjust speeds and values until it feels right
4. **Save Often:** Save your scene and project frequently
5. **One Thing at a Time:** Get movement perfect before adding combat

---

**You're ready to bring your Mage to life! 🔮**

Follow this guide step-by-step, and you'll have a fully animated, playable Mage character!

Good luck! ✨

