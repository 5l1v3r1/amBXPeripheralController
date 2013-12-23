﻿using System.Collections.Generic;
using Common.Server.Applicators;
using Common.Entities;
using Common.Server.Managers;
using amBXLib;

namespace ServerMT
{
  class FanApplicator : ApplicatorBase<Fan>
  {
    public FanApplicator(CompassDirection xiDirection, EngineManager xiEngine) 
      : base (xiEngine, new FanManager(xiDirection))
    {
      mDirection = xiDirection;
    }

    protected override void ActNextFrame()
    {
      var lFanData = mManager.GetNext();

      if (lFanData != null)
      {
        mEngine.UpdateFan(mDirection, lFanData.Item);
      }

      WaitforInterval(lFanData.Length);
    }

    private CompassDirection mDirection;
  }
}
