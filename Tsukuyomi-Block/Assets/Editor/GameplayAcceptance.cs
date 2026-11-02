using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[InitializeOnLoad]
public static class GameplayAcceptance
{
    const string Key = "Tsukuyomi.GameplayAcceptance";
    static int stage, steps, clicks;
    static float stageTime;
    static double start;
    static readonly List<string> errors = new List<string>();
    static GameplayAcceptance() { if (SessionState.GetBool(Key, false)) Attach(); }
    static void Attach() {
        start = EditorApplication.timeSinceStartup;
        Application.logMessageReceived -= CaptureError;
        Application.logMessageReceived += CaptureError;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }
    static void CaptureError(string message, string trace, LogType type) {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert ||
            (type == LogType.Warning && message.Contains("kinematic"))) errors.Add(message);
    }
    static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    public static void Run() {
        MigrationValidation.Validate();
        stage = steps = clicks = 0; errors.Clear();
        SessionState.SetBool(Key, true); Attach();
        EditorApplication.delayCall += () => EditorApplication.delayCall += EditorApplication.EnterPlaymode;
    }
    static void Call(object target, string name) {
        var method = target.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Check(method != null, "Missing gameplay action: " + name); method.Invoke(target, null);
    }
    static void Click(string name) {
        var obj = GameObject.Find(name); Check(obj != null, "Missing visible action: " + name);
        var button = obj.GetComponent<Button>(); Check(button != null && button.interactable, "Action must be an enabled button: " + name);
        var events = EventSystem.current; Check(events != null, "UI needs an active EventSystem.");
        if (name == "Reintentar" && clicks++ == 0) {
            events.SetSelectedGameObject(button.gameObject);
            ExecuteEvents.Execute(button.gameObject, new AxisEventData(events) { moveDir = MoveDirection.Right }, ExecuteEvents.moveHandler);
            Check(events.currentSelectedGameObject == GameObject.Find("Menu"), "Keyboard navigation must reach the menu action.");
            ExecuteEvents.Execute(events.currentSelectedGameObject, new AxisEventData(events) { moveDir = MoveDirection.Left }, ExecuteEvents.moveHandler);
            Check(events.currentSelectedGameObject == button.gameObject, "Keyboard navigation must return to retry.");
            ExecuteEvents.Execute(button.gameObject, new BaseEventData(events), ExecuteEvents.submitHandler);
        } else {
            Canvas.ForceUpdateCanvases();
            var pointer = new PointerEventData(events) {
                position = RectTransformUtility.WorldToScreenPoint(null, button.transform.position),
                button = PointerEventData.InputButton.Left
            };
            var hits = new List<RaycastResult>(); events.RaycastAll(pointer, hits);
            Check(hits.Count > 0 && hits[0].gameObject.GetComponentInParent<Button>() == button,
                "The pointer raycast must reach the visible button: " + name + " at " + pointer.position + " top=" + (hits.Count > 0 ? hits[0].gameObject.name : "none"));
            pointer.pointerCurrentRaycast = pointer.pointerPressRaycast = hits[0];
            ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }
    }
    static void Result(string text) {
        var panel = GameObject.Find("ResultadoPartida"); Check(panel != null, "A finished game must show its result panel.");
        Check(panel.GetComponentsInChildren<Text>().Any(t => t.text.Contains(text)), "Missing outcome: " + text);
    }
    static void Fresh() {
        Check(Vidas.vidas == 3 && Puntos.puntos == 0, "A new game must reset lives and points.");
        Check(UnityEngine.Object.FindObjectsByType<Blocks>(FindObjectsSortMode.None).Length == 30, "A new game must restore all 30 blocks.");
        var ball = UnityEngine.Object.FindFirstObjectByType<movPelota>();
        Check(ball.rigBody.isKinematic && ball.transform.parent == ball.barra, "A new game must wait with its ball attached.");
        Check(UnityEngine.Object.FindFirstObjectByType<Puntos>().textoPuntos.text == "Puntos: 0", "New-game score UI must reset.");
    }
    static void Capture(string name) {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null) return;
        Directory.CreateDirectory("Builds/screenshots");
        typeof(MigrationValidation).GetMethod("CaptureScene", BindingFlags.Static | BindingFlags.NonPublic)
            .Invoke(null, new object[] { "Builds/screenshots/" + name + ".png" });
    }
    static void Place(movPelota ball, Vector3 position, Vector3 velocity) {
        ball.transform.position = position; ball.rigBody.position = position;
        ball.rigBody.linearVelocity = velocity; Physics.SyncTransforms();
    }
    static void Tick() {
        if (!SessionState.GetBool(Key, false)) return;
        if (EditorApplication.timeSinceStartup - start > 120) { Finish(1, "Acceptance timed out at stage " + stage); return; }
        if (!EditorApplication.isPlaying || Time.timeSinceLevelLoad < 0.5f) return;
        try {
            if (stage == 70) {
                if (Time.time - stageTime < 0.2f) return;
                stage = 8; Click("Menu"); return;
            }
            if (stage == 10 || stage == 20) {
                if (Time.time - stageTime < 0.3f) return;
                Capture(stage == 10 ? "defeat" : "victory");
                MigrationValidation.CaptureSceneAtSize("Builds/screenshots/result-portrait.png", 360, 800);
                stage = stage == 10 ? 1 : 3;
                Click("Reintentar"); return;
            }
            if (stage == 8) {
                if (SceneManager.GetActiveScene().name != "Portada") return;
                Check(GameObject.Find("ControlesMenu") != null, "The menu must explain its controls.");
                Capture("menu"); stage = 9; Click("Jugar"); return;
            }
            if (SceneManager.GetActiveScene().name != "Nivel 01") return;
            var lives = UnityEngine.Object.FindFirstObjectByType<Vidas>();
            var ball = lives.pelota;
            switch (stage) {
                case 0:
                    for (int i = 0; i < 5; i++) lives.PerderVida();
                    Check(Vidas.vidas == 0, "Three lost lives must end the game; lives cannot become negative.");
                    Result("Derrota"); Check(ball.rigBody.isKinematic, "Defeat must stop the ball.");
                    stageTime = Time.time; stage = 10; break;
                case 1:
                    Fresh(); Check(GameObject.Find("ControlesNivel") != null, "The level must show controls.");
                    Capture("level"); stage = 2; break;
                case 2:
                    var blocks = UnityEngine.Object.FindObjectsByType<Blocks>(FindObjectsSortMode.None);
                    foreach (var block in blocks) {
                        block.RecibirGolpe(); block.RecibirGolpe();
                    }
                    Check(Puntos.puntos == 30, "Each of 30 blocks must award exactly one point, including duplicate hits.");
                    Result("Victoria"); Check(ball.rigBody.isKinematic, "Victory must stop the ball.");
                    lives.PerderVida(); Check(Vidas.vidas == 3, "A terminal victory must ignore life loss.");
                    stageTime = Time.time; stage = 20; break;
                case 3:
                    Fresh(); Call(ball, "Lanzar");
                    Check(!ball.rigBody.isKinematic && ball.rigBody.linearVelocity.y > 0, "Launch must start upward motion.");
                    Check((ball.rigBody.constraints & RigidbodyConstraints.FreezePositionZ) != 0, "Ball must stay in the XY plane.");
                    ball.rigBody.linearVelocity = new Vector3(100, 0, 7); ball.SendMessage("FixedUpdate");
                    var velocity = ball.rigBody.linearVelocity;
                    Check(velocity.magnitude <= 18.01f && velocity.magnitude >= 9.99f && Mathf.Abs(velocity.y) > 2f && Mathf.Abs(velocity.z) < 0.001f,
                        "Ball must cap speed, recover horizontal movement, and discard Z velocity.");
                    Place(ball, new Vector3(100, 100, 0), velocity); ball.SendMessage("FixedUpdate");
                    for (int i = 0; i < 3; i++) ball.SendMessage("FixedUpdate");
                    Check(Vidas.vidas == 2 && ball.rigBody.isKinematic && ball.transform.parent == ball.barra,
                        "Escaping bounds must lose exactly one life and reset the ball.");
                    stage = 4; break;
                case 4:
                    Call(ball, "Lanzar"); Physics.simulationMode = SimulationMode.Script;
                    var target = UnityEngine.Object.FindObjectsByType<Blocks>(FindObjectsSortMode.None)
                        .OrderBy(b => b.GetComponent<Collider>().bounds.min.y).First().GetComponent<Collider>().bounds;
                    Place(ball, new Vector3(target.center.x, target.min.y - 0.8f, ball.transform.position.z), Vector3.up * 12);
                    steps = 0; stage = 5; break;
                case 5:
                    Physics.Simulate(0.02f);
                    if (Puntos.puntos == 0) { Check(++steps < 120, "A real ball collision must destroy a block."); break; }
                    Check(Puntos.puntos == 1, "A real collision must award one point.");
                    Place(ball, UnityEngine.Object.FindFirstObjectByType<Suelo>().GetComponent<Collider>().bounds.center, Vector3.down * 12);
                    steps = 0; stage = 6; break;
                case 6:
                    Physics.Simulate(0.02f);
                    if (Vidas.vidas == 2) { Check(++steps < 120, "A real floor trigger must lose a life."); break; }
                    Check(Vidas.vidas == 1 && ball.rigBody.isKinematic, "Floor hit must reset the ball once.");
                    Physics.simulationMode = SimulationMode.FixedUpdate; stage = 7; break;
                case 7:
                    lives.PerderVida(); Result("Derrota"); stageTime = Time.time; stage = 70; break;
                case 9:
                    Fresh(); Check(errors.Count == 0, "Runtime errors: " + string.Join("; ", errors));
                    Finish(0, "GAMEPLAY: defeat, victory, duplicate events, retry, menu, fresh starts, visible actions, ball guards and real collisions passed."); break;
            }
        } catch (Exception error) { Finish(1, error.ToString()); }
    }
    static void Finish(int code, string message) {
        SessionState.SetBool(Key, false); EditorApplication.update -= Tick; Application.logMessageReceived -= CaptureError;
        if (code == 0) Debug.Log(message); else Debug.LogError(message);
        EditorApplication.Exit(code);
    }
}
