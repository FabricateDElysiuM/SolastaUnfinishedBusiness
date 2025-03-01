﻿using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class ShieldExpert : AbstractFightingStyle
{
    internal const string ShieldExpertName = "ShieldExpert";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ShieldExpertName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("ShieldExpert", Resources.ShieldExpert, 256))
        .SetFeatures(
            FeatureDefinitionProficiencyBuilder
                .Create("AddExtraAttackShieldExpert")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Armor, EquipmentDefinitions.ShieldCategory)
                .SetCustomSubFeatures(new AddBonusShieldAttack())
                .AddToDB(),
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityShieldExpertShove")
                .SetGuiPresentationNoContent(true)
                .SetActionExecutionModifiers(
                    new ActionDefinitions.ActionExecutionModifier
                    {
                        actionId = ActionDefinitions.Id.Shove,
                        advantageType = RuleDefinitions.AdvantageType.Advantage,
                        equipmentContext = EquipmentDefinitions.EquipmentContext.WieldingShield
                    },
                    new ActionDefinitions.ActionExecutionModifier
                    {
                        actionId = ActionDefinitions.Id.ShoveBonus,
                        advantageType = RuleDefinitions.AdvantageType.Advantage,
                        equipmentContext = EquipmentDefinitions.EquipmentContext.WieldingShield
                    })
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin
    };
}
