using System;
using System.Net.Sockets;

public delegate void OnRUDPError(Exception? ex = null, SocketError e = SocketError.Success);
