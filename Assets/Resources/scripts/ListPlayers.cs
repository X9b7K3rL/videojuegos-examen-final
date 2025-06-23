using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPlayers
{
    public string Nombre { get; set; }
    public string Prefab { get; set; }
    public int Vida { get; set; }
    public int MaxVida { get; set; }
    public IEnumerable<ListPlayers> ListAllPlayers()
    {
        return new List<ListPlayers>
        {
            new ListPlayers { Nombre = "Player1", Prefab = "players/prefabs/player_1", Vida = 10, MaxVida = 10 },
            new ListPlayers { Nombre = "Player2", Prefab = "players/prefabs/player_2", Vida = 8, MaxVida = 10 },
        };
    }

    public void CargarPersonaje(string locationPrefab, GameObject location)
    {
        if (locationPrefab == null || location == null)
        {
            return;
        }

        foreach (Transform child in location.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject prefab = Resources.Load<GameObject>(locationPrefab);
        if (prefab == null)
        {
            return;
        }

        GameObject instancia = GameObject.Instantiate(prefab, location.transform.position, location.transform.rotation);
        instancia.transform.SetParent(location.transform);
        instancia.transform.localPosition = Vector3.zero;
        instancia.transform.localRotation = Quaternion.identity;

    }

    public void CargarPersonajeVisual(string locationPrefab, GameObject location)
    {
        if (locationPrefab == null || location == null)
        {
            return;
        }

        foreach (Transform child in location.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject prefab = Resources.Load<GameObject>(locationPrefab);
        if (prefab == null)
        {
            return;
        }

        GameObject instancia = GameObject.Instantiate(prefab, location.transform.position, location.transform.rotation);
        instancia.transform.SetParent(location.transform);
        instancia.transform.localPosition = Vector3.zero;
        instancia.transform.localRotation = Quaternion.identity;

        Rigidbody2D rb = instancia.GetComponentInChildren<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        Collider2D[] colliders = instancia.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        MonoBehaviour[] scripts = instancia.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script.GetType().Name.Contains("Controller"))
            {
                script.enabled = false;
            }
        }

    }

}
