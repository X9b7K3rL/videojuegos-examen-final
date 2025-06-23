using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float logintudRaycast = 0.1f;
    public LayerMask capaSuelo;
    public bool enSuelo;



    private string direccion = "Derecha";
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;
    public float fuerzaRebote = 10f;
    private bool atacando = false;
    private bool corriendo = false;
    private bool saltando = false;

    private AudioSource _audioSource;
    public AudioClip _audioMoneda;
    public AudioClip _audioClipEspada;
    public AudioClip _audioClipDamage;
    public AudioClip _audioClipJump;


    Repository _repo;
    Database _data;

    public GameObject _proyectil1;
    public GameObject espada;


    void Start()
    {
        espada = GameObject.Find("espada");
        _repo = Repository.GetInstance();
        _data = _repo.GetData();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();

        _repo.SaveData();
    }

    void Update()
    {
        Correr();
        SetupSalto();
        LanzarPower();
        AtacandoEnemigo();

        Animations();
    }
    public void DesactivaDanio()
    {
        rb.linearVelocityX = Vector2.zero.x;
    }

    private void Animations()
    {
        animator.SetBool("atack", atacando);
        animator.SetBool("run", corriendo);
        animator.SetBool("jump", saltando);
        animator.SetBool("diying", _data.isGameOver);
    }

    public void Atacando()
    {
        atacando = true;
        _audioSource.PlayOneShot(_audioClipEspada);
    }

    public void DesactivarAtaque()
    {
        atacando = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == EtiquetasEntidades.COIN)
        {
            _audioSource.PlayOneShot(_audioMoneda);
        }

        if (tag == EtiquetasEntidades.LIMITBOTTOM)
        {
            _data.vidas = 0;
            _repo.SaveData();
        }
    }

    void Correr()
    {
        if (_data.isGameOver) return;

        corriendo = false;

        // correr
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
        {
            rb.linearVelocityX = 17;
            sr.flipX = false;
            direccion = "Derecha";
            corriendo = true;
            espada.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift))
        {
            rb.linearVelocityX = -17;
            sr.flipX = true;
            direccion = "Izquierda";
            corriendo = true;
            espada.transform.localScale = new Vector3(-1, 1, 1);
        }
        //caminar
        else if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocityX = 6;
            sr.flipX = false;
            direccion = "Derecha";
            corriendo = true;
            espada.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocityX = -6;
            sr.flipX = true;
            direccion = "Izquierda";
            corriendo = true;
            espada.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            rb.linearVelocityX = 0;
        }
    }

    void SetupSalto()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, logintudRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        if (enSuelo == true) saltando = false;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _audioSource.PlayOneShot(_audioClipJump);
            rb.linearVelocityY = 12.5f;
            saltando = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * logintudRaycast);
    }

    public void RecibeDanio(int cantDanio)
    {
        _audioSource.PlayOneShot(_audioClipDamage);
        _data.vidas -= cantDanio;
        if (_data.vidas <= 0)
        {
            _data.vidas = 0;
        }
        _repo.SaveData();
    }

    void AtacandoEnemigo()
    {
        if (atacando) return;

        if (Input.GetKeyDown(KeyCode.J) && !saltando)
        {
            Atacando();
        }
    }

    void LanzarPower()
    {
        if (!_data.HabilidadEspecial) return;

        if (Input.GetKeyUp(KeyCode.K))
        {
            Power(_proyectil1, 3);
        }
    }

    void Power(GameObject power, int _damageFire)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.x += 0.5f;
        spawnPosition.y -= 0.5f;

        GameObject _power = Instantiate(power, spawnPosition, Quaternion.Euler(0, 0, 0));
        ProyectilController _ExpadaController = _power.GetComponent<ProyectilController>();

        _ExpadaController._damage = _damageFire;
        _ExpadaController.SetDireccionPlayer(direccion);
    }
}
