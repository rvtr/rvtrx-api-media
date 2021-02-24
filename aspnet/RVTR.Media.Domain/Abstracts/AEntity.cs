namespace RVTR.Media.Domain.Abstracts
{
  /// <summary>
  /// 
  /// </summary>
  public abstract class AEntity
  {
    public long EntityId { get; set; }
    public string id { get; set; }

    public AEntity()
    {
      id = System.DateTime.Now.Ticks.ToString();
      EntityId = long.Parse(id);
    }
  }
}
