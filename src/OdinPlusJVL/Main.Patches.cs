﻿using Digitalroot.Valheim.Common;
using Digitalroot.Valheim.Common.Interfaces;
using OdinPlusJVL.Common.EventArgs;
using OdinPlusJVL.Extensions;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OdinPlusJVL
{
  public partial class Main
  {
    #region Events

    #region ZNetScene

    public event EventHandler ZNetSceneReady;

    public void OnZNetSceneReady(ref ZNetScene zNetScene)
    {
      Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      Log.Debug(Instance, $"[{GetType().Name}] Calling ZNetSceneReady Subscribers");
      try
      {
        Log.Debug(Instance, $"[{GetType().Name}] Calling Managers with IOnZNetSceneReady");
        foreach (var onZNetSceneReady in _managersList.Select(manager => manager as IOnZNetSceneReady))
        {
          try
          {
            onZNetSceneReady?.OnZNetSceneReady(zNetScene);
          }
          catch (Exception e)
          {
            Log.Error(Instance, e);
          }
        }

        Log.Debug(Instance, $"[{GetType().Name}] Calling ZNetSceneReady Event Subscribers");
        if (Instance.ZNetSceneReady != null && Instance.ZNetSceneReady.GetInvocationList().Length > 0)
        {
          foreach (Delegate @delegate in Instance?.ZNetSceneReady?.GetInvocationList()?.ToList())
          {
            try
            {
              Log.Trace(Instance, $"[{GetType().Name}] {@delegate.Method.DeclaringType?.Name}.{@delegate.Method.Name}()");
              EventHandler subscriber = (EventHandler)@delegate;
              subscriber.Invoke(Instance, new OnZNetSceneReadyEventArgs(zNetScene));
            }
            catch (Exception e)
            {
              HandleDelegateError(@delegate.Method, e);
            }
          }
        }

        //_NetSceneRoot.keeper_prefab(Clone)
        while (zNetScene?.m_netSceneRoot?.FindGameObject("keeper_prefab(Clone)") != null)
        {
          var keeper = zNetScene?.m_netSceneRoot?.FindGameObject("keeper_prefab(Clone)");
          if (keeper != null)
          {
            Log.Error(Instance, $"Found {keeper?.name}");
            DestroyImmediate(keeper);
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    public void OnZNetSceneShutdown(ref ZNetScene zNetScene)
    {
      Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");

      try
      {
        Log.Debug(Instance, $"[{GetType().Name}] Calling Managers with IDestroyable");
        foreach (var destroyable in _managersList.Select(manager => manager as IDestroyable))
        {
          try
          {
            destroyable?.OnDestroy();
          }
          catch (Exception e)
          {
            Log.Error(Instance, e);
          }
        }

        //_NetSceneRoot.keeper_prefab(Clone)
        while (zNetScene?.m_netSceneRoot?.FindGameObject("keeper_prefab(Clone)") != null)
        {
          var keeper = zNetScene?.m_netSceneRoot?.FindGameObject("keeper_prefab(Clone)");
          if (keeper != null)
          {
            Log.Error(Instance, $"Found {keeper?.name}");
            DestroyImmediate(keeper);
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    #endregion

    #region ZNetReady

    public event EventHandler ZNetReady;

    public void OnZNetReady(ref ZNet zNet)
    {
      Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      Log.Debug(Instance, $"[{GetType().Name}] Calling ZNetReady Subscribers");
      try
      {
        Log.Debug(Instance, $"[{GetType().Name}] Calling Managers with IOnZNetReady");
        foreach (var onZNetReady in _managersList.Select(manager => manager as IOnZNetReady))
        {
          try
          {
            onZNetReady?.OnZNetReady(zNet);
          }
          catch (Exception e)
          {
            Log.Error(Instance, e);
          }
        }

        Log.Debug(Instance, $"[{GetType().Name}] Calling ZNetReady Event Subscribers");
        if (Instance.ZNetReady != null && Instance.ZNetReady.GetInvocationList().Length > 0)
        {
          foreach (Delegate @delegate in Instance?.ZNetReady?.GetInvocationList()?.ToList())
          {
            try
            {
              Log.Trace(Instance, $"[{GetType().Name}] {@delegate.Target}.{@delegate.Method.Name}()");
              EventHandler subscriber = (EventHandler)@delegate;
              subscriber.Invoke(Instance, new OnZNetReadyEventArgs(zNet));
            }
            catch (Exception e)
            {
              HandleDelegateError(@delegate.Method, e);
            }
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    #endregion

    #region ZoneSystemLoaded

    public event EventHandler ZoneSystemLoaded;

    public void OnZoneSystemLoaded()
    {
      Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      RootObject.SetActive(true);

      Log.Debug(Instance, $"[{GetType().Name}] Calling OnZoneSystemLoaded Subscribers");
      try
      {
        Log.Debug(Instance, $"[{GetType().Name}] Calling Managers with IOnZoneSystemLoaded");
        foreach (var onZoneSystemLoaded in _managersList.Select(manager => manager as IOnZoneSystemLoaded))
        {
          try
          {
            onZoneSystemLoaded?.OnZoneSystemLoaded();
          }
          catch (Exception e)
          {
            Log.Error(Instance, e);
          }
        }

        Log.Debug(Instance, $"[{GetType().Name}] Calling ZoneSystemLoaded Event Subscribers");
        if (Instance.ZoneSystemLoaded != null && Instance.ZoneSystemLoaded.GetInvocationList().Length > 0)
        {
          foreach (Delegate @delegate in Instance.ZoneSystemLoaded.GetInvocationList()?.ToList())
          {
            try
            {
              Log.Trace(Instance, $"[{GetType().Name}] {@delegate.Target}.{@delegate.Method.Name}()");
              EventHandler subscriber = (EventHandler)@delegate;
              subscriber.Invoke(Instance, EventArgs.Empty);
            }
            catch (Exception e)
            {
              HandleDelegateError(@delegate.Method, e);
            }
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    #endregion

    #region SpawnedPlayer

    public event EventHandler SpawnedPlayer;

    public void OnSpawnedPlayer(Vector3 spawnPoint)
    {
      Log.Trace(Instance, $"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      Log.Debug(Instance, $"[{GetType().Name}] Calling SpawnedPlayer Subscribers");
      try
      {
        Log.Debug(Instance, $"[{GetType().Name}] Calling Managers with IOnSpawnedPlayer");
        foreach (var onSpawnedPlayer in _managersList.Select(manager => manager as IOnSpawnedPlayer))
        {
          try
          {
            onSpawnedPlayer?.OnSpawnedPlayer(spawnPoint);
          }
          catch (Exception e)
          {
            Log.Error(Instance, e);
          }
        }

        Log.Debug(Instance, $"[{GetType().Name}] Calling SpawnedPlayer Event Subscribers");
        if (Instance.SpawnedPlayer != null && Instance.SpawnedPlayer.GetInvocationList().Length > 0)
        {
          foreach (Delegate @delegate in Instance?.SpawnedPlayer?.GetInvocationList()?.ToList())
          {
            try
            {
              Log.Trace(Instance, $"[{GetType().Name}] {@delegate.Method.DeclaringType?.Name}.{@delegate.Method.Name}()");
              EventHandler subscriber = (EventHandler)@delegate;
              subscriber.Invoke(Instance, new OnSpawnedPlayerEventArgs(spawnPoint));
            }
            catch (Exception e)
            {
              HandleDelegateError(@delegate.Method, e);
            }
          }
        }

        //_NetSceneRoot.keeper_prefab(Clone)
        while (ZNetScene.instance?.m_netSceneRoot?.FindGameObject("keeper_prefab(Clone)") != null)
        {
          var keeper = ZNetScene.instance?.m_netSceneRoot?.FindGameObject("keeper_prefab(Clone)");
          if (keeper != null)
          {
            Log.Error(Instance, $"Found {keeper?.name}");
            DestroyImmediate(keeper);
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(Instance, e);
      }
    }

    #endregion

    private static void HandleDelegateError(MethodInfo method, Exception exception)
    {
      Log.Error(Instance, $"[{method}] {exception.Message}");
    }

    #endregion
  }
}
