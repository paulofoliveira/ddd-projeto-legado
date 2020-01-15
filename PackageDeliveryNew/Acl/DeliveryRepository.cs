using Dapper;
using PackageDeliveryNew.Deliveries;
using PackageDeliveryNew.Utils;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace PackageDeliveryNew.Acl
{
    public class DeliveryRepository
    {
        public Delivery GetById(int id)
        {
            var legacy = GetDeliveryLegacy(id);
            var delivery = MapDeliveryLegacy(legacy);

            return delivery;
        }

        private Delivery MapDeliveryLegacy(DeliveryLegacy legacy)
        {
            if (string.IsNullOrEmpty(legacy.CT_ST) || !legacy.CT_ST.Contains(' '))
                throw new Exception("Invalid city and state");

            var cityAndState = legacy.CT_ST.Split(' ');

            var address = new Address((legacy.STR ?? string.Empty).Trim(), cityAndState[0].Trim(), cityAndState[1].Trim(), (legacy.ZP ?? string.Empty).Trim());

            return new Delivery(legacy.NMB_CM, address);
        }

        private DeliveryLegacy GetDeliveryLegacy(int id)
        {
            var sql = @"SELECT d.[NMB_CLM], a.* FROM [dbo].[DLVR_TBL] d INNER JOIN [dbo].[ADDR_TBL] a ON (a.[DLVR = d.[NMB_CLM]) WHERE d.[NMB_CLM] = @id;";

            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                return conn.Query<DeliveryLegacy>(sql, new { id }).FirstOrDefault();
            }

        }
        internal class DeliveryLegacy
        {
            public int NMB_CM { get; set; }
            public string STR { get; set; }
            public string CT_ST { get; set; }
            public string ZP { get; set; }
        }
    }
}
