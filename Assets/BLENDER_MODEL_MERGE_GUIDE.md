# 🎨 Blender: Merge Two Character Models - Complete Guide

## 🎯 Your Goal:
Keep the **armature/bones/animations** from one model (Maria with sword), but use the **mesh** from another model (Paladin + Helmet).

## ⚠️ The Problem:
Unity error: `"Copied Avatar Rig Configuration mis-match. Transform hierarchy does not match"`

**Why?** The Paladin mesh is parented to a different armature hierarchy than your animations expect.

---

## ✅ SOLUTION - Proper Workflow

### **Method 1: Transfer Mesh to Existing Armature (RECOMMENDED)**

This keeps your working animations and just swaps the mesh.

#### **Step 1: Open Both Files in Blender**

1. **File → Open** your Maria model (the one with working animations)
2. **File → Append** (or Import) the Paladin model
   - Navigate to Paladin .blend file
   - Select **Object** folder
   - Select Paladin mesh (and helmet if separate)
   - Click **Append**

Now you have both models in the same scene.

#### **Step 2: Identify What You Have**

You should see in Outliner (top right):
- **Armature** (from Maria model) ← Keep this!
- **Maria mesh** (the old character) ← Delete this later
- **Sword** (if attached) ← Keep this!
- **Paladin mesh** (new character) ← We'll parent this to Armature
- **Paladin Helmet** (if separate) ← We'll parent this too

#### **Step 3: Remove Old Mesh**

1. **Select Maria mesh** (the old character body)
2. **Press X** → Delete
3. **DON'T delete the Armature or Sword!**

#### **Step 4: Prepare Paladin Mesh**

1. **Select Paladin mesh** in Outliner
2. Look in Properties panel (right side) → **Modifiers**
3. **Remove any Armature modifier** if present:
   - Find "Armature" modifier
   - Click X to remove it
4. **Clear parent**:
   - Press **Alt+P** → "Clear Parent and Keep Transformation"

#### **Step 5: Parent Paladin to Your Armature**

This is the critical step!

1. **Select Paladin mesh** (click on it)
2. **Shift + Click** on **Armature** (select armature second!)
3. **Press Ctrl+P** (Parent menu)
4. Choose: **"With Automatic Weights"**

Blender will now:
- Automatically weight paint Paladin mesh to bones
- Create proper bone deformation
- Maintain bone hierarchy

Wait for it to calculate (may take a few seconds).

#### **Step 6: Repeat for Helmet (if separate)**

1. **Select Helmet mesh**
2. **Clear parent** (Alt+P)
3. **Select Helmet**, then **Shift+Click Armature**
4. **Ctrl+P** → "With Automatic Weights"

#### **Step 7: Test in Blender**

1. **Select Armature**
2. **Tab** into Pose Mode
3. **Select a bone** (like arm or leg)
4. **Press R** (rotate) and move mouse
5. **Check if Paladin mesh deforms** with the bone

✅ If mesh moves with bones = SUCCESS!
❌ If mesh doesn't move = Go back to Step 5

#### **Step 8: Clean Up Bone Names (Important!)**

Unity expects specific bone names (mixamorig:Hips, etc.)

1. **Select Armature**
2. **Tab** to Edit Mode
3. **Check bone names** in Outliner
4. Make sure bones have **"mixamorig:"** prefix
5. If not, you may need to rename:
   - Select bone
   - Press **N** (properties panel)
   - Change name in "Bone" field

#### **Step 9: Position Sword (if needed)**

If sword needs to be in Paladin's hand:

1. **Select Sword**
2. **Shift+Click** on **Armature**
3. Switch to **Pose Mode**
4. **Select the hand bone** (usually "mixamorig:RightHand")
5. **With Sword still selected**, press **Ctrl+P**
6. Choose: **"Bone"** (not "Bone Relative")
7. Adjust position:
   - **G** = Move
   - **R** = Rotate
   - **S** = Scale

#### **Step 10: Export to Unity**

1. **Select ONLY what you want to export**:
   - Select **Armature**
   - **Shift+Click** to also select:
     - Paladin mesh
     - Helmet mesh (if separate)
     - Sword mesh

2. **File → Export → FBX (.fbx)**

3. **Configure Export Settings**:
   ```
   ✓ Selected Objects (checked)
   ✓ Apply Modifiers (checked)
   ✓ Bake Animation (checked if you have animations)
   
   Armature:
   ✓ Add Leaf Bones (unchecked)
   
   Transform:
   Forward: -Z Forward
   Up: Y Up
   Scale: 1.00
   Apply Scalings: FBX Units Scale
   ```

4. **Name it**: `Paladin_J_Nordstrom.fbx`

5. **Click Export FBX**

#### **Step 11: Import to Unity**

1. Copy FBX to Unity project: `Assets/Models/Characters/`
2. **Select the FBX** in Unity
3. **Inspector → Rig tab**:
   - Animation Type: **Humanoid**
   - Avatar Definition: **Create From This Model**
   - Click **Configure**
4. **Check skeleton mapping**:
   - All bones should be green ✓
   - If red, map manually
5. **Click Apply**

---

## 🔧 Method 2: Transfer Animations to New Armature (Alternative)

If Method 1 doesn't work, try this:

### **Workflow:**

1. Keep Paladin model with its own armature
2. Import Maria animations
3. Retarget animations from Maria armature to Paladin armature

This is more complex but sometimes necessary.

#### **Steps:**

1. **Open Paladin model in Blender**
2. **Import Maria animations**:
   - File → Import → FBX
   - Import Maria animated FBX
3. **Use "NLA Editor"** to transfer animations
4. Or use **Blender's Animation Retargeting** add-on

This method is more advanced and I can provide detailed steps if needed.

---

## 🐛 Troubleshooting Common Issues

### **Issue 1: "Paladin mesh doesn't deform with bones"**

**Cause**: Mesh not properly weighted to bones.

**Fix**:
1. Select Paladin mesh
2. Tab to Edit Mode
3. Select all vertices (A)
4. In Properties → Data → Vertex Groups
5. Check if weight groups exist
6. If not, re-parent with Automatic Weights (Step 5)

### **Issue 2: "Some parts don't move (like helmet)"**

**Cause**: Helmet not parented to armature.

**Fix**:
1. Select helmet
2. Make sure it's parented to armature (Step 6)
3. Or parent helmet directly to head bone:
   - Select helmet
   - Shift+Click armature
   - Ctrl+P → Bone → Select "Head" bone

### **Issue 3: "Bones are in wrong positions"**

**Cause**: Paladin model is different size/proportions than Maria.

**Fix**:
1. **Scale Paladin mesh** to match original:
   - Select Paladin mesh
   - Press **S** (scale)
   - Type a number (try 1.2 or 0.8)
2. **Or adjust bone positions**:
   - Select Armature
   - Edit Mode
   - Move bones to match new mesh
   - (Advanced - not recommended)

### **Issue 4: "Mesh is inside-out or black"**

**Cause**: Normals are flipped.

**Fix**:
1. Select Paladin mesh
2. Tab to Edit Mode
3. Select all (A)
4. **Shift+N** (Recalculate Normals)
5. Or: **Alt+N** → "Flip Normals"

### **Issue 5: "Unity error: Transform hierarchy mismatch"**

**Cause**: Bone names don't match between models.

**Fix**:
1. In Blender, select Armature
2. Edit Mode
3. Check bone names match Unity's expectations:
   - Root
   - Hips (or mixamorig:Hips)
   - Spine, Chest, Neck, Head
   - LeftShoulder, LeftArm, LeftForeArm, LeftHand
   - RightShoulder, RightArm, RightForeArm, RightHand
   - LeftUpLeg, LeftLeg, LeftFoot
   - RightUpLeg, RightLeg, RightFoot

4. Rename bones to match if needed

### **Issue 6: "Sword in wrong position"**

**Cause**: Sword parented to wrong bone or wrong relative position.

**Fix**:
1. Select Sword
2. Clear parent (Alt+P)
3. Re-parent to hand bone (Step 9)
4. In Pose Mode, adjust position:
   - Select hand bone
   - Sword should be child of that bone
   - Adjust sword position with G, R, S

---

## 💡 Quick Tips

### **Before You Start:**
- ✅ **Save a backup** of both files!
- ✅ Make sure both models use **same scale** (1.0 in Blender)
- ✅ Check bone names match (especially mixamorig prefix)

### **During Process:**
- ✅ Work in **Object Mode** when selecting things to parent
- ✅ Switch to **Pose Mode** when testing animations
- ✅ Use **Edit Mode** when adjusting bone names/positions

### **Testing:**
- ✅ Test in Blender before exporting!
- ✅ Rotate bones in Pose Mode
- ✅ Make sure mesh deforms properly
- ✅ Check sword position in hand

### **Export Settings:**
- ✅ Always use **Selected Objects** only
- ✅ **Forward: -Z, Up: Y** for Unity
- ✅ Scale: 1.00
- ✅ Apply modifiers checked

---

## 📋 Checklist Before Export

- [ ] Paladin mesh is parented to Armature (with Automatic Weights)
- [ ] Helmet is parented to Armature (or Head bone)
- [ ] Sword is parented to hand bone
- [ ] Tested bone movement in Pose Mode - mesh deforms correctly
- [ ] Bone names match Unity expectations (mixamorig:Hips, etc.)
- [ ] All objects are properly positioned (not floating or offset)
- [ ] No duplicate armatures in scene
- [ ] Only kept ONE armature (the one with animations)
- [ ] Removed old Maria mesh
- [ ] Applied all transforms (Ctrl+A → All Transforms)

---

## 🎯 Step-by-Step Visual Guide

### **What You're Doing:**

```
BEFORE:
┌─────────────────┐         ┌─────────────────┐
│ Maria Model     │         │ Paladin Model   │
│                 │         │                 │
│ ✓ Armature      │         │ ✗ Armature      │
│ ✗ Maria Mesh    │         │ ✓ Paladin Mesh  │
│ ✓ Animations    │         │ ✓ Helmet Mesh   │
│ ✓ Sword         │         │                 │
└─────────────────┘         └─────────────────┘

AFTER:
┌─────────────────────────────┐
│ New Combined Model          │
│                             │
│ ✓ Armature (from Maria)     │
│ ✓ Paladin Mesh (new!)       │
│ ✓ Helmet Mesh (new!)        │
│ ✓ Animations (from Maria)   │
│ ✓ Sword (from Maria)        │
└─────────────────────────────┘
```

---

## 🔥 Common Mistake to Avoid

### **DON'T DO THIS:**
❌ Import Paladin model with its armature
❌ Try to merge two armatures
❌ Copy bones from one armature to another
❌ Have multiple armatures in the scene

### **DO THIS INSTEAD:**
✅ Keep ONE armature (the one with working animations)
✅ Remove Paladin's armature
✅ Parent Paladin MESH to Maria's armature
✅ Export only ONE armature with all meshes

---

## 🎬 Alternative: Using Mixamo for Both

If both models are from Mixamo (or can be rigged by Mixamo):

1. **Upload Paladin to Mixamo.com**
2. **Auto-rig** Paladin there
3. **Download with your animation** (Great Sword Slash)
4. **Import to Unity**
5. **Done!**

This ensures compatible rigs and animations.

---

## 📞 If You're Still Stuck

### **Quick Diagnostic:**

**In Blender Outliner, you should see ONLY:**
- 📦 **Armature** (orange bone icon)
  - 🦴 Bones (Hips, Spine, etc.)
- 🎭 **Paladin mesh** (mesh icon)
- 🎭 **Helmet mesh** (mesh icon)
- ⚔️ **Sword mesh** (mesh icon)

**If you see:**
- ❌ Two armatures = DELETE one!
- ❌ "Armature.001" = You have duplicates, delete extras
- ❌ Mesh not indented under armature = Not parented correctly

---

## 🎯 Summary - The Key Steps

1. **Open Maria model** (has working armature/animations)
2. **Import Paladin mesh** (append from file)
3. **Delete Maria mesh** (old character)
4. **Clear Paladin's parent** (Alt+P)
5. **Parent Paladin to Armature** (Ctrl+P → Automatic Weights)
6. **Do same for Helmet**
7. **Position Sword** in hand bone
8. **Test in Pose Mode**
9. **Export FBX** with correct settings
10. **Import to Unity** as Humanoid rig

---

## 🚀 Success Indicators

You know it worked when:
- ✅ In Blender Pose Mode, moving bones deforms Paladin mesh
- ✅ Sword stays in hand when moving arm bone
- ✅ Export FBX has only ONE armature hierarchy
- ✅ Unity imports without "hierarchy mismatch" error
- ✅ Unity shows green checkmarks on all bones
- ✅ Animations play correctly in Unity

---

Good luck! The key is: **Keep one armature, parent meshes to it, test before export!** 🎨✨

