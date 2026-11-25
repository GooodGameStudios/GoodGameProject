using System.IO;
using UnityEngine;

// Niskopoziomowy odczyt/zapis SaveData na dysku. Gra wspiera tylko jeden
// zapis, wiec wszystkie operacje dzialaja na jednej, stalej sciezce pliku.
public static class SaveSystem
{
    private const string SaveFileName = "savegame.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool SaveExists() => File.Exists(SavePath);

    // Zapisuje dane na dysk - jesli zapis juz istnieje pod tą sciezką, zostaje nadpisany.
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData Load()
    {
        if (!SaveExists()) return null;

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteSave()
    {
        if (SaveExists())
        {
            File.Delete(SavePath);
        }
    }

    // Dane wczytane z dysku, ktore czekaja na zastosowanie w scenie docelowej -
    // ustawiane przez ekran glowny tuz przed SceneManager.LoadScene, a odbierane
    // przez SaveDataLoader juz w nowo wczytanej scenie. Dziala jako zwykle pole
    // statyczne, bez potrzeby DontDestroyOnLoad - przetrwa SceneManager.LoadScene.
    private static SaveData pendingLoad;

    public static void SetPendingLoad(SaveData data) => pendingLoad = data;

    // Zwraca i czysci czekajace dane - kazdy zapis moze zostac zastosowany tylko raz.
    public static SaveData ConsumePendingLoad()
    {
        SaveData data = pendingLoad;
        pendingLoad = null;
        return data;
    }
}
