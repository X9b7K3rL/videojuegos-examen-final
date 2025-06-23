using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform objetivo;
    public float cameraSpeed = 10f;
    public Vector3 desplazamiento;

    void LateUpdate()
    {
        if (objetivo == null)
        {
            objetivo = GameObject.FindGameObjectWithTag(EtiquetasEntidades.PLAYER).transform;
        }
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + desplazamiento;
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, cameraSpeed * Time.deltaTime);

        // Solo actualiza X e Y, mantiene Z original
        transform.position = new Vector3(posicionSuavizada.x, posicionSuavizada.y, transform.position.z);
    }
}
