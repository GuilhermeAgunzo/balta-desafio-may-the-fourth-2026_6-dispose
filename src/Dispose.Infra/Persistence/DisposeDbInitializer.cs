using Dispose.Core.Entities;
using Dispose.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dispose.Infra.Persistence;

public sealed class DisposeDbInitializer
{
    private readonly DisposeDbContext _dbContext;

    public DisposeDbInitializer(DisposeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await _dbContext.Neighborhoods.AnyAsync(cancellationToken))
        {
            return;
        }

        var coruscantCentroId = Guid.Parse("4b6d35ff-08e4-4a0a-a749-e64d19ca0910");
        var jardimNabooId = Guid.Parse("f21e0fd0-c714-41d7-a53f-67629bbecae0");
        var setorTatooineId = Guid.Parse("5dc0d5eb-4fc0-487c-bcb2-b7d2424e04d8");
        var bosqueEndorId = Guid.Parse("7e5458f4-75a1-4cac-b467-7f9145dc2a86");

        var neighborhoods = new[]
        {
            new Neighborhood
            {
                Id = coruscantCentroId,
                Name = "Coruscant Centro",
                Sector = "Setor Senado",
                Motto = "Painel principal para quem precisa alinhar coleta e descarte sem perder o hiperespaco.",
                Latitude = -23.55052,
                Longitude = -46.63331
            },
            new Neighborhood
            {
                Id = jardimNabooId,
                Name = "Jardim Naboo",
                Sector = "Ala das Fontes",
                Motto = "Rota de coleta com foco em reciclaveis limpos e descarte especial com elegancia.",
                Latitude = -23.56289,
                Longitude = -46.65578
            },
            new Neighborhood
            {
                Id = setorTatooineId,
                Name = "Setor Tatooine",
                Sector = "Quadrante das Dunas",
                Motto = "Agenda resiliente para quem enfrenta dias quentes, vidro e poda em volume alto.",
                Latitude = -23.56833,
                Longitude = -46.61972
            },
            new Neighborhood
            {
                Id = bosqueEndorId,
                Name = "Bosque Endor",
                Sector = "Cintura Verde",
                Motto = "Coleta urbana com lembretes proximos a ecopontos e postos aliados.",
                Latitude = -23.57811,
                Longitude = -46.64122
            }
        };

        var schedules = new[]
        {
            CreateSchedule(coruscantCentroId, WasteType.Organic, DayOfWeek.Monday, "06:00-09:00", "ROT-COR-11", "Separe os organicos em saco bem fechado antes da alvorada."),
            CreateSchedule(coruscantCentroId, WasteType.Organic, DayOfWeek.Thursday, "06:00-09:00", "ROT-COR-17", "Evite deixar recipientes abertos apos as 22h."),
            CreateSchedule(coruscantCentroId, WasteType.Recyclable, DayOfWeek.Tuesday, "07:00-10:00", "ROT-COR-22", "Reciclaveis secos e limpos aceleram a triagem da frota."),
            CreateSchedule(coruscantCentroId, WasteType.Glass, DayOfWeek.Friday, "08:00-11:00", "ROT-COR-31", "Embale cacos e sinalize o volume com etiqueta visivel."),
            CreateSchedule(coruscantCentroId, WasteType.GreenWaste, DayOfWeek.Saturday, "09:00-12:00", "ROT-COR-45", "Poda deve seguir amarrada em feixes curtos."),

            CreateSchedule(jardimNabooId, WasteType.Organic, DayOfWeek.Monday, "07:00-10:00", "ROT-NAB-08", "Organicos seguem primeiro para evitar odor nas areas ajardinadas."),
            CreateSchedule(jardimNabooId, WasteType.Recyclable, DayOfWeek.Wednesday, "07:00-11:00", "ROT-NAB-16", "Papel e plastico precisam estar secos para embarque."),
            CreateSchedule(jardimNabooId, WasteType.Glass, DayOfWeek.Friday, "07:00-10:00", "ROT-NAB-24", "Use caixa reforcada para vidro e destaque material cortante."),
            CreateSchedule(jardimNabooId, WasteType.GreenWaste, DayOfWeek.Saturday, "08:00-11:00", "ROT-NAB-33", "Sacos de folhas devem ficar abaixo de 20 kg."),

            CreateSchedule(setorTatooineId, WasteType.Organic, DayOfWeek.Tuesday, "06:30-09:30", "ROT-TAT-12", "Dias quentes exigem descarte pouco antes da coleta."),
            CreateSchedule(setorTatooineId, WasteType.Recyclable, DayOfWeek.Thursday, "07:30-10:30", "ROT-TAT-19", "Compacte latas e caixas para reduzir volume no ponto."),
            CreateSchedule(setorTatooineId, WasteType.Glass, DayOfWeek.Saturday, "08:30-11:30", "ROT-TAT-28", "Vidros transparentes e coloridos podem seguir juntos, mas bem protegidos."),
            CreateSchedule(setorTatooineId, WasteType.GreenWaste, DayOfWeek.Monday, "08:00-11:00", "ROT-TAT-35", "Galhos mais densos precisam estar cortados em segmentos curtos."),

            CreateSchedule(bosqueEndorId, WasteType.Organic, DayOfWeek.Wednesday, "06:00-09:00", "ROT-END-09", "A rota organica passa cedo nos trechos arborizados."),
            CreateSchedule(bosqueEndorId, WasteType.Recyclable, DayOfWeek.Friday, "07:00-10:00", "ROT-END-18", "Reforce a separacao de embalagens flexiveis e metal."),
            CreateSchedule(bosqueEndorId, WasteType.Glass, DayOfWeek.Tuesday, "08:00-11:00", "ROT-END-27", "Consolide o vidro em caixa unica para evitar dispersao."),
            CreateSchedule(bosqueEndorId, WasteType.GreenWaste, DayOfWeek.Saturday, "08:00-12:00", "ROT-END-36", "Poda e folhas secas seguem em lotes separados.")
        };

        var collectionPoints = new[]
        {
            new CollectionPoint
            {
                Id = Guid.Parse("c0a56acd-cbd4-49eb-b937-8f287ddf2ad4"),
                Name = "Hangar Verde Coruscant",
                NeighborhoodName = "Coruscant Centro",
                Address = "Av. Senado Galactico, 77",
                Landmark = "Ao lado do terminal orbital de reciclaveis",
                FactionTag = "Alianca Urbana",
                Latitude = -23.54894,
                Longitude = -46.63651
            },
            new CollectionPoint
            {
                Id = Guid.Parse("5ba7bbca-43b6-48c0-95c8-7d5980ddc672"),
                Name = "Posto Jedi Saude",
                NeighborhoodName = "Coruscant Centro",
                Address = "Rua dos Arquivos, 102",
                Landmark = "Entrada secundaria da clinica de bairro",
                FactionTag = "Templo Civico",
                Latitude = -23.55211,
                Longitude = -46.63187
            },
            new CollectionPoint
            {
                Id = Guid.Parse("9ae02c49-923a-49f7-a1b5-d6a1760f0908"),
                Name = "Pavilhao Naboo Circular",
                NeighborhoodName = "Jardim Naboo",
                Address = "Alameda das Fontes, 51",
                Landmark = "Em frente ao mercado de produtores",
                FactionTag = "Guarda Real",
                Latitude = -23.56137,
                Longitude = -46.65303
            },
            new CollectionPoint
            {
                Id = Guid.Parse("1f00d31a-0dd8-4b45-b529-517b3982d2d0"),
                Name = "Ecoponto Tatooine Sul",
                NeighborhoodName = "Setor Tatooine",
                Address = "Via dos Dois Sois, 404",
                Landmark = "Perto da estacao de transbordo",
                FactionTag = "Caravana Local",
                Latitude = -23.57062,
                Longitude = -46.61604
            },
            new CollectionPoint
            {
                Id = Guid.Parse("43dcb68b-5f9a-4d77-9465-62882b0c49fb"),
                Name = "Cooperativa Endor Reuso",
                NeighborhoodName = "Bosque Endor",
                Address = "Estrada do Dossel, 18",
                Landmark = "Ao lado do viveiro comunitario",
                FactionTag = "Tribo Recicladora",
                Latitude = -23.57614,
                Longitude = -46.63941
            }
        };

        var pointCategories = new[]
        {
            CreatePointCategory(collectionPoints[0].Id, SpecialItemCategory.Batteries),
            CreatePointCategory(collectionPoints[0].Id, SpecialItemCategory.Electronics),
            CreatePointCategory(collectionPoints[1].Id, SpecialItemCategory.Medication),
            CreatePointCategory(collectionPoints[2].Id, SpecialItemCategory.CookingOil),
            CreatePointCategory(collectionPoints[2].Id, SpecialItemCategory.Batteries),
            CreatePointCategory(collectionPoints[3].Id, SpecialItemCategory.Electronics),
            CreatePointCategory(collectionPoints[3].Id, SpecialItemCategory.CookingOil),
            CreatePointCategory(collectionPoints[4].Id, SpecialItemCategory.Batteries),
            CreatePointCategory(collectionPoints[4].Id, SpecialItemCategory.Medication),
            CreatePointCategory(collectionPoints[4].Id, SpecialItemCategory.Electronics)
        };

        await _dbContext.Neighborhoods.AddRangeAsync(neighborhoods, cancellationToken);
        await _dbContext.CollectionSchedules.AddRangeAsync(schedules, cancellationToken);
        await _dbContext.CollectionPoints.AddRangeAsync(collectionPoints, cancellationToken);
        await _dbContext.CollectionPointAcceptedCategories.AddRangeAsync(pointCategories, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static CollectionSchedule CreateSchedule(
        Guid neighborhoodId,
        WasteType wasteType,
        DayOfWeek dayOfWeek,
        string pickupWindow,
        string routeCode,
        string guidance) =>
        new()
        {
            Id = Guid.NewGuid(),
            NeighborhoodId = neighborhoodId,
            WasteType = wasteType,
            DayOfWeek = dayOfWeek,
            PickupWindow = pickupWindow,
            RouteCode = routeCode,
            Guidance = guidance
        };

    private static CollectionPointAcceptedCategory CreatePointCategory(Guid pointId, SpecialItemCategory category) =>
        new()
        {
            CollectionPointId = pointId,
            Category = category
        };
}
