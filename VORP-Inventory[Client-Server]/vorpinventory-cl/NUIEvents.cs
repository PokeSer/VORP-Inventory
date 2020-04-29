using CitizenFX.Core;
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
            int PID = API.PlayerPedId();
           
        }

        private void NUIGiveItem(ExpandoObject obj)
        {
            foreach (var o in obj)
            {
                Debug.WriteLine(o.Value.ToString());
            }
        }

        private void NUIUseItem(ExpandoObject obj)
        {
            foreach (var o in obj)
            {
                Debug.WriteLine(o.Value.ToString());
            }
        }

        private void NUIDropItem(ExpandoObject obj)
        {
            foreach (var o in obj)
            {
                Debug.WriteLine(o.Value.ToString());
            }
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

        private async Task LoadInv()
        {

            Dictionary<string, dynamic> item = new Dictionary<string, dynamic>();
            item.Add("count", 10);
            item.Add("limit", 64);
            item.Add("label", "Test");
            item.Add("name", "water");
            item.Add("type", "item_standard");
            item.Add("usable", false);
            item.Add("canRemove", true);

            items.Add("action", "setItems");
            gg.Add(item);
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
