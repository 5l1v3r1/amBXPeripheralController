﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace aPC.Common.Entities
{
  public static class SectionBaseExtensions
  {
    public static T GetComponentValueInDirection<T>(this SectionBase<T> xiSection, eDirection xiDirection) 
      where T : Component
    {
      var lField = xiSection.GetComponentInfoInDirection(xiDirection);
      if (lField == null)
      {
        return null;
      }

      return (T)lField.GetValue(xiSection);
    }

    public static bool SetComponentValueInDirection<T>(this SectionBase<T> xiSection, T xiComponent, eDirection xiDirection) 
      where T : Component
    {
      var lField = xiSection.GetComponentInfoInDirection(xiDirection);
      if (lField == null)
      {
        return false;
      }

      lField.SetValue(xiSection, xiComponent);
      return true;
    }

    private static FieldInfo GetComponentInfoInDirection<T>(this SectionBase<T> xiSection, eDirection xiDirection) 
      where T : Component
    {
      if (xiSection == null)
      {
        return null;
      }

      return GetSectionFields(xiSection)
        .SingleOrDefault(field => DirectionAttribute.MatchesDirection(field, xiDirection));
    }


    public static T GetPhysicalComponentValueInDirection<T>(this SectionBase<T> xiSection, eDirection xiDirection)
  where T : Component
    {
      var lField = xiSection.GetPhysicalComponentInfoInDirection(xiDirection);
      if (lField == null)
      {
        return null;
      }

      return (T)lField.GetValue(xiSection);
    }

    public static bool SetPhysicalComponentValueInDirection<T>(this SectionBase<T> xiSection, T xiComponent, eDirection xiDirection)
      where T : Component
    {
      var lField = xiSection.GetPhysicalComponentInfoInDirection(xiDirection);
      if (lField == null)
      {
        return false;
      }

      lField.SetValue(xiSection, xiComponent);
      return true;
    }

    private static FieldInfo GetPhysicalComponentInfoInDirection<T>(this SectionBase<T> xiSection, eDirection xiDirection) where T : Component
    {
      var lFieldInDirection = GetComponentInfoInDirection(xiSection, xiDirection);
      if (lFieldInDirection == null)
      {
        return null;
      }

      return PhysicalComponentAttribute.IsPhysicalDirection(lFieldInDirection)
        ? lFieldInDirection
        : null;
    }

    private static IEnumerable<FieldInfo> GetSectionFields<T>(this SectionBase<T> xiSection) where T : Component
    {
      return xiSection
        .GetType()
        .GetFields();
    }
  }
}
