public class QuestShort
{
    public int Id { get; set; }
    public int OwnerUserId { get; set; } 
    public string QuestName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StartMusic { get; set; } = string.Empty;
    public string StartImage { get; set; } = string.Empty;
    public int Order { get; set; }

    public override string ToString()
    {
        return $"Id: {Id}, QuestName: {QuestName}, DisplayName: {DisplayName}";
    }
}