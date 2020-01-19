using Dapper;
using PackageDeliveryNew.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace PackageDeliveryNew.Deliveries
{
    public class DeliveryRepository
    {
        public Delivery GetById(int id)
        {
            (DeliveryData deliveryData, List<ProductLineData> linesData) = GetRawData(id);
            var delivery = MapData(deliveryData, linesData);

            return delivery;
        }

        private Delivery MapData(DeliveryData deliveryData, List<ProductLineData> linesData)
        {
            var lines = linesData.Select(p => new ProductLine(new Product(p.ProductID, p.ProductName, p.ProductWeightInPounds), p.Amount)).ToList();

            var address = new Address(deliveryData.DestinationStreet, deliveryData.DestinationCity, deliveryData.DestinationState, deliveryData.DestinationZipCode);
            return new Delivery(deliveryData.DeliveryID, address, deliveryData.CostEstimate, lines);
        }

        private (DeliveryData deliveryData, List<ProductLineData> linesData) GetRawData(int id)
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                var sql = @"SELECT * FROM [dbo].[Delivery] d WHERE d.[DeliveryID] = @id
                            
                            SELECT l.*, p.[WeightInPounds] AS [ProductWeightInPounds], p.[Name] AS [ProductName] FROM [dbo].[ProductLine] l 
                            INNER JOIN [dbo].[Product] p ON (l.[ProductID] = p.[ProductID])
                            WHERE [DeliveryID] = @id";

                var reader = conn.QueryMultiple(sql, new { id });

                var deliveryData = reader.ReadFirst<DeliveryData>();
                var linesData = reader.Read<ProductLineData>().ToList();

                return (deliveryData, linesData);
            }
        }

        public void Save(Delivery delivery)
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                var sql = @"UPDATE [dbo].[Delivery] SET [CostEstimate] = @CostEstimate WHERE [DeliveryID] = @Id

                            DELETE FROM [dbo].[ProductLine] WHERE [DeliveryID] = @Id";

                conn.Execute(sql, new { delivery.Id, delivery.CostEstimate });

                var sql2 = @"INSERT INTO [dbo].[ProductLine] ([ProductID], [Amount], [DeliveryID]) VALUES (@ProductID, @Amount, @DeliveryID)";

                conn.Execute(sql2, delivery.Lines.Select(p => new { ProductID = p.Product.Id, p.Amount, DeliveryID = delivery.Id }));

                // Repare que estamos passando uma coleção para o Dapper. Ele irá interagir na coleção.
            }
        }

        private class DeliveryData
        {
            public int DeliveryID { get; set; }
            public decimal? CostEstimate { get; set; }
            public string DestinationStreet { get; set; }
            public string DestinationCity { get; set; }
            public string DestinationState { get; set; }
            public string DestinationZipCode { get; set; }
        }

        private class ProductLineData
        {
            public int ProductID { get; set; }
            public int Amount { get; set; }
            public double ProductWeightInPounds { get; set; }
            public string ProductName { get; set; }
        }
    }
}
