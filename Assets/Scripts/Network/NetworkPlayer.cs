using UnityEngine;

/// <summary>
/// Network player component for multiplayer synchronization.
/// NOTE: This is prepared for Mirror networking integration.
/// Uncomment Mirror-specific code when Mirror is installed.
/// </summary>
public class NetworkPlayer : MonoBehaviour
{
    [Header("Network Identity")]
    [SerializeField] private int playerId;
    [SerializeField] private string playerName = "Player";
    [SerializeField] private bool isLocalPlayer = false;
    
    [Header("Synchronized Data")]
    // These would be [SyncVar] in Mirror
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private WeaponType currentWeapon;
    [SerializeField] private Vector3 networkPosition;
    [SerializeField] private Quaternion networkRotation;
    
    [Header("Network Settings")]
    [SerializeField] private float positionLerpSpeed = 10f;
    [SerializeField] private float rotationLerpSpeed = 10f;
    
    // Components
    private PlayerController playerController;
    private HealthComponent healthComponent;
    
    // Properties
    public int PlayerId => playerId;
    public string PlayerName => playerName;
    public bool IsLocalPlayer => isLocalPlayer;
    
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        healthComponent = GetComponent<HealthComponent>();
    }
    
    void Start()
    {
        // Mirror code:
        // if (isLocalPlayer)
        // {
        //     SetupLocalPlayer();
        // }
        // else
        // {
        //     SetupRemotePlayer();
        // }
        
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged.AddListener(OnHealthChanged);
        }
    }
    
    void Update()
    {
        // For non-local players, interpolate position and rotation
        if (!isLocalPlayer)
        {
            InterpolateTransform();
        }
    }
    
    /// <summary>
    /// Setup for local player (this client's player)
    /// </summary>
    private void SetupLocalPlayer()
    {
        // Enable camera
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.tag = "MainCamera";
        }
        
        // Enable audio listener
        AudioListener audioListener = GetComponentInChildren<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = true;
        }
        
        // Enable player controller
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        Debug.Log($"Local player {playerName} setup complete");
    }
    
    /// <summary>
    /// Setup for remote player (other clients' players)
    /// </summary>
    private void SetupRemotePlayer()
    {
        // Disable camera
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }
        
        // Disable audio listener
        AudioListener audioListener = GetComponentInChildren<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = false;
        }
        
        // Disable local input
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        Debug.Log($"Remote player {playerName} setup complete");
    }
    
    /// <summary>
    /// Interpolate position and rotation for smooth network movement
    /// </summary>
    private void InterpolateTransform()
    {
        // Smoothly move to network position
        transform.position = Vector3.Lerp(
            transform.position,
            networkPosition,
            Time.deltaTime * positionLerpSpeed
        );
        
        // Smoothly rotate to network rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            networkRotation,
            Time.deltaTime * rotationLerpSpeed
        );
    }
    
    /// <summary>
    /// Called when health changes (server-side)
    /// </summary>
    private void OnHealthChanged(int current, int max)
    {
        // Mirror would automatically sync these via SyncVar
        currentHealth = current;
        maxHealth = max;
        
        // Mirror code:
        // if (isServer)
        // {
        //     RpcUpdateHealth(current, max);
        // }
    }
    
    /// <summary>
    /// Update position on network (called by local player)
    /// </summary>
    public void UpdateNetworkPosition(Vector3 position, Quaternion rotation)
    {
        networkPosition = position;
        networkRotation = rotation;
        
        // Mirror code:
        // if (isLocalPlayer)
        // {
        //     CmdUpdatePosition(position, rotation);
        // }
    }
    
    // Mirror Command (client → server)
    // [Command]
    // private void CmdUpdatePosition(Vector3 position, Quaternion rotation)
    // {
    //     networkPosition = position;
    //     networkRotation = rotation;
    // }
    
    // Mirror ClientRpc (server → all clients)
    // [ClientRpc]
    // private void RpcUpdateHealth(int current, int max)
    // {
    //     if (healthComponent != null)
    //     {
    //         healthComponent.SetMaxHealth(max, false);
    //         // Update current health without triggering events
    //     }
    // }
    
    /// <summary>
    /// Request weapon switch
    /// </summary>
    public void RequestWeaponSwitch(WeaponType weaponType)
    {
        // Mirror code:
        // if (isLocalPlayer)
        // {
        //     CmdSwitchWeapon(weaponType);
        // }
        
        currentWeapon = weaponType;
    }
    
    // Mirror Command
    // [Command]
    // private void CmdSwitchWeapon(WeaponType weaponType)
    // {
    //     currentWeapon = weaponType;
    //     RpcSwitchWeapon(weaponType);
    // }
    
    // Mirror ClientRpc
    // [ClientRpc]
    // private void RpcSwitchWeapon(WeaponType weaponType)
    // {
    //     if (playerController != null)
    //     {
    //         // Switch weapon visually for all clients
    //     }
    // }
    
    /// <summary>
    /// Request ability use (synchronized across network)
    /// </summary>
    public void RequestUseAbility(string abilityKey)
    {
        // Mirror code:
        // if (isLocalPlayer)
        // {
        //     CmdUseAbility(abilityKey);
        // }
    }
    
    // Mirror Command
    // [Command]
    // private void CmdUseAbility(string abilityKey)
    // {
    //     RpcUseAbility(abilityKey);
    // }
    
    // Mirror ClientRpc
    // [ClientRpc]
    // private void RpcUseAbility(string abilityKey)
    // {
    //     // Play ability animation/effects for all clients
    //     if (playerController != null && playerController.CurrentWeapon != null)
    //     {
    //         playerController.CurrentWeapon.PerformAbility(abilityKey);
    //     }
    // }
    
    /// <summary>
    /// Set player ID
    /// </summary>
    public void SetPlayerId(int id)
    {
        playerId = id;
    }
    
    /// <summary>
    /// Set player name
    /// </summary>
    public void SetPlayerName(string name)
    {
        playerName = name;
    }
}

