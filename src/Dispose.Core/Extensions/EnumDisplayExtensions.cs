using Dispose.Core.Enums;

namespace Dispose.Core.Extensions;

public static class EnumDisplayExtensions
{
    public static string ToDisplayName(this WasteType wasteType) =>
        wasteType switch
        {
            WasteType.Organic => "Organico",
            WasteType.Recyclable => "Reciclavel",
            WasteType.Glass => "Vidro",
            WasteType.GreenWaste => "Poda",
            _ => wasteType.ToString()
        };

    public static string ToDisplayName(this SpecialItemCategory category) =>
        category switch
        {
            SpecialItemCategory.Batteries => "Pilhas e baterias",
            SpecialItemCategory.Electronics => "Eletronicos",
            SpecialItemCategory.Medication => "Medicamentos",
            SpecialItemCategory.CookingOil => "Oleo de cozinha",
            _ => category.ToString()
        };

    public static string ToDisplayName(this DayOfWeek dayOfWeek) =>
        dayOfWeek switch
        {
            DayOfWeek.Sunday => "Domingo",
            DayOfWeek.Monday => "Segunda",
            DayOfWeek.Tuesday => "Terca",
            DayOfWeek.Wednesday => "Quarta",
            DayOfWeek.Thursday => "Quinta",
            DayOfWeek.Friday => "Sexta",
            DayOfWeek.Saturday => "Sabado",
            _ => dayOfWeek.ToString()
        };
}
