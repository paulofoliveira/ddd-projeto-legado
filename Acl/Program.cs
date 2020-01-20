using System;
using System.Threading;
using System.Threading.Tasks;

namespace Acl
{
    class Program
    {
        private const string LegacyConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=DDDLegacyProjects;Trusted_Connection=true;";
        private const string BubbleConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=DDDLegacyProjectsNew;Trusted_Connection=true;";

        private static readonly TimeSpan IntervalBetweenDeliverySyncs = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan IntervalBetweenProductSyncs = TimeSpan.FromHours(1);

        private static Task _deliverySyncThread;
        private static DeliverySynchronizer _deliverySynchonizer;

        private static Task _productSyncThread;
        private static ProductSynchonizer _productSynchonizer;

        private static CancellationTokenSource _cancellationTokenSource;
        static void Main(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _deliverySynchonizer = new DeliverySynchronizer(LegacyConnectionString, BubbleConnectionString);
            _deliverySyncThread = new Task(() => Sync(_deliverySynchonizer.Sync, IntervalBetweenDeliverySyncs), TaskCreationOptions.LongRunning);

            _productSynchonizer = new ProductSynchonizer();
            _productSyncThread = new Task(() => Sync(_productSynchonizer.Sync, IntervalBetweenProductSyncs), TaskCreationOptions.LongRunning);

            _deliverySyncThread.Start();
            _productSyncThread.Start();

            Console.WriteLine("[Pressione qualquer tecla para parar]");
            Console.ReadKey();

            _cancellationTokenSource.Cancel();

            _deliverySyncThread.Wait(); // Aguarda até que as threads finalizem sua execução, se houver executando algo.
            _productSyncThread.Wait();
        }

        private static async void Sync(Action doSync, TimeSpan intervalBetweenSyncs)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    doSync();
                    await Task.Delay(intervalBetweenSyncs, _cancellationTokenSource.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Log(ex);
                    throw;
                }

            }
        }

        private static void Log(Exception ex)
        {
            // Configuration for logging.
        }
    }
}
