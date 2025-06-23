using System.Collections;
using UnityEngine;

public class JefeFinalController : MonoBehaviour
{
    public int damage;
    private bool recibiendoDanio;
    private Transform player;
    private GameObject playerObject;
    public float detectionRadius = 15.0f;
    public float speed = 1.0f;
    public int puntosVida = 20;
    public bool enMovimiento = false;
    public bool PudeTeletransportarse = true;
    public bool atacarPersonaje = true;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private Vector3 escalaOriginal;
    private Database _data;
    private Repository _repo;
    public GameObject _proyectil1;

    private float distanciaPlayer;
    private string direccion = "Derecha";
    private float tiempoDisparo = 0f;
    private float tiempoDisparoCercano = 0f;
    private bool animacionAtacando = false;
    private bool animacionCorriendo = false;

    private GameObject objetoColisionado;

    void Start()
    {
        _repo = Repository.GetInstance();
        _data = _repo.GetData();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damage = 1;
        puntosVida = 5;
        playerObject = GameObject.FindGameObjectWithTag(EtiquetasEntidades.PLAYER);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        Animaciones();
        if (!atacarPersonaje) return;
        if (_data.isGameOver) atacarPersonaje = false;
        DetectarPlayer();
        AccionesJefe();
    }

    void Animaciones()
    {
        animator.SetBool("run", animacionCorriendo);
        animator.SetBool("atack", animacionAtacando);
    }

    private void DetectarPlayer()
    {
        if (player == null)
        {
            playerObject = GameObject.FindGameObjectWithTag(EtiquetasEntidades.PLAYER);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                return;
            }
        }
        distanciaPlayer = Vector2.Distance(transform.position, player.position);

        if (player.position.x < transform.position.x)
        {
            direccion = "Izquierda";
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
        else
        {
            direccion = "Derecha";
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
    }

    private void AccionesJefe()
    {
        float mitadRango = detectionRadius / 2f;

        if (distanciaPlayer > detectionRadius)
        {
            enMovimiento = false;
        }
        else if (distanciaPlayer > mitadRango)
        {
            tiempoDisparo += Time.deltaTime;
            if (tiempoDisparo >= 0.4f)
            {
                AtacarProyectil();
                tiempoDisparo = 0f;
            }

            Vector2 destino = Vector2.Lerp(transform.position, player.position, 0.5f);
            rb.MovePosition(Vector2.MoveTowards(rb.position, destino, speed * 3f * Time.deltaTime));
            enMovimiento = true;

            if (Mathf.Abs(distanciaPlayer - mitadRango) < 0.5f)
            {
                TeletransportarseDetrasDelJugador();
            }
        }
        else
        {
            tiempoDisparoCercano += Time.deltaTime;
            if (tiempoDisparoCercano >= 2f)
            {
                AtacarProyectil();
                tiempoDisparoCercano = 0f;
            }
            enMovimiento = false;
        }
    }

    private void TeletransportarseDetrasDelJugador()
    {
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        if (!playerController.enSuelo || recibiendoDanio || !PudeTeletransportarse) return;
        PudeTeletransportarse = false;
        StartCoroutine(ActivarTeletransportacion());
        float offset = -2f;
        Vector3 nuevaPos = player.position;
        if (direccion == "Izquierda")
            nuevaPos.x += offset;
        else
            nuevaPos.x -= offset;
        transform.position = nuevaPos;
    }

    public void RecibeDanio(Vector2 direccion, int catidadDannio)
    {
        recibiendoDanio = true;
        puntosVida -= catidadDannio;
        if (puntosVida <= 0)
        {
            _data.EnemigosMuertos++;
            _data.isVictory = true;
            _repo.SaveData();
            Destroy(gameObject);
        }

        float direccionX = Mathf.Sign(transform.position.x - direccion.x);
        Vector2 rebote = new Vector2(direccionX, 0.2f).normalized;
        rb.AddForce(rebote * 2f, ForceMode2D.Impulse);
        StartCoroutine(DesactivarDanio());
    }
    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(2f);
        recibiendoDanio = false;
    }

    IEnumerator ActivarTeletransportacion()
    {
        yield return new WaitForSeconds(4f);
        PudeTeletransportarse = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject objetoColisionado = collision.gameObject;
        string tag = objetoColisionado.tag;
        if (tag == EtiquetasEntidades.PLAYER)
        {
            this.objetoColisionado = objetoColisionado;
            animacionAtacando = true;
            animacionCorriendo = false;
        }
    }

    public void AtacarPersonajeEspada()
    {
        if (!animacionAtacando) return;
        objetoColisionado.GetComponent<PlayerController>().RecibeDanio(damage);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        GameObject objetoColisionado = collision.gameObject;
        string tag = objetoColisionado.tag;
        if (tag == EtiquetasEntidades.PLAYER)
        {
            animacionAtacando = false;
            animacionCorriendo = true;
        }
    }

    private void AtacarProyectil()
    {
        Power(_proyectil1, 3);
    }

    void Power(GameObject power, int _damageFire)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.x += (direccion == "Derecha") ? 0.5f : -0.5f;
        spawnPosition.y -= 0.5f;

        GameObject _power = Instantiate(power, spawnPosition, Quaternion.identity);
        ProyectilController _ExpadaController = _power.GetComponent<ProyectilController>();

        _ExpadaController._damage = _damageFire;
        _ExpadaController.SetDireccionEspecial(direccion);
    }

    void OnDrawGizmos()
    {
        if (player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
