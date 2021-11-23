﻿using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolastaCommunityExpansion.Functors
{
    public class FunctorRespec : Functor
    {
        private const int RESPEC_STATE_NORESPEC = 0;
        private const int RESPEC_STATE_RESPECING = 1;
        private const int RESPEC_STATE_ABORTED = 2;

        private static int RespecState { get; set; }

        internal static bool IsRespecing => RespecState == RESPEC_STATE_RESPECING;

        internal static void AbortRespec() => RespecState = RESPEC_STATE_ABORTED;

        internal static void StartRespec() => RespecState = RESPEC_STATE_RESPECING;

        internal static void StopRespec() => RespecState = RESPEC_STATE_NORESPEC;

        private static readonly List<RulesetItemSpellbook> rulesetItemSpellbooks = new List<RulesetItemSpellbook>();

        internal static void DropSpellbooksIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
            rulesetCharacterHero.CharacterInventory.BrowseAllCarriedItems<RulesetItemSpellbook>(rulesetItemSpellbooks);

            if (rulesetCharacterHero.ClassesHistory[rulesetCharacterHero.ClassesHistory.Count - 1].Name == "Wizard")
            {
                foreach (var rulesetItemSpellbook in rulesetItemSpellbooks)
                {
                    rulesetCharacterHero.LoseItem(rulesetItemSpellbook, false);
                }
            }
        }

        internal static void PickupSpellbooksIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
            if (rulesetCharacterHero.ClassesHistory[rulesetCharacterHero.ClassesHistory.Count - 1].Name == "Wizard")
            {
                foreach (var rulesetItemSpellbook in rulesetItemSpellbooks)
                {
                    rulesetCharacterHero.GrantItem(rulesetItemSpellbook, false);
                }
            }
        }

        public override IEnumerator Execute(FunctorParametersDescription functorParameters, FunctorExecutionContext context)
        {
            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

            var gameCampaignScreen = Gui.GuiService.GetScreen<GameCampaignScreen>();
            var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();
            var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();

            var gameCampaignScreenVisible = gameCampaignScreen?.Visible;
            var gameLocationscreenExplorationVisible = gameLocationScreenExploration?.Visible;
            var guiConsoleScreenVisible = guiConsoleScreen?.Visible;

            StartRespec();

            gameCampaignScreen.Hide(true);
            gameLocationScreenExploration?.Hide(true);
            guiConsoleScreen.Hide(true);

            characterBuildingService.CreateNewCharacter();

            var oldHero = functorParameters.RestingHero;
            var newHero = characterBuildingService.HeroCharacter;

            DropSpellbooksIfRequired(oldHero);

            yield return StartCharacterCreationWizard();

            if (IsRespecing)
            {
                FinalizeRespec(oldHero, newHero);
            }
            else
            {
                PickupSpellbooksIfRequired(oldHero);
            }

            if (gameCampaignScreenVisible == true)
            {
                gameCampaignScreen.Show(true);
            }

            if (gameLocationscreenExplorationVisible == true)
            {
                gameLocationScreenExploration.Show(true);
            }

            if (guiConsoleScreenVisible == true)
            {
                guiConsoleScreen.Show(true);
            }

            StopRespec();

            yield break;
        }

        internal static IEnumerator StartCharacterCreationWizard()
        {
            var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();

            restModalScreen.KeepCurrentstate = true;
            restModalScreen.Hide(true);
            characterCreationScreen.OriginScreen = restModalScreen;
            characterCreationScreen.Show();

            while (characterCreationScreen.isActiveAndEnabled) yield return null;
        }

        internal static void FinalizeRespec(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
        {

                var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;
                var worldLocationEntityFactoryService = ServiceRepository.GetService<IWorldLocationEntityFactoryService>();
                var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
                var gameLocationCharacter = gameLocationCharacterService?.PartyCharacters.Find(x => x.RulesetCharacter == oldHero);
                var gameSerializationService = ServiceRepository.GetService<IGameSerializationService>();

                oldHero.Unregister();
                oldHero.ResetForOutgame();

                CopyInventoryOver(oldHero, newHero);

                gameCampaignCharacters.Find(x => x.RulesetCharacter == oldHero).RulesetCharacter = newHero;
                gameLocationCharacter?.SetRuleset(newHero);

                newHero.Attributes[AttributeDefinitions.Experience] = oldHero.GetAttribute(AttributeDefinitions.Experience);
                newHero.Register(true);

                UpdateRestPanelUi();

                if (gameLocationCharacter != null)
                {
                    // TODO: need to find a way to refresh the hero model instead of this workaround
                    Gui.GuiService.ShowAlert("Please save / reload your game to refresh the hero model.", "EA7171", 4);
                }

        }
        internal static void CopyInventoryOver(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
        {
            foreach (var inventorySlot in oldHero.CharacterInventory.PersonalContainer.InventorySlots)
            {
                if (inventorySlot.EquipedItem != null)
                {
                    var equipedItem = inventorySlot.EquipedItem;

                    equipedItem.AttunedToCharacter = string.Empty;
                    oldHero.CharacterInventory.DropItem(equipedItem);
                    newHero.GrantItem(equipedItem.ItemDefinition, equipedItem.ItemDefinition.ForceEquip, equipedItem.StackCount);
                }
            }

            foreach (var inventorySlot in oldHero.CharacterInventory.InventorySlotsByName)
            {
                if (inventorySlot.Value.EquipedItem != null)
                {
                    var equipedItem = inventorySlot.Value.EquipedItem;

                    equipedItem.AttunedToCharacter = string.Empty;
                    oldHero.CharacterInventory.DropItem(inventorySlot.Value.EquipedItem);
                    newHero.GrantItem(equipedItem.ItemDefinition, equipedItem.ItemDefinition.ForceEquip, equipedItem.StackCount);
                }
            }
        }

        internal static void UpdateRestPanelUi()
        {
            var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();
            var restAfterPanel = AccessTools.Field(restModalScreen.GetType(), "restAfterPanel").GetValue(restModalScreen) as RestAfterPanel;
            var characterPlatesTable = AccessTools.Field(restAfterPanel.GetType(), "characterPlatesTable").GetValue(restAfterPanel) as RectTransform;

            for (int index = 0; index < characterPlatesTable.childCount; ++index)
            {
                Transform child = characterPlatesTable.GetChild(index);
                CharacterPlateGame component = child.GetComponent<CharacterPlateGame>();

                component.Unbind();

                if (index < gameCampaignCharacters.Count)
                {
                    component.Bind(gameCampaignCharacters[index].RulesetCharacter, TooltipDefinitions.AnchorMode.BOTTOM_CENTER);
                    component.Refresh();
                }

                child.gameObject.SetActive(index < gameCampaignCharacters.Count);
            }
        }
    }
}
