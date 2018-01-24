﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Job.BlockchainTransactionsHistoryDetector.Contract;
using Lykke.Job.BlockchainTransactionsHistoryDetector.Core.Services.BLockchains;
using Lykke.Job.BlockchainTransactionsHistoryDetector.Workflow.Commands;
using Lykke.Service.BlockchainApi.Client;

namespace Lykke.Job.BlockchainTransactionsHistoryDetector.Workflow.PeriodicalHandlers
{
    //public class DepositWalletsBalanceProcessingPeriodicalHandler : TimerPeriod
    //{
    //    private readonly int _batchSize;
    //    private readonly string _blockchainType;
    //    private readonly IBlockchainApiClient _blockchainApiClient;
    //    private readonly ICqrsEngine _cqrsEngine;

    //    public DepositWalletsBalanceProcessingPeriodicalHandler(
    //        ILog log, 
    //        TimeSpan period, 
    //        int batchSize, 
    //        string blockchainType,
    //        IBlockchainApiClientProvider blockchainApiClientProvider,
    //        ICqrsEngine cqrsEngine) :

    //        base(
    //            nameof(DepositWalletsBalanceProcessingPeriodicalHandler), 
    //            (int)period.TotalMilliseconds, 
    //            log.CreateComponentScope($"{nameof(DepositWalletsBalanceProcessingPeriodicalHandler)} : {blockchainType}"))
    //    {
    //        _batchSize = batchSize;
    //        _blockchainType = blockchainType;
    //        _blockchainApiClient = blockchainApiClientProvider.Get(blockchainType);
    //        _cqrsEngine = cqrsEngine;
    //    }

    //    public override async Task Execute()
    //    {
    //        var stopwatch = Stopwatch.StartNew();
    //        var wallets = new HashSet<string>();

    //        var blockchainAssets = await _blockchainApiClient.GetAllAssetsAsync(_batchSize);
            
    //        var statistics = await _blockchainApiClient.EnumerateWalletBalanceBatchesAsync(
    //            _batchSize,
    //            assetId =>
    //            {
    //                if (!blockchainAssets.TryGetValue(assetId, out var asset))
    //                {
    //                    // Unknown asset, tries to refresh cached assets

    //                    blockchainAssets = _blockchainApiClient
    //                        .GetAllAssetsAsync(_batchSize)
    //                        .GetAwaiter()
    //                        .GetResult();

    //                    if (!blockchainAssets.TryGetValue(assetId, out asset))
    //                    {
    //                        throw new InvalidOperationException($"Asset {assetId} not found");
    //                    }
    //                }

    //                return asset.Accuracy;
    //            },
    //            batch =>
    //            {
    //                foreach (var balance in batch)
    //                {
    //                    // Balance on the deposit wallet is detected, sends command to let the cashin saga
    //                    // detect it.

    //                    _cqrsEngine.SendCommand(
    //                        new DetectDepositBalanceCommand
    //                        {
    //                            BlockchainType = _blockchainType,
    //                            Amount = balance.Balance,
    //                            DepositWalletAddress = balance.Address,
    //                            BlockchainAssetId = balance.AssetId
    //                        },
    //                        BlockchainCashinDetectorBoundedContext.Name,
    //                        BlockchainCashinDetectorBoundedContext.Name);

    //                    wallets.Add(balance.Address);
    //                }

    //                return Task.FromResult(true);
    //            });

    //        if (statistics.ItemsCount > 0)
    //        {
    //            Log.WriteInfo(nameof(Execute), new
    //            {
    //                balancesCount = statistics.ItemsCount,
    //                walletsCount = wallets.Count,
    //                batchesCount = statistics.BatchesCount,
    //                processingElapsed = statistics.Elapsed,
    //                totalElapsed = stopwatch.Elapsed
    //            }, "Positive balance on the deposit wallets is detected");
    //        }
    //    }
    //}
}
