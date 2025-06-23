using UnityEngine;

public class EspadaController : MonoBehaviour
{
    private int damage;
    private void Start()
    {
        damage = 2;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject objetoColisionado = collision.gameObject;
        string tag = objetoColisionado.tag;
        if (tag == EtiquetasEntidades.ENEMY)
        {
            Vector2 direeccion = new Vector2(collision.transform.position.x, 0);
            EnemiController enemigo = objetoColisionado.GetComponent<EnemiController>();
            enemigo.RecibeDanio(direeccion, damage);
        }
        if (tag == EtiquetasEntidades.JEFEFINAL)
        {
            Vector2 direeccion = new Vector2(collision.transform.position.x, 0);
            JefeFinalController jefe = objetoColisionado.GetComponent<JefeFinalController>();
            jefe.RecibeDanio(direeccion, damage);
        }
    }
}
