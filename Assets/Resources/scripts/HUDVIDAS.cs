using UnityEngine;

public class HUDVIDAS : MonoBehaviour
{
    public GameObject[] vidas;

    public void SetVida(int indice)
    {
        vidas[indice].SetActive(true);
    }

    public void DeleteVida(int indice)
    {
        vidas[indice].SetActive(false);
    }

    public void ApagarTodosLosCorazones()
    {
        foreach (var vida in vidas)
        {
            vida.SetActive(false);
        }
    }
}
