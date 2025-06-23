using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    public AudioClip sonidoCerca;
    private AudioSource audioSource;
    public float rangoDeteccion = 5f;
    public bool sonidoReproducido = false;
    private SpriteRenderer spriteRenderer;
    private bool portalApareciendo = false;
    private bool puedeTeletransportar = false;
    private bool jugadorDentro = false;
    private Animator animator;
    private Repository _repo;
    private Database _data;

    void Start()
    {
        _repo = Repository.GetInstance();
        _data = _repo.GetData();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer.enabled = false;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (!sonidoReproducido && !portalApareciendo)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rangoDeteccion);
            foreach (var col in colliders)
            {
                if (col.CompareTag(EtiquetasEntidades.PLAYER))
                {
                    audioSource.PlayOneShot(sonidoCerca);
                    sonidoReproducido = true;
                    portalApareciendo = true;
                    StartCoroutine(AparecerPortal());
                    break;
                }
            }
        }
    }

    IEnumerator AparecerPortal()
    {
        spriteRenderer.enabled = true;
        Color colorOriginal = spriteRenderer.color;
        spriteRenderer.color = Color.green;
        animator.speed = 4f;
        float t = 0f;
        float duracion = 4f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1f, t / duracion);
            transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        transform.localScale = Vector3.one;
        spriteRenderer.color = colorOriginal;
        animator.speed = 1f;

        puedeTeletransportar = true;
        if (jugadorDentro)
        {
            StartCoroutine(TeletransportarDespuesDeEspera());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(EtiquetasEntidades.PLAYER))
        {
            jugadorDentro = true;
            if (puedeTeletransportar)
            {
                StartCoroutine(TeletransportarDespuesDeEspera());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(EtiquetasEntidades.PLAYER))
        {
            jugadorDentro = false;
        }
    }

    private IEnumerator TeletransportarDespuesDeEspera()
    {
        float tiempo = 2f;
        float transcurrido = 0f;
        if (_data.EnemigosMuertos < 8)
        {
            yield break;
        }
        while (transcurrido < tiempo)
        {
            if (!jugadorDentro)
                yield break;
            transcurrido += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(3);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}
