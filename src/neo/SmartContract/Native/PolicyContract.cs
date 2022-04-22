// Copyright (C) 2015-2021 The Neo Project.
// 
// The neo is free software distributed under the MIT software license, 
// see the accompanying file LICENSE in the main directory of the
// project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable IDE0051

using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Native
{
    /// <summary>
    /// A native contract that manages the system policies.
    /// </summary>
    public sealed class PolicyContract : NativeContract
    {
        /// <summary>
        /// The default execution fee factor.
        /// </summary>
        public const uint DefaultExecFeeFactor = 30;

        /// <summary>
        /// The default storage price.
        /// </summary>
        public const uint DefaultStoragePrice = 100000;

        /// <summary>
        /// The default network fee per byte of transactions.
        /// </summary>
        public const uint DefaultFeePerByte = 1000;

        /// <summary>
        /// The maximum execution fee factor that the committee can set.
        /// </summary>
        public const uint MaxExecFeeFactor = 100;

        /// <summary>
        /// The maximum storage price that the committee can set.
        /// </summary>
        public const uint MaxStoragePrice = 10000000;

        private const byte Prefix_Node = 13;
        private const byte Prefix_UnrestrictedAccount = 14;
        private const byte Prefix_BlockedAccount = 15;
        private const byte Prefix_FeePerByte = 10;
        private const byte Prefix_ExecFeeFactor = 18;
        private const byte Prefix_StoragePrice = 19;

        internal PolicyContract()
        {
        }

        internal override ContractTask Initialize(ApplicationEngine engine)
        {
            engine.Snapshot.Add(CreateStorageKey(Prefix_FeePerByte), new StorageItem(DefaultFeePerByte));
            engine.Snapshot.Add(CreateStorageKey(Prefix_ExecFeeFactor), new StorageItem(DefaultExecFeeFactor));
            engine.Snapshot.Add(CreateStorageKey(Prefix_StoragePrice), new StorageItem(DefaultStoragePrice));
            foreach (var n in engine.ProtocolSettings.StandbyCommittee.Union(engine.ProtocolSettings.StandbyValidators))
                engine.Snapshot.Add(CreateStorageKey(Prefix_Node).Add(n), new(0));
            return ContractTask.CompletedTask;
        }

        /// <summary>
        /// Gets the network fee per transaction byte.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <returns>The network fee per transaction byte.</returns>
        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public long GetFeePerByte(DataCache snapshot)
        {
            return (long)(BigInteger)snapshot[CreateStorageKey(Prefix_FeePerByte)];
        }

        /// <summary>
        /// Gets the execution fee factor. This is a multiplier that can be adjusted by the committee to adjust the system fees for transactions.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <returns>The execution fee factor.</returns>
        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public uint GetExecFeeFactor(DataCache snapshot)
        {
            return (uint)(BigInteger)snapshot[CreateStorageKey(Prefix_ExecFeeFactor)];
        }

        /// <summary>
        /// Gets the storage price.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <returns>The storage price.</returns>
        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public uint GetStoragePrice(DataCache snapshot)
        {
            return (uint)(BigInteger)snapshot[CreateStorageKey(Prefix_StoragePrice)];
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States)]
        private void SetFeePerByte(ApplicationEngine engine, long value)
        {
            if (value < 0 || value > 1_00000000) throw new ArgumentOutOfRangeException(nameof(value));
            if (!CheckCommittee(engine)) throw new InvalidOperationException();
            engine.Snapshot.GetAndChange(CreateStorageKey(Prefix_FeePerByte)).Set(value);
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States)]
        private void SetExecFeeFactor(ApplicationEngine engine, uint value)
        {
            if (value == 0 || value > MaxExecFeeFactor) throw new ArgumentOutOfRangeException(nameof(value));
            if (!CheckCommittee(engine)) throw new InvalidOperationException();
            engine.Snapshot.GetAndChange(CreateStorageKey(Prefix_ExecFeeFactor)).Set(value);
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States)]
        private void SetStoragePrice(ApplicationEngine engine, uint value)
        {
            if (value == 0 || value > MaxStoragePrice) throw new ArgumentOutOfRangeException(nameof(value));
            if (!CheckCommittee(engine)) throw new InvalidOperationException();
            engine.Snapshot.GetAndChange(CreateStorageKey(Prefix_StoragePrice)).Set(value);
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States)]
        private bool BlockAccount(ApplicationEngine engine, UInt160 account)
        {
            if (!CheckCommittee(engine)) throw new InvalidOperationException();
            if (IsNative(account)) throw new InvalidOperationException("It's impossible to block a native contract.");
            if (RoleManagement.GetSystemAccounts(engine.Snapshot).Contains(account))
                throw new InvalidOperationException("It's impossible to block a system account.");
            var key = CreateStorageKey(Prefix_BlockedAccount).Add(account);
            if (engine.Snapshot.Contains(key)) return false;

            engine.Snapshot.Add(key, new StorageItem(Array.Empty<byte>()));
            return true;
        }

        /// <summary>
        /// Determines whether the specified account is blocked.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <param name="account">The account to be checked.</param>
        /// <returns><see langword="true"/> if the account is blocked; otherwise, <see langword="false"/>.</returns>
        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public bool IsBlocked(DataCache snapshot, UInt160 account)
        {
            return snapshot.Contains(CreateStorageKey(Prefix_BlockedAccount).Add(account));
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States)]
        private bool UnblockAccount(ApplicationEngine engine, UInt160 account)
        {
            if (!CheckCommittee(engine)) throw new InvalidOperationException();

            var key = CreateStorageKey(Prefix_BlockedAccount).Add(account);
            if (!engine.Snapshot.Contains(key)) return false;

            engine.Snapshot.Delete(key);
            return true;
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States | CallFlags.AllowNotify)]
        public bool UnrestrictAccount(ApplicationEngine engine, UInt160 account)
        {
            if (!CheckCommittee(engine))
                throw new InvalidOperationException(nameof(UnrestrictAccount) + " permission denied");
            if (IsNative(account))
                throw new InvalidOperationException("It's impossible to unrestrict a native contract.");
            if (RoleManagement.GetAdminAccounts(engine.Snapshot).Contains(account))
                throw new InvalidOperationException("admin accounts are already unrestricted");
            var key = CreateStorageKey(Prefix_UnrestrictedAccount).Add(account);
            if (engine.Snapshot.Contains(key)) return false;
            var height = Ledger.CurrentIndex(engine.Snapshot) ?? 0;
            engine.Snapshot.Add(key, new(height));
            return true;
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States)]
        private bool RestrictAccount(ApplicationEngine engine, UInt160 account)
        {
            if (!CheckCommittee(engine))
                throw new InvalidOperationException(nameof(UnrestrictAccount) + " permission denied");
            if (IsNative(account))
                throw new InvalidOperationException("It's impossible to restrict a native contract.");
            if (RoleManagement.GetAdminAccounts(engine.Snapshot).Contains(account))
                throw new InvalidOperationException("It's impossible to restrict an admin account.");
            var key = CreateStorageKey(Prefix_UnrestrictedAccount).Add(account);
            if (!engine.Snapshot.Contains(key)) return false;
            engine.Snapshot.Delete(key);
            return true;
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public bool IsRestricted(DataCache snapshot, UInt160 account)
        {
            if (RoleManagement.GetAdminAccounts(snapshot).Contains(account))
                return false;
            var key = CreateStorageKey(Prefix_UnrestrictedAccount).Add(account);
            return !snapshot.Contains(key);
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States | CallFlags.AllowNotify)]
        public void AllowNode(ApplicationEngine engine, ECPoint node)
        {
            if (!CheckCommittee(engine))
                throw new InvalidOperationException(nameof(AllowNode) + " permission denied");
            var key = CreateStorageKey(Prefix_Node).Add(node);
            if (engine.Snapshot.Contains(key)) return;
            var height = Ledger.CurrentIndex(engine.Snapshot) ?? 0;
            engine.Snapshot.Add(key, new(height));
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States | CallFlags.AllowNotify)]
        public bool UnallowNode(ApplicationEngine engine, ECPoint node)
        {
            if (!CheckCommittee(engine))
                throw new InvalidOperationException(nameof(UnallowNode) + " permission denied");
            var key = CreateStorageKey(Prefix_Node).Add(node);
            if (RoleManagement.GetSystemKeys(engine.Snapshot).Contains(node))
                throw new InvalidOperationException("Could not unallow a system node");
            engine.Snapshot.Delete(key);
            return true;
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public bool IsAllowed(DataCache snapshot, ECPoint node)
        {
            var key = CreateStorageKey(Prefix_Node).Add(node);
            return snapshot.Contains(key);
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public async void GasMint(ApplicationEngine engine, BigInteger amount)
        {
            if (!CheckCommittee(engine))
                throw new InvalidOperationException(nameof(GasMint) + " permission denied");
            await GAS.Mint(engine, RoleManagement.GetCommitteeAddress(engine.Snapshot, engine.PersistingBlock.Index), amount, false);
        }

        public List<ECPoint> NodesAllowed(DataCache snapshot)
        {
            var nodes = new List<ECPoint>();
            byte[] prefix = CreateStorageKey(Prefix_Node).ToArray();
            foreach (var (key, _) in snapshot.Find(prefix))
            {
                if (key.Key[0] == Prefix_Node)
                    nodes.Add(key.Key[1..].AsSerializable<ECPoint>());
                else
                    break;
            }
            return nodes;
        }

        public bool TransferAllowed(ApplicationEngine engine, UInt160 token, UInt160 from, UInt160 to)
        {
            if (IsBlocked(engine.Snapshot, token) || IsBlocked(engine.Snapshot, from)) return false;
            var systemAccounts = RoleManagement.GetAdminAccounts(engine.Snapshot);
            if (systemAccounts.Contains(from)
                || systemAccounts.Contains(to)
                || !IsRestricted(engine.Snapshot, from))
                return true;
            return false;
        }
    }
}
