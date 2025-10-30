using UnityEngine;

/// <summary>
/// Core game manager implementing singleton pattern.
/// Manages global game state, rune counts, and core game systems.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [SerializeField] private int maxPlayers = 2;
    [SerializeField] private float gameSessionTime = 0f;
    
    [Header("Rune System")]
    [SerializeField] private int criticalRuneCount = 0;
    [SerializeField] private int critDamageRuneCount = 0;
    [SerializeField] private int healthRuneCount = 0;
    [SerializeField] private int speedRuneCount = 0;
    
    [Header("Passive Unlocks")]
    [SerializeField] private bool swordPassiveUnlocked = false;
    [SerializeField] private bool scythePassiveUnlocked = false;
    [SerializeField] private bool bowPassiveUnlocked = false;
    [SerializeField] private bool runebladePassiveUnlocked = false;
    [SerializeField] private bool sabresPassiveUnlocked = false;
    
    // Public accessors for rune counts
    public int CriticalRuneCount => criticalRuneCount;
    public int CritDamageRuneCount => critDamageRuneCount;
    public int HealthRuneCount => healthRuneCount;
    public int SpeedRuneCount => speedRuneCount;
    
    // Public accessors for passive unlocks
    public bool IsSwordPassiveUnlocked() => swordPassiveUnlocked;
    public bool IsScythePassiveUnlocked() => scythePassiveUnlocked;
    public bool IsBowPassiveUnlocked() => bowPassiveUnlocked;
    public bool IsRunebladePassiveUnlocked() => runebladePassiveUnlocked;
    public bool IsSabresPassiveUnlocked() => sabresPassiveUnlocked;
    
    // Game state
    private bool isGamePaused = false;
    private int currentWave = 0;
    
    public bool IsGamePaused => isGamePaused;
    public int CurrentWave => currentWave;
    
    void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Debug.Log("Bloomscythe Unity Project Started!");
        Debug.Log($"Max Players: {maxPlayers}");
    }
    
    void Update()
    {
        if (!isGamePaused)
        {
            gameSessionTime += Time.deltaTime;
        }
    }
    
    private void InitializeGame()
    {
        // Initialize core systems
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        Debug.Log("GameManager initialized successfully");
    }
    
    /// <summary>
    /// Add a rune of specified type to the player's collection
    /// </summary>
    public void AddRune(string runeType, int amount = 1)
    {
        switch (runeType.ToLower())
        {
            case "critical":
            case "crit":
                criticalRuneCount += amount;
                Debug.Log($"Critical Rune added. Total: {criticalRuneCount}");
                break;
            case "critdamage":
            case "critdmg":
                critDamageRuneCount += amount;
                Debug.Log($"Crit Damage Rune added. Total: {critDamageRuneCount}");
                break;
            case "health":
            case "hp":
                healthRuneCount += amount;
                Debug.Log($"Health Rune added. Total: {healthRuneCount}");
                break;
            case "speed":
            case "spd":
                speedRuneCount += amount;
                Debug.Log($"Speed Rune added. Total: {speedRuneCount}");
                break;
            default:
                Debug.LogWarning($"Unknown rune type: {runeType}");
                break;
        }
    }
    
    /// <summary>
    /// Unlock a weapon's passive ability
    /// </summary>
    public void UnlockWeaponPassive(string weaponName)
    {
        switch (weaponName.ToLower())
        {
            case "sword":
                swordPassiveUnlocked = true;
                Debug.Log("Sword passive unlocked!");
                break;
            case "scythe":
                scythePassiveUnlocked = true;
                Debug.Log("Scythe passive unlocked!");
                break;
            case "bow":
                bowPassiveUnlocked = true;
                Debug.Log("Bow passive unlocked!");
                break;
            case "runeblade":
                runebladePassiveUnlocked = true;
                Debug.Log("Runeblade passive unlocked!");
                break;
            case "sabres":
                sabresPassiveUnlocked = true;
                Debug.Log("Sabres passive unlocked!");
                break;
            default:
                Debug.LogWarning($"Unknown weapon: {weaponName}");
                break;
        }
    }
    
    /// <summary>
    /// Pause or unpause the game
    /// </summary>
    public void SetPaused(bool paused)
    {
        isGamePaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }
    
    /// <summary>
    /// Start a new wave
    /// </summary>
    public void StartNewWave()
    {
        currentWave++;
        Debug.Log($"Starting Wave {currentWave}");
    }
    
    /// <summary>
    /// Reset game state (for new game)
    /// </summary>
    public void ResetGame()
    {
        gameSessionTime = 0f;
        currentWave = 0;
        criticalRuneCount = 0;
        critDamageRuneCount = 0;
        healthRuneCount = 0;
        speedRuneCount = 0;
        
        Debug.Log("Game reset");
    }
}

