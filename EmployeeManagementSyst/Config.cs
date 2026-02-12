using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSyst
{
    class Config
    {
        public IConfigurationRoot Builder { get; }

        public Config()
        {
            Builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Config>()
                .Build();

        }

        public string MasterConn => Builder["MasterConn"] ?? throw new Exception("MasterConn missing!");
        public string AppConn => Builder["AppConn"] ?? throw new Exception("AppConn missing!");

    }
}
