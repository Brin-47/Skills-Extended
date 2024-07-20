﻿using EFT;
using SkillsExtended.Models;

namespace SkillsExtended.Helpers;

public class SkillManagerExt
{
    private static SkillDataResponse SkillData => Plugin.SkillData;
    
    public readonly SkillManager.SkillBuffClass FirstAidSpeedBuff = new()
    {
        Id = EBuffId.FirstAidHealingSpeed,
    };
    
    public readonly SkillManager.SkillBuffClass FirstAidHpBuff = new()
    {
        Id = EBuffId.FirstAidMaxHp,
    };
    
    public readonly SkillManager.SkillBuffClass FieldMedicineSpeedBuff = new()
    {
        Id = EBuffId.FieldMedicineSpeed,
    };
    
    public readonly SkillManager.SkillBuffClass UsecArSystemsErgoBuff = new()
    {
        Id = EBuffId.UsecArSystemsErgo,
    };
    
    public readonly SkillManager.SkillBuffClass UsecArSystemsRecoilBuff = new()
    {
        Id = EBuffId.UsecArSystemsRecoil,
    };
    
    public readonly SkillManager.SkillBuffClass BearAkSystemsErgoBuff = new()
    {
        Id = EBuffId.BearAkSystemsErgo,
    };
    
    public readonly SkillManager.SkillBuffClass BearAkSystemsRecoilBuff = new()
    {
        Id = EBuffId.BearAkSystemsRecoil,
    };
    
    public readonly SkillManager.SkillBuffClass LockPickingTimeBuff = new()
    {
        Id = EBuffId.LockpickingTimeReduction,
    };
    
    public readonly SkillManager.GClass1790 LockPickingUseBuffElite = new()
    {
        Id = EBuffId.LockpickingUseElite,
        BuffType = SkillManager.EBuffType.Elite
    };

    public readonly SkillManager.SkillActionClass FirstAidAction = new();
    public readonly SkillManager.SkillActionClass FieldMedicineAction = new();
    public readonly SkillManager.SkillActionClass UsecRifleAction = new();
    public readonly SkillManager.SkillActionClass BearRifleAction = new();
    public readonly SkillManager.SkillActionClass LockPickAction = new();
    
    public SkillManager.SkillBuffAbstractClass[] FirstAidBuffs()
    {
        return !Plugin.RealismConfig.med_changes
            ? new SkillManager.SkillBuffAbstractClass[]
            {
                FirstAidSpeedBuff
                    .Max(SkillData.MedicalSkills.MedicalSpeedBonus)
                    .Elite(SkillData.MedicalSkills.MedicalSpeedBonusElite),
                
                FirstAidHpBuff
                    .Max(SkillData.MedicalSkills.MedkitHpBonus)
                    .Elite(SkillData.MedicalSkills.MedkitHpBonusElite),
            }
            : new SkillManager.SkillBuffAbstractClass[]
            {
                FirstAidSpeedBuff
                    .Max(SkillData.MedicalSkills.MedkitHpBonus)
                    .Elite(SkillData.MedicalSkills.MedkitHpBonusElite),
            };
    }
    
    public SkillManager.SkillBuffAbstractClass[] FieldMedicineBuffs()
    {
        return new SkillManager.SkillBuffAbstractClass[]
        {
            FieldMedicineSpeedBuff
                .Max(SkillData.MedicalSkills.MedicalSpeedBonus)
                .Elite(SkillData.MedicalSkills.MedicalSpeedBonusElite),
        };
    }
    
    public SkillManager.SkillBuffAbstractClass[] UsecArBuffs()
    {
        return new SkillManager.SkillBuffAbstractClass[]
        {
            UsecArSystemsErgoBuff
                .Max(SkillData.UsecRifleSkill.ErgoMod)
                .Elite(SkillData.UsecRifleSkill.ErgoModElite),
            
            UsecArSystemsRecoilBuff
                .Max(SkillData.UsecRifleSkill.RecoilReduction)
                .Elite(SkillData.UsecRifleSkill.RecoilReductionElite),
        };
    }
    
    public SkillManager.SkillBuffAbstractClass[] BearAkBuffs()
    {
        return new SkillManager.SkillBuffAbstractClass[]
        {
            BearAkSystemsErgoBuff
                .Max(SkillData.BearRifleSkill.ErgoMod)
                .Elite(SkillData.BearRifleSkill.ErgoModElite),
            
            BearAkSystemsRecoilBuff
                .Max(SkillData.BearRifleSkill.RecoilReduction)
                .Elite(SkillData.BearRifleSkill.RecoilReductionElite),
        };
    }
    
    public SkillManager.SkillBuffAbstractClass[] LockPickingBuffs()
    {
        return new SkillManager.SkillBuffAbstractClass[]
        {
            LockPickingTimeBuff
                .Max(SkillData.LockPickingSkill.TimeReduction)
                .Elite(SkillData.LockPickingSkill.TimeReductionElite),
            
            LockPickingUseBuffElite
        };
    }
}