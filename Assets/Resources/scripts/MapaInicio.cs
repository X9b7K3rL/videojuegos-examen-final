using UnityEngine;

public class MapaInicio : MonoBehaviour
{
    private Repository _repo;
    private Database _data;
    void Start()
    {
        _repo = Repository.GetInstance();
        _data = _repo.GetData();

        _data.isGameOver = false;
        _data.isVictory = false;
        _data.EnemigosMuertos = 0;
        _data.vidas = 20;
        _data.maxVidas = 20;

        _repo.SaveData();
    }
}
