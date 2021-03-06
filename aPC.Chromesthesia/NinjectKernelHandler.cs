﻿using aPC.Chromesthesia.Communication;
using aPC.Chromesthesia.Sound;
using aPC.Common.Client;
using aPC.Common.Client.Communication;
using NAudio.Wave;
using Ninject;

namespace aPC.Chromesthesia
{
  internal class NinjectKernelHandler
  {
    public StandardKernel Kernel { get; private set; }

    public NinjectKernelHandler()
    {
      Kernel = new StandardKernel();
      Kernel.Bind<IWaveIn>().To<WasapiLoopbackCapture>();
      Kernel.Bind<IPitchDetector>().To<FftPitchDetector>();
      Kernel.Bind<NotificationClientBase>().To<NotificationClient>();
    }
  }
}