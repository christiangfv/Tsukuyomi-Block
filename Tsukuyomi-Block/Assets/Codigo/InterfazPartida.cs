using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfazPartida : MonoBehaviour
{
    static readonly Color Fondo = new Color(0.055f, 0.075f, 0.13f, 0.97f);
    static readonly Color Amarillo = new Color(1f, 0.77f, 0.12f);
    Font fuente;
    GameObject resultado;
    Text titulo, puntuacion, detalle;
    Button reintentar;

    public static InterfazPartida CrearJuego(Partida partida)
    {
        var ui = Crear();
        ui.Texto("ControlesNivel", ui.transform,
            "Mover: A/D o flechas     Lanzar: Ctrl / Z / clic     Menú: Esc",
            18, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 36), new Vector2(0, 20));
        ui.CrearResultado();
        return ui;
    }

    public static void CrearMenu()
    {
        var ui = Crear();
        var panel = ui.Panel("AyudaMenu", ui.transform, Fondo, new Vector2(0.08f, 0), new Vector2(0.92f, 0),
            new Vector2(0, 160), new Vector2(0, 100));
        ui.Boton("Jugar", panel.transform, "Jugar", Partida.ComenzarNuevaPartida,
            new Vector2(240, 48), new Vector2(0, 36), true);
        ui.Texto("ControlesMenu", panel.transform,
            "Comenzar o lanzar: Ctrl izquierdo, Z o clic\nMover: A/D o flechas   ·   Volver al menú: Esc",
            18, new Vector2(0.04f, 0.05f), new Vector2(0.96f, 0.48f), Vector2.zero, Vector2.zero);
    }

    static InterfazPartida Crear()
    {
        var root = new GameObject("InterfazPartida", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var ui = root.AddComponent<InterfazPartida>();
        ui.fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 100;
        var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = 0.5f;
        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        return ui;
    }

    void CrearResultado()
    {
        resultado = Panel("ResultadoPartida", transform, new Color(0.01f, 0.02f, 0.04f, 0.82f),
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        var tarjeta = Panel("TarjetaResultado", resultado.transform, Fondo, Vector2.one * 0.5f, Vector2.one * 0.5f,
            new Vector2(520, 310), Vector2.zero);
        titulo = Texto("TituloResultado", tarjeta.transform, "", 42, Vector2.one * 0.5f, Vector2.one * 0.5f,
            new Vector2(480, 60), new Vector2(0, 100)); titulo.color = Amarillo;
        detalle = Texto("DetalleResultado", tarjeta.transform, "", 20, Vector2.one * 0.5f, Vector2.one * 0.5f,
            new Vector2(480, 48), new Vector2(0, 45));
        puntuacion = Texto("PuntuacionFinal", tarjeta.transform, "", 28, Vector2.one * 0.5f, Vector2.one * 0.5f,
            new Vector2(480, 48), new Vector2(0, -10));
        reintentar = Boton("Reintentar", tarjeta.transform, "Reintentar", Partida.ComenzarNuevaPartida,
            new Vector2(210, 52), new Vector2(-115, -95), true);
        Boton("Menu", tarjeta.transform, "Menú", Partida.VolverAlMenu,
            new Vector2(210, 52), new Vector2(115, -95), false);
        resultado.SetActive(false);
    }

    public void MostrarResultado(bool victoria, int puntos)
    {
        titulo.text = victoria ? "Victoria" : "Derrota";
        detalle.text = victoria ? "¡Has destruido todos los bloques!" : "No quedan vidas. ¡Inténtalo otra vez!";
        puntuacion.text = "Puntos: " + puntos;
        resultado.SetActive(true);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(reintentar.gameObject);
    }

    GameObject Panel(string nombre, Transform parent, Color color, Vector2 min, Vector2 max, Vector2 size, Vector2 position)
    {
        var obj = new GameObject(nombre, typeof(RectTransform), typeof(Image)); obj.transform.SetParent(parent, false);
        Colocar(obj.GetComponent<RectTransform>(), min, max, size, position);
        obj.GetComponent<Image>().color = color; return obj;
    }
    Text Texto(string nombre, Transform parent, string contenido, int size, Vector2 min, Vector2 max, Vector2 dimensions, Vector2 position)
    {
        var obj = new GameObject(nombre, typeof(RectTransform), typeof(Text)); obj.transform.SetParent(parent, false);
        Colocar(obj.GetComponent<RectTransform>(), min, max, dimensions, position);
        var text = obj.GetComponent<Text>(); text.font = fuente; text.fontSize = size; text.text = contenido;
        text.color = Color.white; text.alignment = TextAnchor.MiddleCenter; text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }
    Button Boton(string nombre, Transform parent, string label, UnityAction action, Vector2 size, Vector2 position, bool destacado)
    {
        var obj = Panel(nombre, parent, destacado ? Amarillo : new Color(0.2f, 0.25f, 0.35f),
            Vector2.one * 0.5f, Vector2.one * 0.5f, size, position);
        var button = obj.AddComponent<Button>(); button.targetGraphic = obj.GetComponent<Image>(); button.onClick.AddListener(action);
        var text = Texto(nombre + "Texto", obj.transform, label, 23, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        text.color = destacado ? Fondo : Color.white;
        return button;
    }
    static void Colocar(RectTransform rect, Vector2 min, Vector2 max, Vector2 size, Vector2 position)
    {
        rect.anchorMin = min; rect.anchorMax = max; rect.sizeDelta = size; rect.anchoredPosition = position;
    }
}
