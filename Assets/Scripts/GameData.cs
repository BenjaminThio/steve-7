[System.Serializable]
public class PlayerData
{
    public int health = 100;
    public float speed = 7;
    public float jumpForce = 5;
}

[System.Serializable]
public class SettingsData
{
    public int brightness = 0;
    public int masterVolume = 0;
    public int sfxVolume = 0;
    public int bgmVolume = 0;
    public int sensitivity = 0;
}

[System.Serializable]
public class GameData
{
    public PlayerData playerData;
    public SettingsData settingsData;
}
