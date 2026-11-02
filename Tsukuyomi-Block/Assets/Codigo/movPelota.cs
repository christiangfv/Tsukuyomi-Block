using UnityEngine;

public class movPelota : MonoBehaviour
{
    public float velocidadInicial = 500f;
    public Rigidbody rigBody;
    public Transform barra;
    [SerializeField] float velocidadMinima = 10f;
    [SerializeField] float velocidadMaxima = 18f;
    [SerializeField] float limiteCampo = 14f;
    bool enJuego;
    Vector3 posicionInicial;
    int frameInicial;
    public bool EnJuego => enJuego;

    void Start()
    {
        posicionInicial = transform.position;
        frameInicial = Time.frameCount;
        rigBody.constraints |= RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    public void Reset()
    {
        DetenerMovimiento();
        transform.SetParent(barra);
        transform.position = posicionInicial;
        rigBody.position = posicionInicial;
        frameInicial = Time.frameCount;
        if (Partida.Actual != null) Partida.Actual.PrepararLanzamiento();
    }

    public void DetenerMovimiento()
    {
        enJuego = false;
        if (!rigBody.isKinematic) {
            rigBody.linearVelocity = Vector3.zero;
            rigBody.angularVelocity = Vector3.zero;
        }
        rigBody.isKinematic = true;
    }

    public void Lanzar()
    {
        if (enJuego || (Partida.Actual != null && Partida.Actual.Finalizada)) return;
        enJuego = true;
        transform.SetParent(null);
        rigBody.isKinematic = false;
        float velocidad = Mathf.Clamp(velocidadInicial * Time.fixedDeltaTime * Mathf.Sqrt(2f) / rigBody.mass,
            velocidadMinima, velocidadMaxima);
        rigBody.linearVelocity = new Vector3(1, 1, 0).normalized * velocidad;
        if (Partida.Actual != null) Partida.Actual.NotificarLanzamiento();
    }

    void Update()
    {
        if (Time.frameCount > frameInicial + 1 && Input.GetButtonDown("Fire1")) Lanzar();
    }

    void FixedUpdate()
    {
        if (!enJuego || rigBody.isKinematic) return;
        if (Mathf.Abs(transform.position.x) > limiteCampo || Mathf.Abs(transform.position.y) > limiteCampo) {
            if (Partida.Actual != null) Partida.Actual.GetComponent<Vidas>().PerderVida();
            return;
        }
        var velocity = new Vector2(rigBody.linearVelocity.x, rigBody.linearVelocity.y);
        float speed = Mathf.Clamp(velocity.magnitude, velocidadMinima, velocidadMaxima);
        var direction = velocity.sqrMagnitude > 0.001f ? velocity.normalized : Vector2.up;
        if (Mathf.Abs(direction.y) < 0.25f) {
            direction.y = direction.y < 0 ? -0.25f : 0.25f;
            direction.x = (direction.x < 0 ? -1 : 1) * Mathf.Sqrt(1f - direction.y * direction.y);
        }
        rigBody.linearVelocity = new Vector3(direction.x, direction.y, 0) * speed;
    }
}
