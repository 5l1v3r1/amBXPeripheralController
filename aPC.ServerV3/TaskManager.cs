﻿using aPC.Common;
using aPC.Common.Entities;
using aPC.ServerV3.Engine;
using aPC.ServerV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace aPC.ServerV3
{
  // Handles the masses of tasks flying around.
  internal class TaskManager
  {
    private SceneOrchestrator sceneOrchestrator;
    private EngineActor engineActor;

    private DirectionalComponentActionList directionalComponentActionList;

    public TaskManager(SceneOrchestrator sceneOrchestrator, EngineActor engineActor, DirectionalComponentActionList directionalComponentActionList)
    {
      this.sceneOrchestrator = sceneOrchestrator;
      this.engineActor = engineActor;
      this.directionalComponentActionList = directionalComponentActionList;
    }

    public void RefreshFromSceneOrchestrator()
    {
      switch (sceneOrchestrator.CurrentState)
      {
        case eSceneType.Desync:
          foreach (var directionAndComponent in sceneOrchestrator.UpdatedDirectionalComponents)
          {
            ReScheduleTask(sceneOrchestrator.RunningComponents.Single(component => component.ComponentType == directionAndComponent.Item1 && component.Direction == directionAndComponent.Item2));
          }
          break;
        case eSceneType.Sync:
        case eSceneType.Event:
          directionalComponentActionList.CancelAll();
          ScheduleTask(sceneOrchestrator.RunningComponents.Single(component => !component.ComponentType.HasValue), 0);
          break;
      }
    }

    private void ReScheduleTask(RunningDirectionalComponent runningComponent)
    {
      directionalComponentActionList.Cancel(runningComponent.ComponentType, runningComponent.Direction);
      ScheduleTask(runningComponent, 0);
    }

    private void RunFrameForDirectionalComponent(RunningDirectionalComponent runningComponent, CancellationTokenSource cancellationToken)
    {
      directionalComponentActionList.Remove(cancellationToken);

      var frame = GetFrame(runningComponent);

      if (sceneOrchestrator.CurrentState == eSceneType.Desync)
      {
        var component = frame.GetComponentInDirection(runningComponent.ComponentType.Value, runningComponent.Direction);
        engineActor.UpdateComponent(component);
      }
      else
      {
        foreach (eComponentType componentType in Enum.GetValues(typeof(eComponentType)))
          foreach (eDirection direction in EnumExtensions.GetCompassDirections())
          {
            var component = frame.GetComponentInDirection(componentType, direction);
            if (component != null)
            {
              engineActor.UpdateComponent(component);
            }
          }
      }

      DoPostUpdateActions(runningComponent, frame.Length);
    }

    private Frame GetFrame(RunningDirectionalComponent runningComponents)
    {
      var frames = runningComponents.Ticker.IsFirstRun
        ? runningComponents.Scene.Frames
        : runningComponents.Scene.RepeatableFrames;
      return frames[runningComponents.Ticker.Index];
    }

    private void DoPostUpdateActions(RunningDirectionalComponent runningComponent, int delay)
    {
      runningComponent.Ticker.Advance();

      // When we've run the scene once through, we need to check that there are either:
      // * repeatable frames
      // * that it's not an event
      // If neither of these hold, then we terminate running by NOT scheduling the next task.
      if (runningComponent.Ticker.Index == 0 && (runningComponent.Scene.SceneType == eSceneType.Event || runningComponent.Scene.RepeatableFrames.Count == 0))
      {
        return;
      }

      ScheduleTask(runningComponent, delay);
    }

    private void ScheduleTask(RunningDirectionalComponent runningComponent, int delay)
    {
      var cancellationToken = new CancellationTokenSource();
      var task = Task.Run(async delegate
              {
                await Task.Delay(TimeSpan.FromMilliseconds(delay));
                RunFrameForDirectionalComponent(runningComponent, cancellationToken);
              }, cancellationToken.Token);

      directionalComponentActionList.Add(new DirectionalComponentAction(cancellationToken, runningComponent.ComponentType, runningComponent.Direction));
    }
  }
}