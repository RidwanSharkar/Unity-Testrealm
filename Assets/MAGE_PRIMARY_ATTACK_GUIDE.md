# Mage Primary Attack (Left-Click) Setup Guide

## Overview
This guide explains how to set up and use the Mage's primary attack system - a left-click fireball with casting animation and trail effects.

## What Was Implemented

### 1. **Input System Updates** (`PlayerController.cs`)
- **Left-Click (Primary Attack)**: Triggers single-cast abilities like fireball
- **Right-Click (Secondary Attack)**: Continuous held attacks like spell beams
- The camera rotation is now handled by `ThirdPersonCamera` component

### 2. **Base Weapon Updates** (`BaseWeapon.cs`)
- Added `PerformPrimaryAttack()` virtual method for left-click attacks
- `PerformAttack()` remains for right-click secondary attacks
- Default implementation calls `PerformAttack()` for backwards compatibility

### 3. **Projectile System** (New)
Created a robust projectile system following Unity best practices:

#### **Projectile.cs** - Base Projectile Class
- Handles movement, collision, and lifetime
- Supports trail renderers and visual effects
- Plays impact sounds and spawns impact effects
- Automatically calculates damage and communicates with CombatSystem
- Layer mask filtering for collision detection

#### **MageFireballProjectile.cs** - Mage-Specific Fireball
- Extends `Projectile` class
- Includes fire light with pulsing intensity
- Particle system for fire effects
- Rotates for visual appeal
- Fast-moving (25 units/second)

### 4. **Staff Weapon Updates** (`StaffWeapon.cs`)
- **Primary Attack (Left-Click)**: Casts fireball with casting animation
  - Uses coroutine for proper animation timing
  - Spawns casting effect at staff tip during cast
  - Shoots towards camera forward for accurate aiming
  - Cooldown system prevents spam
  - Mana cost system
- **Secondary Attack (Right-Click)**: Continuous spell projectiles (existing)

### 5. **ProjectileManager.cs** - Object Pooling System
- Singleton pattern for global access
- Object pooling for performance optimization
- Prevents garbage collection spikes
- Configurable pool sizes
- Dynamic pool growth option
- `PooledProjectile` component for automatic pool returns

---

## Unity Setup Instructions

### Step 1: Create the Fireball Prefab

1. **Create a new GameObject** in your scene
   - Name it: `MageFireballPrimary`

2. **Add Components**:
   - `Rigidbody`
     - Use Gravity: OFF
     - Collision Detection: Continuous Dynamic
     - Constraints: Freeze Rotation (all axes)
   - `Sphere Collider`
     - Radius: 0.5
     - Is Trigger: OFF
   - `MageFireballProjectile` script
   - `TrailRenderer` (for trail effect)
     - Time: 0.5
     - Width: 0.3 → 0
     - Material: Use a fire/glow material
     - Color: Gradient from orange to red

3. **Add Visual Effects**:
   - Add a child GameObject named "FireVisual"
   - Add a sphere mesh or fire sprite
   - Add a Point Light component:
     - Color: Orange/Red (RGB: 255, 128, 0)
     - Range: 5
     - Intensity: 2
   - Optional: Add a Particle System for fire particles

4. **Configure MageFireballProjectile**:
   - Speed: 25
   - Lifetime: 5
   - Use Gravity: OFF
   - Fire Light: Drag the Light component
   - Light Intensity: 2
   - Fire Color: Orange
   - Fireball Scale: 1
   - Rotate Over Time: ON
   - Rotation Speed: 360

5. **Set Collision Layer**:
   - Create/use a layer called "Projectile"
   - Set the fireball GameObject to this layer
   - Configure collision matrix in Project Settings → Physics

6. **Save as Prefab**:
   - Drag to `Assets/Prefabs/Projectiles/` folder
   - Delete from scene

---

### Step 2: Configure the Staff Weapon

1. **Find your Staff Weapon GameObject** (should be attached to the Mage character)

2. **Assign References in StaffWeapon Component**:
   
   **Staff Settings:**
   - Spell Cast Point: Create an empty GameObject at the tip of the staff
   
   **Primary Attack - Fireball:**
   - Primary Fireball Prefab: Drag `MageFireballPrimary` prefab
   - Primary Fireball Damage: 15 (adjust as needed)
   - Primary Attack Cast Time: 0.5 (match your animation length)
   - Primary Attack Cooldown: 0.8
   - Casting Effect: (Optional) Create a charging VFX prefab
   
   **Mana System:**
   - Use Mana: ON
   - Max Mana: 100
   - Current Mana: 100
   - Basic Spell Mana Cost: 10
   - Mana Regen Rate: 5

3. **Verify Animator Controller**:
   - Ensure your animator has a "Cast" trigger parameter
   - Create a casting animation or use Mixamo's spell-casting animation
   - Set up animation state machine:
     - Idle → Cast (on "Cast" trigger)
     - Cast → Idle (on animation end)

---

### Step 3: Set Up Animation (Optional but Recommended)

If you have a casting animation:

1. **Import Animation**:
   - Use Mixamo or create your own
   - Recommended: "Spellcasting" or "Magic Cast" animations

2. **Add to Animator Controller**:
   ```
   - Add new Animation State: "Casting"
   - Add Trigger Parameter: "Cast"
   - Create Transition: Idle → Casting (Condition: Cast trigger)
   - Create Transition: Casting → Idle (Has Exit Time: ON)
   ```

3. **Adjust Cast Time**:
   - Match `primaryAttackCastTime` in StaffWeapon to animation length
   - Example: If animation is 0.6 seconds, set cast time to 0.6

---

### Step 4: Set Up ProjectileManager (Optional but Recommended)

For better performance with object pooling:

1. **Create Empty GameObject** in scene:
   - Name: "ProjectileManager"
   - Add `ProjectileManager` script

2. **Configure Settings**:
   - Default Pool Size: 20
   - Allow Pool Growth: ON
   - Pool Parent: Leave empty (auto-created)
   - Projectile Prefabs: Add your `MageFireballPrimary` prefab to array

3. **Optional**: Update `Projectile.cs` to use pooling:
   ```csharp
   // In DestroyProjectile() method, replace Destroy(gameObject) with:
   if (ProjectileManager.Instance != null)
   {
       ProjectileManager.Instance.ReturnProjectile(gameObject);
   }
   else
   {
       Destroy(gameObject);
   }
   ```

---

### Step 5: Configure Input (Already Done in Code)

The input system is already configured in `PlayerController.cs`:
- **Left-Click**: Primary Attack (fireball casting)
- **Right-Click**: Secondary Attack (continuous projectiles)
- **Q, E, R**: Abilities (Fireball AoE, Ice Nova, Lightning Strike)

---

### Step 6: Set Up Collision Layers

1. **Create Layers** (if not already created):
   - `Projectile` - for all projectiles
   - `Player` - for player character
   - `Enemy` - for enemies
   - `Environment` - for walls, ground, etc.

2. **Configure Physics Collision Matrix** (Edit → Project Settings → Physics):
   - Projectile ✓ Enemy
   - Projectile ✓ Environment
   - Projectile ✗ Player (projectiles don't hit caster)
   - Projectile ✗ Projectile (projectiles don't collide with each other)

3. **Set Collision Mask in Projectile**:
   - Select MageFireballPrimary prefab
   - In MageFireballProjectile component:
   - Collision Mask: Select `Enemy` and `Environment` layers

---

## Testing the System

### In-Game Testing:

1. **Start Play Mode**
2. **Equip Staff Weapon** (should auto-equip if set as first weapon)
3. **Left-Click** to cast fireball
   - Should see casting animation
   - Fireball spawns after cast time
   - Fireball shoots forward with trail
   - Hits enemies and deals damage
4. **Right-Click Hold** for continuous spell projectiles

### Debug Console Output:

You should see:
```
Launched fireball! (15 damage)
Fireball hit [EnemyName]!
```

---

## Customization Options

### Fireball Visual Customization:

**Speed**: Adjust `speed` in MageFireballProjectile
- Slower: 15-20 (easier to dodge)
- Faster: 25-30 (harder to dodge)

**Size**: Adjust `fireballScale`
- Smaller: 0.5-0.8
- Larger: 1.2-2.0

**Trail**: Modify TrailRenderer
- Width Curve: Control trail thickness
- Color Gradient: Change colors
- Time: How long trail lasts

**Damage**: Adjust `primaryFireballDamage` in StaffWeapon
- Low: 10-15
- Medium: 20-30
- High: 40-50

### Gameplay Customization:

**Cast Time** (`primaryAttackCastTime`):
- Instant: 0.1-0.2 seconds
- Quick: 0.3-0.5 seconds
- Slow: 0.8-1.2 seconds

**Cooldown** (`primaryAttackCooldown`):
- Rapid Fire: 0.3-0.5 seconds
- Balanced: 0.8-1.2 seconds
- Powerful: 2-3 seconds

**Mana Cost** (`basicSpellManaCost`):
- Low: 5-10 mana
- Medium: 15-20 mana
- High: 25-30 mana

---

## Advanced Features

### Animation Events (Advanced):

Instead of using `WaitForSeconds`, you can use Animation Events:

1. **Open Animation** in Animation window
2. **Add Event** at the frame where fireball should spawn
3. **Create Method** in StaffWeapon:
   ```csharp
   public void OnCastFireball()
   {
       SpawnPrimaryFireball();
   }
   ```
4. **Assign Method** to animation event

### Homing Projectiles (Advanced):

Add to MageFireballProjectile:
```csharp
[SerializeField] private bool homing = false;
[SerializeField] private float homingStrength = 5f;
private Transform target;

protected override void Update()
{
    base.Update();
    
    if (homing && target != null && rb != null)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * speed, homingStrength * Time.deltaTime);
    }
}
```

### Multiple Fireball Types:

Create variants:
- **Ice Bolt**: Blue color, slows enemies
- **Lightning Bolt**: Fast, chain damage
- **Poison Bolt**: Green, DoT effect

Just duplicate the prefab and adjust:
- Colors (light, trail, particles)
- Speed and damage
- Impact effects
- Special behaviors

---

## Troubleshooting

### Fireball doesn't spawn:
- ✓ Check `primaryFireballPrefab` is assigned in StaffWeapon
- ✓ Check `spellCastPoint` is assigned
- ✓ Check console for warnings
- ✓ Verify mana is not depleted

### Fireball doesn't move:
- ✓ Check Rigidbody is attached and not kinematic
- ✓ Verify speed is > 0
- ✓ Check if gravity is correctly set

### Fireball doesn't hit enemies:
- ✓ Check collision layers and physics matrix
- ✓ Verify enemies have colliders
- ✓ Ensure collision mask includes enemy layer
- ✓ Check if enemies have Entity component

### Animation doesn't play:
- ✓ Check Animator has "Cast" trigger parameter
- ✓ Verify animator controller is assigned
- ✓ Check animation state transitions

### No trail effect:
- ✓ Verify TrailRenderer component is present
- ✓ Check trail material is assigned
- ✓ Ensure trail time > 0
- ✓ Verify trail width is not zero

---

## Performance Tips

1. **Use ProjectileManager** for object pooling
2. **Limit particle count** in particle systems
3. **Use simple collision shapes** (sphere is good)
4. **Disable trails** on low-end hardware
5. **Reduce light range** if needed
6. **Use LOD** for projectile meshes if complex

---

## Architecture Overview

```
PlayerController
    ↓ (detects left-click)
StaffWeapon.PerformPrimaryAttack()
    ↓ (starts coroutine)
CastPrimaryFireball()
    ↓ (plays animation)
    ↓ (waits for cast time)
SpawnPrimaryFireball()
    ↓ (instantiates prefab)
MageFireballProjectile.Initialize()
    ↓ (sets velocity and owner)
OnCollisionEnter()
    ↓ (detects hit)
OnHitEntity()
    ↓ (calculates damage)
CombatSystem.QueueDamage()
    ↓ (applies damage to target)
```

---

## File Summary

### New Files:
- `Assets/Scripts/Weapons/Projectile.cs` - Base projectile class
- `Assets/Scripts/Weapons/MageFireballProjectile.cs` - Mage fireball implementation
- `Assets/Scripts/Systems/ProjectileManager.cs` - Object pooling system
- `Assets/MAGE_PRIMARY_ATTACK_GUIDE.md` - This guide

### Modified Files:
- `Assets/Scripts/Core/PlayerController.cs` - Added left-click primary attack input
- `Assets/Scripts/Weapons/BaseWeapon.cs` - Added PerformPrimaryAttack() method
- `Assets/Scripts/Weapons/StaffWeapon.cs` - Implemented primary fireball attack

---

## Next Steps

1. ✅ Create fireball prefab in Unity
2. ✅ Assign prefab to StaffWeapon
3. ✅ Configure cast point transform
4. ✅ Set up collision layers
5. ✅ Import/create casting animation
6. ✅ Test in play mode
7. ✅ Adjust damage, speed, and cooldown values
8. ✅ Create impact effect prefabs (optional)
9. ✅ Set up ProjectileManager for pooling (optional)

---

## Credits

Implementation follows Unity best practices:
- Component-based architecture
- Object pooling for performance
- Clear separation of concerns
- Extensible base classes
- Coroutines for timing
- Physics-based projectiles

---

## Support

If you encounter issues:
1. Check console for error messages
2. Verify all references are assigned
3. Review collision layer setup
4. Ensure animations are configured
5. Test with simplified prefab first

Good luck with your Mage attacks! 🔥✨

