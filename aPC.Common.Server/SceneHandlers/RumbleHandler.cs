﻿using aPC.Common.Entities;
using aPC.Common.Server.Snapshots;
using System;

namespace aPC.Common.Server.SceneHandlers
{
  public class RumbleHandler : ComponentHandler<Rumble>
  {
    public RumbleHandler(amBXScene xiScene, Action xiEventcomplete)
      : base(xiScene, xiEventcomplete)
    {
    }

    public override ComponentSnapshot<Rumble> GetNextSnapshot(eDirection xiDirection)
    {
      var lFrame = GetNextFrame();

      Rumble lRumble = lFrame.Rumbles == null
        ? null
        : lFrame.Rumbles.Rumble;

      return lRumble == null
        ? new ComponentSnapshot<Rumble>(lFrame.Length)
        : new ComponentSnapshot<Rumble>(lRumble, lFrame.Rumbles.FadeTime, lFrame.Length);
    }
  }
}