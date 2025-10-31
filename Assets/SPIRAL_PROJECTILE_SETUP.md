# 🌀 Spiral Projectile Setup Guide

## Overview
Create an amazing twin spiral effect where two projectiles orbit around each other while flying forward! Perfect for void spheres, magic missiles, or any mystical projectiles.

---

## 🎯 Quick Setup (5 Minutes)

### **Step 1: Create the Spiral Pair Prefab**

1. **In Hierarchy**: Right-click → Create Empty
2. **Name it**: `VoidSpiralPair`

3. **Add Components to VoidSpiralPair**:
   - **Rigidbody**:
     - Use Gravity: ❌ OFF
     - Is Kinematic: ✅ ON (we control movement via script)
   - **Sphere Collider** (or Box Collider for detection area):
     - Is Trigger: ✅ ON
     - Radius: `1.0` (detection area for the pair)
   - **SpiralProjectilePair** script

4. **Configure SpiralProjectilePair Component**:
   - Forward Speed: `25`
   - Spiral Radius: `0.5` to `1.0` (how far apart they orbit)
   - Spiral Speed: `360` (one full rotation per second)
   - Lifetime: `5`
   - Projectile Prefab: Drag your `MageFireballPrimary` prefab here
   - Use Existing Children: ❌ OFF

---

### **Step 2: Alternative - Use Existing Children**

If you want to manually place the spheres:

1. **Duplicate** your `MageFireballPrimary` twice
2. **Drag both** into `VoidSpiralPair` as children
3. **In SpiralProjectilePair** component:
   - Use Existing Children: ✅ ON
   - Projectile Prefab: Leave empty

---

### **Step 3: Save as Prefab**

1. **Drag** `VoidSpiralPair` from Hierarchy → `Assets/Prefabs/Projectiles/`
2. **Delete** from Hierarchy (we just need the prefab)

---

### **Step 4: Assign to Mage**

1. **Select** your `Player_Mage` in Hierarchy
2. **Find** `MageSpellCaster` component
3. **Spiral Effect section**:
   - ✅ **Use Spiral Effect**: Check this!
   - **Spiral Pair Prefab**: Drag `VoidSpiralPair` here
   - **Spiral Radius**: `0.5` to `1.0` (adjust to taste)
   - **Spiral Speed**: `360` (adjust rotation speed)

---

### **Step 5: Test!**

1. **Enter Play Mode**
2. **Left-Click** to cast
3. **You should see**: Two void spheres spinning around each other! 🌀

---

## 🎨 Visual Customization

### **Adjust Spiral Pattern:**

**Tight Spiral (Fast, Close):**
```
Spiral Radius: 0.3
Spiral Speed: 540 (1.5 rotations/sec)
Forward Speed: 30
```

**Wide Spiral (Dramatic):**
```
Spiral Radius: 1.2
Spiral Speed: 180 (slow, graceful)
Forward Speed: 20
```

**DNA Helix:**
```
Spiral Radius: 0.6
Spiral Speed: 720 (2 rotations/sec)
Forward Speed: 25
```

**Figure-8 Pattern:**
```
Spiral Radius: 0.8
Spiral Speed: 450
Forward Speed: 22
```

---

## 🔧 Advanced Settings

### **Collision Detection:**

The parent `VoidSpiralPair` handles collisions:
- Individual projectiles have **physics disabled**
- Only the parent detects hits
- Both spheres count as **one hit** (deals damage once)

### **Different Colored Spheres:**

Want one purple and one blue?

1. **Create two prefabs**: `VoidPurpleSphere` and `VoidBlueSphere`
2. **Don't assign** Projectile Prefab in SpiralProjectilePair
3. **Manually add** both as children in the prefab
4. **Check** "Use Existing Children"

---

## 🌟 Effect Variations

### **Triple Spiral:**

Modify `SpiralProjectilePair.cs`:
- Add `projectile3`
- Set angles at `0°`, `120°`, `240°`

### **Opposite Rotation:**

Make spheres rotate opposite directions:
- One uses `currentAngle`
- One uses `-currentAngle`

### **Pulsing Size:**

Add this to `Update()`:
```csharp
float pulse = 1f + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * 0.2f;
projectile1.transform.localScale = Vector3.one * pulse;
projectile2.transform.localScale = Vector3.one * (2f - pulse); // opposite
```

### **Trailing Effect:**

The individual projectiles keep their trails, creating a **braided trail** effect automatically! 🎨

---

## 🎯 Troubleshooting

### **Issue: Spheres not visible**
**Solution**: 
- Check that VoidSpiralPair has children or Projectile Prefab assigned
- Verify SpiralProjectilePair script is added

### **Issue: Spheres fly apart**
**Solution**:
- Reduce Spiral Speed
- Check Forward Speed isn't too high
- Verify Spiral Radius is reasonable (0.3-1.5)

### **Issue: No collision detection**
**Solution**:
- Parent needs a Collider component
- Collider should be a Trigger
- Check collision layer settings

### **Issue: Jerky movement**
**Solution**:
- Make sure parent Rigidbody is Kinematic
- Individual projectile Rigidbodies should also be Kinematic
- Check frame rate (spiral uses Update, not FixedUpdate)

---

## 📊 Performance Tips

1. **Disable individual colliders** - Only parent needs collision
2. **Use simple trail** - Don't go overboard on particle effects for each sphere
3. **Limit lifetime** - 5 seconds is usually enough
4. **Pool the prefabs** - Reuse spiral pairs instead of destroying/creating

---

## 🎮 Gameplay Balance

### **Damage:**
- Twin spheres = **Same total damage** as single fireball
- Don't double the damage (it's a visual upgrade, not a power upgrade)

### **Cooldown:**
- Keep same cooldown as single projectile
- Or slightly longer (0.1-0.2s) if it feels too powerful visually

### **Mana Cost:**
- Same cost as regular fireball
- Or +5 mana for "upgraded" feel

---

## ✨ Example Configurations

### **Void Spiral (Your Current Setup):**
```
Projectile: Dark purple void spheres
Trail: Purple to dark purple
Spiral Radius: 0.6
Spiral Speed: 360
Forward Speed: 25
Effect: Mystical, otherworldly
```

### **Fire Helix:**
```
Projectile: Orange fire spheres
Trail: Yellow to orange to red
Spiral Radius: 0.4
Spiral Speed: 540
Forward Speed: 28
Effect: Fast, aggressive
```

### **Ice Twins:**
```
Projectile: Blue ice spheres
Trail: White to blue
Spiral Radius: 0.8
Spiral Speed: 180
Forward Speed: 20
Effect: Slow, graceful, cold
```

### **Lightning Coil:**
```
Projectile: White/yellow electric spheres
Trail: Bright white (thin)
Spiral Radius: 0.3
Spiral Speed: 900
Forward Speed: 35
Effect: Extremely fast, tight spiral
```

---

## 🔄 Switching Between Single and Spiral

You can toggle anytime:

**In MageSpellCaster:**
- ✅ **Use Spiral Effect** = Twin spiral projectiles
- ❌ **Use Spiral Effect** = Single projectile (normal)

Both use the same cooldown, mana, and damage settings!

---

## 📝 Files Modified

- **New**: `SpiralProjectilePair.cs` - Controls the spiral effect
- **Modified**: `MageSpellCaster.cs` - Added spiral support
- **New**: `VoidSpiralPair` prefab - The spiral projectile pair

---

Perfect for boss abilities, upgraded spells, or just making your attacks look **incredibly cool**! 🌀✨

**Pro Tip**: Combine with particle effects, screen shake, and sound effects for maximum impact!

