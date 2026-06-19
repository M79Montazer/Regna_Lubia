# Persistent State System — Manual Godot Setup

The code changes add a persistent world/puzzle state system. The following **manual steps in the Godot editor** are required to make it work.

---

## 1. Add GameState as an Autoload

**Why**: `GameState` is the central singleton that stores all persistent flags, variables, and picked-item state. It must be alive from game start.

**Steps**:
1. Open Godot, go to **Project > Project Settings > Autoload** tab.
2. In the **Path** field, click the folder icon and select `Scripts/Core/GameState.cs`.
3. In the **Node Name** field, type `GameState`.
4. Click **Add**.
5. Ensure it appears in the list with **Enable** checked.

**Alternative** (if you prefer not to use Autoload):
- Open `Main.tscn`, add a `GameState` node as a child of the root `Main` node.
- This is less reliable because `_Ready()` ordering matters, but the `GameStateLocator` helper will still find it.

---

## 2. Assign PersistentId to Pickup Items

**Why**: Pickups with a `PersistentId` will be remembered as "already picked" across room reloads.

**Steps**:
1. For each `PickupItem` node in your room scenes that should NOT respawn:
   - Select the node.
   - In the Inspector, find the **PersistentId** field (under the PickupItem script section).
   - Enter a unique string, e.g. `"bedroom_letter"`, `"hallway_key"`, `"balcony_note"`.
2. Items **without** a `PersistentId` will work as before (no persistence), but a warning will be printed in the console.

---

## 3. Assign PersistentId to Doors

**Why**: Doors with a `PersistentId` will remember their locked/unlocked state across room reloads.

**Steps**:
1. For each `RoomTransition` node that should persist its lock state:
   - Select the node.
   - In the Inspector, set **PersistentId** to a unique string, e.g. `"bedroom_door"`, `"main_door"`.

---

## 4. Configure Puzzle-Controlled Doors

### 4a. Using UnlockFlagKey

**Why**: A door can be unlocked by a simple boolean flag in `GameState`. This is the simplest way to wire a puzzle solution to a door.

**Steps**:
1. Select the `RoomTransition` node.
2. In the Inspector, set **UnlockFlagKey** to the flag key that the puzzle will set to `true`.
   - Example: If a `StatefulButton` with `SolvedFlagKey = "puzzle.altar.solved"`, set `UnlockFlagKey = "puzzle.altar.solved"`.
   - For a combination lock with `LockId = "safe1"`, the lock sets `"puzzle.safe1.solved"`, so set `UnlockFlagKey = "puzzle.safe1.solved"`.

### 4b. Using UnlockConditions (advanced)

**Why**: When a door should unlock based on multiple conditions (e.g. two buttons pressed, or a specific string value).

**Steps**:
1. In the **FileSystem** dock, create a new `StateCondition` resource (right-click > New Resource > StateCondition).
2. Save it to a file like `Resources/conditions/door_altar_unlock.tres`.
3. Configure the condition in the Inspector:
   - **Key**: The GameState key to check (e.g. `"puzzle.altar.solved"`).
   - **Type**: `Flag`, `String`, or `Int`.
   - **Operator**: `Equals`, `NotEquals`, `GreaterThan`, etc.
   - **ExpectedFlag/ExpectedString/ExpectedInt**: The expected value.
4. Select the `RoomTransition` node.
5. In the Inspector, expand **UnlockConditions** array, set size to the number of conditions, and assign each `StateCondition` resource.
6. All conditions must evaluate to `true` for the door to unlock.

---

## 5. Set Up StatefulButton Nodes

**Why**: `StatefulButton` is a generic interactable that sets flags in `GameState`. Use it for levers, buttons, pressure plates, altar activations, etc.

**Steps**:
1. In your room scene, add an `Area2D` node.
2. Add a `CollisionShape2D` child for the interaction area.
3. Attach the `Scripts/Props/StatefulButton.cs` script.
4. Configure in the Inspector:

   | Property | Description | Example |
   |----------|-------------|---------|
   | `PersistentId` | Unique ID for this button | `"altar_button"` |
   | `LabelText` | What the player sees in the prompt | `"Altar"` |
   | `PuzzleId` | Groups this button into a puzzle | `"altar"` |
   | `Value` | The value this button sets when pressed | `"moon"` |
   | `StartsPressed` | Initial state | `false` |
   | `ToggleMode` | Can it be pressed again to unpress? | `false` |
   | `ExclusiveWithinPuzzle` | Only one button in the puzzle can be active at a time | `true` |
   | `CorrectValue` | If set, `puzzle.{PuzzleId}.solved` is set to true when `Value == CorrectValue` | `"moon"` |

   **Default flag keys** (auto-derived if left empty):
   - Pressed state: `button.{PersistentId}.pressed`
   - Active value: `puzzle.{PuzzleId}.active`
   - Solved flag: `puzzle.{PuzzleId}.solved`
   - Override any of these by setting `PressedFlagKey`, `ActiveValueKey`, or `SolvedFlagKey` explicitly.

   **Example puzzle**: Three buttons (`moon`, `sun`, `star`) in puzzle `"altar"`. Only `"moon"` is correct.
   - Each button gets `PuzzleId = "altar"`, `ExclusiveWithinPuzzle = true`, `CorrectValue = "moon"`.
   - When the moon button is pressed: `puzzle.altar.active = "moon"`, `puzzle.altar.solved = true`.
   - When a wrong button is pressed: `puzzle.altar.active = "sun"`, `puzzle.altar.solved = false`.
   - The target door has `UnlockFlagKey = "puzzle.altar.solved"`.

---

## 6. Configure Combination Lock LockId

**Why**: The `CombinationLockPanel` writes `lock.{LockId}.unlocked` and `puzzle.{LockId}.solved` to `GameState` when the correct code is entered.

**Steps**:
1. In the **FileSystem** dock, select your `CombinationLockItemData` resource (e.g. `Resources/lock1.tres`).
2. In the Inspector, set **LockId** to a unique string, e.g. `"safe1"`.
3. On the target `RoomTransition` door, set `UnlockFlagKey = "lock.safe1.unlocked"` or `UnlockFlagKey = "puzzle.safe1.solved"`.

---

## 7. Important Notes

- **Save/load is not implemented.** State persists only while the game process is running. A future save system can serialize `GameState`'s internal dictionaries.
- **StatefulButton visual feedback**: The code does not change sprites or animations (cannot edit scenes). You can optionally add an `AnimatedSprite2D` child and modify `ApplyVisualState()` in a derived script, or handle it via the existing `_Ready()` / `Interact()` flow.
- **ExclusiveWithinPuzzle** works by overwriting the active value key. When a room reloads, buttons re-read their pressed state from `GameState`, so the exclusivity is preserved even if some buttons are in an unloaded room.
