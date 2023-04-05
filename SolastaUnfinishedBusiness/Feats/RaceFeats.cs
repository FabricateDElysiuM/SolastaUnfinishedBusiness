﻿using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RaceFeats
{
    private const string RevenantMaul = "RevenantMaul";
    private const string ElvenPrecision = "ElvenPrecision";
    private const string FadeAway = "FadeAway";
    private const string RevenantGreatSword = "RevenantGreatSword";
    private const string SquatNimbleness = "SquatNimbleness";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        // Dragon Wings
        var featDragonWings = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatDragonWings")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPowerBuilder
                    .Create("PowerFeatDragonWings")
                    .SetGuiPresentation("FeatDragonWings", Category.Feat,
                        Sprites.GetSprite("PowerCallForCharge", Resources.PowerCallForCharge, 256, 128))
                    .SetUsesProficiencyBonus(ActivationTime.BonusAction)
                    .SetCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.DoesNotHaveHeavyArmor))
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                            .SetDurationData(DurationType.Minute, 1)
                            .SetEffectForms(
                                EffectFormBuilder
                                    .Create()
                                    .SetConditionForm(
                                        DatabaseHelper.ConditionDefinitions.ConditionFlying12,
                                        ConditionForm.ConditionOperation.Add)
                                    .Build())
                            .Build())
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsDragonborn)
            .AddToDB();

        //
        // Fade Away support
        //

        var powerFeatFadeAwayInvisible = FeatureDefinitionPowerBuilder
            .Create("PowerFeatFadeAwayInvisible")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Invisibility.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1)
                .Build())
            .SetReactionContext(ReactionTriggerContext.DamagedByAnySource)
            .AddToDB();

        // Fade Away (Dexterity)
        var featFadeAwayDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Fade Away (Intelligence)
        var featFadeAwayInt = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatFadeAwayInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                powerFeatFadeAwayInvisible)
            .SetValidators(ValidatorsFeat.IsGnome)
            .SetFeatFamily(FadeAway)
            .AddToDB();

        // Elven Accuracy (Dexterity)
        var featElvenAccuracyDexterity = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyDexterity")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(ElvenPrecisionLogic.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Intelligence)
        var featElvenAccuracyIntelligence = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyIntelligence")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(ElvenPrecisionLogic.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Wisdom)
        var featElvenAccuracyWisdom = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyWisdom")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(ElvenPrecisionLogic.ElvenPrecisionContext.Mark)
            .AddToDB();

        // Elven Accuracy (Charisma)
        var featElvenAccuracyCharisma = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatElvenAccuracyCharisma")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta) // accuracy roll is handled by patches
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(ElvenPrecision)
            .SetCustomSubFeatures(ElvenPrecisionLogic.ElvenPrecisionContext.Mark)
            .AddToDB();

        //
        // Revenant Maul support
        //

        var validMaulWeapon = ValidatorsWeapon.IsOfWeaponType(MaulType);

        var attributeModifierFeatRevenantMaulArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatRevenantMaulArmorClass")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasMaulInHands)
            .AddToDB();

        var modifyAttackModeFeatRevenantMaul = FeatureDefinitionBuilder
            .Create("ModifyAttackModeFeatRevenantMaul")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validMaulWeapon))
            .AddToDB();

        // Revenant Maul (Dexterity)
        var featRevenantMaulDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantMaulDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                attributeModifierFeatRevenantMaulArmorClass,
                modifyAttackModeFeatRevenantMaul)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantMaul)
            .AddToDB();

        //
        // Revenant support
        //

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(GreatswordType);

        var attributeModifierFeatRevenantGreatSwordArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatRevenantGreatSwordArmorClass")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasGreatswordInHands)
            .AddToDB();

        var modifyAttackModeFeatRevenantGreatSword = FeatureDefinitionBuilder
            .Create("ModifyAttackModeFeatRevenantGreatSword")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important, validWeapon))
            .AddToDB();

        // Revenant Great Sword (Dexterity)
        var featRevenantGreatSwordDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                attributeModifierFeatRevenantGreatSwordArmorClass,
                modifyAttackModeFeatRevenantGreatSword)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        // Revenant Great Sword (Strength)
        var featRevenantGreatSwordStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRevenantGreatSwordStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                attributeModifierFeatRevenantGreatSwordArmorClass,
                modifyAttackModeFeatRevenantGreatSword)
            .SetValidators(ValidatorsFeat.IsElfOfHalfElf)
            .SetFeatFamily(RevenantGreatSword)
            .AddToDB();

        //
        // Squat Nimbleness
        //
        var featSquatNimblenessDex = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots,
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyFeatSquatNimblenessAcrobatics")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Acrobatics.Name)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsSmallRace)
            .SetFeatFamily(SquatNimbleness)
            .AddToDB();

        var featSquatNimblenessStr = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSquatNimblenessStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots,
                FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyFeatSquatNimblenessAthletics")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Athletics.Name)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.IsSmallRace)
            .SetFeatFamily(SquatNimbleness)
            .AddToDB();

        //
        // set feats to be registered in mod settings
        //

        feats.AddRange(
            featRevenantMaulDex,
            featDragonWings,
            featFadeAwayDex,
            featFadeAwayInt,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom,
            featElvenAccuracyCharisma,
            featRevenantGreatSwordDex,
            featRevenantGreatSwordStr,
            featSquatNimblenessDex,
            featSquatNimblenessStr);

        var featGroupsElvenAccuracy = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupElvenAccuracy",
            ElvenPrecision,
            ValidatorsFeat.IsElfOfHalfElf,
            featElvenAccuracyCharisma,
            featElvenAccuracyDexterity,
            featElvenAccuracyIntelligence,
            featElvenAccuracyWisdom);

        var featGroupFadeAway = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupFadeAway",
            FadeAway,
            ValidatorsFeat.IsGnome,
            featFadeAwayDex,
            featFadeAwayInt);

        var featGroupRevenantMaul = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupRevenantMaul",
            RevenantMaul,
            ValidatorsFeat.IsElfOfHalfElf,
            featRevenantMaulDex);

        var featGroupRevenantGreatSword = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupRevenantGreatSword",
            RevenantGreatSword,
            ValidatorsFeat.IsElfOfHalfElf,
            featRevenantGreatSwordDex,
            featRevenantGreatSwordStr);

        var featGroupSquatNimbleness = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupSquatNimbleness",
            SquatNimbleness,
            ValidatorsFeat.IsSmallRace,
            featSquatNimblenessDex,
            featSquatNimblenessStr);

        GroupFeats.FeatGroupAgilityCombat.AddFeats(featDragonWings);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(featGroupFadeAway);

        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(featGroupRevenantGreatSword, featGroupRevenantMaul);

        GroupFeats.MakeGroup("FeatGroupRaceBound", null,
            featGroupRevenantMaul,
            featDragonWings,
            featGroupsElvenAccuracy,
            featGroupFadeAway,
            featGroupRevenantGreatSword,
            featGroupSquatNimbleness);
    }
}
