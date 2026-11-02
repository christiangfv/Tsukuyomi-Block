using UnityEngine;

public class Blocks : MonoBehaviour
{
    public Puntos puntos;
    public GameObject efectoParticulas;
    bool destruido;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.GetComponent<movPelota>() != null)
            RecibirGolpe();
    }

    public void RecibirGolpe()
    {
        if (destruido || Partida.Actual == null || Partida.Actual.Finalizada) return;
        destruido = true;
        if (efectoParticulas != null) Instantiate(efectoParticulas, transform.position, Quaternion.identity);
        puntos.ganarPunto();
        GetComponent<Collider>().enabled = false;
        Partida.Actual.RegistrarBloqueDestruido(this);
        Destroy(gameObject);
    }
}
