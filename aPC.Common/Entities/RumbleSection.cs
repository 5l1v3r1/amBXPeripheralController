﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace aPC.Common.Entities
{
  public class RumbleSection : IComponentSection
  {
    [XmlArray]
    [XmlArrayItem("Rumble")]
    public List<Rumble> Rumbles;

    public IEnumerable<DirectionalComponent> GetComponents()
    {
      return Rumbles;
    }
  }
}