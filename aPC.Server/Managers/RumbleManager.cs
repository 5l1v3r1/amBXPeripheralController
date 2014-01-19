﻿using aPC.Common.Entities;
using aPC.Common.Integration;
using aPC.Common.Server.Managers;
using amBXLib;
using System.Linq;
using System;

namespace aPC.Server.Managers
{
  class RumbleManager : ManagerBase
  {
    public RumbleManager(CompassDirection xiDirection) 
      : this(xiDirection, null)
    {
    }

    public RumbleManager(CompassDirection xiDirection, Action xiEventCallback) 
      : base(xiEventCallback)
    {
      mDirection = xiDirection;
      SetupNewScene(CurrentScene);
    }

    // A scene is applicable if there is at least one non-null light in the right direction defined.
    protected override bool SceneIsApplicable(amBXScene xiNewScene)
    {
      var lRumbles = xiNewScene
        .Frames
        .Select(frame => frame.Rumbles)
        .Where(section => section != null);

      return lRumbles.Any(rumble => rumble != null);
    }

    public override Data GetNext()
    {
      var lFrame = GetNextFrame();

      var lLength = lFrame.Length;
      int lFadeTime;
      Rumble lRumble;

      if (lFrame.Rumbles == null)
      {
        lFadeTime = 0;
        lRumble = null;
      }
      else
      {
        lFadeTime = lFrame.Rumbles.FadeTime;
        lRumble = lFrame.Rumbles.Rumble;
      }

      return new ComponentData(lRumble, lFadeTime, lLength);
    }

    readonly CompassDirection mDirection;
  }
}