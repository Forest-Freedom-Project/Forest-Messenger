using System;
using System.Net;
using System.Net.Sockets;
using DotI2p;
using Forest.Core.Models;
using MonoTorrent.Connections.Peer;
using ReusableTasks;

namespace Forest.Core.Services.Network
{
    public sealed class I2PConnection : IPeerConnection
    {
        private readonly SamVirtualStream? _virtualStream;
        private readonly TcpClient? _tcpClient;
        private readonly NetworkStream? _stream;
        private readonly I2PEndPoint _remoteEndPoint;
        private bool _disposed;

        public ReadOnlyMemory<byte> AddressBytes { get; }
        public bool CanReconnect => !IsIncoming;
        public bool Disposed => _disposed;
        public IPEndPoint? EndPoint => null;
        public bool IsIncoming { get; }
        public Uri Uri { get; }

        public I2PConnection(SamVirtualStream virtualStream, TcpClient tcpClient, I2PEndPoint remoteEndPoint)
        {
            _virtualStream = virtualStream ?? throw new ArgumentNullException(nameof(virtualStream));
            _tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            _stream = tcpClient.GetStream();
            _remoteEndPoint = remoteEndPoint ?? throw new ArgumentNullException(nameof(remoteEndPoint));
            IsIncoming = false;
            Uri = new Uri($"i2p://{_remoteEndPoint.B32Hostname}");
            AddressBytes = new byte[32];
        }

        public I2PConnection(SamVirtualStream virtualStream, TcpClient tcpClient, I2PEndPoint remoteEndPoint, I2PEndPoint localEndPoint)
        {
            _virtualStream = virtualStream ?? throw new ArgumentNullException(nameof(virtualStream));
            _tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            _stream = tcpClient.GetStream();
            _remoteEndPoint = remoteEndPoint ?? throw new ArgumentNullException(nameof(remoteEndPoint));
            IsIncoming = true;
            Uri = new Uri($"i2p://{_remoteEndPoint.B32Hostname}");
            AddressBytes = new byte[32];
        }

        public ReusableTask ConnectAsync()
        {
            return ReusableTask.CompletedTask;
        }

        public async ReusableTask<int> ReceiveAsync(Memory<byte> buffer)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(I2PConnection));
            if (_stream == null) throw new InvalidOperationException("Соединение не установлено");

            return await _stream.ReadAsync(buffer).ConfigureAwait(false);
        }

        public async ReusableTask<int> SendAsync(Memory<byte> buffer)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(I2PConnection));
            if (_stream == null) throw new InvalidOperationException("Соединение не установлено");

            await _stream.WriteAsync(buffer).ConfigureAwait(false);
            return buffer.Length;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _stream?.Dispose();
            _tcpClient?.Dispose();
            _virtualStream?.Dispose();
        }
    }
}