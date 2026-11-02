# Unity 6 Migration Implementation Plan

> **For agentic workers:** Execute inline using superpowers:executing-plans, task by task.

**Goal:** Open and build Tsukuyomi-Block on the newest supported stable Unity for Apple Silicon.
**Architecture:** Preserve the Built-in pipeline and current scene/script references; upgrade package configuration and removed engine APIs with compilation and scene-based validation.
**Tech Stack:** Unity 6000.6.7f1, C#, macOS ARM64.
**Spec:** docs/superpowers/specs/2026-11-03-unity6-migration-design.md

## Global Constraints
- Preserve gameplay, existing serialized GUIDs, and the two game scenes.
- Keep legacy input and Built-in rendering.
- No previews, cloud services, or additional platform SDKs.

## Review Focus
- Scene transitions must resolve to the original enabled scenes.
- Existing materials and particle effects must keep their shaders.
- Runtime assemblies must not depend on UnityEditor.
- Ball reset must stop motion before becoming kinematic.
- UI, score, and lives references must survive import.

### Task 1: Native editor and package import
- [x] Download, verify checksum/signature, and install Unity 6000.6.7f1 ARM64.
- [x] Run baseline import and capture licensing/package/compiler failures.
- [x] Update Packages/manifest.json using the installed editor's package versions; keep required modules and UI.

### Task 2: Compilation and scene compatibility
- [x] Use the compiler and scene validation as failing integration checks before each compatibility fix.
- [x] Replace removed APIs preserving serialized fields and GUIDs.
- [x] Add editor validation for missing scripts, both game scenes, controls, scene transitions, and score/lives/ball reset behavior.
- [x] Run validation until clean; inspect warnings for runtime or rendering impact.

### Task 3: Build and handoff
- [x] Build standalone macOS ARM64 and verify the build report succeeds.
- [x] Update README run instructions, controls, migration notes, and verified limitations.
- [x] Review diff for unrelated changes, generated content, and unintended asset/reference changes.

## Execution ledger
- Original baseline: Unity 2018/2019 cannot validate the current Personal licensing service on this machine. Modern compiler baseline pending editor installation.
- Native Unity 6000.6.7f1 selected from Unity release API; official ARM64 installer checksum will be verified before extraction.
- Independent review found serialized legacy uGUI references to verify during scene import. Smoke checks now use physics callbacks and wait for particle updates, and verify legacy input and quality pipeline configuration.

- Unity 6000.6.7f1 ARM64 installed and registered in Hub; official installer MD5 and Apple signature verified.
- Compiler baseline exposed seven legacy Devdog API errors; EntityId types propagated without narrowing/hashing and new layout maximum parameter set to PositiveInfinity. Full C# compilation passed.
- Real play-mode integration caught kinematic velocity assignment; stopping dynamic velocity before enabling kinematic mode fixed it. Final smoke exit 0 with zero captured runtime errors.
- Serialized game assets and project settings upgraded using Unity APIs; visual captures inspected for menu and level.
- Final smoke: /tmp/tsukuyomi-unity6-final-smoke.log. Build: /tmp/tsukuyomi-unity6-build.log, exit 0, 102999266 bytes. Executable verified Mach-O arm64.
- Independent final source review found no high/medium gameplay issues. Fixed the remaining smoke verifier gap by checking captured errors again after menu rendering.
- Native Unity YAML emits trailing spaces on empty fields; source/docs/script whitespace checks pass. Preserve native serialized formatting.
- Standalone ARM64 player launched for 12 seconds without startup log errors; intentionally stopped after bounded startup validation. Removed 9.2 GB of downloaded temporary installers. Original repository backup retained outside the checkout.
