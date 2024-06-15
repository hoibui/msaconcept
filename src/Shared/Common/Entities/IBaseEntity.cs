using System.ComponentModel.DataAnnotations;

namespace Common.Entities;

public interface IBaseEntity<TKey>
{
    [Key]
    public TKey Id { get; set; }
}