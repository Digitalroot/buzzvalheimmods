﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.UI;
namespace OdinPlus
{
	[BepInPlugin("buzz.valheim.OdinPlus", "OdinPlus", "0.0.1")]
	public class Plugin : BaseUnityPlugin
	{
		#region Config Var
		//public static ConfigEntry<int> nexusID;
		public static ManualLogSource logger;
		public static ConfigEntry<KeyboardShortcut> KS_SecondInteractkey;
		public static ConfigEntry<KeyboardShortcut> KS_debug;
		public static ConfigEntry<KeyboardShortcut> KS_debug2;
		public static ConfigEntry<string> CFG_ItemSellValue;
		public static ConfigEntry<string> CFG_Pets;
		Harmony _harmony;
		#endregion
		public static GameObject OdinPlusRoot;
		private void Awake()
		{
			Plugin.logger = base.Logger;
			CFG_ItemSellValue = base.Config.Bind<string>("Config", "ItemSellValue", "Wood:1;Coins:1");
			CFG_Pets = base.Config.Bind<string>("Config", "PetList", "Troll,GoblinShaman");
			//Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 354, "Nexus mod ID for updates");
			KS_SecondInteractkey = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Second Interact key", new KeyboardShortcut(KeyCode.F));
			KS_debug = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "debug key", new KeyboardShortcut(KeyCode.F3));
			KS_debug2 = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "debug key2", new KeyboardShortcut(KeyCode.F4));
			_harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

			//notice:: init here
			OdinPlusRoot = new GameObject("OdinPlus");
			OdinPlusRoot.AddComponent<OdinPlus>();
			DontDestroyOnLoad(OdinPlusRoot);
			OdinScore.init();
			DBG.blogInfo("OdinPlus Loadded");
		}

		private void OnDestroy()
		{
			if (_harmony != null) _harmony.UnpatchSelf();
		}
		#region patch		
		#region StoreGui
		[HarmonyPatch(typeof(StoreGui), "Show")]
		private static class Prefix_StoreGui_Show
		{
			private static void Postfix(StoreGui __instance, Trader trader)
			{
				if (OdinPlus.traderNameList.Contains(trader.m_name))
				{
					OdinTrader.TweakGui(__instance, true);
					return;
				}
				return;
			}
		}
		[HarmonyPatch(typeof(StoreGui), "Hide")]
		private static class Prefix_StoreGui_Hide
		{
			private static void Prefix(StoreGui __instance)
			{
				var trader = Traverse.Create(__instance).Field<Trader>("m_trader").Value;
				if (OdinPlus.traderNameList.Contains(trader.m_name))
				{
					OdinTrader.TweakGui(__instance, false);
					return;
				}
				return;
			}
		}
		[HarmonyPatch(typeof(StoreGui), "GetPlayerCoins")]
		private static class Postfix_StoreGui_GetPlayerCoins
		{
			private static void Postfix(StoreGui __instance, ref int __result)
			{
				var t = Traverse.Create(__instance).Field<Trader>("m_trader").Value;
				if (t == null)
				{
					return;
				}
				string name = t.m_name;
				if (OdinPlus.traderNameList.Contains(name))
				{
					__result = OdinScore.score;
					return;
				}
			}
		}
		[HarmonyPatch(typeof(StoreGui), "BuySelectedItem")]
		private static class Prefix_StoreGui_BuySelectedItem
		{
			private static bool Prefix(StoreGui __instance)
			{
				string name = Traverse.Create(__instance).Field<Trader>("m_trader").Value.m_name;
				if (OdinPlus.traderNameList.Contains(name))
				{
					var m_selectedItem = Traverse.Create(__instance).Field<Trader.TradeItem>("m_selectedItem").Value;
					int stack = Mathf.Min(m_selectedItem.m_stack, m_selectedItem.m_prefab.m_itemData.m_shared.m_maxStackSize);
					if (m_selectedItem == null || (m_selectedItem.m_price * stack - OdinScore.score > 0))
					{
						return false;
					}
					int quality = m_selectedItem.m_prefab.m_itemData.m_quality;
					int variant = m_selectedItem.m_prefab.m_itemData.m_variant;
					if (Player.m_localPlayer.GetInventory().AddItem(m_selectedItem.m_prefab.name, stack, quality, variant, 0L, "") != null)
					{
						OdinScore.remove(m_selectedItem.m_price * stack);//?
						__instance.m_buyEffects.Create(__instance.gameObject.transform.position, Quaternion.identity, null, 1f);
						Player.m_localPlayer.ShowPickupMessage(m_selectedItem.m_prefab.m_itemData, m_selectedItem.m_prefab.m_itemData.m_stack);
						Traverse.Create(__instance).Method("FillList").GetValue();
						Gogan.LogEvent("Game", "BoughtItem", m_selectedItem.m_prefab.name, 0L);
					}
					return false;
				}
				return true;
			}
		}

		#endregion
		#region Player and Console and Fejd
		[HarmonyPatch(typeof(Player), "Update")]
		private static class Patch_Player_Update
		{
			private static void Postfix(Player __instance)
			{
				if (CheckPlayerNull())
				{
					return;
				}
				if (KS_SecondInteractkey.Value.IsDown() && __instance.GetHoverObject() != null)
				{
					if (__instance.GetHoverObject().GetComponent<OdinInteractable>() != null)
					{
						__instance.GetHoverObject().GetComponent<OdinInteractable>().SecondaryInteract(__instance);
						return;
					}
					if (__instance.GetHoverObject().GetComponentInParent<OdinInteractable>() != null)
					{
						__instance.GetHoverObject().GetComponentInParent<OdinInteractable>().SecondaryInteract(__instance);
						return;
					}

				}
				#region debug
				if (KS_debug.Value.IsUp())
				{
					OdinPlus.m_instance.Reset();
				}
				if (KS_debug2.Value.IsUp())
				{
					OdinPlus.UnRegister();
					Destroy(OdinPlusRoot);

				}
				#endregion
				//end
			}
		}

		[HarmonyPatch(typeof(Console), "InputText")]
		private static class Patch_Console_InputText
		{
			private static void Prefix()
			{
				OdinPlus.ProcessCommands(global::Console.instance.m_input.text);
			}
		}

		[HarmonyPatch(typeof(FejdStartup), "Start")]
		private static class FejdStartup_Start_Patch
		{
			private static void Postfix()
			{
				if (OdinPlus.isInit)
				{
					return;
				}
				OdinPlus.Init();
			}
		}
		[HarmonyPatch(typeof(Raven), "Awake")]
		private static class Patch_Raven_Awake
		{
			private static void Postfix(Raven __instance)
			{
				if (OdinPlus.isNPCInit) { return; }
				PetManager.excObj = Instantiate(__instance.m_exclamation, Vector3.zero, Quaternion.identity, PetManager.Indicator.transform);
				PetManager.excObj.gameObject.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.red);
				PetManager.excObj.gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;

				NpcManager.RavenPrefab = __instance.gameObject;

				OdinPlus.InitNPC();
			}
		}

		#endregion
		#region Misc


		[HarmonyPatch(typeof(Localization), "SetupLanguage")]
		public static class MyLocalizationPatch
		{
			public static void Postfix(Localization __instance, string language)
			{
				BuzzLocal.init(language, __instance);
				BuzzLocal.UpdateDictinary();
			}
		}

		[HarmonyPatch(typeof(PlayerProfile), "SavePlayerData")]
		public static class PlayerProfile_SavePlayerData_Patch
		{
			public static void Postfix(PlayerProfile __instance, Player player)
			{
				if (CheckPlayerNull())
				{
					return;
				}
				OdinScore.saveOdinData(player.GetPlayerName());
			}
		}

		[HarmonyPatch(typeof(PlayerProfile), "LoadPlayerData")]
		private static class Patch_PlayerProfile_LoadPlayerData
		{
			private static void Postfix(PlayerProfile __instance)
			{
				//DBG.blogWarning("loading");
				if (CheckPlayerNull()) { return; }
				OdinScore.loadOdinData(Player.m_localPlayer.GetPlayerName());
			}
		}


		#endregion
		#region ZnetScene
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class ZNetScene_Awake_Prefix
		{
			private static void Prefix(ZNetScene __instance)
			{
				OdinPlus.PreZNS(__instance);

			}
		}
		[HarmonyPriority(1000)]
		[HarmonyPatch(typeof(ZNetScene), "Awake")]
		private static class ZNetScene_Awake_Patch
		{
			private static void Postfix(ZNetScene __instance)
			{
				//Pet.init(__instance);
				OdinPlus.PostZNS();
			}
		}
		[HarmonyPatch(typeof(ZNetScene), "Shutdown")]
		private static class ZNetScene_Shutdown_Patch
		{
			private static void Postfix()
			{
				OdinPlus.UnRegister();
				OdinPlus.Clear();
			}
		}
		#endregion
		#region ZoneSystem
		[HarmonyPatch(typeof(ZoneSystem), "Awake")]
		private static class Postfix_ZoneSystem_Awake
		{
			private static void Postfix()
			{

			}
		}
		#endregion ZoneSystem
		#region ODB
		[HarmonyPatch(typeof(ObjectDB), "Awake")]
		private static class Patch_ObjectDB_Awake
		{
			private static void Postfix(ObjectDB __instance)
			{
				OdinPlus.PostODB();
			}
		}
		#endregion
		#endregion

		#region Tool
		public static bool CheckPlayerNull(bool log = false)
		{
			if (Player.m_localPlayer == null)
			{
				if (log) { DBG.blogWarning("Player is Null"); }

				return true;
			}
			return false;
		}


		#endregion

		#region Debug
		private static bool isLoaded;
		private static ZoneSystem.LocationInstance loc;
		public static void TestA()
		{
			bool find = ZoneSystem.instance.FindClosestLocation("WoodHouse1", Player.m_localPlayer.transform.position, out loc);
			Minimap.instance.DiscoverLocation(loc.m_position, Minimap.PinType.Icon3, "Odin Quest");
			Chat.instance.SendPing(loc.m_position);
			Debug.LogWarning("Find loc:" + find);

		}
		public static void TestB()
		{
			finds();
		}
		private void Update()
		{
			//finds();
		}
		private static void finds()
		{
			var a =Resources.FindObjectsOfTypeAll<GameObject>();
			string s = "";
			foreach (var item in a)
			{
				if(item.name=="Beehive"&&item.scene.name=="locations")
				{
					s+=(char)34+item.transform.parent.name+(char)34+",";
				}
			}
			Debug.LogWarning(s);
		}

		#endregion Debug
	}

}