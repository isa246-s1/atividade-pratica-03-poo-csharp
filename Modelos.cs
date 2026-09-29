using PetAmigoAgenda.Interfaces;
using System.Text.Json.Serialization;

namespace PetAmigoAgenda.Classes;

public abstract class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";

    protected Pessoa() { }
    protected Pessoa(int id, string nome, string telefone)
    {
        this.Id = id;
        this.Nome = nome;
        this.Telefone = telefone;
    }

    public virtual string Apresentar() => $"{Id} - {Nome} | {Telefone}";
}

public class Cliente : Pessoa, IExibivel
{
    public int Pontos { get; set; }
    public Cliente() { }
    public Cliente(int id, string nome, string telefone) : base(id, nome, telefone) { }
    public override string Apresentar() => base.Apresentar() + $" | pontos: {Pontos}";
    public string ExibirResumo() => Apresentar();
}

public class Pet : IExibivel
{
    private string observacao = "";
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Nome { get; set; } = "";
    public string Especie { get; set; } = "";
    public string Porte { get; set; } = "";
    public string Observacao { get => observacao; set => observacao = value.Trim(); }

    public Pet() { }
    public Pet(int id, int clienteId, string nome, string especie, string porte)
    {
        this.Id = id;
        this.ClienteId = clienteId;
        this.Nome = nome;
        this.Especie = especie;
        this.Porte = porte;
    }
    public string ExibirResumo() => $"Pet {Id}: {Nome} ({Especie}, porte {Porte})";
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(ServicoComCuidados), "cuidados")]
public class Servico
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public decimal ValorBase { get; set; }
    public Servico() { }
    public Servico(int id, string nome, decimal valorBase)
    {
        this.Id = id; this.Nome = nome; this.ValorBase = valorBase;
    }
    public virtual decimal CalcularPreco(string porte) => ValorBase;
}

public class ServicoComCuidados : Servico
{
    public decimal TaxaCuidados { get; set; }
    public ServicoComCuidados() { }
    public ServicoComCuidados(int id, string nome, decimal valor, decimal taxa) : base(id, nome, valor)
    {
        TaxaCuidados = taxa;
    }
    public override decimal CalcularPreco(string porte)
    {
        decimal adicionalPorte = porte.ToLower() == "grande" ? 10 : 0;
        return ValorBase + TaxaCuidados + adicionalPorte;
    }
}

public class Agendamento
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public int ServicoId { get; set; }
    public DateTime DataHora { get; set; }
    public decimal Valor { get; set; }
    public bool Concluido { get; set; }
}

public class Vacinacao
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string Vacina { get; set; } = "";
    public DateTime DataAplicacao { get; set; }
    public DateTime ProximaDose { get; set; }
}

public class SolicitacaoEncaixe
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public DateTime DataDesejada { get; set; }
    public string Motivo { get; set; } = "";
}

public class DadosPetShop
{
    public List<Cliente> Clientes { get; set; } = [];
    public List<Pet> Pets { get; set; } = [];
    public List<Servico> Servicos { get; set; } = [];
    public List<Agendamento> Agendamentos { get; set; } = [];
    public List<Vacinacao> Vacinacoes { get; set; } = [];
    public List<SolicitacaoEncaixe> FilaEncaixe { get; set; } = [];
}
