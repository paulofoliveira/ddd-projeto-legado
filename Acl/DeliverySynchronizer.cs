using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Acl
{
    internal class DeliverySynchronizer
    {
        private string _legacyConnectionString;
        private string _bubbleConnectionString;

        public DeliverySynchronizer(string legacyConnectionString, string bubbleConnectionString)
        {
            _legacyConnectionString = legacyConnectionString;
            _bubbleConnectionString = bubbleConnectionString;
        }

        public void Sync()
        {
            Console.WriteLine("Sync deliveries");

            SyncFromLegacyToBubble();
            SyncFromBubbleToLegacy();
        }

        private void SyncFromLegacyToBubble()
        {
            if (!IsSyncFromLegacyNeeded())
                return;

            var updatedDeliveries = ReadUpdatedLegacyDeliveries();
            var bubbleDeliveries = MapLegacyDeliveries(updatedDeliveries);

            SaveBubbleDeliveries(bubbleDeliveries);
        }

        private void SaveBubbleDeliveries(object bubbleDeliveries)
        {
            using (var conn = new SqlConnection(_bubbleConnectionString))
            {
                var sql = @"UPDATE [dbo].[Delivery]
                            SET [DestinationStreet] = @DestinationStreet,
                                [DestinationCity] = @DestinationCity,
                                [DestinationState] = @DestinationState,
                                [DestinationZipCode] = @DestinationZipCode
                            WHERE [DeliveryID] = @DeliveryID

                            IF (@@ROWCOUNT = 0)
                            BEGIN

	                            INSERT [dbo].[Delivery] ([DeliveryID], [DestinationStreet], [DestinationCity], [DestinationState], [DestinationZipCode])
	                            VALUES (@DeliveryID, @DestinationStreet, @DestinationCity, @DestinationState, @DestinationZipCode)
                            END";

                conn.Execute(sql, bubbleDeliveries);

                // Dapper irá executar a coleção registro a registro.
            }
        }

        private List<DeliveryBubble> MapLegacyDeliveries(List<DeliveryLegacy> updatedDeliveries) => updatedDeliveries.Select(p => MapLegacyDelivery(p)).ToList();

        private DeliveryBubble MapLegacyDelivery(DeliveryLegacy legacy)
        {
            var cityAndState = legacy.CT_ST.Split(' ');

            if (string.IsNullOrEmpty(legacy.CT_ST) || !legacy.CT_ST.Contains(' '))
                throw new Exception("Invalid city and state");

            return new DeliveryBubble()
            {
                DeliveryID = legacy.NMB_CLM,
                //CostEstimate = 0m,
                DestinationStreet = (legacy.STR ?? string.Empty).Trim(),
                DestinationCity = cityAndState[0].Trim(),
                DestinationState = cityAndState[1].Trim(),
                DestinationZipCode = (legacy.ZP ?? string.Empty).Trim()
            };
        }

        private List<DeliveryLegacy> ReadUpdatedLegacyDeliveries()
        {
            using (var conn = new SqlConnection(_legacyConnectionString))
            {
                // Seleciona todas as entregas alteradas.
                // Atualiza todos os alterados.
                // Atualiza tabela de sincronização.

                var sql = @"SELECT d.[NMB_CLM], a.[STR], a.[CT_ST], a.[ZP]
                            FROM [DLVR_TBL] d WITH (UPDLOCK)
                            INNER JOIN [ADDR_TBL] a ON (d.[NMB_CLM] = a.[DLVR])
                            WHERE d.[IsSyncNeeded] = 1

                            UPDATE [DLVR_TBL] SET [IsSyncNeeded] = 0 WHERE [IsSyncNeeded] = 1

                            UPDATE [Synchonization] SET [IsSyncRequired] = 0";

                // UPDLOCK é usado para evitar deadlocks.

                return conn.Query<DeliveryLegacy>(sql)
                        .ToList();
            }
        }

        private bool IsSyncFromLegacyNeeded()
        {
            using (var conn = new SqlConnection(_legacyConnectionString))
            {
                var sql = "SELECT [IsSyncRequired] FROM [dbo].[Synchronization]";
                return conn.Query<bool>(sql).Single();
            }
        }

        private void SyncFromBubbleToLegacy()
        {
            if (!IsSyncFromBubbleNeeded()) return;

            var bubbleDeliveries = ReadUpdateBubbleDeliveries();
            var legacyDeliveries = MapBubbleDeliveries(bubbleDeliveries);

            SaveLegacyDeliveries(legacyDeliveries);
        }

        private void SaveLegacyDeliveries(object legacyDeliveries)
        {
            using (var connection = new SqlConnection(_legacyConnectionString))
            {
                string query = @"
                    UPDATE [dbo].[DLVR_TBL]
                    SET PRD_LN_1 = @PRD_LN_1, PRD_LN_1_AMN = @PRD_LN_1_AMN, PRD_LN_2 = @PRD_LN_2, PRD_LN_2_AMN = @PRD_LN_2_AMN, ESTM_CLM = @ESTM_CLM, STS = 'R'
                    WHERE NMB_CLM = @NMB_CLM
                    IF EXISTS (SELECT TOP 1 1 FROM [dbo].[DLVR_TBL2] WHERE NMB_CLM = @NMB_CLM)
                    BEGIN
                        UPDATE [dbo].[DLVR_TBL2]
                        SET PRD_LN_3 = @PRD_LN_3, PRD_LN_3_AMN = @PRD_LN_3_AMN, PRD_LN_4 = @PRD_LN_4, PRD_LN_4_AMN = @PRD_LN_4_AMN
                        WHERE NMB_CLM = @NMB_CLM
                    END
                    ELSE
                    BEGIN
                        INSERT [dbo].[DLVR_TBL2] (NMB_CLM, PRD_LN_3, PRD_LN_3_AMN, PRD_LN_4, PRD_LN_4_AMN)
                        VALUES (@NMB_CLM, @PRD_LN_3, @PRD_LN_3_AMN, @PRD_LN_4, @PRD_LN_4_AMN)
                    END";

                connection.Execute(query, legacyDeliveries);
            }
        }

        private List<DeliveryLegacy> MapBubbleDeliveries(List<DeliveryBubble> bubbleDeliveries)
        {
            var result = new List<DeliveryLegacy>();

            foreach(var bubbleDelivery in bubbleDeliveries)
            {
                var legacyDelivery = new DeliveryLegacy
                {
                    NMB_CLM = bubbleDelivery.DeliveryID,
                    ESTM_CLM = (double?)bubbleDelivery.CostEstimate
                };

                if (bubbleDelivery.Lines.Count > 0)
                {
                    legacyDelivery.PRD_LN_1 = bubbleDelivery.Lines[0].ProductID;
                    legacyDelivery.PRD_LN_1_AMN = bubbleDelivery.Lines[0].Amount.ToString();
                }
                if (bubbleDelivery.Lines.Count > 1)
                {
                    legacyDelivery.PRD_LN_2 = bubbleDelivery.Lines[1].ProductID;
                    legacyDelivery.PRD_LN_2_AMN = bubbleDelivery.Lines[1].Amount.ToString();
                }
                if (bubbleDelivery.Lines.Count > 2)
                {
                    legacyDelivery.PRD_LN_3 = bubbleDelivery.Lines[2].ProductID;
                    legacyDelivery.PRD_LN_3_AMN = bubbleDelivery.Lines[2].Amount.ToString();
                }
                if (bubbleDelivery.Lines.Count > 3)
                {
                    legacyDelivery.PRD_LN_4 = bubbleDelivery.Lines[3].ProductID;
                    legacyDelivery.PRD_LN_4_AMN = bubbleDelivery.Lines[3].Amount.ToString();
                }

                result.Add(legacyDelivery);
            }

            return result;
        }

        private List<DeliveryBubble> ReadUpdateBubbleDeliveries()
        {
            using (var conn = new SqlConnection(_bubbleConnectionString))
            {
                var sql = @"SELECT d.[DeliveryID], d.[CostEstimate]
                                FROM [dbo].[Delivery] d WITH (UPDLOCK)
                                WHERE d.[IsSyncNeeded] = 1

                                SELECT l.* FROM [dbo].[Delivery] d
                                INNER JOIN [dbo].[ProductLine] l ON d.[DeliveryID] = l.[DeliveryID]
                                WHERE d.[IsSyncNeeded] = 1

                                UPDATE [dbo].[Delivery] SET [IsSyncNeeded] = 0 WHERE [IsSyncNeeded] = 1

                                UPDATE [dbo].[Synchronization] SET [IsSyncRequired] = 0";

                using (var reader = conn.QueryMultiple(sql))
                {
                    var deliveries = reader.Read<DeliveryBubble>().ToList();
                    var lines = reader.Read<ProductLineBubble>().ToList();

                    foreach (var delivery in deliveries)
                    {
                        delivery.Lines = lines.Where(p => p.DeliveryID == delivery.DeliveryID).ToList();
                    }

                    return deliveries;
                }
            }
        }

        private bool IsSyncFromBubbleNeeded()
        {
            using (var conn = new SqlConnection(_bubbleConnectionString))
            {
                var sql = "SELECT [IsSyncRequired] FROM [dbo].[Synchronization]";
                return conn.Query<bool>(sql).Single();
            }
        }
    }
}