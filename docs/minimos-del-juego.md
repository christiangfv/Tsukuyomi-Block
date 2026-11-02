# Mínimos pendientes para cerrar el juego

Revisión del código y de las dos escenas tras la migración a Unity 6.6, el 3 de noviembre de 2026. Esta lista describe trabajo pendiente; no se ha implementado como parte de la migración.

## Imprescindibles

1. **Derrota al agotar las vidas.** `Vidas.PerderVida()` resta una vida y reinicia pelota/barra, pero no comprueba si quedan vidas. Al llegar a cero debe detener la partida, impedir valores negativos y mostrar el resultado.
2. **Victoria al destruir el último bloque.** `Blocks.OnCollisionEnter()` destruye el bloque y suma un punto, pero no contabiliza los bloques restantes. `SiguienteNivel.ActivarCarga()` existe, pero ningún script del juego lo llama automáticamente. Detectar el final del nivel, detener la pelota y mostrar victoria/puntuación. Actualmente solo hay un nivel jugable; no hace falta añadir más para cerrar el juego.
3. **Reintentar y volver al menú desde el resultado.** Reutilizar el reinicio de vidas/puntos que ya hace `ComenzarPartida`, con acciones claras tras victoria o derrota. Esc ya permite regresar al menú durante el nivel.
4. **Controles visibles.** La portada muestra el título, pero no explica cómo empezar. Indicar comenzar/lanzar (Ctrl izquierdo, Z o clic), mover (flechas/A–D) y volver al menú (Esc). No anunciar Espacio mientras no esté configurado.
5. **Asegurar el plano y los límites de la pelota.** El Rigidbody no tiene restricciones de posición/rotación. Para un Breakout que se juega en X/Y, fijar Z y verificar rebotes, velocidad y recuperación si la pelota sale del área o queda en una trayectoria horizontal. Es una medida de robustez pendiente, no un fallo reproducido durante la migración.

## Criterio mínimo de aceptación

- Perder las tres vidas termina la partida una sola vez.
- Destruir los 30 bloques termina el nivel una sola vez.
- Reintentar restaura tres vidas, cero puntos y las posiciones iniciales.
- Volver al menú y empezar otra vez funciona sin conservar una partida anterior.
- Comprobar manualmente teclado/ratón y una partida completa ganada y perdida en la aplicación compilada.

La migración ya verificó compilación ARM64, arranque, referencias de escenas, colisiones reales, partículas, puntos, vidas, reinicio de la pelota y carga del menú. La prueba de carga del menú activó explícitamente `SiguienteNivel`; no demuestra que la victoria esté conectada automáticamente.

## Después de estos mínimos

Pausa, sonidos/música, récord local, más niveles y mejoras visuales pueden venir después. No son necesarios para completar primero el ciclo básico de una partida.

## Evidencia en el repositorio

- [Vidas.cs](../Tsukuyomi-Block/Assets/Codigo/Vidas.cs)
- [Blocks.cs](../Tsukuyomi-Block/Assets/Codigo/Blocks.cs)
- [SiguienteNivel.cs](../Tsukuyomi-Block/Assets/Codigo/SiguienteNivel.cs)
- [ComenzarPartida.cs](../Tsukuyomi-Block/Assets/Codigo/ComenzarPartida.cs)
- [Nivel 01](../Tsukuyomi-Block/Assets/Scenes/Nivel%2001.unity)
- [Controles](../Tsukuyomi-Block/ProjectSettings/InputManager.asset)
