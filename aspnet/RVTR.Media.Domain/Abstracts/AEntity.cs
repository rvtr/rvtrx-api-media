namespace RVTR.Media.Domain.Abstracts
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class AEntity
  {
    public long EntityId { get; set; }

    public AEntity()
    {
      EntityId = System.DateTime.Now.Ticks;
    }
  }
}
