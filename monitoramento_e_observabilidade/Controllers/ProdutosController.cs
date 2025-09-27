using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace monitoramento_e_observabilidade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : Controller
    {
        private readonly IMongoCollection<Produto> _produtos;

        public ProdutosController(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDb:Database"]);
            _produtos = database.GetCollection<Produto>("produtos");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _produtos.Find(_ => true).ToListAsync();
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Produto produto)
        {
            await _produtos.InsertOneAsync(produto);
            return CreatedAtAction(nameof(GetAll), new { id = produto.Id }, produto);
        }
    }

    public class Produto
    {
        public string Id { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public decimal Preco { get; set; }
    }
}
