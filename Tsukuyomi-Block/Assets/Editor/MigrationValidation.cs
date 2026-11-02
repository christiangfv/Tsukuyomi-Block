using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Integration checks against the real scenes, used during editor upgrades.
[InitializeOnLoad]
public static class MigrationValidation
{
    private const string SmokeKey = "Tsukuyomi.MigrationSmoke";
    private static readonly string[] Scenes = {
        "Assets/Scenes/Portada.unity", "Assets/Scenes/Nivel 01.unity"
    };
    private static double smokeStart;
    private static int stage;
    private static int steps;
    private static int scoreStart;
    private static int livesStart;
    private static float stageTime;
    private static Vector3 paddleStart;
    private static Vector3 ballStart;
    private static readonly List<string> RuntimeErrors = new List<string>();

    static MigrationValidation()
    {
        if (SessionState.GetBool(SmokeKey, false))
        {
            smokeStart = EditorApplication.timeSinceStartup;
            Application.logMessageReceived += CaptureRuntimeError;
            EditorApplication.update += TickSmoke;
        }
    }

    private static void CaptureRuntimeError(string message, string stack, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert ||
            (type == LogType.Warning && message.Contains("kinematic"))) RuntimeErrors.Add(message);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    public static void Validate()
    {
        Require(GraphicsSettings.defaultRenderPipeline == null, "The game requires the Built-in Render Pipeline.");
        for (int i = 0; i < QualitySettings.names.Length; i++)
            Require(QualitySettings.GetRenderPipelineAssetAt(i) == null, "Quality levels must use Built-in rendering.");
        var player = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        Require(player.FindProperty("activeInputHandler").intValue != 1, "Legacy input must remain enabled.");
        Require(EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).SequenceEqual(Scenes),
            "The menu and level must remain enabled in build order.");
        foreach (string path in Scenes)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                Require(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject) == 0,
                    path + ": missing script on " + transform.name);
                foreach (var renderer in transform.GetComponents<Renderer>())
                foreach (var material in renderer.sharedMaterials)
                    Require(material != null && material.shader != null && material.shader.name != "Hidden/InternalErrorShader",
                        path + ": missing material/shader on " + transform.name);
            }
            if (path == Scenes[1])
            {
                var lives = UnityEngine.Object.FindFirstObjectByType<Vidas>();
                var score = UnityEngine.Object.FindFirstObjectByType<Puntos>();
                var blocks = UnityEngine.Object.FindObjectsByType<Blocks>(FindObjectsSortMode.None);
                Require(lives != null && lives.textoVidas != null && lives.pelota != null && lives.barra != null,
                    "The level must retain lives, ball, paddle, and UI references.");
                Require(score != null && score.textoPuntos != null, "The score UI must remain connected.");
                Require(lives.pelota.rigBody != null && lives.pelota.barra == lives.barra.transform,
                    "The ball must retain its physics and paddle references.");
                Require(blocks.Length > 0, "The level must contain blocks.");
                foreach (var block in blocks)
                    Require(block.puntos == score && block.efectoParticulas != null,
                        "Each block must retain its score and particle effect references.");
                Require(UnityEngine.Object.FindFirstObjectByType<Suelo>().vidas == lives,
                    "The floor must retain its lives reference.");
                Debug.Log("MIGRATION: validated level with " + blocks.Length + " blocks.");
            }
        }
        var input = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        var axes = input.FindProperty("m_Axes");
        foreach (string name in new[] { "Horizontal", "Fire1" })
            Require(Enumerable.Range(0, axes.arraySize).Any(i => axes.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == name),
                "Missing legacy input axis: " + name);
        Debug.Log("MIGRATION: scene and input validation passed.");
    }

    public static void ReserializeGameAssets()
    {
        Validate();
        var assets = AssetDatabase.GetDependencies(Scenes, true).Where(path => path.StartsWith("Assets/")).ToArray();
        AssetDatabase.ForceReserializeAssets(assets);
        foreach (string settingsPath in Directory.GetFiles("ProjectSettings", "*.asset"))
            foreach (var settings in AssetDatabase.LoadAllAssetsAtPath(settingsPath))
                EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        Debug.Log("MIGRATION: reserialized " + assets.Length + " game assets.");
    }

    public static void StartPlaySmoke()
    {
        Validate();
        RuntimeErrors.Clear();
        stage = 0;
        steps = 0;
        Application.logMessageReceived -= CaptureRuntimeError;
        Application.logMessageReceived += CaptureRuntimeError;
        SessionState.SetBool(SmokeKey, true);
        smokeStart = EditorApplication.timeSinceStartup;
        EditorApplication.update -= TickSmoke;
        EditorApplication.update += TickSmoke;
        // Let the editor initialize its search index before entering play mode.
        EditorApplication.delayCall += () => EditorApplication.delayCall += EditorApplication.EnterPlaymode;
    }

    private static void TickSmoke()
    {
        if (!SessionState.GetBool(SmokeKey, false)) return;
        if (EditorApplication.timeSinceStartup - smokeStart > 90)
        {
            FinishSmoke(1, "Timed out entering play mode.");
            return;
        }
        if (!EditorApplication.isPlaying || Time.timeSinceLevelLoad < 0.5f) return;
        try
        {
            if (stage == 5)
            {
                if (Time.time - stageTime < 0.3f) return;
                Require(File.Exists("Builds/screenshots/level.png") && File.Exists("Builds/screenshots/menu.png"), "Game screenshots must be saved.");
                Require(RuntimeErrors.Count == 0, "Play-mode errors: " + string.Join("; ", RuntimeErrors));
                FinishSmoke(0, "MIGRATION: gameplay smoke and screenshots passed.");
                return;
            }
            if (stage == 4)
            {
                if (SceneManager.GetActiveScene().name != "Portada" || Time.timeSinceLevelLoad < 0.5f) return;
                Require(UnityEngine.Object.FindFirstObjectByType<ComenzarPartida>() != null, "The menu start component must load.");
                Require(RuntimeErrors.Count == 0, "Play-mode errors: " + string.Join("; ", RuntimeErrors));
                if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
                {
                    CaptureScene("Builds/screenshots/menu.png");
                    stageTime = Time.time;
                    stage = 5;
                    return;
                }
                FinishSmoke(0, "MIGRATION: real collisions, particles, score, lives, ball reset, and menu loading passed.");
                return;
            }
            var lives = UnityEngine.Object.FindFirstObjectByType<Vidas>();
            var score = UnityEngine.Object.FindFirstObjectByType<Puntos>();
            var ball = lives.pelota;
            switch (stage)
            {
                case 0:
                    if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
                    {
                        Directory.CreateDirectory("Builds/screenshots");
                        CaptureScene("Builds/screenshots/level.png");
                    }
                    paddleStart = lives.barra.transform.position;
                    ballStart = ball.transform.position;
                    scoreStart = Puntos.puntos;
                    livesStart = Vidas.vidas;
                    var block = UnityEngine.Object.FindObjectsByType<Blocks>(FindObjectsSortMode.None).OrderBy(b => b.GetComponent<Collider>().bounds.min.y).First();
                    var bounds = block.GetComponent<Collider>().bounds;
                    Physics.simulationMode = SimulationMode.Script;
                    ball.Lanzar();
                    ball.transform.position = new Vector3(bounds.center.x, bounds.min.y - 0.8f, ball.transform.position.z);
                    ball.rigBody.position = ball.transform.position;
                    ball.rigBody.linearVelocity = Vector3.up * 12f;
                    Physics.SyncTransforms();
                    Debug.Log("SMOKE target=" + bounds + " ball=" + ball.GetComponent<Collider>().bounds + " enabled=" + block.GetComponent<Collider>().enabled);
                    stage = 1;
                    break;
                case 1:
                    Physics.Simulate(0.02f);
                    if (Puntos.puntos == scoreStart) {
                        Require(++steps < 100, "Ball/block collision did not award a point. pos=" + ball.rigBody.position + " velocity=" + ball.rigBody.linearVelocity + " kinematic=" + ball.rigBody.isKinematic + " constraints=" + ball.rigBody.constraints + " collisions=" + ball.rigBody.detectCollisions + " ignored=" + Physics.GetIgnoreLayerCollision(ball.gameObject.layer, UnityEngine.Object.FindFirstObjectByType<Blocks>().gameObject.layer));
                        break;
                    }
                    Require(Puntos.puntos == scoreStart + 1 && score.textoPuntos.text == "Puntos: " + Puntos.puntos,
                        "A real block collision must update score and visible UI.");
                    ball.rigBody.linearVelocity = Vector3.zero;
                    Require(UnityEngine.Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None).Length > 0,
                        "Block collision must instantiate a particle system.");
                    stageTime = Time.time;
                    stage = 2;
                    break;
                case 2:
                    if (Time.time - stageTime < 0.3f) break;
                    lives.barra.transform.position += Vector3.right;
                    ball.transform.position = UnityEngine.Object.FindFirstObjectByType<Suelo>().GetComponent<Collider>().bounds.center;
                    ball.rigBody.position = ball.transform.position;
                    ball.rigBody.linearVelocity = Vector3.down;
                    Physics.SyncTransforms();
                    steps = 0;
                    stage = 3;
                    break;
                case 3:
                    Physics.Simulate(0.02f);
                    if (Vidas.vidas == livesStart) {
                        Require(++steps < 100, "Ball/floor trigger did not decrement lives.");
                        break;
                    }
                    Require(Vidas.vidas == livesStart - 1 && lives.textoVidas.text == "Vidas: " + Vidas.vidas,
                        "A real floor trigger must update lives and visible UI.");
                    Require(Vector3.Distance(lives.barra.transform.position, paddleStart) < 0.001f &&
                            Vector3.Distance(ball.transform.position, ballStart) < 0.001f,
                        "Losing a life must restore paddle and ball positions.");
                    Require(ball.transform.parent == lives.barra.transform && ball.rigBody.isKinematic &&
                            ball.rigBody.linearVelocity.sqrMagnitude < 0.001f,
                        "Reset must attach the stopped ball to the paddle.");
                    Physics.simulationMode = SimulationMode.FixedUpdate;
                    stage = 4;
                    var transition = UnityEngine.Object.FindFirstObjectByType<SiguienteNivel>();
                    Require(transition != null && transition.nivelACargar == "Portada", "Level transition must target the menu.");
                    transition.ActivarCarga();
                    break;
                case 4:
                    // The level references disappear during the scene transition.
                    break;
            }
        }
        catch (Exception error) { FinishSmoke(1, error.ToString()); }
    }

    private static void CaptureScene(string path) { CaptureSceneAtSize(path, 1280, 720); }

    public static void CaptureSceneAtSize(string path, int width, int height)
    {
        var camera = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None).FirstOrDefault(c => c.enabled);
        Require(camera != null, "Scene requires an enabled camera.");
        var overlays = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None)
            .Where(c => c.renderMode == RenderMode.ScreenSpaceOverlay).ToArray();
        var previousCameras = overlays.Select(c => c.worldCamera).ToArray();
        var distances = overlays.Select(c => c.planeDistance).ToArray();
        var previousTarget = camera.targetTexture;
        var previousActive = RenderTexture.active;
        var target = RenderTexture.GetTemporary(width, height, 24);
        var image = new Texture2D(width, height, TextureFormat.RGB24, false);
        try
        {
            camera.targetTexture = target;
            foreach (var canvas in overlays) {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = camera.nearClipPlane + 1f;
            }
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
        }
        finally
        {
            for (int i = 0; i < overlays.Length; i++) {
                overlays[i].renderMode = RenderMode.ScreenSpaceOverlay;
                overlays[i].worldCamera = previousCameras[i];
                overlays[i].planeDistance = distances[i];
            }
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(target);
            UnityEngine.Object.DestroyImmediate(image);
        }
    }

    private static void FinishSmoke(int code, string message)
    {
        SessionState.SetBool(SmokeKey, false);
        EditorApplication.update -= TickSmoke;
        Application.logMessageReceived -= CaptureRuntimeError;
        if (code == 0) Debug.Log(message); else Debug.LogError(message);
        EditorApplication.Exit(code);
    }

    public static void BuildMac()
    {
        Validate();
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        UnityEditor.OSXStandalone.UserBuildSettings.architecture = UnityEditor.Build.OSArchitecture.ARM64;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = Scenes,
            locationPathName = "Builds/macOS/Tsukuyomi-Block.app",
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        });
        Require(report.summary.result == BuildResult.Succeeded, "macOS build failed: " + report.summary.result);
        Debug.Log("MIGRATION: macOS ARM64 build succeeded (" + report.summary.totalSize + " bytes).");
    }
}
