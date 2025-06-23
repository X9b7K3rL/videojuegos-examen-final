using UnityEngine;

public class HabilidadController : MonoBehaviour
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
            if (_data.HabilidadEspecial)
            {
                return;
            }
            _data.HabilidadEspecial = true;
            _repo.SaveData();
            Destroy(this.gameObject);
        }
    }
}
