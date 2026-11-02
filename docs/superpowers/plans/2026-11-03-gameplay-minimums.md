# Gameplay Minimums Implementation Plan

> Execute inline with superpowers:executing-plans.

**Goal:** Complete all five user-approved minimum gameplay items.
**Architecture:** Partida owns scene outcome and resets; existing components deliver collision/life/launch events; InterfazPartida creates responsive Unity UI without editing open scene assets.
**Tech Stack:** Unity 6000.6.7f1, C#, Unity UI, existing legacy input.
**Spec:** docs/superpowers/specs/2026-11-03-gameplay-minimums-design.md

## Global constraints
- Preserve two scenes, existing GUIDs, serialized references, Built-in pipeline and input bindings.
- Do not close or change unsaved user scenes; verify in a separate copy.

## Review focus
- Duplicate collisions/life loss cannot change a terminal result.
- Last-block score appears on the victory panel.
- Repeated retry/menu cycles cannot retain static state or launch on the same input press.
- Bounds recovery loses exactly one life and resets ball/paddle.
- UI stays readable and buttons work at different aspect ratios.

### Task 1: Acceptance baseline
- [x] Add scene-based acceptance checks, run against existing code, observe missing defeat/result behavior.
### Task 2: Gameplay and UI
- [x] Add Partida and InterfazPartida; wire existing score/lives/block/menu scripts.
- [x] Add planar ball constraints, bounded velocity, escape recovery and terminal input guards.
- [x] Run acceptance checks for loss/win/restart/menu/buttons and motion; fix failures.
### Task 3: Verification and delivery
- [x] Update migration smoke for XY collisions and explicit terminal transitions.
- [x] Inspect menu/level/victory/defeat captures and responsive layout.
- [x] Build macOS ARM64, request independent review, resolve findings.
- [x] Update checklist/docs and push the follow-up commit.

## Verification ledger
- RED: /tmp/tsukuyomi-gameplay-red.log failed because lives became negative after three losses.
- GREEN: /tmp/tsukuyomi-gameplay-events2.log exited 0; complete outcome/reset/motion/real-physics acceptance and pointer/keyboard UI event routing passed.
- Regression: /tmp/tsukuyomi-gameplay-regression.log exited 0. Menu, level, victory, defeat and portrait result captures inspected.
- Independent source review found no high/medium runtime correctness issues. Its event-routing test gap was addressed with EventSystem raycasts, navigation and Submit, rather than invoking button listeners directly.
- ARM64 build: /tmp/tsukuyomi-gameplay-build.log, exit 0, 103006770 bytes. Standalone startup ran 12 seconds with zero log errors; stopped intentionally. Updated app/captures copied back to the project Builds directory.
