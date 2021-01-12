using System;
using System.Collections.Generic;
using System.Linq;
using EFCore.Data;
using EFCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore
{
    /**
    - Para  executar um console app no vscode
    dotnet run

    Scripts Migrations Utilizados:

    - Gerar primeira migração:
    dotnet ef migrations add PrimeiraMigracao -p .\Curso\CursoEFCore.csproj

    - Gerar Script das migrações
     dotnet ef migrations script -p .\Curso\CursoEFCore.csproj -o .\Curso\PrimeiraMigracao.SQL     

     - Atualizar o banco de dados
     dotnet ef database update -p .\Curso\CursoEFCore.csproj -v

     - Rollback de migrações
     basta executar o comando de update usando a migração desejada para voltar até ela
     porém a migração ainda continua na pasta Migrations, para remover a última migração, usar o comando:
     dotnet ef database remove -p .\Curso\CursoEFCore.csproj

    Packages

    - Package para logar no console tudo que está sendo executado
    dotnet add package Microsoft.Extensions.Logging.Console --version 3.1.5
    após isso criar o log no ApplicationContext:
    private static readonly ILoggerFactory _logger = LoggerFactory.Create(p=>p.AddConsole());
    E configurar isso no OnConfiguring
            optionsBuilder
                .UseLoggerFactory(_logger)
                .EnableSensitiveDataLogging()    
    EnableSensitiveDataLogging => Log mais detalhado, mostra os parametros que estão sendo usados
    **/
    class Program
    {
        static void Main(string[] args)
        {
            //InserirDados();            
            //InserirDadosEmMassa();
            //ConsultarDados();
            //CadastrarPedido();
            //ConsultarCarregamentoAdiantado();
            //AtualizarDados();
            RemoverRegistro();
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Descrição Teste",
                CodigoBarras = "123456789123",
                Valor = 100,
                TipoProduto = EFCore.ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new ApplicationContext();
            db.Produtos.Add(produto);

            int registros = db.SaveChanges();
            Console.WriteLine(registros);
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Descrição Teste",
                CodigoBarras = "123456789123",
                Valor = 100,
                TipoProduto = EFCore.ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Caio",
                CEP = "02433110",
                Cidade = "São Paulo",
                Estado = "SP",
                Telefone = "998032038"
            };            

            using var db = new ApplicationContext();
            db.AddRange(produto, cliente);

            int registros = db.SaveChanges();
            Console.WriteLine($"{registros} inseridos.");
        }    

        private static void ConsultarDados()
        {
            using var db = new ApplicationContext();

            var consulta = db.Clientes
              .Where(p => p.Id > 0)
              .OrderBy(p => p.Id)
              .ToList();

            consulta.ForEach(cliente =>
            {
                db.Clientes.Find(cliente.Id);
            });
        } 

        private static void CadastrarPedido()
        {
            using var db = new ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido() 
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Teste",
                Status = EFCore.ValueObjects.StatusPedido.Analise,
                TipoFrete = EFCore.ValueObjects.TipoFrete.SemFrete,
                Itens = new List<PedidoItem>()
                {
                    new PedidoItem()
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };
            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }

        private static void ConsultarCarregamentoAdiantado()
        {
            using var db = new ApplicationContext();
            var pedidos = db.Pedidos
                .Include(pedido => pedido.Itens)
                    .ThenInclude(item => item.Produto)
                .ToList();     
        }         

        private static void AtualizarDados()
        {
            using var db = new ApplicationContext();
            var cliente = db.Clientes.FirstOrDefault();
            cliente.Cidade = "XPTO";

            // Não há necessidade do update quando a entidade está sendo rastreada
            // Quando usamos o Update, o EF cria um update com todos os campos, usando o 
            // rastreamento, ele atualiza somente os campos alterados
            //db.Clientes.Update(cliente);
            db.SaveChanges();
        }

        private static void RemoverRegistro()
        {
            using var db = new ApplicationContext();
            //var cliente = db.Clientes.Find(2);
            
            // Prodemos simular um objeto sem que haja necessidade de carregar-lo do banco
            var cliente = new Cliente { Id = 2 };             
            
            //db.Clientes.Remove(cliente);
            db.Remove(cliente);

            db.SaveChanges();
        }
        
    }
}
