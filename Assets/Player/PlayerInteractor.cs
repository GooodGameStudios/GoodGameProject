using UnityEngine;

// Co klatke wykrywa, na jaki interaktywny obiekt (np. przedmiot w swiecie)
// patrzy gracz, i wizualnie go podswietla (Highlightable).
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform interactionOrigin;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    private Highlightable currentTarget;

    // Obiekt, na ktory aktualnie patrzy gracz.
    public Highlightable CurrentTarget => currentTarget;

    // Punkt i kierunek, z ktorego wychodzi promien interakcji (np. kamera gracza).
    public Transform InteractionOrigin => interactionOrigin != null ? interactionOrigin : transform;

    private void Update()
    {
        Transform origin = interactionOrigin != null ? interactionOrigin : transform;
        Highlightable hitTarget = null;

        // Rzut promieniem w przod, w poszukiwaniu najblizszego obiektu z warstwy interactableLayers.
        if (Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, interactDistance, interactableLayers))
        {
            hitTarget = hit.collider.GetComponentInParent<Highlightable>();
        }

        // Jesli trafiony obiekt sie nie zmienil, nie trzeba nic aktualizowac.
        if (hitTarget == currentTarget) return;

        // Zdejmujemy podswietlenie z poprzedniego celu i podswietlamy nowy.
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
        }

        currentTarget = hitTarget;

        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(true);
        }
    }
}
