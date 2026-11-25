using System;
using UnityEngine;

// Umieszczany w scenie docelowej (np. na bootstrapie sceny). Po wczytaniu sceny
// odbiera dane zapisu przekazane przez MainMenu.ResumeGame (SaveSystem.pendingLoad)
// i rozglasza je dalej - inne komponenty (gracz, ekwipunek, stan swiata) podlapia
// ten event i zastosuja swoja czesc danych. Wywolanie w Start, nie Awake, zeby
// wszyscy sluchajacy mieli juz zdazyc zapisac sie w OnEnable.
public class SaveDataLoader : MonoBehaviour
{
    public static event Action<SaveData> OnSaveDataLoaded;

    private void Start()
    {
        SaveData data = SaveSystem.ConsumePendingLoad();
        if (data == null) return;

        OnSaveDataLoaded?.Invoke(data);
    }
}
