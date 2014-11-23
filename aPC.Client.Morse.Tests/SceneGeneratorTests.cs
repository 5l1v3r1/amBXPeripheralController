﻿using aPC.Client.Morse.Codes;
using aPC.Common;
using aPC.Common.Defaults;
using aPC.Common.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aPC.Client.Morse.Tests
{
  [TestFixture]
  internal class SceneGeneratorTests
  {
    [Test]
    public void Generator_ReturnsScene()
    {
      var generatedScene = new SceneGenerator(new Settings("Test")).Generate();
      Assert.AreEqual(typeof(amBXScene), generatedScene.GetType());
    }

    #region Default Settings

    [Test]
    // "Is not repeated" implies that this is an event
    public void GeneratedScene_WithDefaultSettings_IsNotRepeated()
    {
      var generatedScene = new SceneGenerator(new Settings("Test")).Generate();
      Assert.AreEqual(eSceneType.Event, generatedScene.SceneType);
    }

    [Test]
    public void GeneratedScene_WithDefaultSettings_HasLightsEnabledOnAllFrames()
    {
      var generatedScene = new SceneGenerator(new Settings("Test")).Generate();

      foreach (var light in generatedScene.Frames.Select(frame => frame.Lights))
      {
        foreach (eDirection direction in ApplicableLightDirections)
        {
          Assert.IsNotNull(light.GetComponentValueInDirection(direction));
        }
      }
    }

    [Test]
    public void GeneratedScene_WithDefaultSettings_HasRumblesDisabledOnAllFrames()
    {
      var generatedScene = new SceneGenerator(new Settings("Test")).Generate();

      foreach (var rumble in generatedScene.Frames.Select(frame => frame.Rumbles))
      {
        Assert.IsNull(rumble.GetComponentValueInDirection(eDirection.Center));
      }
    }

    [Test]
    public void GeneratedScene_WithDefaultSettings_HasWhiteLights()
    {
      var lWhiteLight = DefaultLights.White;
      var generatedScene = new SceneGenerator(new Settings("T")).Generate();

      foreach (var light in generatedScene.Frames.Select(frame => frame.Lights))
      {
        foreach (eDirection direction in ApplicableLightDirections)
        {
          Assert.AreEqual(lWhiteLight, light.GetComponentValueInDirection(direction));
        }
      }
    }

    [Test]
    public void GeneratedScene_WithDefaultSettings_HasLengthsInMultiplesOf100()
    {
      var generatedScene = new SceneGenerator(new Settings("Test")).Generate();

      foreach (var light in generatedScene.Frames)
      {
        Assert.AreEqual(0, light.Length % 100);
      }
    }

    #endregion Default Settings

    #region Switch changes

    [Test]
    // "Is repeated" implies that this is a synched event with all frames repeated
    public void GeneratedScene_WithRepetitionEnabled_IsRepeated()
    {
      var settings = new Settings("Test");
      settings.RepeatMessage = true;
      var generatedScene = new SceneGenerator(settings).Generate();

      Assert.AreEqual(eSceneType.Sync, generatedScene.SceneType);

      foreach (var frame in generatedScene.Frames)
      {
        Assert.AreEqual(true, frame.IsRepeated);
      }
    }

    [Test]
    // In general, so long as the Settings class is built using ArgumentReader,
    // this Exception should not be thrown (as we enforce either lights or rumble enabled.
    public void GeneratedScene_WithLightsAndRumblesDisabled_Throws()
    {
      var settings = new Settings("Test");
      settings.LightsEnabled = false;

      Assert.Throws<ArgumentException>(() => new SceneGenerator(settings).Generate());
    }

    [Test]
    public void GeneratedScene_WithRumbleEnabled_HasRumblesEnabledOnAllFrames()
    {
      var settings = new Settings("Test");
      settings.RumblesEnabled = true;
      var generatedScene = new SceneGenerator(settings).Generate();

      foreach (var rumble in generatedScene.Frames.Select(frame => frame.Rumbles))
      {
        Assert.IsNotNull(rumble.GetComponentValueInDirection(eDirection.Center));
      }
    }

    [Test]
    public void GeneratedScene_WithDifferentColouredLights_IsPropogatedToScene()
    {
      var settings = new Settings("T");
      settings.Colour = DefaultLights.Red;
      var generatedScene = new SceneGenerator(settings).Generate();

      foreach (var light in generatedScene.Frames.Select(frame => frame.Lights))
      {
        foreach (eDirection direction in ApplicableLightDirections)
        {
          Assert.AreEqual(DefaultLights.Red, light.GetComponentValueInDirection(direction));
        }
      }
    }

    [Test]
    public void GeneratedScene_WithOverriddenUnitLength_HasLengthsInMultiplesOfNewLength()
    {
      var settings = new Settings("Test");
      settings.UnitLength = 17;
      var generatedScene = new SceneGenerator(settings).Generate();

      foreach (var light in generatedScene.Frames)
      {
        Assert.AreEqual(0, light.Length % 17);
      }
    }

    [Test]
    public void GeneratedScene_WithMessageRepeated_EndsWithMessageEndMarker()
    {
      var settings = new Settings("Test");
      settings.RepeatMessage = true;
      var generatedScene = new SceneGenerator(settings).Generate();

      var scene = generatedScene.Frames.Last();
      var expectedBlock = new MessageEndMarker();

      Assert.AreEqual(expectedBlock.Enabled, scene.Lights.North == settings.Colour);
      Assert.AreEqual(expectedBlock.Length * settings.UnitLength, scene.Length);
    }

    [Test]
    public void GeneratedScene_WithMessageNotRepeated_DoesNotEndWithMessageEndMarker()
    {
      var settings = new Settings("Test");
      var generatedScene = new SceneGenerator(settings).Generate();

      var scene = generatedScene.Frames.Last();
      var expectedBlock = new MessageEndMarker();

      Assert.AreNotEqual(expectedBlock.Enabled, scene.Lights.North == settings.Colour);
      Assert.AreNotEqual(expectedBlock.Length * settings.UnitLength, scene.Length);
    }

    #endregion Switch changes

    #region Message Tests

    [Test]
    public void GeneratedScene_WithSingleCharacterMessage_AndMessageIsNotRepeated_ReturnsCharacter()
    {
      var settings = new Settings("T");
      var generatedScene = new SceneGenerator(settings).Generate();

      Assert.AreEqual(1, generatedScene.Frames.Count);
      var frame = generatedScene.Frames.Single();

      Assert.IsNotNull(frame.Lights);
      Assert.AreEqual(settings.Colour, frame.Lights.North); // Sufficient to just test North
      Assert.AreEqual(new Dash().Length * settings.UnitLength, frame.Length);
    }

    [Test]
    public void GeneratedScene_WithSingleCharacterMessage_AndMessageIsRepeated_ReturnsCharacterAndMessageEndMarker()
    {
      var settings = new Settings("T");
      settings.RepeatMessage = true;
      var generatedScene = new SceneGenerator(settings).Generate();

      Assert.AreEqual(2, generatedScene.Frames.Count);

      // First frame should be a dash (which is T)
      // Second frame should be the "end of message" marker
      var firstExpectedFrame = new Dash();
      var secondExpectedFrame = new MessageEndMarker();

      Assert.IsNotNull(generatedScene.Frames[0].Lights);
      Assert.AreEqual(settings.Colour, generatedScene.Frames[0].Lights.North); // Sufficient to just test North
      Assert.AreEqual(firstExpectedFrame.Length * settings.UnitLength, generatedScene.Frames[0].Length);
      Assert.IsNotNull(generatedScene.Frames[1].Lights);
      Assert.AreEqual(DefaultLights.Off, generatedScene.Frames[1].Lights.North);
      Assert.AreEqual(secondExpectedFrame.Length * settings.UnitLength, generatedScene.Frames[1].Length);
    }

    #endregion Message Tests

    //TODO: Consider moving this idea into SectionBaseExtensions and call it Readirections or similar (non-compound)
    private List<eDirection> ApplicableLightDirections
    {
      get
      {
        return ((eDirection[])Enum.GetValues(typeof(eDirection)))
          .Where(dirn => dirn != eDirection.Center &&
                         dirn != eDirection.Everywhere)
          .ToList();
      }
    }
  }
}