﻿using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAttributeModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        public FeatureDefinitionAttributeModifierBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionAttributeModifierBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public static FeatureDefinitionAttributeModifierBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAttributeModifierBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionAttributeModifierBuilder Create(FeatureDefinitionAttributeModifier original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAttributeModifierBuilder(original, name, namespaceGuid);
        }

        public FeatureDefinitionAttributeModifierBuilder SetModifier(AttributeModifierOperation modifierType, string attribute, int amount)
        {
            Definition.SetModifierType2(modifierType);
            Definition.SetModifiedAttribute(attribute);
            Definition.SetModifierValue(amount);
            return this;
        }

        public FeatureDefinitionAttributeModifierBuilder SetModifierAbilityScore(string abilityScore)
        {
            Definition.SetModifierAbilityScore(abilityScore);
            return this;
        }
    }
}
