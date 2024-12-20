﻿using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SkillsExtended.Helpers;
using SkillsExtended.Skills.Core;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SkillsExtended.Skills.FirstAid.Patches;

public class HealthEffectComponentPatch : ModulePatch
{
    
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Constructor(typeof(HealthEffectsComponent), new []{typeof(Item), typeof(IHealthEffect)});
    }

    private static Dictionary<string, int> _instanceIdsChangedAtLevel = [];
    private static Dictionary<string, OriginalCosts> _originalCosts = [];
    
    [PatchPostfix]
    public static void PostFix(Item item, IHealthEffect template)
    {
        try
        { 
            var skillMgrExt = SkillManagerExt.Instance(EPlayerSide.Usec);
            var skillData = SkillsPlugin.SkillData.FirstAid;
        
            if (!skillData.Enabled) return;
            if (template.DamageEffects is null) return;
            if (item is not MedicalItemClass meds) return;
            if (GameUtils.GetSkillManager() is null) return;
            if (meds.TemplateId.LocalizedName().Contains("Name")) return;
            
            if (_instanceIdsChangedAtLevel.TryGetValue(meds.TemplateId, out var level))
            {
                // We've changed this item at this level
                if (level == GameUtils.GetSkillManager().FirstAid.Level) return;
                
                _instanceIdsChangedAtLevel.Remove(meds.TemplateId);
            }
            
            if (!_originalCosts.TryGetValue(meds.TemplateId, out var originalCosts))
            {
                originalCosts = new(0, 0, 0);
                _originalCosts.Add(meds.TemplateId, originalCosts);
            }
            
            if (template.DamageEffects.TryGetValue(EDamageEffectType.Fracture, out var fracture))
            {
                if (fracture is not null && fracture.Cost > 0)
                {
                    originalCosts.Fracture = originalCosts.Fracture == 0 && fracture.Cost > 0
                        ? fracture.Cost
                        : originalCosts.Fracture;
                
                    var originalCost = originalCosts.Fracture;
                    Logger.LogDebug($"Original Fracture Value: {originalCost}");
                    fracture.Cost = Mathf.FloorToInt(originalCost * (1f - skillMgrExt.FirstAidItemSpeedBuff));
                    Logger.LogDebug($"New Fracture Value: {fracture.Cost}");
                }
            }
                
            if (template.DamageEffects.TryGetValue(EDamageEffectType.LightBleeding, out var lightBleed))
            {
                if (lightBleed is not null && lightBleed.Cost > 0)
                {
                    originalCosts.LightBleed = originalCosts.LightBleed == 0 && lightBleed.Cost > 0
                        ? lightBleed.Cost
                        : originalCosts.Fracture;
                
                    var originalCost = originalCosts.LightBleed;
                    Logger.LogDebug($"Original LightBleeding Value: {originalCost}");
                    lightBleed.Cost = Mathf.FloorToInt(originalCost * (1f - skillMgrExt.FirstAidResourceCostBuff));
                    Logger.LogDebug($"New LightBleeding Value: {lightBleed.Cost}");
                }
            }
                
            if (template.DamageEffects.TryGetValue(EDamageEffectType.HeavyBleeding, out var heavyBleed))
            {
                if (heavyBleed is not null && heavyBleed.Cost > 0)
                {
                    originalCosts.HeavyBleed = originalCosts.HeavyBleed == 0 && heavyBleed.Cost > 0
                        ? heavyBleed.Cost
                        : originalCosts.Fracture;
                
                    var originalCost = originalCosts.HeavyBleed;
                    Logger.LogDebug($"Original HeavyBleeding Value: {originalCost}");
                    heavyBleed.Cost = Mathf.FloorToInt(originalCost * (1f - skillMgrExt.FirstAidResourceCostBuff));
                    Logger.LogDebug($"New HeavyBleeding Value: {heavyBleed.Cost}");
                }
            }

            if (fracture is null && lightBleed is null && heavyBleed is null) return;
            if (fracture?.Cost == 0 || lightBleed?.Cost == 0 || heavyBleed?.Cost == 0) return;
            
            Logger.LogDebug($"Updated Template: {meds.TemplateId.LocalizedName()} \n");
            _instanceIdsChangedAtLevel.Add(item.TemplateId, GameUtils.GetSkillManager().FirstAid.Level);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    

    private class OriginalCosts(int fracture = 0, int lightBleed = 0, int heavyBleed = 0)
    {
        public int Fracture = fracture;
        public int LightBleed = lightBleed;
        public int HeavyBleed = heavyBleed;
    }
}