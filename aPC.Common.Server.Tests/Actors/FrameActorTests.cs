﻿using aPC.Common.Builders;
using aPC.Common.Defaults;
using aPC.Common.Entities;
using aPC.Common.Server.Actors;
using aPC.Common.Server.Snapshots;
using NUnit.Framework;
using System;
using System.Linq;

namespace aPC.Common.Server.Tests.Actors
{
  [TestFixture]
  internal class FrameActorTests
  {
    private TestEngineManager engine;
    private FrameActor actor;

    [SetUp]
    public void Setup()
    {
      engine = new TestEngineManager();
      actor = new FrameActor(engine);
    }

    [Test]
    public void UpdatingOneLight_SuccessfullyUpdated()
    {
      var lightSection = new LightSectionBuilder()
        .WithLightInDirection(eDirection.North, DefaultLights.Green)
        .Build();

      var frame = GetFrameBuilderWithDefaultFrame()
        .WithLightSection(lightSection)
        .Build()
        .Single();

      actor.ActNextFrame(new FrameSnapshot(frame, 100));

      Assert.IsTrue(engine.Updated[eComponentType.Light]);
      Assert.IsFalse(engine.Updated[eComponentType.Fan]);
      Assert.IsFalse(engine.Updated[eComponentType.Rumble]);
      Assert.AreEqual(DefaultLights.Green, engine.Status.LightSection.GetComponentValueInDirection(eDirection.North));
    }

    [Test]
    public void UpdatingOneFan_SuccessfullyUpdated()
    {
      var fanSection = new FanSectionBuilder()
        .WithFanInDirection(eDirection.West, DefaultFans.QuarterPower)
        .Build();

      var frame = GetFrameBuilderWithDefaultFrame()
        .WithFanSection(fanSection)
        .Build()
        .Single();

      actor.ActNextFrame(new FrameSnapshot(frame, 100));

      Assert.IsFalse(engine.Updated[eComponentType.Light]);
      Assert.IsTrue(engine.Updated[eComponentType.Fan]);
      Assert.IsFalse(engine.Updated[eComponentType.Rumble]);
      Assert.AreEqual(DefaultFans.QuarterPower, engine.Status.FanSection.GetComponentValueInDirection(eDirection.West));
    }

    [Test]
    public void UpdatingRumble_SuccessfullyUpdated()
    {
      var frame = GetFrameBuilderWithDefaultFrame()
        .WithRumbleSection(DefaultRumbleSections.SoftThunder)
        .Build()
        .Single();

      actor.ActNextFrame(new FrameSnapshot(frame, 100));

      Assert.IsFalse(engine.Updated[eComponentType.Light]);
      Assert.IsFalse(engine.Updated[eComponentType.Fan]);
      Assert.IsTrue(engine.Updated[eComponentType.Rumble]);
      Assert.AreEqual(DefaultRumbleSections.SoftThunder.Rumbles.Single(), engine.Status.RumbleSection.Rumbles.Single());
    }

    [Test]
    public void WhenActingNextFrame_DirectionIsIgnored()
    {
      var lightSection = new LightSectionBuilder()
        .WithLightInDirection(eDirection.North, DefaultLights.Green)
        .Build();

      var frame = GetFrameBuilderWithDefaultFrame()
        .WithLightSection(lightSection)
        .Build()
        .Single();

      foreach (eDirection direction in Enum.GetValues(typeof(eDirection)))
      {
        actor.ActNextFrame(new FrameSnapshot(frame, 100));

        Assert.IsTrue(engine.Updated[eComponentType.Light]);
        Assert.IsFalse(engine.Updated[eComponentType.Fan]);
        Assert.IsFalse(engine.Updated[eComponentType.Rumble]);
        Assert.AreEqual(DefaultLights.Green, engine.Status.LightSection.Lights.Last());
      }
    }

    private FrameBuilder GetFrameBuilderWithDefaultFrame()
    {
      return new FrameBuilder()
        .AddFrame()
        .WithFrameLength(1000)
        .WithRepeated(true);
    }
  }
}