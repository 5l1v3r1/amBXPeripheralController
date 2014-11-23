﻿using aPC.Common.Entities;
using aPC.Common.Server.Snapshots;
using System;

namespace aPC.Common.Server.SceneHandlers
{
  public abstract class ComponentHandler<T> : SceneHandlerBase<ComponentSnapshot<T>> where T : IComponent
  {
    protected ComponentHandler(amBXScene scene, Action eventComplete)
      : base(scene, eventComplete)
    {
    }
  }
}