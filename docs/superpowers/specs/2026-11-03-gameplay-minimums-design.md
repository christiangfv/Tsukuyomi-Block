# Complete the minimum gameplay loop

User approved all five items in docs/minimos-del-juego.md. Preserve the two scenes, Built-in rendering, existing serialized script fields/GUIDs and controls. Add a scene-level Partida controller for terminal victory/defeat, single-count block destruction, guarded life loss, retry/menu actions, and consistent resets. Build a responsive Unity UI overlay at runtime so existing open scenes and designer changes are not overwritten. Menu and HUD show the existing bindings; result panel shows outcome, score and two buttons.

Constrain ball movement to XY, clamp speed, recover near-horizontal trajectories, and lose one life if it escapes generous court bounds. Preserve legacy launch strength through conversion to initial velocity. Stop paddle and ball after a result. Handle only actual ball collisions on blocks/floor.

Use the existing checkout. Run automated acceptance and builds in /tmp/tsukuyomi-gameplay-validation to preserve the user's open editor. Verify third-life defeat/idempotence, 30-block victory/duplicate hits, retry/menu/new-game resets, visible functional buttons and controls, real collision callbacks, ball speed/plane/escape recovery, rendered results, and macOS ARM64 build. Push the completed follow-up on the existing authorized branch.
