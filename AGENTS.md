# AGENTS.md

## 1. Core Project Rules

- Always inspect the existing Unity project before making changes.
- Reuse existing systems, scripts, assets, and GameObjects when appropriate.
- Do not create duplicate managers, systems, or utilities without a clear reason.
- Make the smallest change that correctly solves the requested task.
- Do not make unrelated changes.
- Preserve existing functionality unless the requested change requires otherwise.

## 2. Agent Workflow

For every non-trivial task:

1. Inspect the relevant scene, hierarchy, scripts, components, and existing architecture.
2. Identify the smallest appropriate implementation.
3. Briefly state what will be changed when useful.
4. Implement the change.
5. Verify the affected objects, references, scripts, and functionality.
6. Report what changed and what was verified.

If requirements are ambiguous and the decision could cause destructive or architectural changes, ask before proceeding.

## 3. Unity Scene Rules

- Before modifying a scene, inspect the current scene hierarchy.
- Before creating a GameObject, check whether an existing object can be reused.
- Before adding a component, check whether the required component already exists.
- Keep the hierarchy organized and meaningful.
- Do not delete existing GameObjects, components, scenes, or assets unless explicitly requested.
- Do not rename existing GameObjects or assets unless required.
- After scene modifications, verify important references are still valid.

## 4. C# Rules

- Use clear and meaningful names.
- Classes and public methods use PascalCase.
- Local variables and parameters use camelCase.
- Follow the existing project's private-field naming convention. If no convention exists, prefer _camelCase.
- Prefer private fields with [SerializeField] for Inspector configuration.
- Avoid public fields unless they are intentionally part of an API.
- Never use unexplained magic numbers in code. Replace hard-coded numeric values with clearly named constants, variables, or serialized fields, depending on their purpose.
- Every numeric value used for gameplay logic, calculations, timing, movement, animation, UI layout, thresholds, limits, or configuration must have a meaningful, self-explanatory name. 
- The name must clearly communicate what the value represents.
- Prefer const for fixed compile-time values, static readonly for fixed values initialized at runtime, local variables for temporary calculations, and [SerializeField] private fields for values that need Inspector configuration.
- Use names that describe the purpose and unit of a value where applicable. For example, prefer pourDurationSeconds, maximumBottleCapacity, shakeIntensity, moveSpeed, and minimumMatchingLayerCount over duration, value, or temp.
- Do not replace magic numbers with generic variable names such as num, value, temp, or amount when a more descriptive name is possible.
- Avoid repeating the same numeric value across multiple locations. Reuse the appropriate named constant, variable, or configuration field when the values represent the same concept.
- Do not introduce unnecessary variables for values that are self-explanatory by their context, such as index++, array indices, loop counters, or standard mathematical expressions. However, extract meaningful domain-specific values when doing so improves readability.
- Do not create constants for every number indiscriminately. Numeric values that are inherently meaningful and unambiguous in context, such as 0 for an initial index or 1 for a simple increment, may remain inline when extracting them would reduce readability.
- Follow existing project naming conventions and architecture when introducing constants, variables, or serialized fields.
- Avoid magic strings as well. Extract repeated or important string values into appropriately named constants when beneficial.
- Keep methods focused and reasonably small.
- Remove unused variables, methods, and imports when making related changes.
- Do not introduce unnecessary abstraction.
- When modifying existing code, replace relevant magic numbers in the affected code rather than leaving unexplained numeric values behind.
- After implementation, review the changed code to ensure numeric values are appropriately named, their purposes are clear, and no unnecessary abstractions were introduced.

## 5. Architecture

- Prefer composition over large monolithic scripts.
- Follow Single Responsibility Principle where practical.
- Keep gameplay logic separate from UI logic.
- Keep data separate from behaviour when appropriate.
- Avoid Singleton patterns unless there is a clear project-level reason.
- Do not introduce Zenject, VContainer, or another dependency-injection framework unless explicitly requested.
- Use ScriptableObjects when they provide a clear benefit for shared or configurable data.
- Prefer existing project architecture over introducing a new architecture for a small feature.

## 6. Gameplay Prototype Rules

- Prioritize a playable prototype before polish or advanced architecture.
- Build systems incrementally.
- Do not implement future systems before they are required.
- Separate gameplay concerns where practical:
  - Input
  - Object movement
  - Destination detection
  - Validation
  - Scoring
  - Game state
  - UI
- Use placeholder assets when final art is unavailable.
- Gameplay should work correctly before adding visual polish.
- Do not add ads, IAP, analytics, live ops, backend services, or other production systems unless explicitly requested.

## 7. Performance

This project targets mobile platforms.

- Consider mobile performance when implementing gameplay systems.
- Avoid unnecessary allocations during gameplay.
- Avoid repeated `GameObject.Find`, `FindObjectOfType`, or similar scene searches in frequently executed code.
- Cache references when appropriate.
- Avoid unnecessary `Update` methods.
- Use object pooling for frequently spawned and destroyed objects when appropriate.
- Consider garbage collection, CPU usage, draw calls, memory usage, and asset size when relevant.
- Do not prematurely optimize simple prototype code without evidence of a problem.

## 8. Unity Lifecycle

- Use `Awake`, `OnEnable`, `Start`, `Update`, `FixedUpdate`, and other lifecycle methods for their intended purposes.
- Initialize internal references in appropriate lifecycle methods.
- Use `FixedUpdate` for physics-related operations when appropriate.
- Do not put expensive or unrelated work into `Update` simply because it is convenient.

## 9. Input and Gameplay

- Keep input detection separate from gameplay logic when practical.
- Gameplay systems should not depend unnecessarily on a specific input device.
- Support the intended mobile input model without hard-coding unnecessary platform assumptions.
- Validate gameplay state before performing state-changing operations.

## 10. Assets and Dependencies

- Do not add external packages unless explicitly requested or clearly required.
- Do not modify Project Settings unless required for the requested task.
- Do not replace existing assets without understanding where they are used.
- Do not create unnecessary duplicate materials, prefabs, textures, or ScriptableObjects.
- Prefer prefabs for reusable GameObjects.

## 11. MCP Rules

- Use Unity MCP to inspect and modify the Unity project when appropriate.
- Inspect before modifying.
- Do not use MCP to make unrelated changes.
- Do not delete files or assets without explicit approval.
- Do not modify project-wide settings unless explicitly requested or required.
- When an MCP operation fails, diagnose the failure before attempting unrelated workarounds.
- After MCP scene modifications, verify the resulting Unity state.

## 12. Safety and Destructive Changes

The following require explicit user approval unless the user has directly requested them:

- Deleting assets
- Deleting scenes
- Removing major systems
- Renaming large groups of assets or GameObjects
- Changing package dependencies
- Changing build settings
- Changing project settings
- Replacing an existing architecture
- Performing broad refactors

## 13. Testing and Verification

After implementing a feature:

- Check for compilation errors when possible.
- Verify serialized references.
- Verify required GameObjects and components exist.
- Verify affected scenes can load correctly.
- Test the affected gameplay flow when possible.
- Do not claim something was tested if it was only inspected.

## 14. Communication

When completing a task, summarize:

- What was changed
- Which files/scenes were affected
- What was verified
- Any remaining issue or limitation

Do not hide errors or silently work around unexpected project behaviour.

## 15. Priority

When rules conflict, follow this priority:

1. Explicit user request
2. Existing project functionality and architecture
3. These project rules
4. General best practices

Do not follow these rules so rigidly that they prevent a clearly requested feature from being implemented.
