using PetAmigoAgenda.Classes;
using PetAmigoAgenda.Services;

DadosPetShop dados = ArquivoPetShop.Carregar();
if (dados.Servicos.Count == 0)
{
    dados.Servicos.Add(new Servico(1, "Banho simples", 45));
    dados.Servicos.Add(new ServicoComCuidados(2, "Banho para pet sensível", 50, 8));
}

bool continuar = true;
while (continuar)
{
    Console.WriteLine("\n==============================");
    Console.WriteLine("       PET AMIGO AGENDA");
    Console.WriteLine("==============================");
    Console.WriteLine("1 - Novo cliente");
    Console.WriteLine("2 - Cadastrar pet");
    Console.WriteLine("3 - Marcar atendimento");
    Console.WriteLine("4 - Ver agenda");
    Console.WriteLine("5 - Concluir atendimento");
    Console.WriteLine("6 - Registrar vacina");
    Console.WriteLine("7 - Conferir próximas vacinas");
    Console.WriteLine("8 - Histórico de um pet");
    Console.WriteLine("9 - Pedir encaixe");
    Console.WriteLine("10 - Ver fila de encaixe");
    Console.WriteLine("11 - Salvar agora");
    Console.WriteLine("0 - Sair");
    Console.Write("Opção: ");
    try
    {
        int opcao = int.Parse(Console.ReadLine() ?? "");
        switch (opcao)
        {
            case 1: CadastrarCliente(); break;
            case 2: CadastrarPet(); break;
            case 3: Agendar(); break;
            case 4: ListarAgenda(); break;
            case 5: Concluir(); break;
            case 6: RegistrarVacina(); break;
            case 7: AlertasVacina(); break;
            case 8: Historico(); break;
            case 9: PedirEncaixe(); break;
            case 10: ListarEncaixes(); break;
            case 11: ArquivoPetShop.Salvar(dados); Console.WriteLine("Dados salvos."); break;
            case 0: ArquivoPetShop.Salvar(dados); continuar = false; break;
            default: Console.WriteLine("Opção não existe."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Entrada inválida: " + ex.Message);
    }
}

void CadastrarCliente()
{
    Console.Write("Nome: "); string nome = Console.ReadLine() ?? "";
    Console.Write("Telefone: "); string telefone = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(nome)) throw new Exception("o nome é obrigatório");
    dados.Clientes.Add(new Cliente(ProximoId(dados.Clientes.Select(x => x.Id)), nome, telefone));
    Console.WriteLine("Cliente cadastrado.");
}

void CadastrarPet()
{
    if (dados.Clientes.Count == 0) { Console.WriteLine("Cadastre um cliente primeiro."); return; }
    dados.Clientes.ForEach(c => Console.WriteLine(c.Apresentar()));
    Console.Write("Id do dono: "); int clienteId = int.Parse(Console.ReadLine() ?? "");
    if (!dados.Clientes.Any(c => c.Id == clienteId)) throw new Exception("cliente não encontrado");
    Console.Write("Nome do pet: "); string nome = Console.ReadLine() ?? "";
    Console.Write("Espécie: "); string especie = Console.ReadLine() ?? "";
    Console.Write("Porte (pequeno/médio/grande): "); string porte = Console.ReadLine() ?? "";
    dados.Pets.Add(new Pet(ProximoId(dados.Pets.Select(x => x.Id)), clienteId, nome, especie, porte));
    Console.WriteLine("Pet cadastrado.");
}

void Agendar()
{
    if (dados.Pets.Count == 0) { Console.WriteLine("Não há pets cadastrados."); return; }
    dados.Pets.ForEach(p => Console.WriteLine(p.ExibirResumo()));
    Console.Write("Id do pet: "); int petId = int.Parse(Console.ReadLine() ?? "");
    Pet pet = dados.Pets.FirstOrDefault(p => p.Id == petId) ?? throw new Exception("pet não encontrado");
    dados.Servicos.ForEach(s => Console.WriteLine($"{s.Id} - {s.Nome} - R$ {s.CalcularPreco(pet.Porte):F2}"));
    Console.Write("Id do serviço: "); int servicoId = int.Parse(Console.ReadLine() ?? "");
    Servico servico = dados.Servicos.FirstOrDefault(s => s.Id == servicoId) ?? throw new Exception("serviço não encontrado");
    Console.Write("Data e hora (dd/MM/yyyy HH:mm): "); DateTime data = DateTime.Parse(Console.ReadLine() ?? "");
    if (data <= DateTime.Now) throw new Exception("o horário precisa ser futuro");
    if (dados.Agendamentos.Any(a => a.DataHora == data && !a.Concluido)) throw new Exception("horário já ocupado");
    dados.Agendamentos.Add(new Agendamento { Id = ProximoId(dados.Agendamentos.Select(x => x.Id)), PetId = petId, ServicoId = servicoId, DataHora = data, Valor = servico.CalcularPreco(pet.Porte) });
    Console.WriteLine("Atendimento marcado.");
}

void ListarAgenda()
{
    if (dados.Agendamentos.Count == 0) { Console.WriteLine("Agenda vazia."); return; }
    foreach (Agendamento a in dados.Agendamentos.OrderBy(a => a.DataHora))
    {
        string pet = dados.Pets.FirstOrDefault(p => p.Id == a.PetId)?.Nome ?? "Pet removido";
        Console.WriteLine($"{a.Id} | {a.DataHora:g} | {pet} | R$ {a.Valor:F2} | {(a.Concluido ? "concluído" : "marcado")}");
    }
}

void Concluir()
{
    ListarAgenda(); Console.Write("Id do atendimento: "); int id = int.Parse(Console.ReadLine() ?? "");
    Agendamento a = dados.Agendamentos.FirstOrDefault(x => x.Id == id) ?? throw new Exception("agendamento não encontrado");
    a.Concluido = true;
    Pet? pet = dados.Pets.FirstOrDefault(p => p.Id == a.PetId);
    Cliente? cliente = dados.Clientes.FirstOrDefault(c => c.Id == pet?.ClienteId);
    if (cliente != null) cliente.Pontos += 1;
    Console.WriteLine("Concluído. O cliente ganhou 1 ponto.");
}

void RegistrarVacina()
{
    if (dados.Pets.Count == 0) { Console.WriteLine("Não há pet para receber vacina."); return; }
    Console.Write("Id do pet: "); int petId = int.Parse(Console.ReadLine() ?? "");
    if (!dados.Pets.Any(p => p.Id == petId)) throw new Exception("pet não encontrado");
    Console.Write("Nome da vacina: "); string vacina = Console.ReadLine() ?? "";
    Console.Write("Data da aplicação: "); DateTime aplicacao = DateTime.Parse(Console.ReadLine() ?? "");
    Console.Write("Próxima dose: "); DateTime proxima = DateTime.Parse(Console.ReadLine() ?? "");
    if (proxima <= aplicacao) throw new Exception("a próxima dose deve ser depois da aplicação");
    dados.Vacinacoes.Add(new Vacinacao { Id = ProximoId(dados.Vacinacoes.Select(x => x.Id)), PetId = petId, Vacina = vacina, DataAplicacao = aplicacao, ProximaDose = proxima });
    Console.WriteLine("Vacina registrada.");
}

void AlertasVacina()
{
    DateTime limite = DateTime.Today.AddDays(30);
    var lista = dados.Vacinacoes.Where(v => v.ProximaDose.Date <= limite).OrderBy(v => v.ProximaDose).ToList();
    if (lista.Count == 0) { Console.WriteLine("Nenhuma dose para os próximos 30 dias."); return; }
    foreach (Vacinacao v in lista) Console.WriteLine($"{dados.Pets.First(p => p.Id == v.PetId).Nome}: {v.Vacina} em {v.ProximaDose:d}");
}

void Historico()
{
    Console.Write("Id do pet: "); int id = int.Parse(Console.ReadLine() ?? "");
    Pet pet = dados.Pets.FirstOrDefault(p => p.Id == id) ?? throw new Exception("pet não encontrado");
    Console.WriteLine(pet.ExibirResumo());
    foreach (Agendamento a in dados.Agendamentos.Where(a => a.PetId == id && a.Concluido)) Console.WriteLine($"Serviço em {a.DataHora:d} - R$ {a.Valor:F2}");
    foreach (Vacinacao v in dados.Vacinacoes.Where(v => v.PetId == id)) Console.WriteLine($"Vacina {v.Vacina} aplicada em {v.DataAplicacao:d}");
}

void PedirEncaixe()
{
    Console.Write("Id do pet: "); int petId = int.Parse(Console.ReadLine() ?? "");
    if (!dados.Pets.Any(p => p.Id == petId)) throw new Exception("pet não encontrado");
    if (dados.FilaEncaixe.Any(f => f.PetId == petId)) throw new Exception("esse pet já está na fila");
    Console.Write("Data desejada: "); DateTime data = DateTime.Parse(Console.ReadLine() ?? "");
    if (data.Date < DateTime.Today) throw new Exception("data passada não é aceita");
    Console.Write("Motivo: "); string motivo = Console.ReadLine() ?? "";
    dados.FilaEncaixe.Add(new SolicitacaoEncaixe { Id = ProximoId(dados.FilaEncaixe.Select(x => x.Id)), PetId = petId, DataDesejada = data, Motivo = motivo });
    Console.WriteLine("Pedido de encaixe incluído.");
}

void ListarEncaixes()
{
    if (dados.FilaEncaixe.Count == 0) { Console.WriteLine("Fila vazia."); return; }
    foreach (SolicitacaoEncaixe f in dados.FilaEncaixe.OrderBy(f => f.DataDesejada)) Console.WriteLine($"{f.Id} - pet {f.PetId} - {f.DataDesejada:d} - {f.Motivo}");
}

static int ProximoId(IEnumerable<int> ids) => ids.Any() ? ids.Max() + 1 : 1;
