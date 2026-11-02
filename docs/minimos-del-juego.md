# Mínimos del juego: implementados

Los cinco puntos solicitados quedaron implementados en Unity 6.6. Se conservaron las dos escenas, sus referencias y el aspecto 3D original. La interfaz nueva se crea al iniciar la escena, sin sobrescribir escenas abiertas en el editor.

- [x] **Derrota al agotar las vidas.** La tercera vida perdida termina la partida, detiene la pelota y la barra y muestra resultado/puntuación. Se ignoran pérdidas posteriores y las vidas no bajan de cero.
- [x] **Victoria al destruir el último bloque.** Se cuentan los 30 bloques; cada uno puntúa una sola vez. El último activa la pantalla de victoria con los 30 puntos, sin volver al menú automáticamente.
- [x] **Reintentar y menú.** Ambas pantallas de resultado tienen botones. Reintentar restaura tres vidas, cero puntos, los 30 bloques y la pelota en la barra; Menú vuelve a la portada, desde donde se puede comenzar otra partida.
- [x] **Controles visibles.** La portada incluye «Jugar» y los controles. Durante la partida se muestran mover (A/D o flechas), lanzar (Ctrl, Z o clic) y menú (Esc). También se puede navegar entre las acciones de resultado y confirmarlas con el sistema de eventos de Unity.
- [x] **Protección de la pelota.** Movimiento limitado al plano X/Y, sin rotación física, velocidad entre 10 y 18 unidades/s, corrección de trayectorias casi horizontales y pérdida de una sola vida si sale de los límites del campo.

## Comprobaciones

`GameplayAcceptance.Run` carga las escenas reales y comprueba:

- Derrota con tres vidas y protección contra eventos posteriores.
- Victoria con 30 bloques y protección contra impactos repetidos.
- Reintentos tras derrota y victoria, regreso al menú y comienzo limpio.
- Botones mediante raycasts/eventos de puntero y navegación/Submit del EventSystem.
- Lanzamiento, velocidad, plano, recuperación fuera del campo y colisiones físicas reales con bloque y suelo.
- Capturas del menú, nivel, victoria y derrota; pantalla de resultado en formato vertical.

La prueba de regresión `MigrationValidation.StartPlaySmoke` sigue pasando. La pulsación física de las teclas del sistema operativo no se simula: la prueba de lanzamiento usa la misma acción pública que llama `Update` al detectar Fire1.

## Mejoras opcionales posteriores

Pausa, sonidos/música, récord local, más niveles y mejoras visuales. El ciclo básico de la partida ya está cerrado.

## Código

- [Partida](../Tsukuyomi-Block/Assets/Codigo/Partida.cs): estado, victoria/derrota y reinicios.
- [InterfazPartida](../Tsukuyomi-Block/Assets/Codigo/InterfazPartida.cs): controles, botones y resultados.
- [Pelota](../Tsukuyomi-Block/Assets/Codigo/movPelota.cs): lanzamiento y protección física.
- [Prueba de aceptación](../Tsukuyomi-Block/Assets/Editor/GameplayAcceptance.cs).
