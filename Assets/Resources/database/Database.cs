using System;
using UnityEngine;

[Serializable]
public class Database
{
    public int EnemigosMuertos = 0;
    public int vidas = 10;
    public int maxVidas = 10;
    public int coins = 0;
    public bool isGameOver = false;
    public bool isVictory = false;
    public bool HabilidadEspecial = false;
    public string personajeSeleccionado = "Player1";
}
