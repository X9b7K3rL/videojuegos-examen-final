using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Repository _repo;
    private Database _data;

    private Text EnemigosMuertos;
    private Text CoinsText;
    private Text HabilidadEspecialText;
    private AudioSource _audioSource;
    public AudioClip _audioClipMuerte;
    public AudioClip _audioClipVictoria;
    private GameObject _locationRender;
    public HUDVIDAS hud;
    private float tiempoHabilidadRestante = 0f;
    private bool habilidadEnCuentaAtras = false;
    private float tiempoHabilidad = 10f;
    private int vidaAnterior;
    private bool audioGameOverReproducido = false;
    private bool audioVictoriaReproducido = false;



    void Start()
    {
        EnemigosMuertos = GameObject.Find("EnemigosMuertos")?.GetComponent<Text>();
        CoinsText = GameObject.Find("CoinsText")?.GetComponent<Text>();
        HabilidadEspecialText = GameObject.Find("habilidad_text")?.GetComponent<Text>();
        _locationRender = GameObject.Find("locationRender");
        _audioSource = GetComponent<AudioSource>();
        _repo = Repository.GetInstance();
        _data = _repo.GetData();


        ListPlayers Players = new ListPlayers();

        ListPlayers personaje = Players.ListAllPlayers().First(p => p.Nombre == _data.personajeSeleccionado);

        personaje.CargarPersonaje(personaje.Prefab, _locationRender);

        vidaAnterior = _data.vidas;
        SincronizarCorazonesConVida();
    }

    void Update()
    {
        EstadoVida();
        if (_data.vidas <= 0 && _data.isGameOver == false)
        {
            _data.isGameOver = true;
            _data.isVictory = false;
            _repo.SaveData();

        }
        EnemigosMuertos.text = $"{_data.EnemigosMuertos}";
        CoinsText.text = $"{_data.coins}";

        GameObject powerObj = GameObject.Find("habilidad_text/power");
        if (powerObj != null)
        {
            SpriteRenderer sr = powerObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = _data.HabilidadEspecial;
            }
        }

        GameObject gameOverObj = GameObject.Find("Backgrouds_G_V/gameover");
        if (gameOverObj != null)
        {
            SpriteRenderer sr = gameOverObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = _data.isGameOver;
                if (_data.isGameOver && !audioGameOverReproducido)
                {
                    _audioSource.Stop();
                    _audioSource.PlayOneShot(_audioClipMuerte);
                    audioGameOverReproducido = true;
                }
            }
        }

        GameObject winObj = GameObject.Find("Backgrouds_G_V/win");
        if (winObj != null)
        {
            SpriteRenderer sr = winObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = _data.isVictory;
                if (_data.isVictory && !audioVictoriaReproducido)
                {
                    _audioSource.Stop();
                    _audioSource.PlayOneShot(_audioClipVictoria);
                    audioVictoriaReproducido = true;
                }
            }
        }

        if (_data.HabilidadEspecial && !habilidadEnCuentaAtras)
        {
            tiempoHabilidadRestante = tiempoHabilidad;
            habilidadEnCuentaAtras = true;
            StartCoroutine(DesactivarhabilidadEspecial());
        }

        if (_data.HabilidadEspecial && tiempoHabilidadRestante > 0f)
        {
            tiempoHabilidadRestante -= Time.deltaTime;
            HabilidadEspecialText.text = $"{tiempoHabilidadRestante:F1}s";
        }
        else
        {
            HabilidadEspecialText.text = "";
        }
    }

    IEnumerator DesactivarhabilidadEspecial()
    {
        yield return new WaitForSeconds(tiempoHabilidad);
        _data.HabilidadEspecial = false;
        _repo.SaveData();
        habilidadEnCuentaAtras = false;
    }

    private void EstadoVida()
    {
        if (_data.vidas > vidaAnterior)
        {
            // Si ganas vidas, activa corazones hasta igualar
            while (vidaAnterior < _data.vidas)
            {
                hud.SetVida(vidaAnterior);
                vidaAnterior++;
            }
        }
        else if (_data.vidas < vidaAnterior)
        {
            // Si pierdes vidas, desactiva corazones hasta igualar
            while (vidaAnterior > _data.vidas)
            {
                if (vidaAnterior <= 0)
                {
                    vidaAnterior = 0;
                    hud.DeleteVida(vidaAnterior);
                    break;
                }
                vidaAnterior--;
                hud.DeleteVida(vidaAnterior);
            }
        }
    }

    private void SincronizarCorazonesConVida()
    {
        // Primero apaga todos los corazones
        hud.ApagarTodosLosCorazones();

        // Luego activa solo los que corresponden a la vida actual
        for (int i = 0; i < _data.vidas; i++)
        {
            hud.SetVida(i);
        }
        vidaAnterior = _data.vidas;
    }
}
