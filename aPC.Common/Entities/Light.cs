﻿using System.Xml.Serialization;

namespace aPC.Common.Entities
{
  public class Light : Component
  {
    [XmlAttribute]
    public float Intensity;

    [XmlAttribute] 
    public float Red;

    [XmlAttribute] 
    public float Green;

    [XmlAttribute] 
    public float Blue;

    public override eSectionType ComponentType()
    {
      return eSectionType.Light;
    }
  }
}