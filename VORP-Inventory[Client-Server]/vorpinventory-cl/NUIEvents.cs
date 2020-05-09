﻿using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vorpinventory_cl
{
    public class NUIEvents : BaseScript
    {
        public static bool InInventory = false;

        public static List<Dictionary<string, dynamic>> gg = new List<Dictionary<string, dynamic>>();
        public static Dictionary<string, object> items = new Dictionary<string, object>();
       

        public NUIEvents()
        {
            Tick += OnKey;

            API.RegisterNuiCallbackType("NUIFocusOff");
            EventHandlers["__cfx_nui:NUIFocusOff"] += new Action<ExpandoObject>(NUIFocusOff);

            API.RegisterNuiCallbackType("DropItem");
            EventHandlers["__cfx_nui:DropItem"] += new Action<ExpandoObject>(NUIDropItem);

            API.RegisterNuiCallbackType("UseItem");
            EventHandlers["__cfx_nui:UseItem"] += new Action<ExpandoObject>(NUIUseItem);

            API.RegisterNuiCallbackType("sound");
            EventHandlers["__cfx_nui:sound"] += new Action<ExpandoObject>(NUISound);

            API.RegisterNuiCallbackType("GiveItem");
            EventHandlers["__cfx_nui:GiveItem"] += new Action<ExpandoObject>(NUIGiveItem);

            API.RegisterNuiCallbackType("GetNearPlayers");
            EventHandlers["__cfx_nui:GetNearPlayers"] += new Action<ExpandoObject>(NUIGetNearPlayers);
        }

        private void NUIGetNearPlayers(ExpandoObject obj)
        {
            int playerPed = API.PlayerPedId();
            List<int> players = Utils.getNearestPlayers();
            bool foundPlayers = false;
            List<Dictionary<string,object>> elements = new List<Dictionary<string, object>>();
            Dictionary<string,object> nuireturn = new Dictionary<string, object>();
            foreach (var player in players)
            {
                foundPlayers = true;
                elements.Add(new Dictionary<string, object>
                {
                    ["label"] = API.GetPlayerName(player),
                    ["player"] = API.GetPlayerServerId(player)
                });
            }

            if (!foundPlayers)
            {
                Debug.WriteLine("No near players");
            }
            else
            {
                Dictionary<string,object> item = new Dictionary<string, object>();
                foreach (var thing in obj)
                {
                    item.Add(thing.Key,thing.Value);
                    Debug.WriteLine(item[thing.Key].ToString());
                }
                nuireturn.Add("action","nearPlayers");
                nuireturn.Add("foundAny",foundPlayers);
                nuireturn.Add("players",elements);
                nuireturn.Add("item",item["item"]);
                nuireturn.Add("hash","hash");
                nuireturn.Add("count",item["count"]);
                nuireturn.Add("type",item["type"]);
                nuireturn.Add("what",item["what"]);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(nuireturn);
                Debug.WriteLine(json);
                API.SendNuiMessage(json);
            }
        }

        private void NUIGiveItem(ExpandoObject obj)
        {
            int playerPed = API.PlayerPedId();
            List<int> players = Utils.getNearestPlayers();
            Dictionary<string, object> data = Utils.expandoProcessing(obj);
            Dictionary<string, object> data2 = Utils.expandoProcessing(data["data"]);
            foreach (var varia in players)
            {
                if (varia != API.PlayerId())
                {
                    if (API.GetPlayerServerId(varia) == int.Parse(data["player"].ToString()))
                    {
                        string itemname = data2["item"].ToString();
                        int amount = int.Parse(data2["count"].ToString());
                        int target = int.Parse(data["player"].ToString());
                        if (amount > 0 && vorp_inventoryClient.useritems[itemname].getCount() >= amount)
                        {
                            TriggerServerEvent("vorpinventory:serverGiveItem",itemname,amount,target,1);
                            vorp_inventoryClient.useritems[itemname].quitCount(amount);
                            if (vorp_inventoryClient.useritems[itemname].getCount() == 0)
                            {
                                vorp_inventoryClient.useritems.Remove(itemname);
                            }
                        }
                        LoadInv();
                    }
                }
            }
        }
        
        private void NUIUseItem(ExpandoObject obj)
        {
            if (Utils.expandoProcessing(obj)["type"] == "item_standard")
            {
                TriggerServerEvent("vorpinventory:useItem",Utils.expandoProcessing(obj)["item"]);
            }
        }

        private void NUIDropItem(ExpandoObject obj)
        {
            Dictionary<string, dynamic> aux = Utils.expandoProcessing(obj);
            string itemname = aux["item"];
            string type = aux["type"].ToString();
            int cuantity = int.Parse(aux["number"].ToString());
            Debug.WriteLine(type);
            Debug.WriteLine(cuantity.ToString());
            if (type == "item_standard")
            {
                if (cuantity > 0 && vorp_inventoryClient.useritems[itemname].getCount() >= cuantity)
                {
                    TriggerServerEvent("vorpinventory:serverDropItem", itemname, cuantity, 1);
                    vorp_inventoryClient.useritems[itemname].quitCount(cuantity);
                    Debug.Write(vorp_inventoryClient.useritems[itemname].getCount().ToString());
                    if (vorp_inventoryClient.useritems[itemname].getCount() == 0)
                    {
                        vorp_inventoryClient.useritems.Remove(itemname);
                    }
                    
                }
            }
            LoadInv();
        }

        private void NUISound(ExpandoObject obj)
        {
            API.PlaySoundFrontend("BACK", "RDRO_Character_Creator_Sounds", true, 0);
        }

        private void NUIFocusOff(ExpandoObject obj)
        {
            CloseInv();
        }

        private async Task OnKey()
        {
            if (API.IsControlJustReleased(1, 0x4CC0E2FE) && API.IsInputDisabled(0))
            {
                if (InInventory)
                {
                    await CloseInv();
                }
                else
                {
                    await OpenInv();
                }
            }

        }

        public static async Task LoadInv()
        {
            Dictionary<string, dynamic> item;
            items.Clear();
            gg.Clear();
            foreach (KeyValuePair<string,ItemClass> userit in vorp_inventoryClient.useritems)
            {
                item = new Dictionary<string, dynamic>();
                item.Add("count", userit.Value.getCount());
                item.Add("limit", userit.Value.getLimit());
                item.Add("label", userit.Value.getLabel());
                item.Add("name", userit.Value.getName());
                item.Add("type", userit.Value.getType());
                item.Add("usable", userit.Value.getUsable());
                item.Add("canRemove", userit.Value.getCanRemove());
                gg.Add(item);
            }
            items.Add("action", "setItems");
            items.Add("itemList", gg);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(items);

            Debug.WriteLine(json);

            API.SendNuiMessage(json);

            Newtonsoft.Json.Linq.JObject wp = Newtonsoft.Json.Linq.JObject.Parse(json);
        }

        private async Task OpenInv()
        {
            API.SetNuiFocus(true, true);

            API.SendNuiMessage("{\"action\": \"display\"}");
            InInventory = true;

            LoadInv();

        }

        private async Task CloseInv()
        {
            API.SetNuiFocus(false, false);
            API.SendNuiMessage("{\"action\": \"hide\"}");
            InInventory = false;
        }

    }
}