using System.Text.Json;
using PetAmigoAgenda.Classes;

namespace PetAmigoAgenda.Services;

public static class ArquivoPetShop
{
    private const string Caminho = "dados_petshop.json";
    public static void Salvar(DadosPetShop dados)
    {
        string json = JsonSerializer.Serialize(dados, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Caminho, json);
    }
    public static DadosPetShop Carregar()
    {
        if (!File.Exists(Caminho)) return new DadosPetShop();
        return JsonSerializer.Deserialize<DadosPetShop>(File.ReadAllText(Caminho)) ?? new DadosPetShop();
    }
}
