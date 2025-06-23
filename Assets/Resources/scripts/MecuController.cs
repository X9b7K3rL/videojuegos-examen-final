using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MecuController : MonoBehaviour
{
    private GameObject LocationRender;
    private ListPlayers[] personajes;
    private Database database;
    private Repository _repo;
    public Text NumeroMapa;
    private int indiceSeleccionado = 0;
    private int EscenaSeleccionada = 1;
    void Start()
    {
        LocationRender = GameObject.Find("locationRender");
        if (LocationRender == null)
        {
            return;
        }

        _repo = Repository.GetInstance();
        database = _repo.GetData();

        ListPlayers lista = new ListPlayers();
        personajes = new ListPlayers[lista.ListAllPlayers().Count()];
        personajes = lista.ListAllPlayers().ToArray();


        indiceSeleccionado = 0;
        personajes[indiceSeleccionado].CargarPersonajeVisual(personajes[indiceSeleccionado].Prefab, LocationRender);
        database.personajeSeleccionado = personajes[indiceSeleccionado].Nombre;
        _repo.SaveData();
    }

    public void IniciarJuego()
    {
        SceneManager.LoadScene(EscenaSeleccionada);
    }

    public void nextPersonaje()
    {
        indiceSeleccionado++;
        if (indiceSeleccionado >= personajes.Length)
            indiceSeleccionado = 0;

        personajes[indiceSeleccionado].CargarPersonajeVisual(personajes[indiceSeleccionado].Prefab, LocationRender);
        database.personajeSeleccionado = personajes[indiceSeleccionado].Nombre;
        _repo.SaveData();
    }

    // alternar entre dos escenas 1 y 2
    public void NextMapa()
    {
        EscenaSeleccionada++;
        if (EscenaSeleccionada > 2)
            EscenaSeleccionada = 1;
        NumeroMapa.text = EscenaSeleccionada.ToString();
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}