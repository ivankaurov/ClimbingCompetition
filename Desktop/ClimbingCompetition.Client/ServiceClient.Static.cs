using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClimbingCompetition.Client
{
    partial class ServiceClient
    {
        private static ServiceClient instance = null;

        public static ServiceClient Instance
        {
            get
            {
                return ServiceClient.instance;
            }
        }

        public static ServiceClient CreateTemporary(string serviceUri, string compPassword)
        {
            if (string.IsNullOrEmpty("serviceUri"))
                throw new ArgumentNullException("serviceUri");
            if (string.IsNullOrEmpty("compPassword"))
                throw new ArgumentNullException("compPassword");

            Uri baseUri;
            if (!Uri.TryCreate(serviceUri, UriKind.Absolute, out baseUri))
                throw new ArgumentException("Invalid Uri");
            return new ServiceClient
            {
                baseUri = baseUri,
                password = compPassword
            };
        }

        public static ServiceClient GetInstance(SqlConnection cn)
        {
            var inst = ServiceClient.Instance;
            if (inst != null)
                return inst;
            instance = new ServiceClient();
            instance.Load(cn);
            return instance;
        }

        public static bool HasConnection
        {
            get
            {
                return instance == null ? false : instance.ConnectionSet;
            }
        }

    }
}