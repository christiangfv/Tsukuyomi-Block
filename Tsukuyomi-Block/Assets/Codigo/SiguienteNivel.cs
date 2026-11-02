using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SiguienteNivel : MonoBehaviour {

    public string nivelACargar;
    public float retraso;

    [ContextMenu("Activar Carga")]
    public void ActivarCarga()
    {
        Invoke("CargarNivel", retraso);
    }

    void CargarNivel()
    {
        SceneManager.LoadScene(nivelACargar);
    }

    public bool EsUltimoNivel()
    {
        return nivelACargar == "Portada";
    }
}
