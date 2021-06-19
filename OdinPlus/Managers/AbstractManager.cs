﻿using OdinPlus.Common;
using System;
using System.Reflection;

namespace OdinPlus.Managers
{
  /// <summary>
  /// Source: https://github.com/Digitalroot/digitalroot-valheim-mods/blob/main/Digitalroot.Valheim.Common/Core/Managers/AbstractManager.cs
  /// Permission granted to OdinPlus under 
  /// License: "GNU Affero General Public License v3.0"
  /// License ref: https://github.com/Digitalroot/digitalroot-valheim-mods/blob/main/LICENSE
  /// </summary>
  public abstract class AbstractManager<T> : Singleton<T> where T : AbstractManager<T>, new()
  {
    private protected const string Namespace = "OdinPlus.Managers";

    public bool IsInitialized { get; private set; }

    protected AbstractManager()
    {
      Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      Log.Trace($"[{GetType().Name}] {nameof(IsInitialized)}: {IsInitialized}");
      if (IsInitialized) return;
      Initialize();
    }

    public void Initialize()
    {
      Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      Log.Trace($"[{GetType().Name}] {nameof(IsInitialized)}: {IsInitialized}");
      if (IsInitialized) return;
      if (HasDependencyError())
      {
        Log.Fatal("Dependency Error");
        HealthCheck();
        return;
      }
      OnInitialize();
      IsInitialized = true;
    }

    public void PostInitialize()
    {
      Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      Log.Trace($"[{GetType().Name}] {nameof(IsInitialized)}: {IsInitialized}");
      if (!IsInitialized)
      {
        Initialize();
      }

      OnPostInitialize();
    }

    public HealthCheckStatus HealthCheck()
    {
      Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      var healthCheckStatus = new HealthCheckStatus
      {
        Name = MethodBase.GetCurrentMethod().DeclaringType?.Name,
        HealthStatus = HealthStatus.Healthy
      };

      try
      {
        if (!IsInitialized)
        {
          healthCheckStatus.HealthStatus = HealthStatus.Unhealthy;
          healthCheckStatus.Reason = $"[{MethodBase.GetCurrentMethod().DeclaringType?.Name}]: IsInitialized:{IsInitialized}";
        }
      }
      catch (Exception e)
      {
        Log.Error(e);
        healthCheckStatus.HealthStatus = HealthStatus.Failed;
        healthCheckStatus.Reason = $"[{healthCheckStatus.Name}]: {e.Message}";
      }

      return OnHealthCheck(healthCheckStatus);
    }

    protected virtual void OnInitialize()
    {
      Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      HealthManager.Instance.OnHealthCheck += HealthCheck;
    }

    protected virtual void OnPostInitialize()
    {
    }

    public abstract bool HasDependencyError();

    protected abstract HealthCheckStatus OnHealthCheck(HealthCheckStatus healthCheckStatus);
  }
}
