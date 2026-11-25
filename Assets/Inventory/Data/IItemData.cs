using UnityEngine;

// Wspolny interfejs dla wszystkich typow danych przedmiotow (surowce, bron, itd.).
public interface IItemData
{
    // Stabilny identyfikator przedmiotu, uzywany do zapisu/wczytania (ItemDatabase.GetById).
    // Nie referencja w pamieci - ScriptableObject nie przezywa JsonUtility.
    string ItemId { get; }
    // Nazwa przedmiotu wyswietlana np. w UI.
    string ItemName { get; }
    // Maksymalna liczba sztuk tego przedmiotu w jednym stacku.
    int MaxStackSize { get; }
    // Ikona przedmiotu uzywana w UI ekwipunku.
    Sprite Icon { get; }
    // Prefab uzywany przy wyrzucaniu/spawnowaniu przedmiotu w swiecie gry.
    GameObject Prefab { get; }
}
