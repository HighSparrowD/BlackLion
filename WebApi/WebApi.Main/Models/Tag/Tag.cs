using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.Tag;

namespace WebApi.Main.Entities.Tag;

public class Tag
{
    [Key]
    public long Id { get; set; }
    [Key]
    public TagType Type { get; set; }
    public string Text { get; set; }
    public long? MotherId { get; set; }
    public long? FatherId { get; set; }
    public TagType? MotherType { get; set; }
    public TagType? FatherType { get; set; }

    [ForeignKey("MotherId, MotherType")]
    public virtual Tag Mother { get; set; }
    [ForeignKey("FatherId, FatherType")]
    public virtual Tag Father { get; set; }

    public Tag()
    { }

    public Tag(string text, TagType type)
    {
        Text = text;
        Type = type;
    }

    public void SetRelatives(List<TagRelative> relatives)
    {
        if (relatives.Count >= 1)
        {
            MotherId = relatives[0].TagId;
            MotherType = relatives[0].TagType;

            if (relatives.Count >= 2)
            {
                FatherId = relatives[1].TagId;
                FatherType = relatives[1].TagType;
            }
        }
    }
}
