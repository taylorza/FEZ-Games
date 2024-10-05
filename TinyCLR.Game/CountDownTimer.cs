// Copyright (c) 2012 Chris Taylor

using System;

namespace TinyCLR.Game
{
  /// <summary>
  /// Provides a count down timer which generates an event when
  /// the count down expires. 
  /// </summary>
  public class CountDownTimer : IDisposable
  {
    private float _timer;
    private long _period;    
    private readonly bool _periodic;
    
    /// <summary>
    /// Returns true if the count down is running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Create a CountDownTimer instance with a default count down time.
    /// </summary>
    /// <param name="millseconds">Default count down time</param>
    public CountDownTimer(long millseconds, bool periodic = false)
    {
      _period = millseconds;
      _periodic = periodic;
      IsRunning = false;
      GameManager.RegisterTimer(this);
    }

    /// <summary>
    /// Starts the count down using the default count down time.
    /// </summary>
    public void Start()
    {
      Start(_period);
    }

    /// <summary>
    /// Starts the count down using a custom count down time.
    /// </summary>
    /// <param name="milliseconds"></param>
    public void Start(long milliseconds)
    {
      _timer = milliseconds / 1000.0f;
      _period = milliseconds;
      IsRunning = true;
    }

    /// <summary>
    /// Cancels the count down without further notification.
    /// </summary>
    public void Cancel()
    {
      _timer = 0;
      IsRunning = false;
    }

    /// <summary>
    /// Expires the count down and raises the Expired event.
    /// </summary>
    public void Expire()
    {
      if (IsRunning)
      {
        Cancel();
        OnExpired();
        if (_periodic) Start(_period);
      }
    }

    /// <summary>
    /// Expires the count down, raises the Expired event and cancels the 
    /// countdown if it was a periodic counter.
    /// </summary>
    public void ExpireAndCancel()
    {
      Expire();
      Cancel();
    }

    /// <summary>
    /// Update the count down timer. This must be called for
    /// the timer to count down.
    /// </summary>
    /// <param name="elapsedTime"></param>
    internal void Update(float elapsedTime)
    {
      if (!IsRunning) return;

      _timer -= elapsedTime;
      if (_timer <= 0)
      {
        Expire();
      }
    }

    protected void OnExpired()
    {
      Expired(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event that is raised when the count down completes.
    /// </summary>
    public event EventHandler Expired = delegate { };

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        
      }

      GameManager.UnRegisterTimer(this);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
