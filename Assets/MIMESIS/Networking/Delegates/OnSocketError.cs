using System;
using System.Net.Sockets;

public delegate void OnSocketError(Exception? ex = null, SocketError e = SocketError.Success);
