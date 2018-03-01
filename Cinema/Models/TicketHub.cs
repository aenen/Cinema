using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class TicketHub:Hub
    {
        [HubMethodName("NotifyClients")]
        public static void NotifyToAllClients()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TicketHub>();

            // the update client method will update the connected client about any recent changes in the server data
            context.Clients.All.updatedClients();
        }
    }
}