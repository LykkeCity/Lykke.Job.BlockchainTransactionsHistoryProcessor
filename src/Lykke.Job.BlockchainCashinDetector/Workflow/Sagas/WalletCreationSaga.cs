﻿using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Job.BlockchainTransactionsHistoryDetector.Contract;
using Lykke.Job.BlockchainTransactionsHistoryDetector.Core;
using Lykke.Job.BlockchainTransactionsHistoryDetector.Core.Domain;
using Lykke.Job.BlockchainTransactionsHistoryDetector.Workflow.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainTransactionsHistoryDetector.Workflow.Sagas
{
    public class WalletCreationSaga
    {
        /// <summary>
        /// -> DepositWalletsBalanceProcessingPeriodicalHandler : DetectDepositBalanceCommand
        /// -> DepositBalanceDetectedEvent
        ///     -> StartCashinCommand
        /// -> CashinStartedEvent
        ///     -> EnrollToMatchingEngineCommand
        /// -> CashinEnrolledToMatchingEngineEvent 
        ///     -> BlockchainOperationsExecutor : StartOperationCommand
        /// -> BlockchainOperationsExecutor : OperationCompleted | OperationFailed
        ///     -> RemoveMatchingEngineDeduplicationLockCommand
        /// -> MatchingEngineDeduplicationLockRemovedEvent
        /// </summary>

        private static readonly string Context = Lykke.Service.BlockchainWallets.Contract.BlockchainWalletsBoundedContext.Name;

        private readonly ILog _log;
        private readonly IWalletHistoryRepository _walletHistoryRepository;

        public WalletCreationSaga(ILog log, IWalletHistoryRepository walletHistoryRepository)
        {
            _log = log.CreateComponentScope(nameof(WalletCreationSaga));
            _walletHistoryRepository = walletHistoryRepository;
        }

        [UsedImplicitly]
        private async Task Handle(Lykke.Service.BlockchainWallets.Contract.Events.WalletCreatedEvent evt, ICommandSender sender)
        {
#if DEBUG
            _log.WriteInfo(nameof(Handle), evt, "");
#endif
            try
            {
                var aggregate = await _walletHistoryRepository.GetOrAddAsync(
                    evt.IntegrationLayerId,
                    evt.Address,
                    () => WalletHistoryAggregate.CreateNew(evt.IntegrationLayerId, evt.Address, evt.AssetId, WalletAddressType.To));

                ChaosKitty.Meow(aggregate.AggregateId);

                if (aggregate.WalletHistoryState == WalletHistoryState.Started)
                {
                    sender.SendCommand(new MonitoringTransactionHistoryCommand
                    {
                        BlockchainType = aggregate.BlockchainType,
                        WalletAddress = aggregate.WalletAddress,
                        WalletAddressType = aggregate.WalletAddressType
                    }, Context);
                }
            }
            catch (Exception ex)
            {
                _log.WriteError(nameof(WalletCreationSaga), evt, ex);
                throw;
            }
        }

        [UsedImplicitly]
        private async Task Handle(Lykke.Service.BlockchainWallets.Contract.Events.WalletDeletedEvent evt, ICommandSender sender)
        {
#if DEBUG
            _log.WriteInfo(nameof(Handle), evt, "");
#endif
            try
            {
                var aggregate = await _walletHistoryRepository.TryGetAsync(evt.IntegrationLayerId, evt.Address, evt.Address);

                if (aggregate == null)
                {
                    return;
                }

                ChaosKitty.Meow(aggregate.AggregateId);

                await _walletHistoryRepository.SaveAsync(WalletHistoryAggregate.StopObservation(
                    aggregate.AggregateId, 
                    aggregate.BlockchainType,
                    aggregate.WalletAddress,
                    aggregate.AssetId,
                    WalletAddressType.Both));

            }
            catch (Exception ex)
            {
                _log.WriteError(nameof(WalletCreationSaga), evt, ex);
                throw;
            }
        }
    }
}
