using System;

public delegate void OnRecvPacket(int size, int pktType, in ArraySegment<byte> data);
