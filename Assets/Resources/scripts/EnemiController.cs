using System.Collections;
using UnityEngine;

public class EnemiController : MonoBehaviour
{
    public int damage;
    private bool recibiendoDanio = false;
    private Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 1.0f;
    public int puntosVida = 15;
    public bool enMovimiento = false;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private Vector3 escalaOriginal;
    private Database _data;
    private Repository _repo;

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
        GameObject playerObject = GameObject.FindGameObjectWithTag(EtiquetasEntidades.PLAYER);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        escalaOriginal = transform.localScale;
    }
    void Update()
    {
        Animaciones();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(EtiquetasEntidades.PLAYER);
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                return;
            }
        }
        float distanceToplayer = Vector2.Distance(transform.position, player.position);
        if (distanceToplayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
            }
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
            }

            movement = new Vector2(direction.x, 0);
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }

    }

    void Animaciones()
    {
        animator.SetBool("run", animacionCorriendo);
        animator.SetBool("atack", animacionAtacando);
    }

    public void RecibeDanio(Vector2 direccion, int catidadDannio)
    {
        recibiendoDanio = true;
        puntosVida -= catidadDannio;
        if (puntosVida <= 0)
        {
            _data.EnemigosMuertos++;
            _repo.SaveData();
            Destroy(gameObject);
        }

        float direccionX = Mathf.Sign(transform.position.x - direccion.x);
        Vector2 rebote = new Vector2(direccionX, 0.2f).normalized;
        rb.AddForce(rebote * 5f, ForceMode2D.Impulse);
        StartCoroutine(DesactivarDanio());
    }
    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(1f);
        recibiendoDanio = false;
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
}
