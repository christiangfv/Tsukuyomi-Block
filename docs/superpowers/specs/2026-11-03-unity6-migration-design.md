# Unity 6 migration

User authorized migration to the newest stable Unity rather than retaining an obsolete editor. Target Unity 6.6 (6000.6.7f1), ARM64, the newest supported non-preview release returned by Unity's release API on 2026-11-03.

Keep the existing Breakout gameplay, serialized script GUIDs, Portada and Nivel 01 scenes, legacy input mappings, and Built-in Render Pipeline. Update packages and removed APIs only as required. Avoid a URP conversion or gameplay redesign. Preserve third-party assets; compatibility fixes must retain their behavior.

Approaches considered: retain Unity 2018 (blocked by legacy licensing); use Unity 2019/2022 (intermediate migration, but user prefers newest); migrate directly to current Unity 6, using compiler/import diagnostics and real scene checks. Selected current Unity 6 per user request.

Work in codex/unity6-migration. Original repository history is preserved and exported to /Users/christianfuentes/Code/personal/Tsukuyomi-Block-backups/2026-11-03-before-unity6.bundle. Verify licensing, package resolution, compilation, both scene loads, missing scripts/references, ball/paddle/score/lives behavior, and a standalone macOS build. Report any graphical limitations from headless verification.
