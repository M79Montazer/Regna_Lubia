# Regna.Lubia — Godot 4.6 C# 2D Metroidvania

## Stack
- **Godot 4.6**, C# (.NET 8), GL Compatibility renderer, Jolt Physics 3D
- Single `.csproj` targeting `net8.0` (net9.0 on Android). No NuGet dependencies beyond Godot.NET.Sdk.

## Key architecture
- Entrypoint: `Scenes/World/Main.tscn` → `Scripts/World/Main.cs` (autoload or root)
- Player: `Scenes/Player/player.tscn` → `PlayerController` (CharacterBody2D). Children: `Inventory`, `Health`, `PlayerCombat`, `InteractionDetector`
- Room system: `RoomManager` loads scenes from `RoomMap` dict, swaps rooms via `LoadRoom()`, teleports player to a `Marker2D` spawn point
- Interaction: `IInteractable` interface on Area2D nodes. `InteractionDetector` polls overlapping areas each physics frame
- Combat: Melee (Area2D hitbox, 0.08s active, 0.25s cooldown) + Ranged (projectile scene, 0.35s cooldown)
- Inventory: 8-slot, slot selection drives `SelectionChanged` event, `HasSelectedKey()` for locked doors
- Items: `ItemData` is a `[GlobalClass] Resource` (Key or Readable). Defined as `.tres` files in Resources/

## Physics layers (from `Resources/mask_layers.txt`)
1: world geometry, 2: player, 3: enemy, 4: interactables, 5: player attacks/projectiles

## Z-index convention
Background walls -10, Floor 0, Items 5, Player/Enemies 10, Foreground 20, UI = CanvasLayer

## Input map
- Movement: A/D or Left/Right arrows
- E: interact, X: melee attack, C: ranged attack, Escape: pause

## How to run / build
```powershell
# Build C# solution
dotnet build

# Run (launches Godot editor; use --headless for CI)
godot --path .           # play mode
godot --path . --headless --build-solutions  # headless build check
```

There are no tests, no CI config, no formatter/linter config. `.editorconfig` is minimal (utf-8 charset only).
`.uid` files next to C# scripts are Godot-internal resource UID caches — do not touch.