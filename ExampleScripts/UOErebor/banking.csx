#load "Specs.csx"

using System;
using Infusion.Commands;

public static class Banking
{
    public static void OpenBankViaBanker(string bankerName = null)
    {
        Gump gump;
    
        int failedCount = 0;
    
        do
        {
            if (!string.IsNullOrEmpty(bankerName))
                UO.Say(bankerName + " hi");
            else
                UO.Say("hi");
                
            gump = UO.WaitForGump(TimeSpan.FromSeconds(10));
            if (gump == null)
            {
                failedCount++;
                if (failedCount > 5)
                    UO.Alert("Cannot open bank");
            }
        } while (gump == null);
        
        UO.Wait(1000);
    
        UO.SelectGumpButton("Bankovni sluzby", GumpLabelPosition.After);
        UO.WaitForGump();
    
        UO.Wait(1000);
        UO.SelectGumpButton("Otevrit banku.", GumpLabelPosition.After);
    }
    
    public static void OpenBankViaHouseMenu(string houseMenuEquip = null)
    {
        if (string.IsNullOrEmpty(houseMenuEquip))
        {        
            var menu = UO.Items.Matching(Specs.HouseMenu).OrderByDistance().FirstOrDefault();

            if (menu != null)
                UO.Use(menu);
            else
                throw new CommandInvocationException("Cannot find HouseMenu");
        }
        else
            UO.Say(houseMenuEquip);
        
        UO.WaitForGump();
        UO.SelectGumpButton("Otevrit banku", GumpLabelPosition.Before);
    }
}