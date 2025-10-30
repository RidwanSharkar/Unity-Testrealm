# Mixamo Character Import Guide for Unity

## 📥 Step 1: Download from Mixamo

**Format Selection:**
- ✅ **Use "FBX for Unity"** format (not FBX Binary)
- Select **T-pose** for character models
- For animations, download separately in T-pose or the animation pose

**Important:** Mixamo downloads are **single .FBX files** with **NO texture files**. This is normal!

---

## 📦 Step 2: Import into Unity

1. **Place FBX file** in `Assets/Models/` folder
2. Unity will automatically import and show in Project window
3. Select the imported model and check the **Inspector**

### Import Settings (Unity automatically handles most of this with "FBX for Unity" format):

- **Rig Tab:**
  - Animation Type: `Humanoid` (Unity will auto-detect)
  - Avatar Definition: `Create From This Model`

- **Materials Tab:**
  - Material Creation Mode: `Standard` or `Use External Materials (Legacy)`
  - Extract Materials: ✅ Enabled (if available)
  - Location: `Assets/Materials`

- **Meshes Tab:**
  - Scale Factor: Usually `0.01` for Mixamo models (Unity format handles this)

---

## 🎨 Step 3: Create Materials for Your Character

Mixamo models come without textures, so you need to create materials:

### Option A: Stylized Character Material (Recommended for Bloomscythe)

1. **Create Material:**
   - Right-click in `Assets/Materials/` → `Create → Material`
   - Name it: `CharacterMaterial` or `[CharacterName]_Material`

2. **Configure Shader:**
   - **Shader:** Universal Render Pipeline → `Lit` (for URP) or `Standard` (Built-in RP)
   - For stylized look (WoW/V Rising inspired):
     - **Surface Type:** Opaque
     - **Rendering Mode:** Opaque
     - **Smoothness:** 0.3-0.5 (less glossy = more stylized)
     - **Metallic:** 0 (for non-metal characters)

3. **Add Textures (if you have them):**
   - **Base Map (Albedo):** Drag texture to Base Map slot
   - **Normal Map:** If available, add to Normal Map slot
   - **Metallic/Smoothness:** If you have packed texture
   - **Emission:** Optional for glowing parts

4. **Apply Color Scheme:**
   - Use **Base Color** picker to set main skin/clothing color
   - For stylized look, use more saturated colors

5. **Apply to Model:**
   - Select your character prefab/model in scene
   - Drag material onto the mesh in Scene view or Inspector

### Option B: Create Multiple Materials (for detailed characters)

If your character has multiple parts (skin, clothing, armor):

1. Create separate materials:
   - `CharacterSkin_Material`
   - `CharacterClothing_Material`
   - `CharacterArmor_Material`

2. Assign each material to specific meshes:
   - Select character model → Check meshes in Inspector
   - Assign materials to each sub-mesh

---

## 🎨 Creating Textures (Optional)

Since Mixamo doesn't provide textures, you can:

### Option 1: Use Unity's Texture Tools
- Create procedural textures using Unity's Texture Generator
- Use gradients and patterns

### Option 2: Download/Buy Textures
- Search for "character textures" on Unity Asset Store
- Use free texture packs
- Use texture painting software (Photoshop, GIMP, Substance Painter)

### Option 3: Use Solid Colors (Stylized Approach)
- For a stylized look (WoW/V Rising style), solid colors with slight gradients work well
- Focus on silhouette and shape rather than detailed textures

---

## 🖌️ Texturing in Blender (Complete Guide)

If you've imported a Unity FBX character into Blender for texturing, follow these steps:

### 📋 Step 1: Select the Mesh (Not the Armature)

1. **In the Outliner (top-right panel):**
   - Click to select the mesh object (usually named like "Body", "CharacterMesh", or "mixamorig:Mesh")
   - **DO NOT select the Armature** - you need the actual mesh geometry

2. **Verify Selection:**
   - Selected object should show orange outline in viewport
   - Properties panel should show mesh icon (not armature icon)

---

### 🗺️ Step 2: Check/Create UV Map

The model needs a UV map for textures to work properly.

1. **Switch to UV Editing Workspace:**
   - Click the "UV Editing" tab at the top menu bar
   - This splits the viewport: UV editor on left, 3D view on right

2. **Enter Edit Mode:**
   - Press `Tab` or click "Edit Mode" button (top of viewport)
   - Select all: Press `A` (or `Select → Select All`)

3. **Check Existing UVs:**
   - Look at the UV editor (left side)
   - If you see unwrapped geometry, UVs already exist ✅
   - If empty or messy, continue to unwrap

4. **Unwrap UVs (if needed):**
   - In Edit Mode, select all (`A`)
   - Press `U` to open UV Mapping menu
   - Choose:
     - **"Unwrap"** - Basic unwrap (good for simple models)
     - **"Smart UV Project"** - Automated unwrap with options
     - **"Lightmap Pack"** - Efficient packing for games
   
   - **For Smart UV Project:**
     - Angle Limit: 66° (default)
     - Island Margin: 0.02
     - Click "OK"

5. **Verify UV Layout:**
   - UVs should be within the 0-1 range (the grid in UV editor)
   - Each colored island = one part of the model
   - No overlapping (for clean textures)

---

### 🎨 Step 3: Create Material

1. **Switch to Shading Workspace:**
   - Click "Shading" tab at the top
   - This shows: Shader Editor (bottom), 3D Viewport (top)

2. **Select Mesh Object:**
   - Make sure your character mesh is selected in 3D viewport

3. **Open Material Properties:**
   - In Properties panel (right side), click red sphere icon (Material Properties)
   - Click **"New"** button to create a material
   - Name it: `CharacterMaterial` or similar

4. **Basic Material Setup:**
   - The default Principled BSDF shader is good for Unity export
   - Adjust Base Color:
     - Click color swatch
     - Choose your character's main color
     - Or use a color picker

---

### 🖼️ Method A: Apply Image Texture

**Best for:** Using existing texture images

1. **In Shading Workspace → Shader Editor:**
   - Press `Shift + A` → `Texture → Image Texture`
   - Click "Open" or drag image from File Browser
   - Connect to Base Color of Principled BSDF shader

2. **Alternative - Material Properties Panel:**
   - Properties panel → Material Properties tab
   - Under "Base Color" → Click folder icon
   - Browse and select your texture image

3. **Adjust Settings:**
   - Metallic: 0 (for non-metal characters)
   - Roughness: 0.5-0.7 (adjust shininess)
   - Specular: 0.5 (default)

---

### 🖌️ Method B: Texture Painting (Paint Directly on Model)

**Best for:** Creating custom painted textures

1. **Create Texture Image:**
   - Shading Workspace → Switch to "Texture Paint" workspace
   - In Properties panel → Material tab → Base Color
   - Click dropdown → "New" → Name: `CharacterDiffuse`
   - Set Width/Height: 2048x2048 (or 1024x1024 for testing)
   - Color: White (base color)
   - Click "OK"

2. **Enter Texture Paint Mode:**
   - Top of viewport → "Texture Paint" mode (paintbrush icon)
   - Viewport should show model with texture

3. **Setup Brush:**
   - Left toolbar shows paint options
   - Brush: "Draw" (paintbrush icon)
   - Color: Pick your paint color
   - Size: `F` key to adjust, or slider in toolbar
   - Strength: Controls opacity

4. **Paint on Model:**
   - Click and drag on model to paint
   - Use `F` to resize brush
   - `Shift + F` to adjust brush strength
   - `Ctrl + Z` to undo

5. **Save Your Texture:**
   - In Texture Paint workspace
   - Image menu (top bar) → "Save As"
   - Choose location and name: `CharacterDiffuse.png`
   - **IMPORTANT:** Save before exporting!

---

### 🎨 Method C: Multiple Materials (Different Colors for Body Parts)

**Best for:** Skin, clothing, armor in different colors

1. **In Edit Mode:**
   - Switch to "Modeling" workspace
   - Press `Tab` for Edit Mode
   - Select faces you want to color (e.g., face area)

2. **Create New Material Slot:**
   - Properties panel → Material tab
   - Click "+" next to material slots
   - Click "New" to create material
   - Name it: `SkinMaterial`

3. **Assign Material:**
   - With faces selected, click "Assign" button
   - Selected faces now use this material

4. **Repeat for Other Parts:**
   - Create: `ClothingMaterial`, `ArmorMaterial`, etc.
   - Select relevant faces and assign each material

5. **Color Each Material:**
   - Select material slot in Properties panel
   - Adjust Base Color for that material
   - Repeat for all materials

---

### 💾 Step 4: Save Your Work

1. **Save Blender File:**
   - `File → Save As`
   - Save as: `CharacterTextured.blend`

2. **Save Texture Images (if painted):**
   - Texture Paint workspace
   - `Image → Save As`
   - Save as PNG: `CharacterDiffuse.png`

---

### 📤 Step 5: Export Back to Unity

1. **Select Your Mesh:**
   - Make sure character mesh is selected (not armature)

2. **Export:**
   - `File → Export → FBX (.fbx)`
   - **Important Settings:**
     - ✅ **Selected Objects Only** (uncheck if you want everything)
     - ✅ **Apply Transform** (to reset scale/rotation)
     - Scale: `1.00`
     - Forward: `-Z Forward` (Blender default)
     - Up: `Y Up` (Unity standard)
     - ✅ **Mesh** (geometry)
     - ✅ **Materials** (your materials will export)
     - ✅ **Embed Textures** (includes textures in FBX)
     - OR uncheck and use **External** textures (separate PNG files)

3. **Choose Export Path:**
   - Save to: `Assets/Models/[CharacterName]_Textured.fbx`

4. **If Using External Textures:**
   - Copy texture PNGs to: `Assets/Textures/`
   - Unity will automatically link them

---

### ✅ Quick Checklist

- [ ] Selected mesh (not armature)
- [ ] UV map exists and looks good
- [ ] Material created and colored
- [ ] Texture applied (image or painted)
- [ ] Saved Blender file (.blend)
- [ ] Saved texture images (.png) if painted
- [ ] Exported FBX with correct settings
- [ ] Textures copied to Unity folder (if external)

---

### 💡 Pro Tips

**For Stylized Characters (WoW/V Rising style):**
- Use solid colors with slight gradients
- Lower Roughness (0.3-0.5) for subtle shine
- Use Emission for glowing parts
- Keep textures simple and readable

**For Realistic Characters:**
- Use high-res textures (2048x2048 or 4096x4096)
- Add Normal maps for detail
- Use Roughness maps for surface variation
- Consider using Substance Painter for advanced texturing

**Workflow Tip:**
- Paint base colors first
- Add details with smaller brush
- Save frequently while painting
- Test export early to verify texture appearance in Unity

---

## 🔧 Step 5: Setup for Your Game

### Create Prefab:
1. Drag imported model from Project to Scene
2. Configure in Inspector:
   - Add `Animator` component (if using animations)
   - Add `CharacterController` (for player/enemy movement)
   - Add your scripts (`PlayerController`, `BaseEnemy`, etc.)
3. Save as Prefab: Drag from Hierarchy to `Assets/Prefabs/`

### Configure Animator:
1. Create Animator Controller: `Assets/Animations/[CharacterName]_Controller.controller`
2. Add animation clips:
   - Import animation FBX files separately from Mixamo
   - Each animation becomes a clip in Unity
   - Create states and transitions in Animator Controller
3. Assign Animator Controller to Animator component

---

## 🎯 Quick Setup for Bloomscythe

### For Player Characters:

1. **Import Model:** `Assets/Models/[CharacterName].fbx` (FBX for Unity format)
2. **Create Material:** `Assets/Materials/PlayerMaterial.mat`
   - Base Color: Choose your character's color scheme
   - Smoothness: 0.4
   - Metallic: 0
3. **Create Prefab:**
   - Add `CharacterController`
   - Add `PlayerController` script
   - Add `MovementComponent`
   - Add `HealthComponent`
   - Apply your material
4. **Save:** `Assets/Prefabs/Player_[CharacterName].prefab`

### For Enemy Characters:

1. **Import Model:** Same as above
2. **Create Material:** `Assets/Materials/EnemyMaterial.mat`
3. **Create Prefab:**
   - Add `NavMeshAgent`
   - Add `BaseEnemy` script
   - Add `HealthComponent`
4. **Save:** `Assets/Prefabs/Enemy_[CharacterName].prefab`

---

## ⚠️ Common Issues & Solutions

### Issue: Model looks grey/white in Unity
**Solution:** Create and assign a material (see Step 3)

### Issue: Model is too large/small
**Solution:** 
- Select model in Project → Inspector → Meshes tab
- Adjust Scale Factor (try 0.01 or 100)

### Issue: Character doesn't move correctly
**Solution:**
- Check Rig tab → Animation Type is set to `Humanoid`
- Verify Avatar is created and assigned

### Issue: Animations don't play
**Solution:**
- Ensure Animator Controller is assigned
- Check animation clips are imported
- Verify animation state machine is set up

---

## 📝 Summary

✅ **Use "FBX for Unity" format from Mixamo**  
✅ **Single .FBX file = Normal (no textures included)**  
✅ **Create materials manually in Unity**  
✅ **Assign materials to character meshes**  
✅ **Use stylized colors for WoW/V Rising aesthetic**

The key is: **Mixamo provides geometry, you provide materials/textures in Unity!**

