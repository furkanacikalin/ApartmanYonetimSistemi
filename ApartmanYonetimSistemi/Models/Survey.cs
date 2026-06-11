namespace ApartmanYonetimSistemi.Models;

public class Survey
{
    public int Id { get; set; }
    public int ApartmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7); // Varsayılan 7 gün süre
    public bool IsActive { get; set; } = true;

    // İlişkili Seçenekler
    public List<SurveyOption> Options { get; set; } = new();
}

public class SurveyOption
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string OptionText { get; set; } = string.Empty;

    // Bu seçeneğe gelen oylar
    public List<SurveyVote> Votes { get; set; } = new();
}

public class SurveyVote
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public int SurveyOptionId { get; set; }
    public int ResidentUserId { get; set; } // Oy kullanan sakinin Id'si (Mükerrer oy engellemek için)
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}