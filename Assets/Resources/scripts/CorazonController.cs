using UnityEngine;

public class CorazonController : MonoBehaviour
{
    private Repository _repo;
    private Database _data;
    void Start()
    {
        _repo = Repository.GetInstance();
        _data = _repo.GetData();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(EtiquetasEntidades.PLAYER))
        {
            if (_data.vidas >= _data.maxVidas) return;

            int vidasRestantes = _data.maxVidas - _data.vidas;
            int vidasASumar = Mathf.Min(2, vidasRestantes);
            _data.vidas += vidasASumar;

            _repo.SaveData();
            Destroy(this.gameObject);
        }
    }
}
