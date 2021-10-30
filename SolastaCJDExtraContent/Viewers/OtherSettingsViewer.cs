using UnityModManagerNet;
using ModKit;

namespace SolastaCJDExtraContent.Menus.Viewers
{
    public class OtherSettingsViewer : IMenuSelectablePage
    {
        public string Name => "Other Settings";

        public int Priority => 1;

        public static void DisplaySettings()
        {
            bool toggle;

            UI.Label("");
            UI.Label("Settings:".yellow());

            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enables both ASI and Feat instead of ASI or Feat", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                Models.AsiAndFeatContext.Switch(toggle);
            }
        }

        private static void DisplaySpellPanelSettings()
        {
            UI.Label("");
            UI.Label("Spells Panel:".yellow());

            int intValue = Main.Settings.MaxSpellLevelsPerLine;
            if (UI.Slider("Max Levels per line", ref intValue, 3, 7, 5, "", UI.AutoWidth()))
            {
                Main.Settings.MaxSpellLevelsPerLine = intValue;
            }

            float floatValue = Main.Settings.SpellPanelGapBetweenLines;
            if (UI.Slider("Gap between spell lines", ref floatValue, 0f, 200f, 50f, 0, "", UI.AutoWidth()))
            {
                Main.Settings.SpellPanelGapBetweenLines = floatValue;
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());
            UI.Div();

            DisplaySettings();
            DisplaySpellPanelSettings();
        }
    }
}