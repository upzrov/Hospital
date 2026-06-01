using DAL.Enums;

namespace BLL.Extensions;

public static class SpecialtyExtensions
{
    public static string ToDisplayName(this Specialty specialty)
    {
        return specialty switch
        {
            Specialty.Therapist => "Терапевт",
            Specialty.Cardiologist => "Кардіолог",
            Specialty.Surgeon => "Хірург",
            Specialty.Dentist => "Стоматолог",
            Specialty.Neurologist => "Невролог",
            _ => specialty.ToString()
        };
    }
}