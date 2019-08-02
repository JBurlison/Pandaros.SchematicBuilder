using Chatting;
using Pandaros.API;
using Pandaros.API.Entities;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pipliz.JSON;
using System;
using System.Collections.Generic;

namespace Pandaros.SchematicBuilder
{
    [ModLoader.ModManager]
    public class SchematicBuilderChatCommand : IChatCommand
    {
        private static string _Setters = GameLoader.NAMESPACE + ".SchematicBuilder";
        private static LocalizationHelper _localizationHelper = new LocalizationHelper(GameLoader.NAMESPACE, "SchematicBuilder");

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnConstructWorldSettingsUI, GameLoader.NAMESPACE + "SchematicBuilder.AddSetting")]
        public static void AddSetting(Players.Player player, NetworkUI.NetworkMenu menu)
        {
            if (player.ActiveColony != null)
            {
                menu.Items.Add(new NetworkUI.Items.DropDown("Random SchematicBuilder", _Setters, new List<string>() { "Prompt", "Always Accept", "Disabled" }));
                var ps = ColonyState.GetColonyState(player.ActiveColony);
                menu.LocalStorage.SetAs(_Setters, Convert.ToInt32(ps.SchematicBuilderEnabled));
            }
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerChangedNetworkUIStorage, GameLoader.NAMESPACE + "SchematicBuilder.ChangedSetting")]
        public static void ChangedSetting(ValueTuple<Players.Player, JSONNode, string> data)
        {
            if (data.Item1.ActiveColony != null)
                switch (data.Item3)
                {
                    case "server_popup":
                        var cs = ColonyState.GetColonyState(data.Item1.ActiveColony);
                        var maxToggleTimes = SchematicBuilderConfiguration.GetorDefault("MaxSchematicBuilderToggle", 4);

                        if (cs != null)
                        {
                            if (!SchematicBuilderConfiguration.GetorDefault("SchematicBuilderEnabled", true))
                                PandaChat.Send(data.Item1, _localizationHelper, "AdminDisabledSchematicBuilder", ChatColor.red);
                            else if (!HasToggeledMaxTimes(maxToggleTimes, cs, data.Item1))
                            {
                                var def = (int)cs.SchematicBuilderEnabled;
                                var enabled = data.Item2.GetAsOrDefault(_Setters, def);

                                if (def != enabled)
                                {
                                    TurnSchematicBuilderOn(data.Item1, cs, maxToggleTimes, (SchematicBuilderState)enabled);
                                    PandaChat.Send(data.Item1, _localizationHelper, "SchematicBuilderState", ChatColor.green, cs.SchematicBuilderEnabled.ToString());
                                }
                            }
                        }

                        break;
                }
        }

        public bool TryDoCommand(Players.Player player, string chat, List<string> split)
        {
            if (!chat.StartsWith("/SchematicBuilder", StringComparison.OrdinalIgnoreCase))
                return false;

            if (player == null || player.ID == NetworkID.Server || player.ActiveColony == null)
                return true;

            var array = new List<string>();
            CommandManager.SplitCommand(chat, array);
            var state          = ColonyState.GetColonyState(player.ActiveColony);
            var maxToggleTimes = SchematicBuilderConfiguration.GetorDefault("MaxSchematicBuilderToggle", 4);

            if (maxToggleTimes == 0 && !SchematicBuilderConfiguration.GetorDefault("SchematicBuilderEnabled", true))
            {
                PandaChat.Send(player, _localizationHelper, "AdminDisabledSchematicBuilder.", ChatColor.red);

                return true;
            }

            if (HasToggeledMaxTimes(maxToggleTimes, state, player))
                return true;

            if (array.Count == 1)
            {
                PandaChat.Send(player,
                               _localizationHelper,
                               "SchematicBuilderToggleTime",
                               ChatColor.green, 
                               state.SchematicBuilderEnabled.ToString(),
                               state.SchematicBuilderToggledTimes.ToString(),
                               maxToggleTimes.ToString());
                return true;
            }

            if (array.Count == 2 && state.SchematicBuilderToggledTimes <= maxToggleTimes && Enum.TryParse<SchematicBuilderState>(array[1].ToLower().Trim(), out var SchematicBuilderState))
            {
                TurnSchematicBuilderOn(player, state, maxToggleTimes, SchematicBuilderState);
            }

            return true;
        }

        private static bool HasToggeledMaxTimes(int maxToggleTimes, ColonyState state, Players.Player player)
        {
            if (state.SchematicBuilderToggledTimes >= maxToggleTimes)
            {
                PandaChat.Send(player, _localizationHelper, "AbuseWarning", ChatColor.red, maxToggleTimes.ToString());

                return true;
            }

            return false;
        }

        private static void TurnSchematicBuilderOn(Players.Player player, ColonyState state, int maxToggleTimes, SchematicBuilderState enabled)
        {
            if (state.SchematicBuilderEnabled == SchematicBuilderState.Disabled && enabled != SchematicBuilderState.Disabled)
                state.SchematicBuilderToggledTimes++;

            state.SchematicBuilderEnabled = enabled;

            PandaChat.Send(player,
                            _localizationHelper,
                            "SchematicBuilderToggleTime",
                            ChatColor.green,
                            state.SchematicBuilderEnabled.ToString(),
                            state.SchematicBuilderToggledTimes.ToString(),
                            maxToggleTimes.ToString());

            NetworkUI.NetworkMenuManager.SendColonySettingsUI(player);
        }
    }
}