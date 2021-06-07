using System;

namespace RVTR.Media.Domain.Abstracts
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class AEntity
  {
    public Nullable<long> EntityId { get; set; }
    public string id { get; set; }

    protected AEntity()
    {
      id = System.DateTime.Now.Ticks.ToString();
      EntityId = long.Parse(id);
    }
  }
}
