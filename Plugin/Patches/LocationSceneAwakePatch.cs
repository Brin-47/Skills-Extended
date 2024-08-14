﻿using System;
using System.Linq;
using EFT;
using SkillsExtended.Helpers;
using SPT.Reflection.Patching;
using System.Reflection;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using HarmonyLib;
using SkillsExtended.LockPicking;
using SPT.Reflection.Utils;

namespace SkillsExtended.Patches
{
    internal class LocationSceneAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
            => typeof(LocationScene).GetMethod(nameof(LocationScene.Awake));

        [PatchPostfix]
        private static void Postfix(LocationScene __instance)
        {
            foreach (var interactableObj in __instance.WorldInteractiveObjects)
            {
                if (interactableObj.KeyId != null && interactableObj.KeyId != string.Empty)
                {
                    if (Plugin.Keys.KeyLocale.ContainsKey(interactableObj.KeyId))
                    {
                        Plugin.Log.LogDebug($"Door ID: {interactableObj.Id} KeyID: {interactableObj.KeyId} Key Name: {Plugin.Keys.KeyLocale[interactableObj.KeyId]}");
                    }
                    else
                    {
                        Plugin.Log.LogError($"Door ID: {interactableObj.Id} KeyID: {interactableObj.KeyId} Key locale missing...");
                    }
                }
            }
        }
    }

    internal class OnGameStartedPatch : ModulePatch
    {
        private static Type _stimType;
        private static Type _painKillerType;
        private static Type _medEffectType;
        
        protected override MethodBase GetTargetMethod()
        {
            var healthControllerType = PatchConstants.EftTypes.Single(t => t.Name is nameof(ActiveHealthController));
            var nestedTypes = healthControllerType.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance);
            _stimType = nestedTypes.First(t => t.Name == "Stimulator");
            _painKillerType = nestedTypes.First(t => t.Name == "PainKiller");
            _medEffectType = nestedTypes.First(t => t.Name == "MedEffect");
            
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }
        
        [PatchPostfix]
        private static void Postfix(GameWorld __instance)
        {
#if DEBUG
            Plugin.Log.LogDebug($"Player map id: {__instance.MainPlayer.Location}");
#endif
            LockPicking.Helpers.InspectedDoors.Clear();
            LockPicking.Helpers.DoorAttempts.Clear();
            
            __instance.MainPlayer.ActiveHealthController.EffectStartedEvent += ApplyMedicalXp;
        }
        
        private static void ApplyMedicalXp(IEffect effect)
        {
            var skillMgrExt = Plugin.PlayerSkillManagerExt;
            var xpGain = Plugin.SkillData.FieldMedicine.FieldMedicineXpPerAction;
            
            if (_stimType.IsInstanceOfType(effect) || _painKillerType.IsInstanceOfType(effect))
            {
                if (!Plugin.SkillData.FieldMedicine.Enabled) return;
                
                skillMgrExt.FieldMedicineAction.Complete(xpGain);
                Logger.LogDebug("Applying Field Medicine XP");
                return;
            }

            if (_medEffectType.IsInstanceOfType(effect))
            {
                if (!Plugin.SkillData.FirstAid.Enabled) return;
                
                skillMgrExt.FirstAidAction.Complete(xpGain);
                Logger.LogDebug("Applying First Aid XP");
            }
        }
    }
}