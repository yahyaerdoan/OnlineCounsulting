using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Testimonial : BaseEntity
{

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    //TODO: random fotograf dongusu saglanabilir
    public string ImageUrl { get; set; } = string.Empty;
    [NotMapped]
    public override string EntityName => "Testimonial Informations";
}
