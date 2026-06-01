using DAL.Enums;

namespace BLL.Extensions;

public static class GenderExtensions
{
    public static string ToDisplayName(this Gender gender)
    {
        return gender switch
        {
            Gender.Male => "Чоловік",
            Gender.Female => "Жінка",
            _ => gender.ToString()
        };
    }
}