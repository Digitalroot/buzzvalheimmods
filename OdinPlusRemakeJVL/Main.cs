﻿using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using Jotunn.Managers;
using OdinPlusRemakeJVL.Common;
using OdinPlusRemakeJVL.ConsoleCommands;
using OdinPlusRemakeJVL.Items;
using OdinPlusRemakeJVL.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OdinPlusRemakeJVL
{
  [BepInPlugin(Guid, Name, Version)]
  [BepInIncompatibility("buzz.valheim.OdinPlus")]
  public class Main : BaseUnityPlugin
  {
    public const string Version = "1.0.0";
    public const string Name = "OdinPlusRemakeJVL";
    public const string Guid = "digitalroot.valheim.mods.odinplusremakejvl";
    public const string Namespace = "OdinPlusRemakeJVL";

    private Harmony _harmony;
    private readonly List<IAbstractManager> _managers = new List<IAbstractManager>();
    public static Main Instance;
    public static ConfigEntry<int> NexusId;
    
    public Main()
    {
      Log.EnableTrace();
      Instance = this;
    }

    [UsedImplicitly]
    private void Awake()
    {
      try
      {
        Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
        NexusId = Config.Bind("General", "NexusID", 000, "Nexus mod ID for updates");

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Guid);

        ItemManager.OnVanillaItemsAvailable += AddClonedItems; // ToDo move this to an item manager.

        ConsoleCommandManager.Instance.AddConsoleCommand(new ForceLoadCommand());

        if (!_managers.Contains(HealthManager.Instance)) _managers.Add(HealthManager.Instance);
        if (!_managers.Contains(ConsoleCommandManager.Instance)) _managers.Add(ConsoleCommandManager.Instance);
        if (!_managers.Contains(SpriteManager.Instance)) _managers.Add(SpriteManager.Instance);
        if (!_managers.Contains(StatusEffectsManager.Instance)) _managers.Add(StatusEffectsManager.Instance);
        if (!_managers.Contains(FxAssetManager.Instance)) _managers.Add(FxAssetManager.Instance);

        foreach (var manager in _managers)
        {
          manager.Initialize();
        }

        foreach (var manager in _managers)
        {
          manager.PostInitialize();
        }
      }
      catch (Exception e)
      {
        Log.Error(e);
      }
    }

    [UsedImplicitly]
    private void OnDestroy()
    {
      _harmony?.UnpatchSelf();
    }

    #region Event

    public event EventHandler ZNetSceneReady;

    public void OnZNetSceneReady()
    {
      Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
      Log.Debug($"Calling ZNetSceneReady Subscribers");
      try
      {
        Log.Trace($"[{GetType().Name}] Instance != null: {Instance != null}");
        Log.Trace($"[{GetType().Name}] Instance?.ZNetSceneReady != null: {Instance?.ZNetSceneReady != null}");
        Log.Trace($"[{GetType().Name}] Instance?.ZNetSceneReady?.GetInvocationList().ToList() != null: {Instance?.ZNetSceneReady?.GetInvocationList().ToList() != null}");

        foreach (Delegate @delegate in Instance?.ZNetSceneReady?.GetInvocationList()?.ToList())
        {
          try
          {
            Log.Trace($"[{GetType().Name}] {@delegate.Target}.{@delegate.Method.Name}()");
            EventHandler subscriber = (EventHandler)@delegate;
            subscriber.Invoke(this, EventArgs.Empty);
          }
          catch (Exception e)
          {
            HandleDelegateError(@delegate.Method, e);
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(e);
      }
    }

    private static void HandleDelegateError(MethodInfo method, Exception exception)
    {
      Log.Error($"[{method}] {exception.Message}");
    }

    #endregion

    private void AddClonedItems()
    {
      try
      {
        Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
        OdinLegacy.Create();
      }
      catch (Exception e)
      {
        Log.Error(e);
        throw;
      }
      finally
      {
        ItemManager.OnVanillaItemsAvailable -= AddClonedItems;
      }
    }

    private void RegisterObjects()
    {
      Log.Trace($"{GetType().Namespace}.{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
    }
  }
}
