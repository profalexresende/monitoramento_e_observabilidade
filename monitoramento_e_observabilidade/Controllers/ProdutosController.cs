using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace monitoramento_e_observabilidade.Controllers
{
    // Indica que esta classe é um controlador de API
    [ApiController]
    // Define a rota de acesso: api/produtos
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        // Declara uma variável para acessar a coleção de produtos no MongoDB
        private readonly IMongoCollection<Produto> _produtos;

        // Construtor da classe, executado ao criar o controlador
        public ProdutosController(IConfiguration config)
        {
            // Cria um cliente para conectar ao MongoDB usando a configuração
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            // Seleciona o banco de dados informado na configuração
            var database = client.GetDatabase(config["MongoDb:Database"]);
            // Seleciona a coleção de produtos dentro do banco de dados
            _produtos = database.GetCollection<Produto>("produtos");
        }

        // Define o método para buscar todos os produtos (GET)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Busca todos os produtos na coleção do MongoDB
            var lista = await _produtos.Find(_ => true).ToListAsync();
            // Retorna a lista de produtos com status 200 (OK)
            return Ok(lista);
        }

        // Define o método para criar um novo produto (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Produto produto)
        {
            // Insere o novo produto na coleção do MongoDB
            await _produtos.InsertOneAsync(produto);
            // Retorna o produto criado com status 201 (Created)
            return CreatedAtAction(nameof(GetAll), new { id = produto.Id }, produto);
        }
    }

    // Classe que representa o produto
    public class Produto
    {
        // Identificador do produto
        public string Id { get; set; } = null!;
        // Nome do produto
        public string Nome { get; set; } = null!;
        // Preço do produto
        public decimal Preco { get; set; }
    }
}
