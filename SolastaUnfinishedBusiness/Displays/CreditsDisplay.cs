﻿using System.Collections.Generic;
using System.IO;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using UnityExplorer;
using static SolastaUnfinishedBusiness.Displays.PatchesDisplay;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CreditsDisplay
{
    private static bool _displayPatches;

    // ReSharper disable once MemberCanBePrivate.Global
    internal static readonly List<(string, string)> CreditsTable = new()
    {
        ("Zappastuff",
            "maintenance, mod UI, infrastructure, gameplay, rules, quality of life, feats, fighting styles, invocations, metamagic, spells, " +
            "Fairy, Half-elf, Roguish Acrobat, Roguish Arcane Scoundrel, Roguish Duelist, Roguish Slayer, College of Guts, College of Life, " +
            "Circle of the Ancient Forest, Circle of the Eternal Grove, Wizard Bladedancer, Wizard Deadmaster, Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Sorr-Akkath, " +
            "Oath of Dread, Oath of Hatred, Path of the Elements, Path of the Reaver, Path of the Savagery, Ranger Hellwalker, Ranger Lightbearer, Ranger Survivalist, Ranger Wildmaster, " +
            "Martial Royal Knight, Martial Weapon Master, Way of the Discordance, Way of the Silhouette, Way of the Tempest, Innovation Artillerist, Multiclass"),
        ("TPABOBAP",
            "custom behaviors, game UI, infrastructure, gameplay, rules, quality of life, feats, fighting styles, invocations, metamagic, spells, " +
            "quality of life, Patron Elementalist, Patron Moonlit, Patron Riftwalker, Patron Soulblade, Martial Tactician, Way of Distant Hand, " +
            "Innovation Armor, Innovation Grenadier, Innovation Weapon, Inventor, Multiclass"),
        ("ImpPhil", "api, builders, gameplay, rules, quality of life"),
        ("ChrisJohnDigital",
            "builders, gameplay, feats, fighting styles, Wizard Arcane Fighter, Wizard Spellmaster, Martial Spell Shield"),
        ("HiddenHax",
            "homebrew content design [Circle of the Eternal Grove, Path of the Elemental Fury, Path of the Reaver, Path of the Savagery, Oath of Dread, Roguish Arcane Scoundrel, Roguish Duelist, Roguish Slayer, Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Sorr-Akkath, Martial Weapon Master, Way of the Discordance, Way of the Dragon, Way of the Tempest]"),
        ("Haxermn", "spells, Domain Defiler, Domain Smith, Oath of Ancient, Oath of Hatred, Way of Dragon"),
        ("Nd", "College of Harlequin, College of Wardancer, Martial Marshal, Roguish Opportunist, Roguish Raven"),
        ("SilverGriffon", "gameplay, visuals, spells, Dark Elf, Draconic Kobold, Grey Dwarf, Sorcerous Divine Heart"),
        ("tivie", "Circle of the Night, Path of the Spirits"),
        ("ElAntonius", "feats, fighting styles, Ranger Arcanist"),
        ("RedOrca", "Path of the Light"),
        ("DreadMaker", "Circle of the Forest Guardian"),
        ("Bazou", "fighting styles, rules, spells"),
        ("Holic75", "spells, Bolgrif"),
        ("Taco",
            "homebrew content design [Roguish Acrobat, Defiler Domain, Oath of Altruism], fighting styles, races, subclasses, powers, weapons, favored terrain and preferred enemy icons"),
        ("DubhHerder", "quality of life, spells, homebrew content design [Elementalist, Moonlit, Riftwalker]"),
        ("Stuffies12", "homebrew content design [Ranger Hellwalker, Ranger Lightbearer]"),
        ("Earendil", "homebrew content design [Ranger Survivalist]"),
        ("Narria", "modKit, mod UI improvements, Party Editor"),
        ("Pikachar2", "spells")
    };

    private static readonly bool IsUnityExplorerInstalled =
        File.Exists(Path.Combine(Main.ModFolder, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.ModFolder, "UniverseLib.Mono.dll"));

    private static bool IsUnityExplorerEnabled { get; set; }

    private static void EnableUnityExplorerUi()
    {
        IsUnityExplorerEnabled = true;

        try
        {
            ExplorerStandalone.CreateInstance();
        }
        catch
        {
            // ignored
        }
    }

    internal static void DisplayCredits()
    {
        UI.Label();

        if (IsUnityExplorerInstalled && !IsUnityExplorerEnabled)
        {
            UI.ActionButton("Unity Explorer UI".Bold().Khaki(), EnableUnityExplorerUi, UI.Width((float)150));
            UI.Label();
        }

        UI.DisclosureToggle(Gui.Localize("ModUi/&Patches"), ref _displayPatches, 200);
        UI.Label();

        if (_displayPatches)
        {
            DisplayPatches();
        }
        else
        {
            // credits
            foreach (var (author, content) in CreditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(author.Orange(), UI.Width((float)150));
                    UI.Label(content, UI.Width((float)550));
                }
            }
        }

        UI.Label();
    }
}
