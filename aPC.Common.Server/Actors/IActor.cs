﻿using aPC.Common.Server.Snapshots;

namespace aPC.Common.Server.Actors
{
  internal interface IActor<T> where T : SnapshotBase
  {
    void ActNextFrame(eDirection xiDirection, T xiSnapshot);
  }
}