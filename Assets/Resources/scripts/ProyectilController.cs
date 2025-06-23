using System.Collections;
using UnityEngine;

public class ProyectilController : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip _audioClipFire;
    private string _direccion = "Derecha";
    Rigidbody2D _rigidbody;
    SpriteRenderer _spriteRenderer;
    Repository _repo;
    Database _data;
    public int _damage = 1;
    private float _speed = 15f;
    public bool isDisparedPlayer = false;


    void Start()
    {
        _repo = Repository.GetInstance();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // _playerController = GetComponent<PlayerController>();
        _data = _repo.GetData();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(_audioClipFire);

        Destroy(this.gameObject, 2f);
    }

    void Update()
    {
        if (_direccion == "Derecha")
        {
            SetSpeed(_speed);
            _spriteRenderer.flipX = false;
        }
        else if (_direccion == "Izquierda")
        {
            SetSpeed(-_speed);
            _spriteRenderer.flipX = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == EtiquetasEntidades.PLAYER && !isDisparedPlayer)
        {
            // Solo daña al player si NO fue disparado por el player
            other.gameObject.GetComponent<PlayerController>().RecibeDanio(_damage);
            Destroy(this.gameObject);
        }
        if (tag == EtiquetasEntidades.ENEMY && isDisparedPlayer)
        {
            // Solo daña al enemigo si fue disparado por el player
            EnemiController _enemie = other.gameObject.GetComponent<EnemiController>();
            if (_enemie == null) return;
            _enemie.puntosVida -= _damage;
            if (_enemie.puntosVida <= 0)
            {
                _data.EnemigosMuertos++;
                _repo.SaveData();
                Destroy(other.gameObject);
            }
            _audioSource.Stop();
            Destroy(this.gameObject);

        }
        if (tag == EtiquetasEntidades.JEFEFINAL && isDisparedPlayer)
        {
            JefeFinalController _enemie = other.gameObject.GetComponent<JefeFinalController>();
            if (_enemie == null) return;
            _enemie.puntosVida -= _damage;
            if (_enemie.puntosVida <= 0)
            {
                _data.EnemigosMuertos++;
                _data.isVictory = true;
                _repo.SaveData();
                Destroy(other.gameObject);
            }
            _audioSource.Stop();
            Destroy(this.gameObject);
        }
        if (tag == EtiquetasEntidades.ESPADA)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetSpeed(float speed)
    {
        _rigidbody.linearVelocityX = speed;
    }

    public void SetDireccionPlayer(string direccion)
    {
        isDisparedPlayer = true;
        this._direccion = direccion;
    }

    public void SetDireccion(string direccion)
    {
        this._direccion = direccion;
    }

    public void SetDireccionEspecial(string direccion)
    {
        isDisparedPlayer = false;
        this._direccion = direccion;
        StartCoroutine(CambiarVelocidadEspecial());
    }

    private IEnumerator CambiarVelocidadEspecial()
    {
        _speed = 0.5f;

        yield return new WaitForSeconds(0.8f);

        _speed = 20f;
    }

    public void SetEsProyectilDelPlayer(bool esDelPlayer)
    {
        isDisparedPlayer = esDelPlayer;
    }
}
