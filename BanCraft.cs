using Oxide.Core;
using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Ban Craft", "Oryx", "1.0.0")]
    [Description("Blacklist items from crafting")]

    public class BanCraft : RustPlugin
    {
        #region Fields
        private readonly string perm1 = "bancraft.bypass";
        private readonly string perm2 = "bancraft.banall";

        private List<string> blacklist = new List<string>();
        #endregion

        #region UMod Hooks
        bool CanCraft(ItemCrafter itemCrafter, ItemBlueprint bp, int amount)
        {
            var player = itemCrafter.GetComponent<BasePlayer>();
            var item = bp.GetComponent<ItemDefinition>();

            if(player == null || item == null)
            {
                return false;
            }

            if (permission.UserHasPermission(player.UserIDString, perm1))
            {
                return true;
            }

            if(permission.UserHasPermission(player.UserIDString, perm2))
            {
                SendReply(player, "You are not allowed to craft at all");
                return false;
            }

            if (blacklist.Contains(item.shortname))
            {
                SendReply(player, "You are not allowed to craft <color=#eb4034>" + item.shortname + "</color>");
                return false;
            }

            return true;
        }

        private void Init()
        {
            permission.RegisterPermission(perm1, this);
            permission.RegisterPermission(perm2, this);

            ReadData();
        }
        #endregion

        #region Stored Data
        private void ReadData()
        {
            try
            {
                blacklist = Interface.Oxide.DataFileSystem.ReadObject<List<string>>("BanCraft");

                if(blacklist == null || blacklist.Count == 0)
                {
                    throw new Exception();
                }
            }
            catch
            {
                PrintWarning("Creating new saved data file");
                blacklist = new List<string>() { "rock", "torch" };
                SaveData();
            }
        }

        private void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject("BanCraft", blacklist);
        }
        #endregion
    }
}
