﻿using System.Collections.Generic;
using aPC.Common.Client.Communication;

namespace aPC.Common.Client.Tests.Communication
{
  public class TestNotificationClient : NotificationClientBase
  {
    public TestNotificationClient() : base()
    {
      IntegratedScenesPushed = new List<string>();
      CustomScenesPushed = new List<string>();
    }

    public override void PushCustomScene(string xiScene)
    {
      CustomScenesPushed.Add(xiScene);
    }

    public override void PushIntegratedScene(string xiScene)
    {
      IntegratedScenesPushed.Add(xiScene);
    }

    public int NumberOfCustomScenesPushed
    {
      get { return CustomScenesPushed.Count; }
    }

    public int NumberOfIntegratedScenesPushed
    {
      get { return IntegratedScenesPushed.Count; }
    }

    public readonly List<string> IntegratedScenesPushed;
    public readonly List<string> CustomScenesPushed;
  }
}