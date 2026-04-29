using System;
using System.Net.Sockets;

public delegate void OnSDRError(Exception? ex = null, SocketError e = SocketError.Success);
