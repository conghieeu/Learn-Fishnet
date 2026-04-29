using System;
using System.Net;

public delegate void OnUDPRecvFrom(int size, int pktType, in ArraySegment<byte> data, IPEndPoint endPoint);
