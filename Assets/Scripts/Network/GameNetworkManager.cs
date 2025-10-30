using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Custom network manager for multiplayer game.
/// Handles player spawning, network synchronization, and game state.
/// NOTE: This is prepared for Mirror networking integration.
/// Uncomment Mirror-specific code when Mirror is installed.
/// </summary>
public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager Instance { get; private set; }
    
    [Header("Network Settings")]
    [SerializeField] private string serverAddress = "localhost";
    [SerializeField] private int serverPort = 7777;
    [SerializeField] private int maxPlayers = 2;
    
    [Header("Player Prefabs")]
    [SerializeField] private GameObject swordPlayerPrefab;
    [SerializeField] private GameObject scythePlayerPrefab;
    
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    
    // Connected players
    private List<NetworkPlayerData> connectedPlayers = new List<NetworkPlayerData>();
    private int nextPlayerId = 1;
    
    // Properties
    public int MaxPlayers => maxPlayers;
    public int ConnectedPlayerCount => connectedPlayers.Count;
    public bool IsFull => connectedPlayers.Count >= maxPlayers;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeNetwork();
    }
    
    /// <summary>
    /// Initialize network manager
    /// </summary>
    private void InitializeNetwork()
    {
        Debug.Log("Network Manager initialized");
        Debug.Log($"Max players: {maxPlayers}");
        Debug.Log($"Server: {serverAddress}:{serverPort}");
    }
    
    /// <summary>
    /// Start hosting a game (server + client)
    /// </summary>
    public void StartHost()
    {
        // Mirror code:
        // NetworkManager.singleton.StartHost();
        
        Debug.Log("Started hosting game");
    }
    
    /// <summary>
    /// Start as dedicated server
    /// </summary>
    public void StartServer()
    {
        // Mirror code:
        // NetworkManager.singleton.StartServer();
        
        Debug.Log("Started server");
    }
    
    /// <summary>
    /// Connect to server as client
    /// </summary>
    public void StartClient()
    {
        // Mirror code:
        // NetworkManager.singleton.networkAddress = serverAddress;
        // NetworkManager.singleton.StartClient();
        
        Debug.Log($"Connecting to {serverAddress}:{serverPort}");
    }
    
    /// <summary>
    /// Stop all networking
    /// </summary>
    public void StopNetwork()
    {
        // Mirror code:
        // if (NetworkServer.active && NetworkClient.isConnected)
        //     NetworkManager.singleton.StopHost();
        // else if (NetworkClient.isConnected)
        //     NetworkManager.singleton.StopClient();
        // else if (NetworkServer.active)
        //     NetworkManager.singleton.StopServer();
        
        connectedPlayers.Clear();
        Debug.Log("Network stopped");
    }
    
    /// <summary>
    /// Called when player connects (server-side)
    /// </summary>
    public void OnPlayerConnected(string connectionId)
    {
        if (IsFull)
        {
            Debug.LogWarning("Server is full, cannot add player");
            return;
        }
        
        NetworkPlayerData playerData = new NetworkPlayerData
        {
            playerId = nextPlayerId++,
            connectionId = connectionId,
            isReady = false
        };
        
        connectedPlayers.Add(playerData);
        
        Debug.Log($"Player {playerData.playerId} connected ({connectedPlayers.Count}/{maxPlayers})");
        
        // Check if we can start the game
        if (connectedPlayers.Count == maxPlayers)
        {
            CheckAllPlayersReady();
        }
    }
    
    /// <summary>
    /// Called when player disconnects
    /// </summary>
    public void OnPlayerDisconnected(string connectionId)
    {
        NetworkPlayerData player = connectedPlayers.Find(p => p.connectionId == connectionId);
        
        if (player != null)
        {
            connectedPlayers.Remove(player);
            Debug.Log($"Player {player.playerId} disconnected ({connectedPlayers.Count}/{maxPlayers})");
        }
    }
    
    /// <summary>
    /// Spawn player at designated spawn point
    /// </summary>
    public GameObject SpawnPlayer(int playerId, bool isSwordClass)
    {
        Vector3 spawnPosition = GetSpawnPosition(playerId);
        Quaternion spawnRotation = GetSpawnRotation(playerId);
        
        GameObject playerPrefab = isSwordClass ? swordPlayerPrefab : scythePlayerPrefab;
        
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned!");
            return null;
        }
        
        GameObject player = Instantiate(playerPrefab, spawnPosition, spawnRotation);
        
        // Setup player
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Configure player based on playerId
        }
        
        // Mirror code to spawn on network:
        // NetworkServer.Spawn(player, connectionToClient);
        
        Debug.Log($"Spawned {(isSwordClass ? "Sword" : "Scythe")} player at {spawnPosition}");
        
        return player;
    }
    
    /// <summary>
    /// Get spawn position for player
    /// </summary>
    private Vector3 GetSpawnPosition(int playerId)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int spawnIndex = (playerId - 1) % spawnPoints.Length;
            return spawnPoints[spawnIndex].position;
        }
        
        // Default spawn with some offset
        return new Vector3((playerId - 1) * 3f, 0f, 0f);
    }
    
    /// <summary>
    /// Get spawn rotation for player
    /// </summary>
    private Quaternion GetSpawnRotation(int playerId)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int spawnIndex = (playerId - 1) % spawnPoints.Length;
            return spawnPoints[spawnIndex].rotation;
        }
        
        return Quaternion.identity;
    }
    
    /// <summary>
    /// Set player ready status
    /// </summary>
    public void SetPlayerReady(int playerId, bool isReady)
    {
        NetworkPlayerData player = connectedPlayers.Find(p => p.playerId == playerId);
        
        if (player != null)
        {
            player.isReady = isReady;
            Debug.Log($"Player {playerId} ready status: {isReady}");
            
            CheckAllPlayersReady();
        }
    }
    
    /// <summary>
    /// Check if all players are ready to start
    /// </summary>
    private void CheckAllPlayersReady()
    {
        if (connectedPlayers.Count < 2)
            return;
        
        bool allReady = true;
        foreach (NetworkPlayerData player in connectedPlayers)
        {
            if (!player.isReady)
            {
                allReady = false;
                break;
            }
        }
        
        if (allReady)
        {
            StartGame();
        }
    }
    
    /// <summary>
    /// Start the game when all players are ready
    /// </summary>
    private void StartGame()
    {
        Debug.Log("All players ready! Starting game...");
        
        // Load game scene or start game logic
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewWave();
        }
    }
    
    /// <summary>
    /// Synchronize game state across network
    /// </summary>
    public void SyncGameState()
    {
        // This would sync things like:
        // - Wave number
        // - Enemy spawns
        // - Rune pickups
        // - Player positions/health
        
        // Mirror would handle most of this automatically with SyncVars
    }
}

/// <summary>
/// Data for a connected network player
/// </summary>
[System.Serializable]
public class NetworkPlayerData
{
    public int playerId;
    public string connectionId;
    public bool isReady;
    public string playerName;
    public bool isSwordClass; // true = Sword, false = Scythe
}

