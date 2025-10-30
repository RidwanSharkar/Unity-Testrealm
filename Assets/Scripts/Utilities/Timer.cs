using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple timer utility for countdown and interval timing.
/// </summary>
public class Timer
{
    private float duration;
    private float elapsed;
    private bool isRunning;
    private bool isRepeating;
    
    public UnityEvent OnComplete;
    public UnityEvent<float> OnTick;
    
    public float Duration => duration;
    public float Elapsed => elapsed;
    public float Remaining => Mathf.Max(0, duration - elapsed);
    public float Progress => duration > 0 ? elapsed / duration : 1f;
    public bool IsRunning => isRunning;
    public bool IsComplete => elapsed >= duration;
    
    public Timer(float duration, bool autoStart = false, bool repeating = false)
    {
        this.duration = duration;
        this.isRepeating = repeating;
        this.elapsed = 0f;
        this.isRunning = autoStart;
        
        OnComplete = new UnityEvent();
        OnTick = new UnityEvent<float>();
    }
    
    /// <summary>
    /// Update the timer
    /// </summary>
    public void Update(float deltaTime)
    {
        if (!isRunning) return;
        
        elapsed += deltaTime;
        OnTick?.Invoke(deltaTime);
        
        if (elapsed >= duration)
        {
            OnComplete?.Invoke();
            
            if (isRepeating)
            {
                elapsed = 0f;
            }
            else
            {
                isRunning = false;
            }
        }
    }
    
    /// <summary>
    /// Start or resume the timer
    /// </summary>
    public void Start()
    {
        isRunning = true;
    }
    
    /// <summary>
    /// Pause the timer
    /// </summary>
    public void Pause()
    {
        isRunning = false;
    }
    
    /// <summary>
    /// Stop and reset the timer
    /// </summary>
    public void Stop()
    {
        isRunning = false;
        elapsed = 0f;
    }
    
    /// <summary>
    /// Reset the timer without stopping
    /// </summary>
    public void Reset()
    {
        elapsed = 0f;
    }
    
    /// <summary>
    /// Set new duration
    /// </summary>
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }
}

/// <summary>
/// MonoBehaviour wrapper for Timer (for Inspector use)
/// </summary>
public class TimerComponent : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private bool repeating = false;
    
    public UnityEvent OnComplete;
    public UnityEvent<float> OnTick;
    
    private Timer timer;
    
    public float Remaining => timer?.Remaining ?? 0f;
    public float Progress => timer?.Progress ?? 0f;
    public bool IsRunning => timer?.IsRunning ?? false;
    
    void Awake()
    {
        timer = new Timer(duration, autoStart, repeating);
        timer.OnComplete = OnComplete;
        timer.OnTick = OnTick;
    }
    
    void Update()
    {
        timer?.Update(Time.deltaTime);
    }
    
    public void StartTimer() => timer?.Start();
    public void PauseTimer() => timer?.Pause();
    public void StopTimer() => timer?.Stop();
    public void ResetTimer() => timer?.Reset();
}

