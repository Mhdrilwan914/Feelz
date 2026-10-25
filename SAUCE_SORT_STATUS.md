# Sauce Sort Lab Prototype Status

_Last updated: 2026-10-20_

## Completed

- Created the Assets/SauceSort folder structure for art, audio, prefabs, scenes, scripts, data, and editor tools.
- Created the Game scene with a Game root, SortingArea, ConveyorArea, portrait camera, Canvas, and EventSystem.
- Implemented bottle state and visuals with four-layer capacity and whole-group pour validation.
- Implemented bottle selection, pour animation, and invalid-move feedback. Invalid moves leave bottle state unchanged and unlock input after the shake.
- Added LevelData, a fixed catalog, and LevelManager for three hand-authored puzzles:
  - Level 1: 4 bottles, 2 sauce types, 2 empty bottles.
  - Level 2: 5 bottles, 3 sauce types, 2 empty bottles.
  - Level 3: 6 bottles, 4 sauce types, 2 empty bottles.
- Set up progression from Level 1 through Level 3; final completion is shown in game and logged to the Unity Console.
- Added six bottle instances to the scene and assigned the LevelManager references.
- Implemented the conveyor and completed-bottle matching:
  - ConveyorController is attached to the existing ConveyorArea.
  - It creates the belt and sauce-colored target markers at runtime.
  - When a bottle is full and contains one sauce type, it moves to a matching target and is disabled for further input.
  - A level advances after all required sauce targets are matched.
- Simplified ConveyorController while preserving its matching and delivery behavior.
- Saved Assets/SauceSort/Scenes/Game.unity with the conveyor component attached.
- Added a small player-facing level counter, a brief message before automatic progression, and a final completion message.
- Simplified the active bottle-selection API and removed an unused level-transition handle.
- Created `Assets/SauceSort/Scenes/Test Scene.unity` as an isolated gameplay scene for trying the replacement bottle art.
- In Test Scene, replaced Bottle_1's frame renderers with a generated bear-themed transparent bottle sprite at `Assets/SauceSort/Art/Sprites/BearBottleSprite.png`. Set its local scale to `(0.25, 0.30, 1)`; the sauce layers remain visible through the glass. Saved the scene; `Game.unity` was not changed for this art preview.
- Replaced the runtime square sauce blocks with generated liquid meshes that taper at the bottle heel and shoulder, use per-sauce vertical gradients and a glossy curved top surface, and remain inside the bottle silhouette. Pour visuals now travel as rounded sauce drops along the existing eased pour path.
- Added an original transparent bottle glass overlay at `Assets/SauceSort/Art/Bottles/Experimental/BottleGlassOverlay.png` and assigned it to the bottle prefab above the liquid. Existing frame renderers were disabled on the prefab; source artwork was preserved.
- Updated `BottleController` to reuse mesh buffers, close small seams between liquid units, and render source/destination fill previews during a pour without changing logical bottle state.
- Updated `BottlePourAnimator` to reuse a curved `LineRenderer` stream and restore visuals, bottle transforms, and input state when a pour is cancelled.
- Applied Android import settings to the new overlay texture: alpha enabled, mipmaps off, bilinear/clamp, 1024 maximum size, ETC2 RGBA8 compression.

## Experimental Shader Graph Trial

- Created `Assets/SauceSort/Art/Experimental/BottleSauceMaskExperimental.shadergraph` and its material from the Shader Graph Built-In Unlit template.
- Assigned the experimental material to `Bottle_1`'s bottom liquid layer in `Assets/SauceSort/Scenes/Test Scene.unity`. This bottle is an instance of the same `Bottle` prefab used in `Game.unity`.
- The Shader Graph imports and its shader pass reports compiled, but the bottle layer renders magenta in the Scene and Game views. The experiment is not visually working yet; the material remains assigned in Test Scene so the result can be inspected.
- The trial uses the existing shaped liquid mesh for the bottle silhouette. A separate bottle mask texture has not been created. `Game.unity` and the project render-pipeline settings were not changed.
- Preview screenshot: `Assets/Screenshots/screenshot-20261020-231337.png`.

## Verification

- Unity compilation completed with zero console errors after the conveyor controller update.
- Unity recompiled the level UI update successfully with zero console errors.
- A Play Mode walkthrough completed all three levels through the bottle selection controller using 20 legal pours, matched every conveyor target, and showed progression and final-completion UI.
- Invalid selection preserved both bottle states and released the input lock after its shake.
- The core bottle-rule examples all passed.
- ConveyorController.cs and LevelManager.cs reported zero script diagnostics.
- Earlier shared work history verified legal move sequences for all three puzzles and level progression.
- Normal pointer/touch input and Android hardware have not been tested.
- Visually checked the bear bottle in Test Scene Play Mode; its height and width align with neighboring bottle frames, and the sauce layers show through the glass in the current portrait preview.
- Visually checked the shaped red and blue liquid fills in Test Scene Play Mode; Unity compilation completed without new errors after the liquid visual update.
- Visually checked empty, one-unit, stacked-color, and full bottle states; checked transparent glass against light and dark backgrounds and a portrait aspect ratio.
- Checked a valid pour through completion, an invalid move, and cancellation of an active pour. Contents remained unchanged on invalid/cancelled moves and input unlocked afterward.
- Built the enabled Game scene for Android with IL2CPP/Gradle successfully to a temporary APK (`FeelzBottleVisualValidation.apk`, about 33.7 MB). The APK has not been installed or tested on an Android device.

## Remaining Gameplay Tests

- Play through each level using normal player input and confirm bottle selection, valid pours, and invalid-pour recovery.
- Confirm completed bottles move to the correct sauce targets and cannot be selected again.
- Confirm delivered bottles cannot be selected again through normal pointer/touch input.
- Check conveyor, target, and bottle placement at portrait aspect ratio; adjust layout if any objects overlap or fall outside the camera view.
- Install and verify the Android build on device; the build succeeded but device-side rendering and performance remain unchecked.

## Remaining Implementation

- Diagnose the magenta Shader Graph output in Test Scene before relying on the shader for liquid rendering; create a dedicated bottle mask only after the graph renders correctly.
- Enable selecting a level in Edit Mode; the current LevelManager context-menu actions require Play Mode.
- Decide whether hand-authored levels should remain in code or move to editable Unity data assets.
- Review the extra prototype drag-and-drop scripts and materials and remove them if confirmed unused.
- Consider random level generation and a solver later, if needed.
