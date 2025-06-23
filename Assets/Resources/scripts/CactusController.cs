using System.Collections;
using UnityEngine;

public class CactusController : MonoBehaviour
{
    private bool haciendoDanio = false;
    private Database _data;
    private Repository _repo;

    void Start()
    {
        _repo = Repository.GetInstance();
        _data = _repo.GetData();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(EtiquetasEntidades.PLAYER) && !haciendoDanio)
        {
            StartCoroutine(HacerDanio(collision));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(EtiquetasEntidades.PLAYER))
        {
            haciendoDanio = false;
        }
    }

    IEnumerator HacerDanio(Collider2D collision)
    {
        haciendoDanio = true;
        while (haciendoDanio)
        {
            collision.GetComponent<PlayerController>().RecibeDanio(1);

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null && _data.isGameOver == false)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5f);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
