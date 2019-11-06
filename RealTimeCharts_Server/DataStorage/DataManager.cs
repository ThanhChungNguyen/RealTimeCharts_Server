using AlertRealTimeServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertRealTimeServer.DataStorage
{
    public static class DataManager
    {
        public static bool HaveMessageToTransfer { get; set; } = false;
        public static AlertModel AlertModel { get; set; }
    }
}

