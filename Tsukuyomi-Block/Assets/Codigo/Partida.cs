using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Partida : MonoBehaviour
{
    public enum EstadoPartida { Preparada, EnJuego, Victoria, Derrota }
    public const string EscenaNivel = "Nivel 01";
    public const string EscenaMenu = "Portada";
    public static Partida Actual { get; private set; }
    public EstadoPartida Estado { get; private set; }
    public bool Finalizada => Estado == EstadoPartida.Victoria || Estado == EstadoPartida.Derrota;
    public int BloquesRestantes => bloques.Count;

    readonly HashSet<Blocks> bloques = new HashSet<Blocks>();
    Vidas vidas;
    InterfazPartida interfaz;

    void Awake()
    {
        Actual = this;
        Estado = EstadoPartida.Preparada;
        Vidas.vidas = 3;
        Puntos.puntos = 0;
        vidas = GetComponent<Vidas>();
        foreach (var bloque in FindObjectsByType<Blocks>(FindObjectsSortMode.None))
            bloques.Add(bloque);
        interfaz = InterfazPartida.CrearJuego(this);
    }

    void OnDestroy() { if (Actual == this) Actual = null; }
    public void PrepararLanzamiento() { if (!Finalizada) Estado = EstadoPartida.Preparada; }
    public void NotificarLanzamiento() { if (!Finalizada) Estado = EstadoPartida.EnJuego; }

    public void RegistrarBloqueDestruido(Blocks bloque)
    {
        if (Finalizada || !bloques.Remove(bloque)) return;
        if (bloques.Count == 0) Terminar(EstadoPartida.Victoria);
    }

    public void Perder() { Terminar(EstadoPartida.Derrota); }
    void Terminar(EstadoPartida resultado)
    {
        if (Finalizada) return;
        Estado = resultado;
        vidas.pelota.DetenerMovimiento();
        interfaz.MostrarResultado(resultado == EstadoPartida.Victoria, Puntos.puntos);
    }

    public static void ComenzarNuevaPartida()
    {
        Vidas.vidas = 3;
        Puntos.puntos = 0;
        SceneManager.LoadScene(EscenaNivel);
    }

    public static void VolverAlMenu() { SceneManager.LoadScene(EscenaMenu); }
}
